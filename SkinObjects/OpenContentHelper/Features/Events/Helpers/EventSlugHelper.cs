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