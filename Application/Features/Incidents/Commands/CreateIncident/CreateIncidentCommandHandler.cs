using Application.Common.Dtos;
using Application.Common.Helpers;
using Application.Features.Incidents.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Incidents.Commands.CreateIncident
{
    public class CreateIncidentCommandHandler : IRequestHandler<CreateIncidentCommand, Result<CreateIncidentResponseDto>>
    {
        private readonly IIncidentRepository _incidentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageManager _storageManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateIncidentCommandHandler> _logger;

        public CreateIncidentCommandHandler(IIncidentRepository incidentRepository, ICurrentUserService currentUserService, IStorageManager storageManager, IUnitOfWork unitOfWork, ILogger<CreateIncidentCommandHandler> logger)
        {
            _incidentRepository = incidentRepository;
            _currentUserService = currentUserService;
            _storageManager = storageManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CreateIncidentResponseDto>> Handle(CreateIncidentCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            // _logger.LogInformation("Handling CreateIncidentCommand for user {UserId}, anonymous: {IsAnonymous}", _currentUserService.UserId, request.Model.IsAnonymous);

            // if (request.Model == null)
            // {
            //     _logger.LogWarning("CreateIncidentCommand received with null model.");
            //     return Result<CreateIncidentResponseDto>.Failure("Invalid incident data.");
            // }

            // Guid currentUserId = _currentUserService.UserId;

            // if (!Enum.IsDefined(typeof(IncidentType), request.Model.Type))
            // {
            //     _logger.LogWarning("Invalid IncidentType: {IncidentType}", request.Model.Type);
            //     return Result<CreateIncidentResponseDto>.Failure($"IncidentType '{request.Model.Type}' is not valid.");
            // }

            // if (currentUserId == Guid.Empty && !request.Model.IsAnonymous)
            // {
            //     _logger.LogWarning("Unauthenticated user tried to create non-anonymous incident.");
            //     return Result<CreateIncidentResponseDto>.Failure("User must be authenticated to create a non-anonymous incident.");
            // }

            // var incident = new Incident(request.Model.Type, new GeoLocation(request.Model.Location.Latitude, request.Model.Location.Longitude), request.Model.OccurredAt, request.Model.IsAnonymous, currentUserId == Guid.Empty ? null : currentUserId);

            // if (request.Model.IsAnonymous)
            // {
            //     if (string.IsNullOrWhiteSpace(request.Model.ReporterName))
            //     {
            //         _logger.LogWarning("Anonymous incident missing reporter name.");
            //         return Result<CreateIncidentResponseDto>.Failure("Reporter name is required for anonymous incidents.");
            //     }
            //     if (string.IsNullOrWhiteSpace(request.Model.ReporterEmail) || string.IsNullOrWhiteSpace(request.Model.ReporterPhoneNumber))
            //     {
            //         _logger.LogWarning("Anonymous incident missing reporter email or phone.");
            //         return Result<CreateIncidentResponseDto>.Failure("Phonenumber and email is required for anonymous incidents.");
            //     }

            //     incident.SetReporterDetails(request.Model.ReporterName, new PhoneNumber(request.Model.ReporterPhoneNumber), new Email(request.Model.ReporterEmail));
            //     incident.SetCreatedBy(request.Model.ReporterEmail);
            // }
            // else
            // {
            //     incident.SetCreatedBy(currentUserId.ToString());
            // }

            // if (request.Model.IncidentMedias != null && request.Model.IncidentMedias.Count > 0)
            // {
            //     var uploadedMediaUrls = new List<string>();

            //     foreach (var mediaDto in request.Model.IncidentMedias)
            //     {
            //         var file = mediaDto.File;
            //         MediaType mediaType = MediaTypeMapper.MapContentType(file.ContentType);
            //         if (!Enum.IsDefined(typeof(MediaType), mediaType))
            //         {
            //             _logger.LogWarning("Unsupported media type: {ContentType}", file.ContentType);
            //             return Result<CreateIncidentResponseDto>.Failure($"Unsupported file type: {file.ContentType}");
            //         }
            //         try
            //         {
            //             var incidentFileUrl = await _storageManager.UploadMediaAsync(file.OpenReadStream(), file.FileName, file.ContentType);
            //             uploadedMediaUrls.Add(incidentFileUrl);
            //             incident.AddMedia(incidentFileUrl, mediaType);
            //             _logger.LogInformation("Uploaded media file {FileName} for incident.", file.FileName);
            //         }
            //         catch (Exception ex)
            //         {
            //             _logger.LogError(ex, "Failed to upload media file {FileName}", file.FileName);

            //             foreach (var url in uploadedMediaUrls)
            //             {
            //                 try
            //                 {
            //                     await _storageManager.DeleteMediaAsync(url);
            //                     _logger.LogInformation("Deleted uploaded media file {Url} due to error.", url);
            //                 }
            //                 catch (Exception deleteEx)
            //                 {
            //                     _logger.LogError(deleteEx, "Failed to delete uploaded media file {Url}", url);
            //                 }
            //             }

            //             return Result<CreateIncidentResponseDto>.Failure($"Failed to upload media: {ex.Message}");
            //         }
            //     }
            // }

            // await _incidentRepository.AddAsync(incident);
            // await _unitOfWork.SaveChangesAsync(cancellationToken);

            // _logger.LogInformation("Incident {IncidentId} created successfully.", incident.Id);

            // return Result<CreateIncidentResponseDto>.Success(new CreateIncidentResponseDto(incident.Id, incident.Type, incident.OccurredAt, incident.IsAnonymous, incident.ReporterName, incident.ReporterPhoneNumber?.Value, incident.ReporterEmail?.Value, incident.Status.ToString(), incident.CreatedAt), "Incident created successfully.");
        }
    }
}
