using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Comands.RegisterUser
{
    public record RegisterUserCommand(RegisterUserRequestModel Model) : IRequest<Result<Guid>>;
}
