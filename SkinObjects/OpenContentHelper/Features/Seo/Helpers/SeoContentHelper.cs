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