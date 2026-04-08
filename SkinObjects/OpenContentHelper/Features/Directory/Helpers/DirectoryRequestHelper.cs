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
using Upendo.OpenContentHelper.Features.Directory.Common;
using Upendo.OpenContentHelper.Features.Seo.Helpers;

namespace Upendo.OpenContentHelper.Features.Directory.Helpers
{
    public static class DirectoryRequestHelper
    {
        public static bool IsDirectoryListRequest(HttpRequest request)
        {
            if (request == null)
            {
                return false;
            }

            var requestPath = SeoContentHelper.NormalizePath(request.RawUrl);
            var listPath = DirectoryPageUrlHelper.BuildListPageUrl(DirectoryConstants.DirectoryListPageRoute);

            return requestPath.Equals(listPath, StringComparison.OrdinalIgnoreCase);
        }

        public static bool TryGetCompanyDetailSlug(HttpRequest request, out string slug)
        {
            slug = string.Empty;

            if (request == null)
            {
                return false;
            }

            var requestPath = SeoContentHelper.NormalizePath(request.RawUrl);
            var detailBasePath = DirectoryPageUrlHelper.BuildCompanyDetailBaseUrl(DirectoryConstants.DirectoryListPageRoute);

            if (!requestPath.StartsWith(string.Concat(detailBasePath, "/"), StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var relative = requestPath.Substring(detailBasePath.Length).Trim('/');
            if (string.IsNullOrWhiteSpace(relative))
            {
                return false;
            }

            var firstSegment = relative.Split('/')[0];
            var queryIndex = firstSegment.IndexOf('?');
            if (queryIndex >= 0)
            {
                firstSegment = firstSegment.Substring(0, queryIndex);
            }
            slug = HttpUtility.UrlDecode(firstSegment ?? string.Empty);

            return !string.IsNullOrWhiteSpace(slug);
        }
    }
}