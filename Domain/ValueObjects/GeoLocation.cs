namespace Domain.ValueObjects
{
    public class GeoLocation
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private GeoLocation() { } // EF Core

        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}