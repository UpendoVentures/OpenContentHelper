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

using AngleSharp.Dom;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.SkinObjects.OpenContentHelper.Helpers
{
    public static class TemplateHelper
    {
        private static readonly Lazy<HtmlSanitizer> RenderSanitizer = new Lazy<HtmlSanitizer>(CreateRenderSanitizer);

        public static void HideAdminBorder(int moduleID, int tabID)
        {
            ModuleInfo currentModule = ModuleController.Instance.GetModule(moduleID, tabID, false);
            if (currentModule != null && currentModule.TabModuleID > 0)
            {
                object setting = null;
                if (currentModule.TabModuleSettings.ContainsKey(Common.Constants.HideAdminBorderKey))
                {
                    setting = currentModule.TabModuleSettings[Common.Constants.HideAdminBorderKey];
                }

                if (setting == null || !setting.ToString().Equals(Common.Constants.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    ModuleController.Instance.UpdateTabModuleSetting(currentModule.TabModuleID, Common.Constants.HideAdminBorderKey, Common.Constants.TrueString);
                }
            }
        }

        public static bool IsOffsiteLink(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            // Absolute URLs only can be offsite
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return false;
            }

            // Compare against current request host
            var currentHost = HttpContext.Current?.Request?.Url?.Host;

            if (string.IsNullOrEmpty(currentHost))
            {
                return false;
            }

            return !uri.Host.Equals(currentHost, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CurrentUserCanEdit()
        {
            var currentUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
            return currentUser.IsSuperUser || currentUser.IsInRole(PortalSettings.Current.AdministratorRoleName);
        }

        public static string BuildAbsoluteUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            Uri absoluteUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
            {
                return absoluteUri.ToString();
            }

            var request = HttpContext.Current != null ? HttpContext.Current.Request : null;
            if (request == null || request.Url == null)
            {
                return url;
            }

            var baseUri = request.Url.GetLeftPart(UriPartial.Authority);
            return new Uri(new Uri(baseUri), url).ToString();
        }

        public static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }

            var parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }

            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpperInvariant();
        }

        public static string RenderRichTextHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var sanitizer = RenderSanitizer.Value;
            var document = sanitizer.SanitizeDom(html);

            var requestUrl = HttpContext.Current != null ? HttpContext.Current.Request?.Url : null;
            var currentHost = requestUrl != null ? requestUrl.Host : null;

            foreach (var link in document.QuerySelectorAll("a").OfType<IElement>())
            {
                var href = link.GetAttribute("href");
                if (string.IsNullOrWhiteSpace(href))
                {
                    continue;
                }

                if (!IsExternalAbsoluteUrl(href, currentHost))
                {
                    continue;
                }

                link.SetAttribute("target", "_blank");
                link.SetAttribute("rel", MergeRelToken(link.GetAttribute("rel"), "noopener"));
                //link.SetAttribute("rel", MergeRelToken(MergeRelToken(link.GetAttribute("rel"), "noopener"), "noreferrer"));
            }

            return document.Body != null ? document.Body.InnerHtml : string.Empty;
        }

        private static HtmlSanitizer CreateRenderSanitizer()
        {
            var sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedSchemes.Clear();
            sanitizer.UriAttributes.Clear();

            foreach (var tag in new[]
            {
        "p",
        "br",
        "strong",
        "b",
        "em",
        "i",
        "ul",
        "ol",
        "li",
        "a"
    })
            {
                sanitizer.AllowedTags.Add(tag);
            }

            foreach (var attribute in new[]
            {
        "href",
        "target",
        "rel"
    })
            {
                sanitizer.AllowedAttributes.Add(attribute);
            }

            sanitizer.UriAttributes.Add("href");

            foreach (var scheme in new[]
            {
        "http",
        "https",
        "mailto",
        "tel"
    })
            {
                sanitizer.AllowedSchemes.Add(scheme);
            }

            sanitizer.AllowDataAttributes = false;

            return sanitizer;
        }

        private static bool IsExternalAbsoluteUrl(string url, string currentHost)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(currentHost))
            {
                return false;
            }

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return false;
            }

            if (!uri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) &&
                !uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return !uri.Host.Equals(currentHost, StringComparison.OrdinalIgnoreCase);
        }

        private static string MergeRelToken(string rel, string tokenToAdd)
        {
            var tokens = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(rel))
            {
                foreach (var token in rel.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    tokens.Add(token.Trim());
                }
            }

            if (!string.IsNullOrWhiteSpace(tokenToAdd))
            {
                tokens.Add(tokenToAdd.Trim());
            }

            return string.Join(" ", tokens);
        }

        public static class Arrays
        {
            public static int[] NormalizeIntArray(int[] source, int targetLength)
            {
                if (targetLength < 0)
                {
                    targetLength = 0;
                }

                var result = new int[targetLength];

                if (source == null)
                {
                    return result;
                }

                for (var i = 0; i < targetLength; i++)
                {
                    result[i] = i < source.Length ? source[i] : 0;
                }

                return result;
            }

            public static decimal[] NormalizeDecimalArray(decimal[] source, int targetLength)
            {
                if (targetLength < 0)
                {
                    targetLength = 0;
                }

                var result = new decimal[targetLength];

                if (source == null)
                {
                    return result;
                }

                for (var i = 0; i < targetLength; i++)
                {
                    result[i] = i < source.Length ? source[i] : 0M;
                }

                return result;
            }
        }

        public static class Deltas
        {
            public static string GetDeltaClass(decimal? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Bootstrap.TextMuted;
                }

                if (value.Value > 0)
                {
                    return Common.Constants.Bootstrap.TextSuccess;
                }

                if (value.Value < 0)
                {
                    return Common.Constants.Bootstrap.TextDanger;
                }

                return Common.Constants.Bootstrap.TextMuted;
            }

            public static string GetDeltaClass(int? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Bootstrap.TextMuted;
                }

                if (value.Value > 0)
                {
                    return Common.Constants.Bootstrap.TextSuccess;
                }

                if (value.Value < 0)
                {
                    return Common.Constants.Bootstrap.TextDanger;
                }

                return Common.Constants.Bootstrap.TextMuted;
            }

            public static string GetDeltaIconClass(decimal? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Icons.FaMinus;
                }

                if (value.Value > 0)
                {
                    return Common.Constants.Icons.FaArrowUp;
                }

                if (value.Value < 0)
                {
                    return Common.Constants.Icons.FaArrowDown;
                }

                return Common.Constants.Icons.FaMinus;
            }

            public static string GetDeltaIconClass(int? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Icons.FaMinus;
                }

                if (value.Value > 0)
                {
                    return Common.Constants.Icons.FaArrowUp;
                }

                if (value.Value < 0)
                {
                    return Common.Constants.Icons.FaArrowDown;
                }

                return Common.Constants.Icons.FaMinus;
            }
        }

        public static class Formatting
        {
            public static string FormatDecimal(decimal? value)
            {
                return value.HasValue ? value.Value.ToString("N2") : Common.Constants.Hyphen;
            }

            public static string FormatInt(int? value)
            {
                return value.HasValue ? value.Value.ToString("N0") : Common.Constants.Hyphen;
            }

            public static string FormatDate(DateTime? value)
            {
                return value.HasValue ? value.Value.ToString("MMM d, yyyy") : Common.Constants.Hyphen;
            }

            public static string FormatCurrency(decimal? value)
            {
                return value.HasValue ? string.Concat(Common.Constants.Dollar, value.Value.ToString("N2")) : Common.Constants.Hyphen;
            }

            public static string FormatPercent(decimal? value)
            {
                return value.HasValue ? (value.Value * 100M).ToString("N2") + Common.Constants.Percent : Common.Constants.Hyphen;
            }

            public static string FormatPercentNoMultiply(decimal? value)
            {
                return value.HasValue ? value.Value.ToString("N2") + Common.Constants.Percent : Common.Constants.Hyphen;
            }

            public static string FormatSignedPercentNoMultiply(decimal? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Hyphen;
                }

                return GetPositiveSign(value.Value) + value.Value.ToString("N2") + Common.Constants.Percent;
            }

            public static string FormatSignedInt(int? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Hyphen;
                }

                return GetPositiveSign(value.Value) + value.Value.ToString("N0");
            }

            public static string FormatSignedDecimal(decimal? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Hyphen;
                }

                return GetPositiveSign(value.Value) + value.Value.ToString("N2");
            }

            public static string FormatSignedCurrency(decimal? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Hyphen;
                }

                return GetPositiveSign(value.Value) + Common.Constants.Dollar + value.Value.ToString("N2");
            }

            public static string FormatDurationFromSeconds(decimal? value)
            {
                if (!value.HasValue)
                {
                    return Common.Constants.Hyphen;
                }

                var totalSeconds = Convert.ToInt32(Math.Round(value.Value, MidpointRounding.AwayFromZero));
                if (totalSeconds < 0)
                {
                    totalSeconds = 0;
                }

                var ts = TimeSpan.FromSeconds(totalSeconds);

                if (ts.TotalHours >= 1)
                {
                    return ts.ToString(@"h\:mm\:ss");
                }

                return ts.ToString(@"m\:ss");
            }

            private static string GetPositiveSign(decimal value)
            {
                return value > 0 ? Common.Constants.Plus : string.Empty;
            }

            private static string GetPositiveSign(int value)
            {
                return value > 0 ? Common.Constants.Plus : string.Empty;
            }
        }

        public static class Labels
        {
            public static string TruncateLabel(string value, int maxLength)
            {
                return TruncateLabel(value, maxLength, "(Untitled Page)");
            }

            public static string TruncateLabel(string value, int maxLength, string emptyFallback)
            {
                if (string.IsNullOrWhiteSpace(emptyFallback))
                {
                    emptyFallback = Common.Constants.Hyphen;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    return emptyFallback;
                }

                value = value.Trim();

                if (maxLength <= 0)
                {
                    return emptyFallback;
                }

                if (value.Length <= maxLength)
                {
                    return value;
                }

                if (maxLength <= 3)
                {
                    return value.Substring(0, maxLength);
                }

                return value.Substring(0, maxLength - 3) + Common.Constants.Ellipsis;
            }

            public static string FormatChannelLabel(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "(Unknown)";
                }

                value = value.Trim();

                if (value.Equals("Organic Search", StringComparison.OrdinalIgnoreCase))
                {
                    return "Organic Search";
                }

                if (value.Equals("Direct", StringComparison.OrdinalIgnoreCase))
                {
                    return "Direct";
                }

                if (value.Equals("Referral", StringComparison.OrdinalIgnoreCase))
                {
                    return "Referral";
                }

                if (value.Equals("Organic Social", StringComparison.OrdinalIgnoreCase))
                {
                    return "Organic Social";
                }

                if (value.Equals("Unassigned", StringComparison.OrdinalIgnoreCase))
                {
                    return "Unassigned";
                }

                return value;
            }
        }

        public static class Urls
        {
            public static string BuildPageUrl(string baseUrl, string path)
            {
                if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(baseUrl))
                {
                    return null;
                }

                if (!path.StartsWith(Common.Constants.Slash))
                {
                    path = string.Concat(Common.Constants.Slash, path);
                }

                return string.Concat(baseUrl.TrimEnd('/'), path);
            }
        }
    }
}