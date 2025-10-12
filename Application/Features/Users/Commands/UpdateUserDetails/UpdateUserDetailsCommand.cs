using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUserDetails
{
    public record UpdateUserDetailsCommand(UpdateUserDetailsRequestModel Model) : IRequest<Result<Unit>>;
}