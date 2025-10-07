using Application.Features.Incidents.Dtos;
using FluentValidation;

namespace Application.Features.Incidents.Validators
{
    public class VictimDetailsDtoValidator : AbstractValidator<VictimDetailsDto>
    {
        public VictimDetailsDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Victim name is required.")
                .MinimumLength(2).WithMessage("Victim name must be at least 2 characters.")
                .MaximumLength(100).WithMessage("Victim name cannot exceed 100 characters.")
                .Matches(@"^[\p{L}\s'-]+$").WithMessage("Victim name contains invalid characters.");

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Incident description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^(?:\+234|0)[789][01]\d{8}$")
                .WithMessage("Phone number must be a valid Nigerian number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}
