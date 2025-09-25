using MediatR;
using Application.Common.Dtos;
using Application.Features.Responders.Dtos;

namespace Application.Features.Responders.Commands.RegisterResponder
{
    public record RegisterResponderCommand (RegisterResponderRequestModel Model) : IRequest<Result<Guid>>;
}