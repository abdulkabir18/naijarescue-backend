using Domain.Enums;

namespace Application.Features.Incidents.Dtos
{
    public record CreateIncidentResponseDto(
        Guid Id,
        IncidentType Type,
        DateTime OccurredAt,
        bool IsAnonymous,
        string? ReporterName,
        string? ReporterPhoneNumber,
        string? ReporterEmail,
        string Status,              // e.g., "Pending", "Active", etc.
        DateTime CreatedAt);
}
