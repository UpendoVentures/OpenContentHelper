using System;
using System.Web;
using Upendo.SkinObjects.OpenContentHelper.Common;
using Upendo.OpenContentHelper.Features.Directory.Common;

namespace Upendo.OpenContentHelper.Features.Directory.Helpers
{
    public static class DirectoryPageUrlHelper
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
                value = Constants.Slash + value;
            }

            if (value.Length > 1)
            {
                value = value.TrimEnd('/');
            }

            return value;
        }

        public static string BuildListPageUrl(string listPagePath)
        {
            return NormalizePagePath(listPagePath, DirectoryConstants.DirectoryListPageRoute);
        }

        public static string BuildCompanyDetailBaseUrl(string listPagePath)
        {
            return BuildListPageUrl(listPagePath) + "/company";
        }

        public static string BuildCompanyDetailUrl(string listPagePath, string slug)
        {
            var basePath = BuildCompanyDetailBaseUrl(listPagePath);

            if (string.IsNullOrWhiteSpace(slug))
            {
                return basePath;
            }

            var normalizedSlug = slug.Trim().Trim('/');
            var encodedSlug = HttpUtility.UrlPathEncode(normalizedSlug);

            return basePath + "/" + encodedSlug;
        }

        public static string GetEditUrl(int companyId)
        {
            return companyId <= 0
                ? string.Empty
                : GetEditUrl(companyId, DirectoryConstants.DirectoryEditPageRoute);
        }

        public static string GetEditUrl(int companyId, string editPagePath)
        {
            if (companyId <= 0 || string.IsNullOrWhiteSpace(editPagePath))
            {
                return string.Empty;
            }

            return DirectoryPageUrlHelper.BuildEditPageUrl(editPagePath, companyId);
        }

        public static string BuildEditPageUrl(string editPagePath, int companyId)
        {
            var basePath = NormalizePagePath(editPagePath, DirectoryConstants.DirectoryEditPageRoute);

            if (companyId <= 0)
            {
                return basePath;
            }

            return basePath + "#/edit/" + companyId;
        }
    }
}