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