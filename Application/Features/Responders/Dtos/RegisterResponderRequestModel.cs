using Application.Features.Users.Dtos;
using Application.Common.Dtos;

namespace Application.Features.Responders.Dtos
{
    public record RegisterResponderRequestModel(
        RegisterUserRequestModel RegisterUserRequest,
        Guid AgencyId, string? BadgeNumber, string? Rank,
        GeoLocationDto? AssignedLocation,
        WorkTypesDto Capabilities,
        IncidentTypesDto Specialties);

    public record WorkTypesDto(ICollection<WorkTypeDto> Types);
    public record IncidentTypesDto(ICollection<IncidentTypeDto> Types);
}