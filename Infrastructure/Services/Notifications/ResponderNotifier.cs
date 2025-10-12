using Application.Common.Interfaces.Notifications;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Notifications
{
    public class ResponderNotifier : IResponderNotifier
    {
        private readonly IResponderRepository _responderRepository;
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ResponderNotifier> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ResponderNotifier(IResponderRepository responderRepository, IInAppNotificationService inAppNotificationService, IEmailService emailService, ILogger<ResponderNotifier> logger, IUserRepository userRepository, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork)
        {
            _responderRepository = responderRepository;
            _inAppNotificationService = inAppNotificationService;
            _emailService = emailService;
            _logger = logger;
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task NotifyNearbyRespondersAsync(Incident incident, double radiusInKm = 5.0)
        {
            if (incident == null || incident.Location == null)
            {
                _logger.LogWarning("Cannot notify responders: invalid incident or location.");
                return;
            }

            var responders = await _responderRepository.GetNearbyRespondersForIncidentAsync(
                incident.Location,
                incident.Type,
                radiusInKm
            );

            if (!responders.Any())
            {
                _logger.LogWarning(
                    "No nearby responders found within {Radius}km for incident {IncidentId}",
                    radiusInKm, incident.Id
                );

                var superAdmin = await _userRepository.GetAsync(u =>
                    u.Role == UserRole.SuperAdmin && u.IsActive && !u.IsDeleted
                );

                if (superAdmin != null)
                {
                    var subject = "⚠️ No Nearby Responders Found";
                    var message = $@"
                        <h3>Incident Alert</h3>
                        <p><strong>Type:</strong> {incident.Type}</p>
                        <p><strong>Location:</strong> {incident.Address?.Street ?? "Unknown area"}</p>
                        <p>No responders were available within {radiusInKm} km.</p>";

                    await Task.WhenAll(
                        _inAppNotificationService.SendToUserAsync(
                            superAdmin.Id,
                            subject,
                            $"No responders were available within {radiusInKm}km for {incident.Type} near {incident.Address?.Street ?? "unknown area"}.",
                            NotificationType.Warning,
                            incident.Id,
                            nameof(Incident)
                        ),
                        _emailService.SendEmailAsync(
                            superAdmin.Email.Value,
                            subject,
                            message
                        )
                    );
                }

                var audit = new AuditLog(
                    AuditActionType.Warning,
                    nameof(Incident),
                    incident.Id,
                    $"No responders available for {incident.Type} near {incident.Address?.Street ?? "unknown location"}."
                );
                await _auditLogRepository.AddAsync(audit);
                await _unitOfWork.SaveChangesAsync();
                return;
            }

            var closestResponders = responders
                .OrderBy(r => Distance(incident.Location, r.AssignedLocation!))
                .Take(3)
                .ToList();

            var responderIds = closestResponders.Select(r => r.UserId).ToList();
            var responderMessage = $"🚨 {incident.Type} reported near {incident.Address?.Street ?? "your area"}!";

            await _inAppNotificationService.BroadcastAsync(
                responderIds,
                "Emergency Dispatch Alert 🚑",
                responderMessage,
                NotificationType.Incident,
                incident.Id,
                nameof(Incident)
            );

            var responderEmails = closestResponders
                .Where(r => r.User?.Email?.Value != null)
                .Select(r => r.User.Email.Value)
                .ToList();

            if (responderEmails.Any())
            {
                var emailSubject = "New Incident Alert";
                var emailBody = $@"<h3>New Incident Reported</h3>
                                   <p><strong>Type:</strong> {incident.Type}</p>
                                   <p><strong>Location:</strong> {incident.Address?.Street ?? "Unknown area"}</p>";
                await _emailService.SendEmailAsync(responderEmails, emailSubject, emailBody);
            }

            var agencies = closestResponders
                .Select(r => r.Agency)
                .Where(a => a != null && a.IsActive && !a.IsDeleted)
                .DistinctBy(a => a.Id)
                .ToList();

            foreach (var agency in agencies)
            {
                var admin = agency.AgencyAdmin;
                if (admin == null || !admin.IsActive || admin.IsDeleted)
                    continue;

                var subject = "🚨 Incident Assigned to Your Agency";
                var emailBody = $@"
                    <h3>New Emergency Reported</h3>
                    <p><strong>Type:</strong> {incident.Type}</p>
                    <p><strong>Location:</strong> {incident.Address?.Street ?? "Unknown area"}</p>
                    <p>Your agency's responders have been alerted to this incident.</p>";

                await Task.WhenAll(
                    _inAppNotificationService.SendToUserAsync(
                        admin.Id,
                        subject,
                        $"A new {incident.Type} incident has been reported near {incident.Address?.Street ?? "your jurisdiction"}.",
                        NotificationType.Info,
                        incident.Id,
                        nameof(Incident)
                    ),
                    _emailService.SendEmailAsync(admin.Email.Value, subject, emailBody)
                );
            }

            var successAudit = new AuditLog(
                AuditActionType.Created,
                nameof(Incident),
                incident.Id,
                $"Notified {closestResponders.Count} responders and their agencies for {incident.Type} near {incident.Address?.Street ?? "unknown area"}."
            );
            await _auditLogRepository.AddAsync(successAudit);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "Notified {ResponderCount} responders and their agencies for incident {IncidentId}",
                closestResponders.Count, incident.Id
            );
        }

        private static double Distance(GeoLocation loc1, GeoLocation loc2)
        {
            const double R = 6371;
            var lat1 = loc1.Latitude * Math.PI / 180.0;
            var lon1 = loc1.Longitude * Math.PI / 180.0;
            var lat2 = loc2.Latitude * Math.PI / 180.0;
            var lon2 = loc2.Longitude * Math.PI / 180.0;
            var dLat = lat2 - lat1;
            var dLon = lon2 - lon1;
            var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);
            return 2 * R * Math.Asin(Math.Sqrt(a));
        }
    }
}
