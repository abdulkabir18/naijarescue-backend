using Domain.Enums;

namespace Domain.ValueObjects
{
    public class EmergencyContact
    {
        public string Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Email Email { get; private set; }
        public RelationshipType Relationship { get; private set; }
        public string? OtherRelationship { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public bool IsPhoneNumberVerified { get; private set; }

        private EmergencyContact() { }

        public EmergencyContact(string name, Email email, RelationshipType relationship, string? otherRelationship = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (relationship == RelationshipType.Other && string.IsNullOrWhiteSpace(otherRelationship))
                throw new ArgumentException("OtherRelationship must be provided when relationship is 'Other'.");


            Name = name;
            Email = email;
            Relationship = relationship;
            OtherRelationship = relationship == RelationshipType.Other ? otherRelationship : null;

            IsEmailVerified = false;
            IsPhoneNumberVerified = false;
        }

        public void SetPhoneNumber(PhoneNumber phoneNumber)
        {
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }

        public string GetRelationshipLabel()
        {
            return Relationship == RelationshipType.Other ? OtherRelationship! : Relationship.ToString();
        }

        public void VerifyEmail() => IsEmailVerified = true;
        public void VerifyPhoneNumber() => IsPhoneNumberVerified = true;
    }
}