﻿using Application.Common.Dtos;
using Application.Features.Agencies.Commands.RegisterAgency;
using Application.Features.Agencies.Dtos;
using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.LoginUser;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Dtos;
using Application.Features.Responders.Commands.RegisterResponder;
using Application.Features.Responders.Dtos;
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
        //public async Task<ActionResult<Result<Guid>>> RegisterAgency([FromForm] RegisterAgencyRequestModel model, [FromBody] IncidentWorkTypesDto typesDto)
        //{
        //    //var typesDto = new IncidentWorkTypesDto();
        //    var command = new RegisterAgencyCommand(model, typesDto);
        //    var result = await _mediator.Send(command);

        //    if (!result.Succeeded)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        public async Task<ActionResult<Result<Guid>>> RegisterAgency([FromForm] RegisterAgencyFullRequestModel model)
        {
            var typesDto = new IncidentWorkTypesDto
            {
                SupportedIncidents = model.IncidentTypesEnums.Select(a => new IncidentTypeDto
                { AcceptedIncidentType = a }).ToList(),

                SupportedWorkTypes = model.WorkTypesEnums.Select(b => new WorkTypeDto
                { AcceptedWorkType = b }).ToList()
            };

            var commandModel = new RegisterAgencyRequestModel(
                model.RegisterUserRequest,
                model.AgencyName,
                model.AgencyEmail,
                model.AgencyPhoneNumber,
                model.AgencyLogo,
                model.AgencyAddress
            );

            var command = new RegisterAgencyCommand(commandModel, typesDto);
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }



        //[HttpPost("add")]
        //public IActionResult Add([FromBody] YourModel value)
        //{
        //    foreach (var item in value.SupportedIncidents)
        //    {
        //        Console.WriteLine("Incident: " + item.AcceptedIncidentType.ToString());
        //    }

        //    foreach (var item in value.SupportedWorkTypes)
        //    {
        //        Console.WriteLine("Work: " + item.AcceptedWorkType.ToString());
        //    }

        //    return Ok();
        //}

        //public async Task<ActionResult<Result<Guid>>> RegisterAgency([FromForm] RegisterAgencyRequestModel model, [FromForm] string supportedIncidentsJson, [FromForm] string supportedWorkTypesJson)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };
        //    options.Converters.Add(new JsonStringEnumConverter());

        //    var supportedIncidents = JsonSerializer.Deserialize<List<IncidentTypeDto>>(supportedIncidentsJson, options);
        //    var supportedWorkTypes = JsonSerializer.Deserialize<List<WorkTypeDto>>(supportedWorkTypesJson, options);

        //    model.SupportedIncidents = supportedIncidents ?? new List<IncidentTypeDto>();
        //    model.SupportedWorkTypes = supportedWorkTypes ?? new List<WorkTypeDto>();

        //    var command = new RegisterAgencyCommand(model);
        //    var result = await _mediator.Send(command);

        //    if (!result.Succeeded)
        //        return BadRequest(result);

        //    return Ok(result);
        //}


        //public async Task<ActionResult<Result<Guid>>> RegisterAgency(
        //[FromForm] RegisterAgencyRequestModel model,
        //[JsonProperty] string supportedIncidentsJson,
        //[JsonProperty] string supportedWorkTypesJson)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };
        //    options.Converters.Add(new JsonStringEnumConverter());

        //    try
        //    {
        //        model.SupportedIncidents = JsonSerializer.Deserialize<List<IncidentTypeDto>>(supportedIncidentsJson, options) ?? new();
        //        model.SupportedWorkTypes = JsonSerializer.Deserialize<List<WorkTypeDto>>(supportedWorkTypesJson, options) ?? new();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(Result<Guid>.Failure($"Invalid JSON format: {ex.Message}"));
        //    }

        //    var command = new RegisterAgencyCommand(model);
        //    var result = await _mediator.Send(command);

        //    if (!result.Succeeded)
        //        return BadRequest(result);

        //    return Ok(result);
        //}


        [Authorize(Roles = "SuperAdmin, AgencyAdmin")]
        [HttpPost("register-responder")]
        [SwaggerOperation(
            Summary = "Register a new responder",
            Description = "Allows a SuperAdmin or AgencyAdmin to register a new responder."
        )]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<Guid>>> RegisterResponder([FromForm] RegisterResponderFullRequestModel model)
        {
            var typesDto = new IncidentWorkTypesDto
            {
                SupportedIncidents = model.SpecialtiesEnums.Select(a => new IncidentTypeDto
                { AcceptedIncidentType = a }).ToList(),

                SupportedWorkTypes = model.CapabilitiesEnums.Select(b => new WorkTypeDto
                { AcceptedWorkType = b }).ToList()
            };

            var commandModel = new RegisterResponderRequestModel(
                model.RegisterUserRequest,
                model.AgencyId,
                model.BadgeNumber,
                model.Rank,
                model.AssignedLocation
            );

            var command = new RegisterResponderCommand(commandModel, typesDto);
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

        [HttpPost("forgot-password")]
        [SwaggerOperation(
            Summary = "Forgot password",
            Description = "Sends a password reset verification code to the user's registered email address."
        )]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(
            Summary = "Reset password (token/OTP)",
            Description = "Resets a user's password using a verification code (OTP) sent to their email. Provide Email, Code and the new password."
        )]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<bool>>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }
    }

    //public class YourModel
    //{
    //    public List<IncidentTypeDto> SupportedIncidents { get; set; }
    //    public List<WorkTypeDto> SupportedWorkTypes { get; set; }
    //}
}