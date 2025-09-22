using Domain.Enums;

namespace Domain.ValueObjects
{
    public class EmergencyContact
    {
        public string Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Email? Email { get; private set; }
        public RelationshipType Relationship { get; private set; }
        public string? OtherRelationship { get; private set; }

        private EmergencyContact() { }

        public EmergencyContact( string name, PhoneNumber phoneNumber,Email email , RelationshipType relationship, string? otherRelationship = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (relationship == RelationshipType.Other && string.IsNullOrWhiteSpace(otherRelationship))
                throw new ArgumentException("OtherRelationship must be provided when relationship is 'Other'.");

            

            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Relationship = relationship;
            OtherRelationship = relationship == RelationshipType.Other ? otherRelationship : null;
        }

        public string GetRelationshipLabel()
        {
            return Relationship == RelationshipType.Other ? OtherRelationship! : Relationship.ToString();
        }
    }
}