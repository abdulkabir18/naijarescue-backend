using Application.Common.Dtos;
using Application.Features.Incidents.Dtos;
using MediatR;

namespace Application.Features.Incidents.Commands.AcceptIncident
{
    public record AcceptIncidentCommand(AcceptIncidentRequestModel Model) : IRequest<Result<Guid>>;
}