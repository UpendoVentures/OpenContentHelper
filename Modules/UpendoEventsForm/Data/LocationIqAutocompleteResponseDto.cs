/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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