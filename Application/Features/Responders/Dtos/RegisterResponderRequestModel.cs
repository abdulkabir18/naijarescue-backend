using System;
using Application.Features.Users.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Dtos;

namespace Application.Features.Responders.Dtos
{
    public record RegisterResponderRequestModel (RegisterUserRequestModel RegisterUserRequest, Guid AgencyId, string? BadgeNumber, string? Rank, GeoLocationDto? AssignedLocation);
}