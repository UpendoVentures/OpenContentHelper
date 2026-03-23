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

using DotNetNuke.Entities.Modules;
using System;
using System.Web;

namespace Upendo.SkinObjects.OpenContentHelper.Helpers
{
    public static class TemplateHelper
    {
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