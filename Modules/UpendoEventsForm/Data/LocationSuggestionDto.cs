namespace Upendo.Modules.UpendoEventsForm.Data
{
    public class LocationSuggestionDto
    {
        public string DisplayName { get; set; }

        public string VenueName { get; set; }

        public string PublicLocationText { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string WebsiteUrl { get; set; }

        public string PhoneNumber { get; set; }
    }
}