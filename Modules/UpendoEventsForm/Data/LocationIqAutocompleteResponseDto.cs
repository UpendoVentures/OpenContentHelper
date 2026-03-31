using Newtonsoft.Json;
using System.Collections.Generic;

namespace Upendo.Modules.UpendoEventsForm.Data
{
    internal class LocationIqAutocompleteResponseDto
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("lon")]
        public string Longitude { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("display_place")]
        public string DisplayPlace { get; set; }

        [JsonProperty("display_address")]
        public string DisplayAddress { get; set; }

        [JsonProperty("address")]
        public LocationIqAddressDto Address { get; set; }
    }

    internal class LocationIqAddressDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("house_number")]
        public string HouseNumber { get; set; }

        [JsonProperty("road")]
        public string Road { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }

        [JsonProperty("village")]
        public string Village { get; set; }

        [JsonProperty("hamlet")]
        public string Hamlet { get; set; }

        [JsonProperty("municipality")]
        public string Municipality { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
    }
}