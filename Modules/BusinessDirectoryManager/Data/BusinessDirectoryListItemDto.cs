namespace Upendo.Modules.BusinessDirectoryManager.Data
{
    public class BusinessDirectoryListItemDto
    {
        public int CompanyId { get; set; }

        public int PortalId { get; set; }

        public string CompanyName { get; set; }

        public string Slug { get; set; }

        public string ShortDescription { get; set; }

        public string WebsiteUrl { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public byte MembershipStatus { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsPublic { get; set; }

        public bool IsActive { get; set; }

        public string PrimaryCategoryName { get; set; }

        public string LogoUrl { get; set; }

        public System.DateTime? UpdatedOn { get; set; }
    }
}