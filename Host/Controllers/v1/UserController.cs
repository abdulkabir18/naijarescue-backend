using Application.Common.Dtos;
using Application.Features.Users.Commands.AddEmergencyContact;
using Application.Features.Users.Commands.SetProfileImage;
using Application.Features.Users.Commands.UpdateUserDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Host.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [SwaggerOperation(
            Summary = "Upload or update the current user's profile image.",
            Description = "Requires authentication. The request must be `multipart/form-data` and include an image file under the 'image' key."
        )]
        [Authorize]
        [HttpPatch("profile-image")]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result<Unit>>> SetProfileImage([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(Result<Unit>.Failure("An image file is required."));
            }

            var result = await _mediator.Send(new SetProfileImageCommand(image));

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [SwaggerOperation(
            Summary = "Update the current user's details.",
            Description = "Requires authentication. Update fields like first name, last name, and username."
        )]
        [Authorize]
        [HttpPatch("details")]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Unit>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result<Unit>>> UpdateUserDetails([FromBody] UpdateUserDetailsCommand command)
        {
            if (command == null || command.Model == null)
            {
                return BadRequest(Result<Unit>.Failure("Invalid user details provided."));
            }

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [SwaggerOperation(
            Summary = "Add an emergency contact for the current user.",
            Description = "Requires authentication. Provide contact details including name, email, relationship, and optionally phone number."
        )]
        [Authorize]
        [HttpPost("emergency-contact")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Result<string>>> AddEmergencyContact([FromBody] EmergencyContactCommand command)
        {
            if (command == null || command.Model == null)
            {
                return BadRequest(Result<string>.Failure("Invalid emergency contact data."));
            }

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}