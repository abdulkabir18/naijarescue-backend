using Application.Common.Dtos;
using FluentValidation;

namespace Application.Common.Validators
{
    public class WorkTypeDtoValidator : AbstractValidator<WorkTypeDto>
    {
        public WorkTypeDtoValidator()
        {
            RuleFor(x => x.AcceptedWorkType)
                .IsInEnum().WithMessage("Invalid work type.");
        }
    }
}
