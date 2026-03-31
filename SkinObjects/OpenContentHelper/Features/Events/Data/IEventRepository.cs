using Upendo.OpenContentHelper.Features.Events.Models;
using System.Collections.Generic;

namespace Upendo.OpenContentHelper.Features.Events.Data
{
    public interface IEventRepository
    {
        PagedResult<EventListItemDto> ListPaged(EventListRequest request);
        EventDetailDto GetDetail(int portalId, string slug, int relatedMaxResults);
        EventDetailDto GetBySlug(int portalId, string slug);

        IList<EventCategoryDto> ListActiveCategories();
        IList<EventTagDto> ListActiveTags();

        IList<EventCategoryDto> ListCategoriesForPortal(int portalId, bool isUpcoming);
        IList<EventTagDto> ListTrendingTags(int portalId, bool isUpcoming, int maxResults);
        IList<EventOrganizerDto> ListTopOrganizers(int portalId, bool isUpcoming, int maxResults);
    }
}