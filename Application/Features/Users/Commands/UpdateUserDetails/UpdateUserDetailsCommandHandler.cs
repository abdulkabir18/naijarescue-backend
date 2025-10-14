using Application.Common.Dtos;
using Application.Common.Helpers;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.UpdateUserDetails
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, Result<Unit>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserDetailsCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserDetailsCommandHandler(ICurrentUserService currentUserService, IUserRepository userRepository, ILogger<UpdateUserDetailsCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Model == null)
                {
                    _logger.LogWarning("UpdateUserDetailsCommand received with null model.");
                    return Result<Unit>.Failure("Invalid user details provided.");
                }

                var userId = _currentUserService.UserId;
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("No user is currently logged in.");
                    return Result<Unit>.Failure("User not logged in.");
                }

                var user = await _userRepository.GetAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return Result<Unit>.Failure("User not found.");
                }

                string fullName = BuildUserFullName.BuildFullName(request.Model.FirstName, request.Model.LastName);
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    _logger.LogWarning("Invalid first name or last name provided.");
                    return Result<Unit>.Failure("First name and last name are required.");
                }
                user.UpdateFullName(fullName);

                if (string.IsNullOrEmpty(user.UserName) && string.IsNullOrEmpty(request.Model.UserName))
                {
                    _logger.LogWarning("Username is required for users without an existing username.");
                    return Result<Unit>.Failure("Username is required.");
                }
                else if (!string.IsNullOrEmpty(request.Model.UserName) && user.UserName != request.Model.UserName)
                {
                    bool isUsernameExist = await _userRepository.IsUserNameExistAsync(request.Model.UserName);
                    if (isUsernameExist)
                    {
                        _logger.LogWarning("Username {UserName} is already taken.", request.Model.UserName);
                        return Result<Unit>.Failure("Username is already taken.");
                    }
                    user.UpdateUserName(request.Model.UserName);
                }

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User details updated successfully for user {UserId}.", user.Id);
                return Result<Unit>.Success(Unit.Value, "User details updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the UpdateUserDetailsCommand.");
                return Result<Unit>.Failure("An unexpected error occurred.");
            }
        }
    }
}