using Application.Common.Dtos;
using Application.Common.Helpers;
using Application.Interfaces.CurrentUser;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Common.Security;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IStorageManager _storageManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IStorageManager storageManager, IUnitOfWork unitOfWork, IAuditLogRepository auditLogRepository, ICurrentUserService currentUserService, ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _storageManager = storageManager;
            _unitOfWork = unitOfWork;
            _auditLogRepository = auditLogRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting user registration process for email {Email}.", request.Model?.Email);

                if (request.Model == null)
                {
                    _logger.LogWarning("RegisterUserCommand received with a null model.");
                    return Result<Guid>.Failure("Invalid request payload.");
                }

                if (await _userRepository.IsEmailExistAsync(request.Model.Email))
                    return Result<Guid>.Failure($"Email {request.Model.Email} is associated with another account.");

                if (await _userRepository.IsPhoneNumberExistAsync(request.Model.PhoneNumber))
                    return Result<Guid>.Failure($"PhoneNumber {request.Model.PhoneNumber} is associated with another account.");

                string fullName = BuildUserFullName.BuildFullName(request.Model.FirstName, request.Model.LastName);
                if (string.IsNullOrEmpty(fullName)) return Result<Guid>.Failure("Name validation failed");

                var user = new User(fullName, new Email(request.Model.Email), new PhoneNumber(request.Model.PhoneNumber), request.Model.Gender);

                user.SetPassword($"{request.Model.Password}{user.Id}", _passwordHasher);
                user.SetCreatedBy(user.Id.ToString());

                if (!string.IsNullOrEmpty(request.Model.UserName))
                {
                    bool isUserNameExist = await _userRepository.IsUserNameExistAsync(request.Model.UserName);
                    if (isUserNameExist)
                        return Result<Guid>.Failure("The UserName is in use.");

                    user.SetUserName(request.Model.UserName);
                }

                if (request.Model.Address != null)
                {
                    user.SetAddress(new Address(request.Model.Address.Street!, request.Model.Address.City!, request.Model.Address.State!, request.Model.Address.LGA!, request.Model.Address.Country!, request.Model.Address.PostalCode!));
                }

                if (request.Model.ProfilePicture != null)
                {
                    using var stream = request.Model.ProfilePicture.OpenReadStream();
                    string url = await _storageManager.UploadProfileImageAsync(stream, request.Model.ProfilePicture.FileName, request.Model.ProfilePicture.ContentType);

                    user.SetProfilePicture(url);
                }

                await _userRepository.AddAsync(user);

                var auditLog = new AuditLog(
                    Domain.Enums.AuditActionType.Created,
                    nameof(User),
                    user.Id,
                    $"User '{user.FullName}' registered successfully.",
                    user.Id,
                    _currentUserService.IpAddress,
                    _currentUserService.UserAgent
                );
                await _auditLogRepository.AddAsync(auditLog);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User {UserId} created successfully.", user.Id);
                return Result<Guid>.Success(user.Id, "User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during user registration for email {Email}.", request.Model?.Email);
                return Result<Guid>.Failure("An unexpected error occurred during registration.");
            }
        }
    }
}
