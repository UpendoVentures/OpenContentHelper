namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventCategoryDto
    {
        public int EventCategoryId { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public int EventCount { get; set; }
        public int SortOrder { get; set; }
    }
}