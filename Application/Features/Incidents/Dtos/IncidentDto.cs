using Application.Common.Dtos;
using Domain.Enums;

namespace Application.Features.Incidents.Dtos
{
    public record IncidentMediaDetailsDto(Guid Id, string Url, MediaType MediaType);

    public record IncidentLiveStreamDetailsDto(Guid Id, string StreamUrl, DateTime StartedAt, ICollection<IncidentLiveStreamParticipantDto> Participants);

    public record IncidentLiveStreamParticipantDto(Guid UserId, string UserName, DateTime JoinedAt, DateTime? LeftAt);

    public record IncidentDto
    {
        public Guid Id { get; set; }
        public string ReferenceCode { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Status { get; set; } = default!;
        public GeoLocationDto Location { get; set; } = default!;
        public AddressDto? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime OccurredAt { get; set; }

        public Guid? UserId { get; set; }
        public string? UserName { get; set; }

        public Guid? ChatId { get; set; }

        public string? ReporterName { get; set; }
        public string? ReporterPhoneNumber { get; set; }
        public string? ReporterEmail { get; set; }

        public string? VictimName { get; set; }
        public string? VictimPhoneNumber { get; set; }
        public string? VictimEmail { get; set; }
        public string? VictimDescription { get; set; }

        public Guid? AssignedResponderId { get; set; }
        public string? AssignedResponderName { get; set; }
        public ICollection<IncidentMediaDetailsDto> IncidentMedias { get; set; } = [];
        public ICollection<IncidentLiveStreamDetailsDto> LiveStreams { get; set; } = [];
    }

    public record IncidentSummaryDto
    {
        public Guid Id { get; set; }
        public string ReferenceCode { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime OccurredAt { get; set; }
        public GeoLocationDto Location { get; set; } = default!;
    }
}
