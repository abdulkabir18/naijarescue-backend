using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Agencies.Dtos
{
    public record RegisterAgencyRequestModel(
        RegisterUserRequestModel RegisterUserRequest,
        string AgencyName, string AgencyEmail, string AgencyPhoneNumber, IFormFile? AgencyLogo,
        AddressDto? AgencyAddress,
        IncidentTypesDto SupportedIncidents,
        WorkTypesDto SupportedWorkTypes);

    public record IncidentTypesDto(ICollection<IncidentTypeDto> Types);
    public record WorkTypesDto(ICollection<WorkTypeDto> Types);
}