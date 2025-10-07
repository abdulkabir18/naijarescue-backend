using Application.Common.Dtos;
using Domain.Enums;

namespace Application.Features.Incidents.Dtos
{
    public record CreateIncidentRequestModel(
        IncidentType Type,
        GeoLocationDto Location,
        AddressDto? Address,
        DateTime OccurredAt,
        bool IsAuthenticatedUser,
        bool IsReportingForAnotherPerson,
        ReporterDetailsDto? ReporterDetails,
        VictimDetailsDto? VictimDetails,
        IncidentMediaDto? IncidentMedias
    );
}
