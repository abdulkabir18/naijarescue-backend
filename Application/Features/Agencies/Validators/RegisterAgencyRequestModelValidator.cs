using Application.Common.Validators;
using Application.Features.Agencies.Dtos;
using Application.Features.Users.Validators;
using FluentValidation;

namespace Application.Features.Agencies.Validators
{
    public class RegisterAgencyRequestModelValidator : AbstractValidator<RegisterAgencyRequestModel>
    {
        public RegisterAgencyRequestModelValidator()
        {
            RuleFor(x => x.RegisterUserRequest)
                .NotNull().WithMessage("User registration details are required.")
                .SetValidator(new RegisterUserRequestModelValidator());

            RuleFor(x => x.AgencyName)
                .NotEmpty().WithMessage("Agency name is required.")
                .MaximumLength(100).WithMessage("Agency name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z0-9\s\-\&]+$").WithMessage("Agency name can only contain letters, numbers, spaces, hyphens, and ampersands.");

            RuleFor(x => x.AgencyEmail)
                .NotEmpty().WithMessage("Agency email is required.")
                .NotEqual(x => x.RegisterUserRequest.Email).WithMessage("Agency email must be different from user email.")
                .EmailAddress().WithMessage("Invalid agency email format.");


            RuleFor(x => x.AgencyPhoneNumber)
                .NotEmpty().WithMessage("Agency phone number is required.")
                .Matches(@"^(?:\+234|0)[789][01]\d{8}$")
                .WithMessage("Agency phone number must be a valid Nigerian number.");

            When(x => x.AgencyLogo != null, () =>
            {
                RuleFor(x => x.AgencyLogo!.Length)
                    .LessThanOrEqualTo(5 * 1024 * 1024)
                    .WithMessage("Agency logo size cannot exceed 5MB.");

                RuleFor(x => x.AgencyLogo!.ContentType.ToLower())
                    .Must(type => type == "image/jpeg" || type == "image/jpg" || type == "image/png")
                    .WithMessage("Only JPEG, JPG, and PNG images are allowed.");
            });

            When(x => x.AgencyAddress != null, () =>
            {
                RuleFor(x => x.AgencyAddress!)
                    .SetValidator(new AddressDtoValidator());
            });

            RuleFor(x => x.SupportedIncidents)
                .NotNull().WithMessage("At least one supported incident type is required.")
                .Must(list => list.Any()).WithMessage("At least one supported incident type must be provided.")
                .Must(c => c.Select(x => x.AcceptedIncidentType).Distinct().Count() == c.Count)
                .WithMessage("Duplicate incident types are not allowed.");

            RuleForEach(x => x.SupportedIncidents)
                .SetValidator(new IncidentTypeDtoValidator());

            RuleFor(x => x.SupportedWorkTypes)
                .NotNull().WithMessage("At least one supported work type is required.")
                .Must(list => list.Any()).WithMessage("At least one supported work type must be provided.")
                .Must(c => c.Select(x => x.AcceptedWorkType).Distinct().Count() == c.Count)
                .WithMessage("Duplicate work types are not allowed.");

            RuleForEach(x => x.SupportedWorkTypes)
                .SetValidator(new WorkTypeDtoValidator());
        }
    }
}
