using Application.Common.Dtos;
using Application.Features.Incidents.Dtos;
using Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Queries.GetIncidentById
{
    public class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, Result<IncidentDto>>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ILogger<GetIncidentByIdQueryHandler> _logger;

        public GetIncidentByIdQueryHandler(IIncidentRepository incidentRepository, ILogger<GetIncidentByIdQueryHandler> logger)
        {
            _incidentRepository = incidentRepository;
            _logger = logger;
        }

        public async Task<Result<IncidentDto>> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching incident details for ID: {IncidentId}", request.IncidentId);

                var incident = await _incidentRepository.GetByIdWithDetailsAsync(request.IncidentId);

                if (incident == null || incident.IsDeleted)
                {
                    _logger.LogWarning("Incident with ID {IncidentId} not found or deleted", request.IncidentId);
                    return Result<IncidentDto>.Failure("Incident not found.");
                }

                var activeResponder = incident.AssignedResponders
                    .FirstOrDefault(r => r.IsActive);

                var dto = new IncidentDto
                {
                    Id = incident.Id,
                    ReferenceCode = incident.ReferenceCode,
                    Type = incident.Type.ToString(),
                    Status = incident.Status.ToString(),
                    Location = new GeoLocationDto
                    (
                        incident.Location.Latitude,
                        incident.Location.Longitude
                    ),
                    Address = incident.Address != null
                    ? new AddressDto
                    {
                        Street = incident.Address.Street,
                        City = incident.Address.City,
                        State = incident.Address.State,
                        LGA = incident.Address.LGA,
                        PostalCode = incident.Address.PostalCode,
                        Country = incident.Address.Country
                    }
                    : null,
                    CreatedAt = incident.CreatedAt,
                    OccurredAt = incident.OccurredAt,
                    UserId = incident.UserId,
                    UserName = incident.User?.FullName,
                    ChatId = incident.ChatId,
                    ReporterName = incident.Reporter?.Name,
                    ReporterPhoneNumber = incident.Reporter?.PhoneNumber?.Value,
                    ReporterEmail = incident.Reporter?.Email?.Value,
                    VictimName = incident.Victim?.Name,
                    VictimPhoneNumber = incident.Victim?.PhoneNumber?.Value,
                    VictimEmail = incident.Victim?.Email?.Value,
                    VictimDescription = incident.Victim?.Description,
                    AssignedResponderId = incident.AssignedResponders.FirstOrDefault(r => r.IsActive)?.ResponderId,
                    AssignedResponderName = incident.AssignedResponders.FirstOrDefault(r => r.IsActive)?.Responder?.User.FullName,
                    IncidentMedias = incident.IncidentMedias.Select(m =>
                        new IncidentMediaDetailsDto(m.Id, m.FileUrl, m.MediaType)
                    ).ToList(),

                    LiveStreams = incident.LiveStreams.Select(ls =>
                        new IncidentLiveStreamDetailsDto(
                            ls.Id,
                            ls.StreamKey,
                            ls.StartedAt,
                            ls.Participants.Select(p =>
                                new IncidentLiveStreamParticipantDto(
                                    p.UserId,
                                    p.User?.FullName ?? "Unknown",
                                    p.JoinedAt,
                                    p.LeftAt
                                )
                            ).ToList()
                        )
                    ).ToList()
                };


                _logger.LogInformation("Successfully retrieved incident {IncidentId}", incident.Id);
                return Result<IncidentDto>.Success(dto, $"Successfully retrieved incident {incident.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident with ID {IncidentId}", request.IncidentId);
                return Result<IncidentDto>.Failure($"An error occurred while fetching the incident: {ex.Message}");
            }
        }
    }
}