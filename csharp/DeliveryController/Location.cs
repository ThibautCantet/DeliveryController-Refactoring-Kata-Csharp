namespace DeliveryController
{
    public class Location
    {
        public float Latitude { get; }
        public float Longitude { get; }

        public Location(float latitude, float longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override bool Equals(object? obj)
        {
            return ((Location)obj).Latitude == Latitude && ((Location)obj).Longitude == Longitude;
        }
    }
}