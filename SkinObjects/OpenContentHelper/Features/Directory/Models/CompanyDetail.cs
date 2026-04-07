using System;
using System.Collections.Generic;

namespace Upendo.OpenContentHelper.Features.Directory.Models
{
    public sealed class CompanyDetail
    {
        public int CompanyId { get; set; }
        public int PortalId { get; set; }

        public string CompanyName { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;
        public string LongDescription { get; set; } = string.Empty;

        public string WebsiteUrl { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PrimaryBusinessEmail { get; set; } = string.Empty;

        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public byte MembershipStatus { get; set; }
        public int? MemberSinceYear { get; set; }

        public bool IsFeatured { get; set; }
        public int FeaturedSortOrder { get; set; }

        public int? LogoFileId { get; set; }
        public string LogoUrl { get; set; } = string.Empty;

        public string LinkedInUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string TikTokUrl { get; set; } = string.Empty;

        public string SeoTitle { get; set; } = string.Empty;
        public string SeoDescription { get; set; } = string.Empty;
        public string CanonicalUrl { get; set; } = string.Empty;
        public string MetaRobots { get; set; } = string.Empty;
        public string OgImageUrl { get; set; } = string.Empty;

        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }

        public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    }
}