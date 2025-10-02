using Application.Common.Dtos;
using Application.Features.Incidents.Dtos;
using Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Queries.GetIncidentById
{
    public class GetIncidentByIdQueryHandler : IRequestHandler<GetIncidentByIdQuery, Result<IncidentDto>>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ILogger<GetIncidentByIdQueryHandler> _logger;

        public GetIncidentByIdQueryHandler(IIncidentRepository incidentRepository, ILogger<GetIncidentByIdQueryHandler> logger)
        {
            _incidentRepository = incidentRepository;
            _logger = logger;
        }
        public Task<Result<IncidentDto>> Handle(GetIncidentByIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
