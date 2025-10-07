using Application.Common.Dtos;
using Application.Features.Incidents.Commands.CreateIncident;
using Application.Features.Incidents.Dtos;
using Application.Features.Incidents.Queries.GetIncidentById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class IncidentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IncidentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("report")]
        [HttpPost("panic-alert")]
        [SwaggerOperation(
            Summary = "Create a new incident",
            Description = "Creates a new incident. Authenticated users will be linked to the report; guests can report on behalf of victims."
        )]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Result<Guid>>> CreateIncident([FromForm] CreateIncidentCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }


        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get incident by ID",
            Description = "Retrieves the details of a specific incident by its unique identifier."
        )]
        [ProducesResponseType(typeof(Result<IncidentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<IncidentDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetIncidentByIdQuery(id));

            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }
    }
}
