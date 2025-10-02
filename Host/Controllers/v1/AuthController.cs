using Application.Common.Dtos;
using Application.Features.Agencies.Commands.RegisterAgency;
using Application.Features.Auth.Commands.LoginUser;
using Application.Features.Auth.Dtos;
using Application.Features.Responders.Commands.RegisterResponder;
using Application.Features.Users.Commands.RegisterUser;
using Application.Features.Users.Commands.VerifyUserEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("signup")]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "Creates a new user account in the system."
        )]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<Guid>>> Signup([FromForm] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("register-agency")]
        [SwaggerOperation(
            Summary = "Register a new agency",
            Description = "Allows a SuperAdmin to register a new agency."
        )]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<Guid>>> RegisterAgency([FromForm] RegisterAgencyCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "SuperAdmin, AgencyAdmin")]
        [HttpPost("register-responder")]
        [SwaggerOperation(
            Summary = "Register a new responder",
            Description = "Allows a SuperAdmin or AgencyAdmin to register a new responder."
        )]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<Guid>>> RegisterResponder([FromForm] RegisterResponderCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Authenticates a user and returns a JWT token."
        )]
        [ProducesResponseType(typeof(Result<LoginResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<LoginResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<LoginResponseModel>>> Login([FromBody] LoginUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("verify-email")]
        [SwaggerOperation(
            Summary = "Verify user email",
            Description = "Verifies a user's email address using a verification code."
        )]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> VerifyEmail([FromBody] VerifyUserEmailCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
