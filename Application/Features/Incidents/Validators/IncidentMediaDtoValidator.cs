using Application.Features.Incidents.Dtos;
using FluentValidation;

namespace Application.Features.Incidents.Validators
{
    public class IncidentMediaDtoValidator : AbstractValidator<IncidentMediaDto>
    {
        private static readonly string[] ImageTypes = ["image/jpeg", "image/png", "image/jpg", "image/gif"];
        private static readonly string[] VideoTypes = ["video/mp4", "video/quicktime", "video/x-msvideo"];
        private static readonly string[] AudioTypes = ["audio/mpeg", "audio/wav", "audio/mp3"];

        public IncidentMediaDtoValidator()
        {
            When(x => x.Files != null && x.Files.Any(), () =>
            {
                RuleForEach(x => x.Files).ChildRules(file =>
                {
                    file.RuleFor(f => f.Length).GreaterThan(0).WithMessage("File cannot be empty.");
                    file.RuleFor(f => f.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size must not exceed 10MB.");
                    file.RuleFor(f => f.ContentType).Must(BeAValidContentType).WithMessage("Invalid or unsupported file type.");
                });
            });
        }

        private bool BeAValidContentType(string contentType)
        {
            contentType = contentType.ToLowerInvariant();

            return ImageTypes.Contains(contentType) ||
                   VideoTypes.Contains(contentType) ||
                   AudioTypes.Contains(contentType);
        }
    }
}