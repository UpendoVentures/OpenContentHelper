namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventFeatureDto
    {
        public int EventFeatureId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public int SortOrder { get; set; }
    }
}