using MediatR;
using Application.Common.Dtos;
using Application.Interfaces.Repositories;
using Application.Interfaces.CurrentUser;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.Enums;
using Domain.Common.Security;
using Application.Interfaces.External;
using Application.Interfaces.UnitOfWork;

namespace Application.Features.Responders.Commands.RegisterResponder
{
    public class RegisterResponderCommandHandler : IRequestHandler<RegisterResponderCommand, Result<Guid>>
    {
        private readonly IResponderRepository _responderRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IStorageManager _storageManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterResponderCommandHandler(IResponderRepository responderRepository, ICurrentUserService currentUserService, IUserRepository userRepository, IAgencyRepository agencyRepository, IPasswordHasher passwordHasher, IStorageManager storageManager, IUnitOfWork unitOfWork)
        {
            _responderRepository = responderRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _agencyRepository = agencyRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _storageManager = storageManager;
        }

        private static string Capitalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return char.ToUpper(input[0]) + input[1..].ToLower();
        }

        private static string BuildFullName(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) return string.Empty;

            return $"{Capitalize(firstName)} {Capitalize(lastName)}";
        }

        public async Task<Result<Guid>> Handle(RegisterResponderCommand request, CancellationToken cancellationToken)
        {
            if (request.Model == null)
                return Result<Guid>.Failure("Invalid request payload.");

            Guid currentUserId = _currentUserService.UserId;
            if (currentUserId == Guid.Empty)
                return Result<Guid>.Failure("User is not authenticated.");

            Task<bool> isEmailExist = _userRepository.IsEmailExistAsync(request.Model.RegisterUserRequest.Email);
            Task<bool> isPhoneNumberExist = _userRepository.IsPhoneNumberExistAsync(request.Model.RegisterUserRequest.PhoneNumber);
            Task<bool> isAgencyExist = _agencyRepository.IsAgencyExist(request.Model.AgencyId);

            await Task.WhenAll(isEmailExist, isPhoneNumberExist, isAgencyExist);

            if (!await isAgencyExist)
                return Result<Guid>.Failure("Agency does not exist.");

            if (await isEmailExist)
                return Result<Guid>.Failure($"Email {request.Model.RegisterUserRequest.Email} already exists.");

            if (await isPhoneNumberExist)
                return Result<Guid>.Failure($"PhoneNumber {request.Model.RegisterUserRequest.PhoneNumber} already exists.");

            string fullName = BuildFullName(request.Model.RegisterUserRequest.FirstName, request.Model.RegisterUserRequest.LastName);
            if (string.IsNullOrEmpty(fullName)) return Result<Guid>.Failure("Name validation failed");

            var user = new User(fullName, new Email(request.Model.RegisterUserRequest.Email), new PhoneNumber(request.Model.RegisterUserRequest.PhoneNumber), request.Model.RegisterUserRequest.Gender, UserRole.Responder);

            user.SetPassword($"{request.Model.RegisterUserRequest.Password}{user.Id}", _passwordHasher);

            if (!string.IsNullOrEmpty(request.Model.RegisterUserRequest.UserName))
            {
                if (await _userRepository.IsUserNameExistAsync(request.Model.RegisterUserRequest.UserName))
                    return Result<Guid>.Failure("The UserName is in use.");

                user.SetUserName(request.Model.RegisterUserRequest.UserName);
            }

            if (request.Model.RegisterUserRequest.Address != null)
            {
                user.SetAddress(new Address(request.Model.RegisterUserRequest.Address.Street!, request.Model.RegisterUserRequest.Address.City!, request.Model.RegisterUserRequest.Address.State!, request.Model.RegisterUserRequest.Address.LGA!, request.Model.RegisterUserRequest.Address.Country!, request.Model.RegisterUserRequest.Address.PostalCode!));
            }

            if (request.Model.RegisterUserRequest.ProfilePicture != null)
            {
                using var stream = request.Model.RegisterUserRequest.ProfilePicture.OpenReadStream();
                string imageUrl = await _storageManager.UploadProfileImageAsync(stream, request.Model.RegisterUserRequest.ProfilePicture.FileName, request.Model.RegisterUserRequest.ProfilePicture.ContentType);

                user.SetProfilePicture(imageUrl);
            }

            user.SetCreatedBy(currentUserId.ToString());

            var responder = new Responder(user.Id, request.Model.AgencyId);

            if (request.Model.AssignedLocation != null)
                responder.AssignLocation(new GeoLocation(request.Model.AssignedLocation.Latitude, request.Model.AssignedLocation.Longitude));

            if (!string.IsNullOrEmpty(request.Model.BadgeNumber))
                responder.SetBadgeNumber(request.Model.BadgeNumber);

            if (!string.IsNullOrEmpty(request.Model.Rank))
                responder.SetRank(request.Model.Rank);

            foreach (var capability in request.Model.Capabilities)
            {
                if (!Enum.IsDefined(typeof(WorkType), capability.AcceptedWorkType))
                    return Result<Guid>.Failure($"Invalid work type: {capability.AcceptedWorkType}");

                responder.AddCapability(capability.AcceptedWorkType, responder.Id);
            }

            foreach (var specialty in request.Model.Specialties)
            {
                if (!Enum.IsDefined(typeof(IncidentType), specialty.AcceptedIncidentType))
                    return Result<Guid>.Failure($"Invalid incident type: {specialty.AcceptedIncidentType}");

                responder.AddSpecialty(specialty.AcceptedIncidentType, responder.Id);
            }

            if (_currentUserService.Role == UserRole.AgencyAdmin || _currentUserService.Role == UserRole.SuperAdmin)
                responder.Verify();

            user.SetResponderId(responder.Id);
            responder.SetCreatedBy(currentUserId.ToString());

            await _userRepository.AddAsync(user);
            await _responderRepository.AddAsync(responder);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(responder.Id);
        }
    }
}