using Application.Common.Interfaces.Notifications;
using Application.Interfaces.UnitOfWork;
using Application.Interfaces.Repositories;
using Domain.Events;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Application.Interfaces.External;

namespace Application.Features.Incidents.EventHandlers
{
    public class IncidentCreatedEventHandler : INotificationHandler<IncidentCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IResponderNotifier _responderNotifier;
        private readonly IIncidentRepository _incidentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IncidentCreatedEventHandler> _logger;
        private readonly IAuditLogRepository _auditLogRepository;


        public IncidentCreatedEventHandler(INotificationService notificationService, IEmailService emailService, IResponderNotifier responderNotifier, IIncidentRepository incidentRepository, IUnitOfWork unitOfWork, ILogger<IncidentCreatedEventHandler> logger, IAuditLogRepository auditLogRepository)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _responderNotifier = responderNotifier;
            _incidentRepository = incidentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
        }

        public async Task Handle(IncidentCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("IncidentCreatedEvent received for IncidentId: {IncidentId}", notification.IncidentId);

                var incident = await _incidentRepository.GetAsync(notification.IncidentId);
                if (incident is null)
                {
                    _logger.LogWarning("Incident not found for IncidentId: {IncidentId}", notification.IncidentId);
                    return;
                }

                if (notification.UserId.HasValue)
                {
                    await _notificationService.NotifyEmergencyContactsAsync(notification.UserId.Value, incident);
                }

                await _responderNotifier.NotifyNearbyRespondersAsync(incident);

                var reporterEmail = incident.Reporter?.Email.Value;
                var victimEmail = incident.Victim?.Email?.Value;

                if (!string.IsNullOrEmpty(reporterEmail))
                {
                    var subject = "Incident Report Confirmation";
                    var body = $@"
                        <h3>Thank you for your report.</h3>
                        <p>We have received your report for a <strong>{incident.Type}</strong> incident and are processing it.</p>
                        <p>Your incident reference code is: <strong>{incident.ReferenceCode}</strong></p>
                        <p>Help is on the way.</p>
                    ";
                    await _emailService.SendEmailAsync(reporterEmail, subject, body);
                }

                if (!string.IsNullOrEmpty(victimEmail) && victimEmail != reporterEmail && !string.IsNullOrEmpty(reporterEmail))
                {
                    var subject = "Emergency Incident Reported For You";
                    var body = $@"
                        <h3>An incident has been reported on your behalf.</h3>
                        <p>An emergency incident (<strong>{incident.Type}</strong>) has been reported. Help is being dispatched to your location.</p>
                        <p>Your incident reference code is: <strong>{incident.ReferenceCode}</strong></p>
                    ";
                    await _emailService.SendEmailAsync(victimEmail, subject, body);
                }
                else if (string.IsNullOrEmpty(reporterEmail) && !string.IsNullOrEmpty(victimEmail))
                {
                    var subject = "Incident Report Confirmation";
                    var body = $@"
                    <h3>Your incident report has been received.</h3>
                    <p>Help is on the way. Your reference code is <strong>{incident.ReferenceCode}</strong>.</p>";
                    await _emailService.SendEmailAsync(victimEmail, subject, body);
                }

                var auditLog = new AuditLog(
                    AuditActionType.Accessed,
                    nameof(Incident),
                    incident.Id,
                    "Initiated notification process for newly created incident.",
                    incident.UserId
                );
                await _auditLogRepository.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("IncidentCreatedEvent handled successfully for IncidentId: {IncidentId}", notification.IncidentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling IncidentCreatedEvent for IncidentId: {IncidentId}", notification.IncidentId);
            }
        }
    }
}
