using Application.Common.Dtos;
using Domain.Enums;

namespace Application.Features.Incidents.Dtos
{
    public record CreateIncidentRequestModel(
        IncidentType Type,
        GeoLocationDto Location,
        DateTime OccurredAt,
        bool IsAnonymous,
        string? ReporterName,
        string? ReporterPhoneNumber,
        string? ReporterEmail,
        ICollection<IncidentMediaDto>? IncidentMedias
    );
}
