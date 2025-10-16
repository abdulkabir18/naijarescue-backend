using Application.Common.Dtos;
using Domain.Enums;

namespace Application.Features.Incidents.Dtos
{
    public record CreateIncidentRequestModel
    {
        public IncidentType Type { get; set; }
        public GeoLocationDto Location { get; set; } = default!;
        public AddressDto? Address { get; set; }
        public DateTime OccurredAt { get; set; }
        public bool IsAuthenticatedUser { get; set; }
        public bool IsReportingForAnotherPerson { get; set; }
        public ReporterDetailsDto? ReporterDetails { get; set; }
        public VictimDetailsDto? VictimDetails { get; set; }
        public IncidentMediaDto? IncidentMedias { get; set; }
    }
}
