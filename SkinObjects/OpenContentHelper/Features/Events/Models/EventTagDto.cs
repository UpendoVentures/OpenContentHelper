namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventTagDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string Slug { get; set; }
        public int EventCount { get; set; }
        public int SortOrder { get; set; }
    }
}