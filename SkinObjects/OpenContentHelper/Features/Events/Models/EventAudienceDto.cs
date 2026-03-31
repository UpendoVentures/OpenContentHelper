namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventAudienceDto
    {
        public int EventAudienceId { get; set; }
        public int? AudienceTypeId { get; set; }
        public string AudienceName { get; set; }
        public string AudienceSlug { get; set; }
        public string AudienceTitle { get; set; }
        public string AudienceDescription { get; set; }
        public int SortOrder { get; set; }
    }
}