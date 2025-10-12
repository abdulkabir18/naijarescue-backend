using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.SetProfileImage
{
    public class SetProfileImageCommandHandler : IRequestHandler<SetProfileImageCommand, Result<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageManager _storageManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SetProfileImageCommandHandler> _logger;

        public SetProfileImageCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IStorageManager storageManager, IUnitOfWork unitOfWork, ILogger<SetProfileImageCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _storageManager = storageManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(SetProfileImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Image == null)
                {
                    _logger.LogWarning("SetProfileImageCommand received with a null model.");
                    return Result<Unit>.Failure("SetProfileImageCommand received with a null model.");
                }
                else if (request.Image.Length >= 5 * 1024 * 1024)
                {
                    _logger.LogWarning("SetProfileImageCommand received with a image bigger than 5 MB");
                    return Result<Unit>.Failure("SetProfileImageCommand received with a image bigger than 5 MB");
                }

                Guid currentUserId = _currentUserService.UserId;
                if (currentUserId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthenticated user attempted to set profile image.");
                    return Result<Unit>.Failure("User is not authenticated.");
                }

                var user = await _userRepository.GetAsync(user => user.IsActive && user.Id == currentUserId && !user.IsDeleted);
                if (user == null)
                {
                    _logger.LogWarning("User not found.");
                    return Result<Unit>.Failure("User not found.");
                }

                using var stream = request.Image.OpenReadStream();
                string url = await _storageManager.UploadProfileImageAsync(stream, request.Image.FileName, request.Image.ContentType);

                user.SetProfilePicture(url);
                user.MarkUpdated();

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Profile image set successfully for user {UserId}.", user.Id);

                return Result<Unit>.Success(Unit.Value, "Profile image set successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred when user want to set profile image.");
                return Result<Unit>.Failure("An unexpected error occurred when user want to set profile image.");
            }
        }

    }
}