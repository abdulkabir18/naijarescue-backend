using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(string Email, string Code, string NewPassword, string ConfirmPassword) : IRequest<Result<bool>>;
}