using Application.Common.Validators;
using Application.Features.Incidents.Dtos;
using FluentValidation;

namespace Application.Features.Incidents.Validators
{
    public class CreateIncidentRequestModelValidator : AbstractValidator<CreateIncidentRequestModel>
    {
        public CreateIncidentRequestModelValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Incident type is required.");

            RuleFor(x => x.Location)
                .NotNull().WithMessage("Location is required.")
                .SetValidator(new GeoLocationDtoValidator());

            RuleFor(x => x.OccurredAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-15)).WithMessage("OccurredAt must be within the last 15 minutes and cannot be a future time.");

            RuleFor(x => x.Address)
                .SetValidator(new AddressDtoValidator()!)
                .When(x => x.Address != null);

            When(x => x.IncidentMedias?.Files?.Any() == true, () =>
            {
                RuleFor(x => x.IncidentMedias).SetValidator(new IncidentMediaDtoValidator()!);
            });

            When(x => x.IsReportingForAnotherPerson, () =>
            {
                RuleFor(x => x.ReporterDetails)
                    .NotNull().WithMessage("Reporter details are required when reporting for another person.")
                    .SetValidator(new ReporterDetailsDtoValidator()!);

                RuleFor(x => x.VictimDetails)
                    .NotNull().WithMessage("Victim details are required when reporting for another person.")
                    .SetValidator(new VictimDetailsDtoValidator()!);
            });

            When(x => !x.IsAuthenticatedUser, () =>
            {
                RuleFor(x => x.VictimDetails)
                    .NotNull().WithMessage("Victim details are required for anonymous reports.")
                    .SetValidator(new VictimDetailsDtoValidator()!);
            });
        }
    }
}
