namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventDetailViewModel
    {
        public EventDetailViewModel()
        {
            ListPagePath = EventConstants.EventListPageRoute;
            DetailPagePath = EventConstants.EventDetailPageRoute;
            RequestedSlug = string.Empty;
            EventFound = false;
            FriendlyMessageTitle = "We Couldn’t Find That Event";
            FriendlyMessageBody = "The event you were looking for may have been removed, renamed, or the link may be incomplete.";
            IcsDownloadUrl = string.Empty;
        }

        public EventDetailDto Event { get; set; }

        public string ListPagePath { get; set; }

        public string DetailPagePath { get; set; }

        public string RequestedSlug { get; set; }

        public bool EventFound { get; set; }

        public string FriendlyMessageTitle { get; set; }

        public string FriendlyMessageBody { get; set; }

        public string IcsDownloadUrl { get; set; }
    }
}