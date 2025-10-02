using Application.Features.Incidents.Dtos;
using Domain.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Incidents.Validators
{
    public class IncidentMediaDtoValidator : AbstractValidator<IncidentMediaDto>
    {
        private static readonly string[] ImageTypes = ["image/jpeg", "image/png", "image/jpg", "image/gif"];
        private static readonly string[] VideoTypes = ["video/mp4", "video/quicktime", "video/x-msvideo"];
        private static readonly string[] AudioTypes = ["audio/mpeg", "audio/wav", "audio/mp3"];

        public IncidentMediaDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
                .Must(BeAValidContentType).WithMessage("Invalid or unsupported file type.")
                .Must(f => f.Length <= 10 * 1024 * 1024).WithMessage("File size must not exceed 10MB.");
        }

        private bool BeAValidContentType(IFormFile file)
        {
            var contentType = file.ContentType.ToLowerInvariant();

            return ImageTypes.Contains(contentType) ||
                   VideoTypes.Contains(contentType) ||
                   AudioTypes.Contains(contentType);
        }
    }
}