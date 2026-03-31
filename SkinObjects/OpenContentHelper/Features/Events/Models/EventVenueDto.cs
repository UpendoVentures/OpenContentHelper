namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventVenueDto
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string TimeZoneId { get; set; }
        public string PublicLocationText { get; set; }
        public string WebsiteUrl { get; set; }
        public string PhoneNumber { get; set; }

        public string FullDisplayLocation
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(PublicLocationText))
                {
                    return PublicLocationText;
                }

                var cityRegion = string.Empty;

                if (!string.IsNullOrWhiteSpace(City) && !string.IsNullOrWhiteSpace(Region))
                {
                    cityRegion = City + ", " + Region;
                }
                else if (!string.IsNullOrWhiteSpace(City))
                {
                    cityRegion = City;
                }
                else if (!string.IsNullOrWhiteSpace(Region))
                {
                    cityRegion = Region;
                }

                return cityRegion;
            }
        }
    }
}