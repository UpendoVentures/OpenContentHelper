namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventCalendarMetadataDto
    {
        public int EventId { get; set; }
        public string IcsUid { get; set; }
        public string RecurrenceRule { get; set; }
        public string LocationText { get; set; }
        public string CalendarDescriptionText { get; set; }
    }
}