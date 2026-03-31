using System.Collections.Generic;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events.Services
{
    public interface IEventService
    {
        PagedResult<EventListItemDto> ListEvents(EventListRequest request);
        EventDetailDto GetEventDetail(int portalId, string slug, int relatedMaxResults);
        EventDetailDto GetEventSummary(int portalId, string slug);
        IList<EventCategoryDto> ListCategoriesForPortal(int portalId, bool isUpcoming);
        IList<EventTagDto> ListTrendingTags(int portalId, bool isUpcoming, int maxResults);
        IList<EventOrganizerDto> ListTopOrganizers(int portalId, bool isUpcoming, int maxResults);
    }
}