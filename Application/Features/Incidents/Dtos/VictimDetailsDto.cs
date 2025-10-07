namespace Application.Features.Incidents.Dtos
{
    public record VictimDetailsDto(
        string Name,
        string? PhoneNumber,
        string Email,
        string Description
    );
}
