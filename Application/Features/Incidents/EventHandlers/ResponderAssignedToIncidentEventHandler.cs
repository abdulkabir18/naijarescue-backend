using Application.Common.Interfaces.Notifications;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.EventHandlers
{
    public class ResponderAssignedToIncidentEventHandler : INotificationHandler<ResponderAssignedToIncidentEvent>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IResponderRepository _responderRepository;
        private readonly IInAppNotificationService _inAppNotificationService;
        private readonly IEmailService _emailService;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResponderAssignedToIncidentEventHandler> _logger;

        public ResponderAssignedToIncidentEventHandler(IIncidentRepository incidentRepository, IResponderRepository responderRepository, IEmailService emailService, IInAppNotificationService inAppNotificationService, IAuditLogRepository auditLogRepository, IUnitOfWork unitOfWork, ILogger<ResponderAssignedToIncidentEventHandler> logger)
        {
            _incidentRepository = incidentRepository;
            _responderRepository = responderRepository;
            _emailService = emailService;
            _inAppNotificationService = inAppNotificationService;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ResponderAssignedToIncidentEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("ResponderAssignedToIncidentEvent received for IncidentId: {IncidentId}", notification.IncidentId);

                var incident = await _incidentRepository.GetAsync(notification.IncidentId);
                if (incident is null)
                {
                    _logger.LogWarning("Incident not found for IncidentId: {IncidentId}", notification.IncidentId);
                    return;
                }

                var responder = await _responderRepository.GetResponderWithDetailsAsync(notification.ResponderId);
                if (responder is null || responder.User is null)
                {
                    _logger.LogWarning("Responder or associated user not found for ResponderId: {ResponderId}", notification.ResponderId);
                    return;
                }

                if (incident.UserId.HasValue)
                {
                    var userMessage = $"A responder, {responder.User.FullName}, is on the way for the {incident.Type} incident.";
                    await _inAppNotificationService.SendToUserAsync(
                        incident.UserId.Value,
                        "Responder Assigned",
                        userMessage,
                        NotificationType.Success,
                        incident.Id,
                        nameof(Incident)
                    );
                }

                var responderMessage = $"You have been assigned to a {incident.Type} incident. Location: {incident.Address?.Street ?? "details in app"}.";
                await _inAppNotificationService.SendToUserAsync(
                    responder.UserId,
                    "Incident Assignment Confirmed",
                    responderMessage,
                    NotificationType.Incident,
                    incident.Id,
                    nameof(Incident)
                );

                if (responder.Agency?.AgencyAdmin != null)
                {
                    var adminMessage = $"Responder {responder.User.FullName} has been dispatched to a {incident.Type} incident.";
                    await _inAppNotificationService.SendToUserAsync(
                        responder.Agency.AgencyAdmin.Id,
                        "Unit Dispatched",
                        adminMessage,
                        NotificationType.Info,
                        incident.Id,
                        nameof(Incident)
                    );
                }

                var auditLog = new AuditLog(
                    AuditActionType.Updated,
                    nameof(Incident),
                    incident.Id,
                    $"Responder '{responder.User.FullName}' (ID: {responder.Id}) was assigned to the incident.",
                    responder.UserId
                );
                await _auditLogRepository.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully handled ResponderAssignedToIncidentEvent for IncidentId: {IncidentId}", notification.IncidentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling ResponderAssignedToIncidentEvent for IncidentId: {IncidentId}", notification.IncidentId);
            }
        }
    }
}