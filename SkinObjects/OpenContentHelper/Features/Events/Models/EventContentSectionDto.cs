namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventContentSectionDto
    {
        public int EventContentSectionId { get; set; }
        public string SectionType { get; set; }
        public string SectionTitle { get; set; }
        public string BodyHtml { get; set; }
        public string BodyText { get; set; }
        public string IconClass { get; set; }
        public int SortOrder { get; set; }
    }
}