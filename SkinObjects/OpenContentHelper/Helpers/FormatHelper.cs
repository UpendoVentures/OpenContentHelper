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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Upendo.SkinObjects.OpenContentHelper.Helpers
{
    public static class FormatHelper
    {
        public static string FormatInt(int? value)
        {
            return value.HasValue ? value.Value.ToString("N0") : Common.Constants.Hyphen;
        }

        public static string FormatDecimal(decimal? value, string format)
        {
            if (!value.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            if (string.IsNullOrWhiteSpace(format))
            {
                format = "N2";
            }

            return value.Value.ToString(format);
        }

        public static string FormatPercent(decimal? value, bool multiplyBy100)
        {
            if (!value.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            decimal output = value.Value;
            if (multiplyBy100)
            {
                output = output * 100M;
            }

            return string.Concat(output.ToString("N2"), Common.Constants.Percent);
        }

        public static string FormatCurrency(decimal? value, string currencyCode)
        {
            if (!value.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            string symbol = GetCurrencySymbol(currencyCode);
            return symbol + value.Value.ToString("N2");
        }

        public static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("MMM d, yyyy") : Common.Constants.Hyphen;
        }

        public static string FormatDurationSeconds(decimal? seconds)
        {
            if (!seconds.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            var totalSeconds = Convert.ToInt32(Math.Round(seconds.Value, MidpointRounding.AwayFromZero));
            if (totalSeconds < 0)
            {
                totalSeconds = 0;
            }

            var ts = TimeSpan.FromSeconds(totalSeconds);

            if (ts.TotalHours >= 1)
            {
                return string.Format("{0}:{1:00}:{2:00}", (int)ts.TotalHours, ts.Minutes, ts.Seconds);
            }

            return string.Format("{0}:{1:00}", ts.Minutes, ts.Seconds);
        }

        public static string FormatSignedInt(int? value)
        {
            if (!value.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            if (value.Value > 0)
            {
                return string.Concat(Common.Constants.Plus, value.Value.ToString("N0"));
            }

            return value.Value.ToString("N0");
        }

        public static string FormatSignedDecimal(decimal? value, string format)
        {
            if (!value.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            if (string.IsNullOrWhiteSpace(format))
            {
                format = "N2";
            }

            if (value.Value > 0)
            {
                return string.Concat(Common.Constants.Plus, value.Value.ToString(format));
            }

            return value.Value.ToString(format);
        }

        public static string FormatSignedPercent(decimal? value)
        {
            if (!value.HasValue)
            {
                return Common.Constants.Hyphen;
            }

            if (value.Value > 0)
            {
                return string.Concat(Common.Constants.Plus + value.Value.ToString("N2"), Common.Constants.Percent);
            }

            return string.Concat(value.Value.ToString("N2"), Common.Constants.Percent);
        }

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

        public static string SafeText(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value;
        }

        private static string GetCurrencySymbol(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return Common.Constants.Dollar;
            }

            currencyCode = currencyCode.Trim().ToUpperInvariant();

            try
            {
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                for (int i = 0; i < cultures.Length; i++)
                {
                    RegionInfo region = new RegionInfo(cultures[i].LCID);
                    if (region != null && string.Equals(region.ISOCurrencySymbol, currencyCode, StringComparison.OrdinalIgnoreCase))
                    {
                        return region.CurrencySymbol;
                    }
                }
            }
            catch
            {
                // Intentionally swallow and fall back below.
            }

            if (currencyCode == "USD") return Common.Constants.Dollar;
            if (currencyCode == "EUR") return "€";
            if (currencyCode == "GBP") return "£";
            if (currencyCode == "CAD") return Common.Constants.Dollar;
            if (currencyCode == "AUD") return Common.Constants.Dollar;

            return string.Concat(currencyCode, Common.Constants.Space);
        }
    }
}