using System;
using System.Collections.Generic;

namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventListItemDto
    {
        public EventListItemDto()
        {
            Tags = new List<EventTagDto>();
        }
        public int EventId { get; set; }
        public int PortalId { get; set; }
        public int EventStatusId { get; set; }

        public int? EventCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }

        public int? VenueId { get; set; }
        public string VenueName { get; set; }
        public string PublicLocationText { get; set; }
        public string City { get; set; }
        public string Region { get; set; }

        public string Title { get; set; }
        public string Slug { get; set; }
        public string ShortSummary { get; set; }

        public DateTime StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
        public string TimeZoneId { get; set; }
        public bool IsAllDay { get; set; }
        public string DisplayDateText { get; set; }
        public string DisplayTimeText { get; set; }

        public string ListImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public string ImageAltText { get; set; }

        public string RegistrationUrl { get; set; }
        public string RegistrationButtonText { get; set; }

        public decimal? PriceAmount { get; set; }
        public string PriceCurrencyCode { get; set; }
        public string PriceLabel { get; set; }
        public bool IsFree { get; set; }

        public int SortOrder { get; set; }
        public bool IsFeatured { get; set; }

        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public IList<EventTagDto> Tags { get; set; }
    }
}