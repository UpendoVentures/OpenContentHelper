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