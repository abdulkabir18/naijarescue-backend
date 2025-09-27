using Application.Common.Validators;
using Application.Features.Responders.Dtos;
using Application.Features.Users.Validators;
using FluentValidation;

namespace Application.Features.Responders.Validators
{
    public class RegisterResponderRequestModelValidator : AbstractValidator<RegisterResponderRequestModel>
    {
        public RegisterResponderRequestModelValidator()
        {
            RuleFor(x => x.RegisterUserRequest)
                .NotNull().WithMessage("User registration details are required.")
                .SetValidator(new RegisterUserRequestModelValidator());

            RuleFor(x => x.AgencyId)
                .NotEmpty().WithMessage("AgencyId is required.");

            When(x => !string.IsNullOrWhiteSpace(x.BadgeNumber), () =>
            {
                RuleFor(x => x.BadgeNumber!)
                    .MaximumLength(50).WithMessage("Badge number must not exceed 50 characters.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.Rank), () =>
            {
                RuleFor(x => x.Rank!)
                    .MaximumLength(50).WithMessage("Rank must not exceed 50 characters.");
            });

            When(x => x.AssignedLocation != null, () =>
            {
                RuleFor(x => x.AssignedLocation!)
                    .SetValidator(new GeoLocationDtoValidator());
            });

            RuleFor(x => x.Capabilities)
                .NotNull().WithMessage("Responder capabilities are required.")
                .Must(c => c.Any()).WithMessage("At least one capability is required.")
                .Must(c => c.Select(x => x.AcceptedWorkType).Distinct().Count() == c.Count)
                .WithMessage("Duplicate work types are not allowed.");

            RuleForEach(x => x.Capabilities)
                .SetValidator(new WorkTypeDtoValidator());

            RuleFor(x => x.Specialties)
                .NotNull().WithMessage("Responder specialties are required.")
                .Must(s => s.Any()).WithMessage("At least one specialty is required.")
                .Must(s => s.Select(x => x.AcceptedIncidentType).Distinct().Count() == s.Count)
                .WithMessage("Duplicate incident types are not allowed.");

            RuleForEach(x => x.Specialties)
                .SetValidator(new IncidentTypeDtoValidator());
        }
    }
}
