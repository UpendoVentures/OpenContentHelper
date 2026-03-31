using System.Collections.Generic;

namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventListViewModel
    {
        public EventListViewModel()
        {
            Request = new EventListRequest();
            Results = new PagedResult<EventListItemDto>();
            ListPagePath = EventConstants.EventListPageRoute;
            DetailPagePath = EventConstants.EventDetailPageRoute;
            EmptyStateTitle = "No Events Found";
            EmptyStateText = "There are no events matching the current filters.";
            ActiveCategories = new List<EventCategoryDto>();
            TrendingTags = new List<EventTagDto>();
            TopOrganizers = new List<EventOrganizerDto>();
        }

        public EventListRequest Request { get; set; }

        public PagedResult<EventListItemDto> Results { get; set; }

        public string ListPagePath { get; set; }

        public string DetailPagePath { get; set; }

        public string EmptyStateTitle { get; set; }

        public string EmptyStateText { get; set; }

        public IList<EventCategoryDto> ActiveCategories { get; set; }

        public IList<EventTagDto> TrendingTags { get; set; }

        public IList<EventOrganizerDto> TopOrganizers { get; set; }

        public string ActiveCategorySlug { get; set; }

        public string ActiveTagSlug { get; set; }

        public string ActiveOrganizerSlug { get; set; }
    }
}