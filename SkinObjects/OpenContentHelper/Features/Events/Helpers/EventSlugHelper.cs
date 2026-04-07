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
    public static class EventSlugHelper
    {
        /// <summary>
        /// Extracts the slug from the request URL based on the detail page path.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="detailPagePath">The base path of the detail page.</param>
        /// <param name="ignoreTrailingPath">Whether to ignore the trailing path after the slug.</param>
        /// <returns>The extracted slug, or an empty string if not found.</returns>
        /// <remarks>
        /// Adding the ability to ignore the trailing path allows us to support two different use cases:
        /// `/root/detail/slug` and `/root/detail/slug/extra-path`.
        /// 
        /// The first format is the most common scenario, where we might want to have a list of articles, events, and other content items - then enforce querystring via SEO settings.  
        /// The second format allows us to have an open-ended, free-form URL structure to support dynamic URL patterns using URL- and data-driven page content.
        /// 
        /// Example:
        /// Load events/landmarks based on the URL pattern `/real-page/slug` where it might be `/real-page/country`, `/real-page/country/region`, `/real-page/country/region/city`, etc.
        /// </remarks>
        public static string GetSlugFromRequest(HttpRequest request, string detailPagePath, bool ignoreTrailingPath = false)
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

            if (ignoreTrailingPath)
            {
                // Extract only the first segment (everything before the next slash)
                var firstSlashIndex = slug.IndexOf('/');
                if (firstSlashIndex > 0)
                {
                    slug = slug.Substring(0, firstSlashIndex);
                }
            }

            return HttpUtility.UrlDecode(slug);
        }
    }
}