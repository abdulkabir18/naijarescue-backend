using Domain.Common;
using Domain.Common.Security;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class User : AuditableEntity
    {
        public string FullName { get; private set; }
        public Email Email { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public string? PasswordHash { get; private set; }
        public string? UserName { get; private set; }
        public string? ProfilePictureUrl { get; private set; }
        public Gender Gender { get; private set; }
        public Address? Address { get; private set; }

        public bool IsEmailVerified { get; private set; }
        public bool IsPhoneNumberVerified { get; private set; }
        public bool TwoFactorEnabled { get; private set; }
        public string? GoogleAuthenticatorSecretKey { get; private set; }
        public string? GoogleId { get; private set; } 

        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }

        public Guid? AgencyId { get; private set; }
        public Guid? ResponderId { get; private set; }
        public Responder? Responder { get; private set; }
        public Agency? Agency { get; private set; } = default!;

        private readonly HashSet<EmergencyContact> _emergencyContacts = new();
        public IReadOnlyCollection<EmergencyContact> EmergencyContacts => _emergencyContacts;

        private readonly HashSet<Incident> _incidents = new();
        public IReadOnlyCollection<Incident> Incidents => _incidents;

        private User() { }

        public User(string fullName, Email email, PhoneNumber phoneNumber, Gender gender, UserRole role = UserRole.Victim)
        {
            FullName = string.IsNullOrWhiteSpace(fullName) ? throw new DomainException("Full name is required.") : fullName;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Gender = gender;

            Role = role;
            IsActive = true;
            IsEmailVerified = false;
            IsPhoneNumberVerified = false;
            TwoFactorEnabled = false;

            AddDomainEvent(new UserRegisteredEvent(Id, FullName, Email.Value, Role));
        }

        public static User RegisterWithGoogle(string fullName, Email email, string googleId, string? profilePictureUrl, Gender gender, UserRole role = UserRole.Victim)
        {
            if (string.IsNullOrWhiteSpace(googleId))
                throw new DomainException("Google ID is required for Google Sign-In.");

            var user = new User
            {
                FullName = fullName,
                Email = email,
                GoogleId = googleId,
                ProfilePictureUrl = profilePictureUrl,
                Gender = gender,
                Role = role,
                IsActive = true,
                IsEmailVerified = true, 
                IsPhoneNumberVerified = false,
                TwoFactorEnabled = false
            };

            user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.FullName, user.Email.Value, user.Role));
            return user;
        }

            
        public void SetPassword(string rawPassword, IPasswordHasher hasher)
        {
            if (string.IsNullOrWhiteSpace(rawPassword))
                throw new DomainException("Password cannot be empty.");

            PasswordHash = hasher.HashPassword(rawPassword);
        }

        public bool VerifyPassword(string password, IPasswordHasher hasher)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException("Password cannot be empty.");

            return hasher.VerifyPassword(PasswordHash!, password);
        }

        public void SetUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new DomainException("Username cannot be empty.");

            UserName = userName;
        }

        public void SetProfilePicture(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new DomainException("Profile picture URL cannot be empty.");

            ProfilePictureUrl = imageUrl;
        }

        public void SetAddress(Address newAddress) =>
            Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));

        public void ChangeEmail(Email newEmail)
        {
            if(Email == newEmail)
                throw new ValidationException("New email cannot be the same as the old one.");

            Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
            IsEmailVerified = false;
        }

        public void ChangePhoneNumber(PhoneNumber newPhone)
        {
            if (PhoneNumber == newPhone)
                throw new ValidationException("New phonenumber cannot be the same as the old one.");

            PhoneNumber = newPhone ?? throw new ArgumentNullException(nameof(newPhone));
            IsPhoneNumberVerified = false;
        }

        public void VerifyEmail() => IsEmailVerified = true;
        public void VerifyPhoneNumber() => IsPhoneNumberVerified = true;

        public void EnableTwoFactorAuth(string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new DomainException("Secret key cannot be empty.");
            TwoFactorEnabled = true;
            GoogleAuthenticatorSecretKey = secretKey;
        }

        public void DisableTwoFactorAuth()
        {
            TwoFactorEnabled = false;
            GoogleAuthenticatorSecretKey = null;
        }

        public void Deactivate() => IsActive = false;
        public void Reactivate() => IsActive = true;
    }
}

    //public class User : AuditableEntity
    //{
    //    public string FullName { get; private set; }
    //    public Email Email { get; private set; }
    //    public PhoneNumber PhoneNumber { get; private set; }
    //    public string? PasswordHash { get; private set; }
    //    public string? UserName { get; private set; }
    //    public string? ProfilePictureUrl { get; private set; }
    //    public Gender Gender { get; private set; }
    //    public Address? Address { get; private set; }
    //    public bool IsEmailVerified { get; private set; }
    //    public bool IsPhoneNumberVerified { get; private set; }
    //    public bool TwoFactorEnabled { get; private set; }
    //    public string? GoogleAuthenticatorSecretKey { get; private set; }
    //    public string? GoogleId { get; private set; }

    //    public UserRole Role { get; private set; }
    //    public bool IsActive { get; private set; }

    //    public Guid? AgencyId { get; private set; }
    //    public Guid? ResponderId { get; private set; }
    //    public Responder? Responder { get; private set; }
    //    public Agency? Agency { get; private set; } = default!;

    //    public ICollection<EmergencyContact> EmergencyContacts { get; private set; } = [];
    //    public ICollection<Incident> Incidents { get; private set; } = [];

    //    private User() { }

    //    public User(string fullName, Email email, PhoneNumber phoneNumber, string passwordHash, UserRole role = UserRole.Victim)
    //    {
    //        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
    //        Email = email ?? throw new ArgumentNullException(nameof(email));
    //        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    //        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    //        Role = role;
    //        IsActive = true;
    //        IsEmailVerified = false;
    //        IsPhoneNumberVerified = false;
    //        TwoFactorEnabled = false;

    //        AddDomainEvent(new UserRegisteredEvent(Id, FullName, Email.Value, Role));
    //    }

    //    public void SetPassword(string rawPassword, IPasswordHasher hasher)
    //    {
    //        if (string.IsNullOrWhiteSpace(rawPassword))
    //            throw new ArgumentException("Password cannot be empty.");

    //        PasswordHash = hasher.HashPassword(rawPassword);
    //    }

    //    public bool VerifyPassword(string password, IPasswordHasher hasher)
    //    {
    //        return hasher.VerifyPassword(PasswordHash, password);
    //    }

    //    public void SetUserName(string userName)
    //    {
    //        if (string.IsNullOrWhiteSpace(userName))
    //            throw new ArgumentException("Username cannot be empty.");
    //        UserName = userName;
    //    }

    //    public void ChangeProfilePicture(string imageUrl)
    //    {
    //        if (string.IsNullOrWhiteSpace(imageUrl))
    //            throw new ArgumentException("Profile picture URL cannot be empty.");
    //        ProfilePictureUrl = imageUrl;
    //    }

    //    public void UpdateAddress(Address newAddress)
    //    {
    //        Address = newAddress ?? throw new ArgumentNullException(nameof(newAddress));
    //    }

    //    public void ChangeEmail(Email newEmail)
    //    {
    //        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
    //        IsEmailVerified = false;
    //    }

    //    public void ChangePhoneNumber(PhoneNumber newPhone)
    //    {
    //        PhoneNumber = newPhone ?? throw new ArgumentNullException(nameof(newPhone));
    //        IsPhoneNumberVerified = false;
    //    }

    //    public void VerifyEmail() => IsEmailVerified = true;
    //    public void VerifyPhoneNumber() => IsPhoneNumberVerified = true;

    //    public void EnableTwoFactorAuth(string secretKey)
    //    {
    //        if (string.IsNullOrWhiteSpace(secretKey))
    //            throw new ArgumentException("Secret key cannot be empty.");
    //        TwoFactorEnabled = true;
    //        GoogleAuthenticatorSecretKey = secretKey;
    //    }

    //    public void DisableTwoFactorAuth()
    //    {
    //        TwoFactorEnabled = false;
    //        GoogleAuthenticatorSecretKey = null;
    //    }

    //    public void Deactivate() => IsActive = false;
    //    public void Reactivate() => IsActive = true;

    //    //public bool IsVerified() => IsEmailVerified && IsPhoneNumberVerified;
    //}

    //public class User : AuditableEntity
    //{
    //    public string FullName { get; private set; }

    //    public Email Email { get; private set; }
    //    public bool IsEmailVerified { get; private set; }

    //    public PhoneNumber PhoneNumber { get; private set; }
    //    public bool IsPhoneNumberVerified { get; private set; }

    //    public string? UserName { get; private set; }
    //    public string PasswordHash { get; private set; }

    //    public string? ProfilePictureUrl { get; private set; }

    //    public Address? Address { get; private set; }

    //    public bool TwoFactorEnabled { get; private set; }
    //    public string? GoogleAuthenticatorSecretKey { get; private set; }

    //    public UserRole Role { get; private set; }

    //    public bool IsActive { get; private set; }

    //    // -------------------- Constructors --------------------

    //    private User() { } // For EF Core

    //    public User(
    //        string fullName,
    //        Email email,
    //        PhoneNumber phoneNumber,
    //        string passwordHash,
    //        UserRole role = UserRole.Victim
    //    )
    //    {
    //        FullName = fullName;
    //        Email = email;
    //        PhoneNumber = phoneNumber;
    //        PasswordHash = passwordHash;
    //        Role = role;

    //        IsActive = true;
    //    }

    //    // -------------------- Setters & Updates --------------------

    //    public void SetUserName(string userName) => UserName = userName;
    //    public void SetProfilePicture(string imageUrl) => ProfilePictureUrl = imageUrl;
    //    public void VerifyEmail() => IsEmailVerified = true;
    //    public void VerifyPhone() => IsPhoneNumberVerified = true;
    //    public void UpdateAddress(Address newAddress) => Address = newAddress;

    //    public void EnableTwoFactor(string secretKey)
    //    {
    //        TwoFactorEnabled = true;
    //        GoogleAuthenticatorSecretKey = secretKey;
    //    }

    //    public void DisableTwoFactor()
    //    {
    //        TwoFactorEnabled = false;
    //        GoogleAuthenticatorSecretKey = null;
    //    }

    //    public void Deactivate() => IsActive = false;
    //    public void Reactivate() => IsActive = true;
    //}

    //// Basic Info
    //public string FirstName { get; set; } = default!;
    //public string LastName { get; set; } = default!;
    //public string Email { get; set; } = default!;
    //public string PhoneNumber { get; set; } = default!;
    //public string PasswordHash { get; set; } = default!; public Address? Address { get; private set; }


    //// Profile Picture
    //public string? ProfileImageUrl { get; set; }

    //// Role & Status
    //public UserRole Role { get; set; }
    //public bool IsActive { get; set; } = true;

    //// Verifications
    //public bool IsEmailVerified { get; set; } = false;
    //public bool IsPhoneNumberVerified { get; set; } = false;

    //// MFA (Google Authenticator TOTP)
    //public string? TwoFactorSecret { get; private set; }

    //public bool IsTwoFactorEnabled => !string.IsNullOrEmpty(TwoFactorSecret);

    //public void UpdateAddress(Address newAddress)
    //{
    //    Address = newAddress;
    //    MarkUpdated(); // AuditableEntity method
    //}
    //public void EnableTwoFactor(string secret)
    //{
    //    TwoFactorSecret = secret;
    //}

    //public void DisableTwoFactor()
    //{
    //    TwoFactorSecret = null;
    //}

    //// Navigation Properties
    //public Guid? AgencyId { get; set; }
    //public Agency? Agency { get; set; }

    //public ResponderProfile? ResponderProfile { get; set; }

    //// Computed Properties
    //public string FullName => $"{FirstName} {LastName}";
