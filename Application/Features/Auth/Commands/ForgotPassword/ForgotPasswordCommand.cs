using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;
}