namespace Domain.ValueObjects
{
    public class ReporterDetails
    {
        public string? Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public Email? Email { get; private set; }

        private ReporterDetails() { }

        public ReporterDetails(string? name, PhoneNumber? phoneNumber, Email? email)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
