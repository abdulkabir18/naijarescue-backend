using Application.Common.Dtos;
using Application.Features.Auth.Commands.LoginUser;
using Application.Features.Auth.Dtos;
using Application.Features.Users.Comands.RegisterUser;
using Application.Features.Users.Comands.VerifyUserEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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


        [HttpPost("register")]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<Guid>>> Register([FromForm] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpPost("login")]
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
