using Application.Common.Constants;
using Application.Common.Dtos;
using FluentValidation;

namespace Application.Common.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(x => x)
                .Must(AllOrNoneProvided)
                .WithMessage("If any address field is provided, all address fields must be provided.");

            RuleFor(x => x.Street)
                .MinimumLength(3).WithMessage("Street must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Street cannot exceed 100 characters.");

            RuleFor(x => x.State)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.City))
                .WithMessage("State is required.")
                .Must(s => NigeriaData.States.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Invalid state. Must be one of Nigeria's 36 states or FCT.");

            RuleFor(x => x.City)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.Street))
                .WithMessage("City is required.")
                .DependentRules(() =>
                {
                    RuleFor(x => x.City)
                        .Must((dto, city) =>
                            NigeriaData.Cities.TryGetValue(dto.State, out var cities) &&
                            cities.Any(c => c.Equals(city, StringComparison.OrdinalIgnoreCase)))
                        .WithMessage(ci => $"City '{ci.City}' is not valid for state '{ci.State}'.");
                });

            RuleFor(x => x.PostalCode)
                .Matches(@"^\d{6}$").When(x => !string.IsNullOrEmpty(x.PostalCode))
                .WithMessage("Postal code must be 6 digits.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .Must(c => string.Equals(c?.Trim(), "Nigeria", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(c?.Trim(), "NG", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Country must be Nigeria (or NG).");
        }

        private bool AllOrNoneProvided(AddressDto dto)
        {
            var fields = new[]
            {
                dto.Street,
                dto.City,
                dto.State,
                dto.Country,
                dto.PostalCode
            };

            bool anyProvided = fields.Any(f => !string.IsNullOrWhiteSpace(f));
            bool allProvided = fields.All(f => !string.IsNullOrWhiteSpace(f));

            return !anyProvided || allProvided;
        }
    }
}
