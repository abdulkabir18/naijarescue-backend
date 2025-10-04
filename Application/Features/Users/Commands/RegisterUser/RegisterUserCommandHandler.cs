using Application.Common.Dtos;
using Application.Interfaces.External;
using Application.Interfaces.Repositories;
using Application.Interfaces.UnitOfWork;
using Domain.Common.Security;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IStorageManager _storageManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IStorageManager storageManager, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _storageManager = storageManager;
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

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Model == null)
                return Result<Guid>.Failure("Invalid request payload.");

            if (await _userRepository.IsEmailExistAsync(request.Model.Email))
                return Result<Guid>.Failure($"Email {request.Model.Email} is associated with another account.");

            if (await _userRepository.IsPhoneNumberExistAsync(request.Model.PhoneNumber))
                return Result<Guid>.Failure($"PhoneNumber {request.Model.PhoneNumber} is associated with another account.");

            string fullName = BuildFullName(request.Model.FirstName, request.Model.LastName);
            if (string.IsNullOrEmpty(fullName)) return Result<Guid>.Failure("Name validation failed");

            var user = new User(fullName, new Email(request.Model.Email), new PhoneNumber(request.Model.PhoneNumber), request.Model.Gender);

            user.SetPassword($"{request.Model.Password}{user.Id}", _passwordHasher);

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

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);

        }
    }
}
