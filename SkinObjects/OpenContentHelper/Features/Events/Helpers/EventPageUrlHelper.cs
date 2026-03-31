/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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