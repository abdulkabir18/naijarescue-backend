using Application.Features.Users.Dtos;
using Application.Common.Dtos;

namespace Application.Features.Responders.Dtos
{
    public record RegisterResponderRequestModel(
        RegisterUserRequestModel RegisterUserRequest,
        Guid AgencyId, string? BadgeNumber, string? Rank,
        GeoLocationDto? AssignedLocation,
        ICollection<WorkTypeDto> Capabilities,
        ICollection<IncidentTypeDto> Specialties);
}