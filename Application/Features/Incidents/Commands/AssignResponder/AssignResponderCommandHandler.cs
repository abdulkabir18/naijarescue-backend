using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Common.Exceptions;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Commands.AssignResponder
{

    public class AssignResponderCommandHandler : IRequestHandler<AssignResponderCommand, Result<Guid>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIncidentRepository _incidentRepository;
        private readonly IResponderRepository _responderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignResponderCommandHandler> _logger;
        private readonly IAuditLogRepository _auditLogRepository;

        public AssignResponderCommandHandler(ICurrentUserService currentUserService, IIncidentRepository incidentRepository, IResponderRepository responderRepository, IUnitOfWork unitOfWork, ILogger<AssignResponderCommandHandler> logger, IAuditLogRepository auditLogRepository)
        {
            _currentUserService = currentUserService;
            _incidentRepository = incidentRepository;
            _responderRepository = responderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<Result<Guid>> Handle(AssignResponderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to assign responder for incident {IncidentId}", request.Model.IncidentId);

                if (request.Model == null)
                {
                    _logger.LogWarning("AssignResponderCommand received with null model.");
                    return Result<Guid>.Failure("Invalid assign responder data.");
                }

                Guid currentUserId = _currentUserService.UserId;
                if (currentUserId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthenticated user attempted to assign a responder.");
                    return Result<Guid>.Failure("User is not authenticated.");
                }
                else if (_currentUserService.Role != UserRole.SuperAdmin && _currentUserService.Role != UserRole.AgencyAdmin)
                {
                    _logger.LogWarning("User {UserId} with role {UserRole} is not authorized to assign responders to incidents.", currentUserId, _currentUserService.Role);
                    return Result<Guid>.Failure("Only agency admin or super admin can assign responders to incidents.");
                }

                var incident = await _incidentRepository.GetAsync(request.Model.IncidentId);
                if (incident == null)
                {
                    _logger.LogWarning("Incident with ID {IncidentId} not found.", request.Model.IncidentId);
                    return Result<Guid>.Failure("Incident not found.");
                }

                var responder = await _responderRepository.GetAsync(r => r.Id == request.Model.ResponderId && !r.IsDeleted);
                if (responder == null)
                {
                    _logger.LogWarning("Responder with ID {ResponderId} not found.", request.Model.ResponderId);
                    return Result<Guid>.Failure("Responder not found.");
                }

                if (!responder.IsVerified || responder.Status != ResponderStatus.Available)
                {
                    _logger.LogWarning("Responder {ResponderId} is not available or not verified.", responder.Id);
                    return Result<Guid>.Failure("Responder is not available or not verified.");
                }

                incident.AssignResponder(responder.Id, request.Model.Role);
                responder.UpdateResponderStatus(ResponderStatus.OnDuty);

                incident.MarkUpdated();
                responder.MarkUpdated();

                await _incidentRepository.UpdateAsync(incident);
                await _responderRepository.UpdateAsync(responder);

                var auditLog = new AuditLog(
                    AuditActionType.Updated,
                    nameof(Incident),
                    incident.Id,
                    $"Assigned responder {responder.Id} to incident {incident.Id} with role {request.Model.Role}.",
                    currentUserId,
                    _currentUserService.IpAddress,
                    _currentUserService.UserAgent);

                await _auditLogRepository.AddAsync(auditLog);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully assigned responder {ResponderId} to incident {IncidentId}", responder.Id, incident.Id);
                return Result<Guid>.Success(incident.Id, "Responder assigned successfully.");
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while assigning responder to incident {IncidentId}.", request.Model.IncidentId);
                return Result<Guid>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning responder to incident {IncidentId}", request.Model.IncidentId);
                return Result<Guid>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}