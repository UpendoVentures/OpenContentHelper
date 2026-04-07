namespace Upendo.Modules.BusinessDirectoryManager.Data
{
    public class BusinessDirectoryListRequestDto
    {
        public int PortalId { get; set; }

        public string SearchText { get; set; }

        public string StatusFilter { get; set; }

        public string VisibilityFilter { get; set; }

        public bool FeaturedOnly { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}