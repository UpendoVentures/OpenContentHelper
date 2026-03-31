using System;
using System.Web;
using Upendo.SkinObjects.OpenContentHelper.Common;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventSlugHelper
    {
        public static string GetSlugFromRequest(HttpRequest request, string detailPagePath)
        {
            if (request == null || request.Url == null)
            {
                return string.Empty;
            }

            var normalizedDetailPagePath = EventPageUrlHelper.BuildDetailBasePageUrl(detailPagePath);
            var absolutePath = request.RawUrl ?? string.Empty;

            if (string.IsNullOrWhiteSpace(absolutePath))
            {
                return string.Empty;
            }

            absolutePath = absolutePath.Trim();

            if (!absolutePath.StartsWith(Constants.Slash, StringComparison.Ordinal))
            {
                absolutePath = string.Concat(Constants.Slash, absolutePath);
            }

            if (absolutePath.Length > 1)
            {
                absolutePath = absolutePath.TrimEnd('/');
            }

            if (absolutePath.Equals(normalizedDetailPagePath, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            var detailPrefix = string.Concat(normalizedDetailPagePath, Constants.Slash);

            if (!absolutePath.StartsWith(detailPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            var slug = absolutePath.Substring(detailPrefix.Length).Trim('/');

            if (string.IsNullOrWhiteSpace(slug))
            {
                return string.Empty;
            }

            return HttpUtility.UrlDecode(slug);
        }
    }
}