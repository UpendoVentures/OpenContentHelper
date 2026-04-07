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

using System.Collections.Generic;

namespace Upendo.Modules.BusinessDirectoryManager.Data
{
    public class BusinessDirectorySaveRequest
    {
        public int? CompanyId { get; set; }
        public int PortalId { get; set; }

        public string LegacySystem { get; set; }
        public string LegacyCompanyId { get; set; }

        public string CompanyName { get; set; }
        public string Slug { get; set; }

        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public string WebsiteUrl { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PrimaryBusinessEmail { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public byte MembershipStatus { get; set; }
        public int? MemberSinceYear { get; set; }
        public System.DateTime? PaidThroughDate { get; set; }
        public System.DateTime? RenewalDate { get; set; }

        public bool IsFeatured { get; set; }
        public int FeaturedSortOrder { get; set; }

        public int? LogoFileId { get; set; }
        public string LogoUrl { get; set; }

        public string LinkedInUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string TikTokUrl { get; set; }

        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }

        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string CanonicalUrl { get; set; }
        public string MetaRobots { get; set; }
        public string OgImageUrl { get; set; }

        public List<int> CategoryIds { get; set; }
    }
}