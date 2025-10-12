using Application.Features.Users.Dtos;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public class EmergencyContactRequestModelValidator : AbstractValidator<EmergencyContactRequestModel>
    {
        public EmergencyContactRequestModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Emergency contact name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Name can only contain letters, spaces, hyphens, and apostrophes.");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(\+234|0)\d{10}$").WithMessage("Please provide a valid Nigerian phone number.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Relationship)
                .IsInEnum().WithMessage("A valid relationship type is required.");

            RuleFor(x => x.OtherRelationship)
                .NotEmpty().WithMessage("Please specify the relationship.")
                .When(x => x.Relationship == RelationshipType.Other);
        }
    }
}
