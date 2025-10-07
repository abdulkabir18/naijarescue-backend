using Application.Features.Incidents.Dtos;
using FluentValidation;

namespace Application.Features.Incidents.Validators
{
    public class ReporterDetailsDtoValidator : AbstractValidator<ReporterDetailsDto>
    {
        public ReporterDetailsDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Reporter name is required.")
                .MinimumLength(2).WithMessage("Reporter name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Reporter name cannot exceed 100 characters.")
                .Matches(@"^[\p{L}\s'-]+$").WithMessage("Reporter name contains invalid characters.");

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(?:\+234|0)[789][01]\d{8}$")
                .WithMessage("Phone number must be a valid Nigerian number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            // RuleFor(x => x.PhoneNumber)
            //     .NotEmpty().WithMessage("Phone number is required.");
        }
    }
}
