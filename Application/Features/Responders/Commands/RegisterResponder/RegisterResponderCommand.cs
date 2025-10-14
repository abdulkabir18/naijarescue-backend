using Application.Common.Dtos;
using Application.Features.Agencies.Dtos;
using Application.Features.Responders.Dtos;
using MediatR;

namespace Application.Features.Responders.Commands.RegisterResponder
{
    public record RegisterResponderCommand (RegisterResponderRequestModel Model, IncidentWorkTypesDto IncidentWorkTypes) : IRequest<Result<Guid>>;
}