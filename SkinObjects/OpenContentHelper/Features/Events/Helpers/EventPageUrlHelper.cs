using System;
using System.Web;
using Upendo.SkinObjects.OpenContentHelper.Common;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventPageUrlHelper
    {
        public static string NormalizePagePath(string path, string fallbackPath)
        {
            var value = string.IsNullOrWhiteSpace(path) ? fallbackPath : path.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                value = Constants.Slash;
            }

            if (!value.StartsWith(Constants.Slash, StringComparison.Ordinal))
            {
                value = string.Concat(Constants.Slash, value);
            }

            if (value.Length > 1)
            {
                value = value.TrimEnd('/');
            }

            return value;
        }

        public static string BuildListPageUrl(string listPagePath)
        {
            return NormalizePagePath(listPagePath, EventConstants.EventListPageRoute);
        }

        public static string BuildDetailBasePageUrl(string detailPagePath)
        {
            return NormalizePagePath(detailPagePath, EventConstants.EventDetailPageRoute);
        }

        public static string BuildDetailPageUrl(string detailPagePath, string slug)
        {
            var basePath = BuildDetailBasePageUrl(detailPagePath);

            if (string.IsNullOrWhiteSpace(slug))
            {
                return basePath;
            }

            var normalizedSlug = slug.Trim().Trim('/');
            var encodedSlug = HttpUtility.UrlPathEncode(normalizedSlug);

            return string.Concat(basePath, Constants.Slash, encodedSlug);
        }

        public static string BuildEditPageUrl(string editPagePath, int eventId)
        {
            var basePath = NormalizePagePath(editPagePath, EventConstants.EventEditPageRoute);
            if (eventId <= 0)
            {
                return basePath;
            }
            return string.Concat(basePath, "?eventid=", eventId);
        }

        public static string BuildIcsDownloadUrl(string apiBasePath, string slug)
        {
            if (string.IsNullOrWhiteSpace(apiBasePath) || string.IsNullOrWhiteSpace(slug))
            {
                return string.Empty;
            }

            var normalizedApiBasePath = NormalizePagePath(apiBasePath, EventConstants.EndpointCalendar);
            var encodedSlug = HttpUtility.UrlEncode(slug.Trim());

            return string.Concat(normalizedApiBasePath, "?slug=", encodedSlug);
        }
    }
}