using Application.Common.Dtos;
using Application.Features.Auth.Dtos;
using MediatR;

namespace Application.Features.Auth.Commands.LoginUser
{
    public record LoginUserCommand(LoginRequestModel Model) : IRequest<Result<LoginResponseModel>>;
}
