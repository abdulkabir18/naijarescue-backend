using Application.Common.Validators;
using Application.Features.Incidents.Dtos;
using FluentValidation;

namespace Application.Features.Incidents.Validators
{
    public class CreateIncidentRequestDtoValidator : AbstractValidator<CreateIncidentRequestModel>
    {
        public CreateIncidentRequestDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid incident type.");

            RuleFor(x => x.Location)
                .NotNull().WithMessage("Location is required.")
                .SetValidator(new GeoLocationDtoValidator());

            RuleFor(x => x.OccurredAt)
                .NotEmpty().WithMessage("OccurredAt is required.")
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Occurred date cannot be in the future.");

            When(x => x.IsAnonymous, () =>
            {
                RuleFor(x => x.ReporterName)
                    .NotEmpty().WithMessage("Reporter name is required for anonymous reports.")
                    .MaximumLength(100);

                RuleFor(x => x.ReporterPhoneNumber)
                    .NotEmpty().WithMessage("Reporter phone number is required for anonymous reports.")
                    .Matches(@"^\+?[0-9]{8,18}$")
                    .WithMessage("Invalid phone number format.");

                RuleFor(x => x.ReporterEmail)
                    .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.ReporterEmail))
                    .WithMessage("Invalid reporter email format.");
            });

            When(x => x.IncidentMedias != null && x.IncidentMedias.Count > 0, () =>
            {
                RuleForEach(x => x.IncidentMedias!)
                    .SetValidator(new IncidentMediaDtoValidator());
            });
        }
    }
}
