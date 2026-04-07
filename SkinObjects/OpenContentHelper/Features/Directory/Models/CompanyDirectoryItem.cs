
namespace Upendo.OpenContentHelper.Features.Directory.Models
{
    public sealed class CompanyDirectoryItem
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        public string Address1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public byte MembershipStatus { get; set; }
        public int? MemberSinceYear { get; set; }

        public bool IsFeatured { get; set; }
        public int FeaturedSortOrder { get; set; }

        public int? LogoFileId { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
    }
}