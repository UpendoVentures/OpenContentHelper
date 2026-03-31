namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventOrganizerDto
    {
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; }
        public string ContactName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string WebsiteUrl { get; set; }
        public int HostedEventCount { get; set; }
        public string Slug { get; set; }
    }
}