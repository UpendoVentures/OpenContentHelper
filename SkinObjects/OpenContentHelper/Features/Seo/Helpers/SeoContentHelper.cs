using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Seo.Helpers
{
    public static class SeoContentHelper
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "/";
            }

            path = path.Trim();

            if (!path.StartsWith("/", StringComparison.Ordinal))
            {
                path = "/" + path;
            }

            if (path.Length > 1)
            {
                path = path.TrimEnd('/');
            }

            return path;
        }

        public static string BuildAbsoluteUrl(string authority, string path)
        {
            if (string.IsNullOrWhiteSpace(authority))
            {
                return path ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                return authority;
            }

            if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                return path;
            }

            return authority + NormalizePath(path);
        }

        public static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var value = Regex.Replace(input, "<.*?>", " ");
            value = HttpUtility.HtmlDecode(value);
            value = Regex.Replace(value, "\\s+", " ").Trim();

            return value;
        }

        public static string NormalizeMetaText(string input)
        {
            input = StripHtml(input);

            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            return Regex.Replace(input, "\\s+", " ").Trim();
        }

        public static string TrimToLength(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var value = NormalizeMetaText(input);

            if (value.Length <= maxLength)
            {
                return value;
            }

            value = value.Substring(0, maxLength).Trim();

            var lastSpace = value.LastIndexOf(' ');
            if (lastSpace > 0)
            {
                value = value.Substring(0, lastSpace).Trim();
            }

            return value;
        }

        public static string JoinKeywords(params string[] values)
        {
            return string.Join(", ", values
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray());
        }
    }
}