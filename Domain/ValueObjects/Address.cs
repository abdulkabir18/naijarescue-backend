namespace Domain.ValueObjects
{
    public class Address
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string LGA { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }

        private Address() { }

        public Address(string street, string city, string state,string lGA, string country, string postalCode)
        {
            Street = street;
            City = city;
            State = state;
            LGA = lGA;
            Country = country;
            PostalCode = postalCode;
        }
    }
}
