using Application.Common.Dtos;
using Application.Features.Users.Dtos;
using MediatR;

namespace Application.Features.Users.Commands.AddEmergencyContact
{
    public record EmergencyContactCommand(EmergencyContactRequestModel Model) : IRequest<Result<string>>;
}