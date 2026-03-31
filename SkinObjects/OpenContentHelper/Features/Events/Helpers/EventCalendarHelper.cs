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

using Upendo.OpenContentHelper.Features.Events.Models;
using System;
using System.Globalization;
using System.Text;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventCalendarHelper
    {
        public static string BuildIcs(EventDetailDto item)
        {
            if (item == null) return string.Empty;

            var uid = item.CalendarMetadata != null && !string.IsNullOrWhiteSpace(item.CalendarMetadata.IcsUid)
                ? item.CalendarMetadata.IcsUid
                : item.EventId + "@runsonupendo";

            var location = item.CalendarMetadata != null && !string.IsNullOrWhiteSpace(item.CalendarMetadata.LocationText)
                ? item.CalendarMetadata.LocationText
                : (item.Venue != null ? item.Venue.FullDisplayLocation : string.Empty);

            var description = item.CalendarMetadata != null && !string.IsNullOrWhiteSpace(item.CalendarMetadata.CalendarDescriptionText)
                ? item.CalendarMetadata.CalendarDescriptionText
                : item.ShortSummary;

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//Upendo-Ventures//Clients//Events//EN");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("UID:" + Escape(uid));
            sb.AppendLine("DTSTAMP:" + ToUtcString(DateTime.UtcNow));
            sb.AppendLine("DTSTART:" + ToUtcString(item.StartDateTimeUtc));

            if (item.EndDateTimeUtc.HasValue)
            {
                sb.AppendLine("DTEND:" + ToUtcString(item.EndDateTimeUtc.Value));
            }

            sb.AppendLine("SUMMARY:" + Escape(item.Title));
            sb.AppendLine("DESCRIPTION:" + Escape(description));
            sb.AppendLine("LOCATION:" + Escape(location));

            if (item.CalendarMetadata != null && !string.IsNullOrWhiteSpace(item.CalendarMetadata.RecurrenceRule))
            {
                sb.AppendLine("RRULE:" + item.CalendarMetadata.RecurrenceRule);
            }

            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");

            return sb.ToString();
        }

        private static string ToUtcString(DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
        }

        private static string Escape(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            return input
                .Replace(@"\", @"\\")
                .Replace(";", @"\;")
                .Replace(",", @"\,")
                .Replace("\r\n", @"\n")
                .Replace("\n", @"\n");
        }
    }
}