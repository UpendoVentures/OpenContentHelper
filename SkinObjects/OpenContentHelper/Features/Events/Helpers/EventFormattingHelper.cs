using System;
using System.Collections.Generic;
using System.Globalization;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventFormattingHelper
    {
        public static string GetDisplayDate(EventListItemDto item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.DisplayDateText)) return item.DisplayDateText;

            return item.StartDateTimeUtc.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string GetDisplayDate(EventDetailDto item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.DisplayDateText)) return item.DisplayDateText;

            return item.StartDateTimeUtc.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string GetDisplayTime(EventListItemDto item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.DisplayTimeText)) return item.DisplayTimeText;
            if (item.IsAllDay) return "All day";

            return item.StartDateTimeUtc.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }

        public static string GetDisplayTime(EventDetailDto item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.DisplayTimeText)) return item.DisplayTimeText;
            if (item.IsAllDay) return "All day";

            return item.StartDateTimeUtc.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }

        public static string GetDisplayPrice(EventDetailDto item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.PriceLabel)) return item.PriceLabel;
            if (item.IsFree) return "Free";
            if (!item.PriceAmount.HasValue) return string.Empty;

            return item.PriceAmount.Value.ToString("C", CultureInfo.CurrentCulture);
        }

        public static string GetDisplayPrice(EventListItemDto item)
        {
            if (item == null) return string.Empty;
            if (!string.IsNullOrWhiteSpace(item.PriceLabel)) return item.PriceLabel;
            if (item.IsFree) return "Free";
            if (!item.PriceAmount.HasValue) return string.Empty;

            return item.PriceAmount.Value.ToString("C", CultureInfo.CurrentCulture);
        }

        public static string GetAbsoluteDisplayDate(DateTime value)
        {
            return value.ToString("dddd, MMMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string GetShortDisplayDate(DateTime value)
        {
            return value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string GetShortAccessibleDisplayDate(DateTime value)
        {
            return value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
        }

        public static string GetIsoDateTime(DateTime value)
        {
            return value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }

        public static string GetIsoDateTimeNullable(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)
                : string.Empty;
        }

        public static string GetImageAltText(EventListItemDto item)
        {
            if (item == null)
            {
                return "Event image";
            }

            if (!string.IsNullOrWhiteSpace(item.ImageAltText))
            {
                return item.ImageAltText;
            }

            if (!string.IsNullOrWhiteSpace(item.Title))
            {
                return item.Title;
            }

            return "Event image";
        }

        public static string GetImageAltText(EventDetailDto item)
        {
            if (item == null)
            {
                return "Event image";
            }

            if (!string.IsNullOrWhiteSpace(item.ImageAltText))
            {
                return item.ImageAltText;
            }

            if (!string.IsNullOrWhiteSpace(item.Title))
            {
                return item.Title;
            }

            return "Event image";
        }

        public static string GetLocationText(EventListItemDto item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(item.PublicLocationText))
            {
                return item.PublicLocationText;
            }

            if (!string.IsNullOrWhiteSpace(item.VenueName))
            {
                return item.VenueName;
            }

            if (!string.IsNullOrWhiteSpace(item.City) && !string.IsNullOrWhiteSpace(item.Region))
            {
                return item.City + ", " + item.Region;
            }

            if (!string.IsNullOrWhiteSpace(item.City))
            {
                return item.City;
            }

            return item.Region ?? string.Empty;
        }

        public static string GetBestLocationText(EventDetailDto item)
        {
            if (item == null || item.Venue == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(item.Venue.PublicLocationText))
            {
                return item.Venue.PublicLocationText;
            }

            if (!string.IsNullOrWhiteSpace(item.Venue.VenueName))
            {
                return item.Venue.VenueName;
            }

            return item.Venue.FullDisplayLocation ?? string.Empty;
        }

        public static string GetVenueAddressText(EventVenueDto venue)
        {
            if (venue == null)
            {
                return string.Empty;
            }

            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(venue.AddressLine1))
            {
                parts.Add(venue.AddressLine1);
            }

            if (!string.IsNullOrWhiteSpace(venue.AddressLine2))
            {
                parts.Add(venue.AddressLine2);
            }

            if (!string.IsNullOrWhiteSpace(venue.City))
            {
                parts.Add(venue.City);
            }

            if (!string.IsNullOrWhiteSpace(venue.Region))
            {
                parts.Add(venue.Region);
            }

            if (!string.IsNullOrWhiteSpace(venue.PostalCode))
            {
                parts.Add(venue.PostalCode);
            }

            if (!string.IsNullOrWhiteSpace(venue.CountryCode))
            {
                parts.Add(venue.CountryCode.ToUpperInvariant());
            }

            return string.Join(", ", parts);
        }
    }
}