using Domain.Common.Exceptions;

namespace Domain.ValueObjects
{
    public class VictimDetails
    {
        public string? Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Email? Email { get; private set; }
        public string Description { get; private set; }

        private VictimDetails() { }

        public VictimDetails(string? name, PhoneNumber? phoneNumber, Email? email, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ValidationException("Description is required if victim identity is incomplete.");

            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
            Description = description;
        }

    }
}
