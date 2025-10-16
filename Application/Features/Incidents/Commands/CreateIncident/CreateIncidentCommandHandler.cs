using Application.Common.Dtos;
using Application.Common.Helpers;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Commands.CreateIncident
{
    public class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, Result<Guid>>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageManager _storageManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateIncidentCommandHandler> _logger;
        private readonly IAuditLogRepository _auditLogRepository;

        public CreateIncidentCommandHandler(IIncidentRepository incidentRepository, ICurrentUserService currentUserService, IStorageManager storageManager, IUnitOfWork unitOfWork, ILogger<CreateIncidentCommandHandler> logger, IAuditLogRepository auditLogRepository)
        {
            _incidentRepository = incidentRepository;
            _currentUserService = currentUserService;
            _storageManager = storageManager;
            _unitOfWork = unitOfWork;
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Model == null)
                {
                    _logger.LogWarning("CreateIncidentCommand received with null model.");
                    return Result<Guid>.Failure("Invalid incident data.");
                }

                if (!Enum.IsDefined(typeof(IncidentType), request.Model.Type))
                {
                    _logger.LogWarning("Invalid IncidentType: {IncidentType}", request.Model.Type);
                    return Result<Guid>.Failure($"IncidentType '{request.Model.Type}' is not valid.");
                }

                Guid currentUserId = _currentUserService.UserId;

                var location = new GeoLocation(request.Model.Location.Latitude, request.Model.Location.Longitude);
                var address = request.Model.Address != null ? new Address(request.Model.Address.Street!, request.Model.Address.City!, request.Model.Address.State!, request.Model.Address.LGA!, request.Model.Address.Country!, request.Model.Address.PostalCode!) : null;

                var incident = new Incident(request.Model.Type, location, request.Model.OccurredAt, currentUserId == Guid.Empty ? null : currentUserId);

                if (address != null)
                    incident.UpdateAddress(address);

                if (request.Model.IsReportingForAnotherPerson)
                {
                    if (request.Model.ReporterDetails == null)
                    {
                        _logger.LogWarning("CreateIncidentCommand received with null ReporterDetails.");
                        return Result<Guid>.Failure("Invalid reporter details.");
                    }
                    incident.SetReporterDetails(request.Model.ReporterDetails.Name!, request.Model.ReporterDetails.PhoneNumber!, request.Model.ReporterDetails.Email!);

                    if (request.Model.VictimDetails == null)
                    {
                        _logger.LogWarning("CreateIncidentCommand received with null VictimDetails.");
                        return Result<Guid>.Failure("Invalid victim details.");
                    }
                    incident.SetVictimDetails(request.Model.VictimDetails.Name, request.Model.VictimDetails.PhoneNumber!, request.Model.VictimDetails.Email, request.Model.VictimDetails.Description);
                }
                else if (!request.Model.IsAuthenticatedUser)
                {
                    if (request.Model.VictimDetails == null)
                    {
                        _logger.LogWarning("CreateIncidentCommand received with null VictimDetails.");
                        return Result<Guid>.Failure("Invalid victim details.");
                    }
                    incident.SetVictimDetails(request.Model.VictimDetails!.Name, request.Model.VictimDetails.PhoneNumber!, request.Model.VictimDetails.Email, request.Model.VictimDetails.Description);
                }

                if (request.Model.IncidentMedias != null && request.Model.IncidentMedias.Files.Count > 0)
                {
                    foreach (var mediaDto in request.Model.IncidentMedias.Files)
                    {
                        MediaType mediaType = MediaTypeMapper.MapContentType(mediaDto.ContentType);
                        if (!Enum.IsDefined(typeof(MediaType), mediaType))
                        {
                            _logger.LogWarning("Unsupported media type: {ContentType}", mediaDto.ContentType);
                            // return Result<Guid>.Failure($"Unsupported file type: {file.ContentType}");
                            continue;
                        }

                        try
                        {
                            var incidentFileUrl = await _storageManager.UploadMediaAsync(mediaDto.OpenReadStream(), mediaDto.FileName, mediaDto.ContentType);
                            incident.AddMedia(incidentFileUrl, mediaType, currentUserId.ToString());
                            _logger.LogInformation("Uploaded media file {FileName} for incident.", mediaDto.FileName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to upload media file {FileName}", mediaDto.FileName);
                            continue;
                        }
                    }
                }

                if (currentUserId != Guid.Empty)
                    incident.SetCreatedBy(currentUserId.ToString());

                await _incidentRepository.AddAsync(incident);

                var auditLog = new AuditLog(
                    AuditActionType.Created,
                    nameof(Incident),
                    incident.Id,
                    $"Incident created with ReferenceCode: {incident.ReferenceCode}",
                    currentUserId == Guid.Empty ? null : currentUserId,
                    _currentUserService.IpAddress,
                    _currentUserService.UserAgent
                );

                await _auditLogRepository.AddAsync(auditLog);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Incident {IncidentId} created successfully with ReferenceCode: {RefCode}.", incident.Id, incident.ReferenceCode);

                return Result<Guid>.Success(incident.Id, "Incident created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an incident.");
                return Result<Guid>.Failure("An unexpected error occurred while creating the incident.");
            }
        }
    }
}