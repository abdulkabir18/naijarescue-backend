using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Commands.AcceptIncident
{
    public class AcceptIncidentCommandHandler : IRequestHandler<AcceptIncidentCommand, Result<Guid>>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AcceptIncidentCommandHandler> _logger;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IResponderRepository _responderRepository;

        public AcceptIncidentCommandHandler(IIncidentRepository incidentRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<AcceptIncidentCommandHandler> logger, IAuditLogRepository auditLogRepository, IResponderRepository responderRepository)
        {
            _incidentRepository = incidentRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _responderRepository = responderRepository;
        }

        public async Task<Result<Guid>> Handle(AcceptIncidentCommand request, CancellationToken cancellationToken)
        {
            if (request.Model == null)
            {
                _logger.LogWarning("AcceptIncidentCommand received with null model.");
                return Result<Guid>.Failure("Invalid accept incident data.");
            }

            Guid currentUserId = _currentUserService.UserId;
            if (currentUserId == Guid.Empty)
            {
                _logger.LogWarning("User is not authenticated.");
                return Result<Guid>.Failure("User is not authenticated.");
            }
            else if (_currentUserService.Role != UserRole.Responder)
            {
                _logger.LogWarning("User is not a responder.");
                return Result<Guid>.Failure("Only responders can accept incidents.");
            }

            var incident = await _incidentRepository.GetAsync(request.Model.IncidentId);
            if (incident == null)
            {
                _logger.LogWarning("Incident not found.");
                return Result<Guid>.Failure("Incident not found.");
            }

            var responder = await _responderRepository.GetAsync(r => r.UserId == currentUserId);
            if (responder == null)
            {
                _logger.LogWarning("Responder profile not found for the current user.");
                return Result<Guid>.Failure("Responder profile not found.");
            }

            incident.AssignResponder(responder.Id, ResponderRole.Primary);
            responder.UpdateResponderStatus(ResponderStatus.OnDuty);

            await _incidentRepository.UpdateAsync(incident);
            await _responderRepository.UpdateAsync(responder);

            var auditLog = new AuditLog(
                AuditActionType.Updated,
                nameof(Incident),
                incident.Id,
                $"Responder {responder.Id} accepted the incident.",
                currentUserId,
                _currentUserService.IpAddress,
                _currentUserService.UserAgent);

            await _auditLogRepository.AddAsync(auditLog);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Incident {IncidentId} accepted successfully.", incident.Id);
            return Result<Guid>.Success(incident.Id, "Incident accepted successfully.");
        }
    }
}