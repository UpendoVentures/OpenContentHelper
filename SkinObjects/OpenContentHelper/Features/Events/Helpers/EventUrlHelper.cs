using Upendo.SkinObjects.OpenContentHelper.Common;
using Upendo.OpenContentHelper.Features.Events.Models;
using System;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventUrlHelper
    {
        public static string GetDetailUrl(EventListItemDto item, string detailPagePath)
        {
            if (item == null)
            {
                return EventPageUrlHelper.BuildDetailBasePageUrl(detailPagePath);
            }

            return EventPageUrlHelper.BuildDetailPageUrl(detailPagePath, item.Slug);
        }

        public static string GetEditUrl(int eventId)
        {
            return eventId <= 0 ? string.Empty : GetEditUrl(eventId, EventConstants.EventEditPageRoute);
        }

        public static string GetEditUrl(int eventId, string editPagePath)
        {
            if (eventId <= 0 || string.IsNullOrEmpty(editPagePath))
            {
                return string.Empty;
            }

            return EventPageUrlHelper.BuildEditPageUrl(editPagePath, eventId);
        }
    }
}