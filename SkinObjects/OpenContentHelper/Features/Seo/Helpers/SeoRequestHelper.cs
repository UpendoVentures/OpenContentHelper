using System;
using System.Web;
using Upendo.OpenContentHelper.Features.Events;
using Upendo.OpenContentHelper.Features.Events.Helpers;

namespace Upendo.OpenContentHelper.Features.Seo.Helpers
{
    public static class SeoRequestHelper
    {
        public static bool IsEventListRequest(HttpRequest request)
        {
            if (request == null) return false;

            var requestPath = SeoContentHelper.NormalizePath(request.RawUrl);
            var listPagePath = EventPageUrlHelper.BuildListPageUrl(EventConstants.EventListPageRoute);

            return requestPath.Equals(listPagePath, StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryGetEventDetailSlug(HttpRequest request, out string slug)
        {
            slug = string.Empty;

            if (request == null)
            {
                return false;
            }

            var requestPath = SeoContentHelper.NormalizePath(request.RawUrl);
            var detailPagePath = EventPageUrlHelper.BuildDetailBasePageUrl(EventConstants.EventDetailPageRoute);

            if (!requestPath.StartsWith(detailPagePath + "/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            slug = EventSlugHelper.GetSlugFromRequest(request, detailPagePath, true);
            return !string.IsNullOrWhiteSpace(slug);
        }
    }
}