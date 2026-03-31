namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventListRequest
    {
        public int PortalId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string CategorySlug { get; set; }
        public string OrganizerSlug { get; set; }
        public string TagSlug { get; set; }
        public string Keyword { get; set; }
        public bool IsUpcoming { get; set; }
        public bool OnlyFeatured { get; set; }
        public string SortBy { get; set; }

        public EventListRequest()
        {
            PageNumber = 1;
            PageSize = 10;
            IsUpcoming = true;
            OnlyFeatured = false;
            SortBy = "date_asc";
        }
    }
}