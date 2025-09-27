using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Agencies.Dtos
{
    public record RegisterAgencyRequestModel(
        RegisterUserRequestModel RegisterUserRequest,
        string AgencyName, string AgencyEmail, string AgencyPhoneNumber, IFormFile? AgencyLogo,
        AddressDto? AgencyAddress,
        ICollection<IncidentTypeDto> SupportedIncidents,
        ICollection<WorkTypeDto> SupportedWorkTypes);
}