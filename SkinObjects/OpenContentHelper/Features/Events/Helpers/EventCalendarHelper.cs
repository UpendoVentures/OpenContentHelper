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