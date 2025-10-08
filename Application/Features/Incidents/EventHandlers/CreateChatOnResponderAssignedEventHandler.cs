using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.EventHandlers
{
    public class CreateChatOnResponderAssignedEventHandler : INotificationHandler<ResponderAssignedToIncidentEvent>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IResponderRepository _responderRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<CreateChatOnResponderAssignedEventHandler> _logger;

        public CreateChatOnResponderAssignedEventHandler(IIncidentRepository incidentRepository, IResponderRepository responderRepository, IChatRepository chatRepository, IUnitOfWork unitOfWork, ILogger<CreateChatOnResponderAssignedEventHandler> logger, IAuditLogRepository auditLogRepository)
        {
            _incidentRepository = incidentRepository;
            _responderRepository = responderRepository;
            _chatRepository = chatRepository;
            _unitOfWork = unitOfWork;
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(ResponderAssignedToIncidentEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("CreateChatOnResponderAssignedEvent received for IncidentId: {IncidentId}", notification.IncidentId);

                var incident = await _incidentRepository.GetAsync(notification.IncidentId);
                if (incident is null)
                {
                    _logger.LogWarning("Incident not found for IncidentId: {IncidentId}", notification.IncidentId);
                    return;
                }

                if (!incident.UserId.HasValue)
                {
                    _logger.LogInformation("Skipping chat creation for anonymous incident {IncidentId}.", incident.Id);
                    return;
                }

                if (incident.ChatId.HasValue)
                {
                    _logger.LogInformation("Incident {IncidentId} already has a chat linked. Skipping chat creation.", incident.Id);
                    return;
                }

                var responder = await _responderRepository.GetResponderWithDetailsAsync(notification.ResponderId);
                if (responder is null || responder.User is null)
                {
                    _logger.LogWarning("Responder or associated user not found for ResponderId: {ResponderId}", notification.ResponderId);
                    return;
                }

                var chat = new Chat(ChatType.Incident, incident.Id);

                chat.AddParticipant(incident.UserId.Value, "Victim");
                chat.AddParticipant(responder.UserId, "Responder");

                if (responder.Agency != null)
                {
                    chat.AddParticipant(responder.Agency.AgencyAdminId, "AgencyAdmin");
                }

                incident.LinkChat(chat.Id);

                await _chatRepository.AddAsync(chat);
                await _incidentRepository.UpdateAsync(incident);

                var auditLog = new AuditLog(
                    AuditActionType.Created,
                    nameof(Chat),
                    chat.Id,
                    $"Chat created for incident {incident.ReferenceCode}.",
                    responder.UserId
                );
                await _auditLogRepository.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully created and linked chat for IncidentId: {IncidentId}", incident.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat for IncidentId: {IncidentId}", notification.IncidentId);
            }
        }
    }
}