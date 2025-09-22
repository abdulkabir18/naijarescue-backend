using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.VerifyUserEmail
{
    public record VerifyUserEmailCommand (VerifyUserEmailRequestModel Model) : IRequest<Result<bool>>;
}
