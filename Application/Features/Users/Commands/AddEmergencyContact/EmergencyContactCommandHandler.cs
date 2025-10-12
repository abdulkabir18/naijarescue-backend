using Application.Common.Dtos;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.AddEmergencyContact
{
    public class EmergencyContactCommandHandler : IRequestHandler<EmergencyContactCommand, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmergencyContactCommandHandler> _logger;

        public EmergencyContactCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<EmergencyContactCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(EmergencyContactCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Model == null)
                {
                    _logger.LogWarning("Emergency contact model is null");
                    return Result<string>.Failure("Invalid emergency contact data.");
                }

                Guid userId = _currentUserService.UserId;
                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("User is not authenticated");
                    return Result<string>.Failure("User not authenticated.");
                }

                var user = await _userRepository.GetAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found", userId);
                    return Result<string>.Failure("User not found.");
                }

                bool emailExists = await _userRepository.IsEmergencyContactEmailExistAsync(userId, request.Model.Email);
                if (emailExists)
                {
                    _logger.LogWarning("Emergency contact email {Email} already exists for user {UserId}", request.Model.Email, userId);
                    return Result<string>.Failure("This email is already associated with an existing emergency contact.");
                }

                var contact = user.AddEmergencyContact(request.Model.Name, new Email(request.Model.Email), request.Model.Relationship, request.Model.OtherRelationship);

                if (!string.IsNullOrEmpty(request.Model.PhoneNumber))
                {
                    bool phoneExists = await _userRepository.IsEmergencyContactPhoneNumberExistAsync(userId, request.Model.PhoneNumber);
                    if (phoneExists)
                    {
                        _logger.LogWarning("Emergency contact phone number {PhoneNumber} already exists for user {UserId}", request.Model.PhoneNumber, userId);
                        return Result<string>.Failure("This phone number is already associated with an existing emergency contact.");
                    }

                    contact.SetPhoneNumber(new PhoneNumber(request.Model.PhoneNumber));
                }

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Emergency contact {ContactName} added successfully for user {UserId}", request.Model.Name, userId);

                return Result<string>.Success(contact.Email, "Emergency contact added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding emergency contact");
                return Result<string>.Failure("An error occurred while adding the emergency contact.");
            }
        }
    }
}