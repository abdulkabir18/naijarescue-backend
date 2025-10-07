using System.ComponentModel.DataAnnotations;

namespace Application.Features.Incidents.Dtos
{
    public record ReporterDetailsDto(
        string? Name,
        string? PhoneNumber,
        string? Email
    );
}
