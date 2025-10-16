using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Commands.AcceptIncident
{
    public class AcceptIncidentCommandHandler : IRequestHandler<AcceptIncidentCommand, Result<Guid>>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly IIncidentResponderRepository _incidentResponderRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AcceptIncidentCommandHandler> _logger;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IResponderRepository _responderRepository;

        public AcceptIncidentCommandHandler(IIncidentRepository incidentRepository, IIncidentResponderRepository incidentResponderRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<AcceptIncidentCommandHandler> logger, IAuditLogRepository auditLogRepository, IResponderRepository responderRepository)
        {
            _incidentRepository = incidentRepository;
            _incidentResponderRepository = incidentResponderRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _responderRepository = responderRepository;
        }

        public async Task<Result<Guid>> Handle(AcceptIncidentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Model == null)
                {
                    _logger.LogWarning("AcceptIncidentCommand received with null model.");
                    return Result<Guid>.Failure("Invalid accept incident data.");
                }

                Guid currentUserId = _currentUserService.UserId;
                if (currentUserId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthenticated user attempted to accept an incident.");
                    return Result<Guid>.Failure("User is not authenticated.");
                }
                else if (_currentUserService.Role != UserRole.Responder)
                {
                    _logger.LogWarning("User {UserId} with role {UserRole} is not a responder and cannot accept incidents.", currentUserId, _currentUserService.Role);
                    return Result<Guid>.Failure("Only responders can accept incidents.");
                }

                var incident = await _incidentRepository.GetAsync(request.Model.IncidentId);
                if (incident == null)
                {
                    _logger.LogWarning("Incident {IncidentId} not found.", request.Model.IncidentId);
                    return Result<Guid>.Failure("Incident not found.");
                }

                var responder = await _responderRepository.GetAsync(r => r.UserId == currentUserId && !r.IsDeleted);
                if (responder == null)
                {
                    _logger.LogWarning("Responder profile not found for the current user {UserId}.", currentUserId);
                    return Result<Guid>.Failure("Responder profile not found for the current user.");
                }

                if (!responder.IsVerified || responder.Status != ResponderStatus.Available)
                {
                    _logger.LogWarning("Responder {ResponderId} is not available or not verified. Current status: {ResponderStatus}", responder.Id, responder.Status);
                    return Result<Guid>.Failure("Responder is not available or not verified.");
                }

                // incident.AssignResponder(responder.Id, ResponderRole.Primary);
                var incidentResponder = new IncidentResponder(incident.Id, responder.Id, ResponderRole.Primary);

                await _incidentResponderRepository.AddAsync(incidentResponder);

                incident.MarkAsReport();
                responder.UpdateResponderStatus(ResponderStatus.OnDuty);

                // incident.MarkUpdated();
                // responder.MarkUpdated();

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

                _logger.LogInformation("Incident {IncidentId} accepted successfully by Responder {ResponderId}.", incident.Id, responder.Id);
                return Result<Guid>.Success(incident.Id, "Incident accepted successfully.");
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while accepting incident {IncidentId}.", request.Model.IncidentId);
                return Result<Guid>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting incident {IncidentId}", request.Model.IncidentId);
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}