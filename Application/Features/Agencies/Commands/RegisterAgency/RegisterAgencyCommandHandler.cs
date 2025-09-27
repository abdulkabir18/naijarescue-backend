using Application.Common.Dtos;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Common.Security;
using MediatR;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.Enums;
using Application.Interfaces.CurrentUser;


namespace Application.Features.Agencies.Commands.RegisterAgency
{
    public class RegisterAgencyCommandHandler : IRequestHandler<RegisterAgencyCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAgencyRepository _agencyRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IStorageManager _storageManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterAgencyCommandHandler(IUserRepository userRepository, IAgencyRepository agencyRepository, IPasswordHasher passwordHasher, IStorageManager storageManager, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _agencyRepository = agencyRepository;
            _passwordHasher = passwordHasher;
            _storageManager = storageManager;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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

        public async Task<Result<Guid>> Handle(RegisterAgencyCommand request, CancellationToken cancellationToken)
        {
            if (request.Model == null)
                return Result<Guid>.Failure("Invalid request payload.");

            Guid currentUserId = _currentUserService.UserId;
            if (currentUserId == Guid.Empty)
                return Result<Guid>.Failure("User is not authenticated.");

            Task<bool> isEmailExist = _userRepository.IsEmailExistAsync(request.Model.RegisterUserRequest.Email);
            Task<bool> isPhoneNumberExist = _userRepository.IsPhoneNumberExistAsync(request.Model.RegisterUserRequest.PhoneNumber);
            Task<bool> isAgencyNameExist = _agencyRepository.IsNameExistAsync(request.Model.AgencyName);
            Task<bool> isAgencyEmailExist = _agencyRepository.IsEmailExistAsync(request.Model.AgencyEmail);
            Task<bool> isAgencyPhoneNumberExist = _agencyRepository.IsPhoneNumberExistAsync(request.Model.AgencyPhoneNumber);

            await Task.WhenAll(isEmailExist, isPhoneNumberExist, isAgencyNameExist, isAgencyEmailExist, isAgencyPhoneNumberExist);



            if (await isEmailExist)
                return Result<Guid>.Failure($"Email {request.Model.RegisterUserRequest.Email} is associated with another account.");
            if (await isPhoneNumberExist)
                return Result<Guid>.Failure($"PhoneNumber {request.Model.RegisterUserRequest.PhoneNumber} is associated with another account.");
            if (await isAgencyNameExist)
                return Result<Guid>.Failure($"Agency Name {request.Model.AgencyName} is associated with another account.");
            if (await isAgencyEmailExist)
                return Result<Guid>.Failure($"Agency Email {request.Model.AgencyEmail} is associated with another account.");
            if (await isAgencyPhoneNumberExist)
                return Result<Guid>.Failure($"Agency PhoneNumber {request.Model.AgencyPhoneNumber} is associated with another account.");

            string fullName = BuildFullName(request.Model.RegisterUserRequest.FirstName, request.Model.RegisterUserRequest.LastName);
            if (string.IsNullOrEmpty(fullName)) return Result<Guid>.Failure("Name validation failed");

            var user = new User(fullName, new Email(request.Model.RegisterUserRequest.Email), new PhoneNumber(request.Model.RegisterUserRequest.PhoneNumber), request.Model.RegisterUserRequest.Gender, UserRole.AgencyAdmin);

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

            var agency = new Agency(user.Id, request.Model.AgencyName, new Email(request.Model.AgencyEmail), new PhoneNumber(request.Model.AgencyPhoneNumber));

            if (request.Model.AgencyAddress != null)
            {
                agency.SetAddress(new Address(request.Model.AgencyAddress.Street!, request.Model.AgencyAddress.City!, request.Model.AgencyAddress.State!, request.Model.AgencyAddress.LGA!, request.Model.AgencyAddress.Country!, request.Model.AgencyAddress.PostalCode!));
            }

            if (request.Model.AgencyLogo != null)
            {
                using var stream = request.Model.AgencyLogo.OpenReadStream();
                string imageUrl = await _storageManager.UploadProfileImageAsync(stream, request.Model.AgencyLogo.FileName, request.Model.AgencyLogo.ContentType);

                agency.SetLogo(imageUrl);
            }

            foreach (var incident in request.Model.SupportedIncidents)
            {
                if (!Enum.IsDefined(typeof(IncidentType), incident.AcceptedIncidentType))
                    return Result<Guid>.Failure($"IncidentType '{incident.AcceptedIncidentType}' is not valid.");

                agency.AddSupportedIncident(incident.AcceptedIncidentType);
            }

            foreach (var work in request.Model.SupportedWorkTypes)
            {
                if (!Enum.IsDefined(typeof(WorkType), work.AcceptedWorkType))
                    return Result<Guid>.Failure($"WorkType '{work.AcceptedWorkType}' is not valid.");

                agency.AddSupportedWorkType(work.AcceptedWorkType);
            }

            if (_currentUserService.Role == UserRole.AgencyAdmin)
                agency.Reactivate();

            user.SetAgencyId(agency.Id);
            agency.SetCreatedBy(currentUserId.ToString());

            await _userRepository.AddAsync(user);
            await _agencyRepository.AddAsync(agency);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(agency.Id);
        }
    }

}

