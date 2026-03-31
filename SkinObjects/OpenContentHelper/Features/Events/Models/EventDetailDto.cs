using Upendo.OpenContentHelper.Features.Events.Models;
using System;
using System.Collections.Generic;

namespace Upendo.OpenContentHelper.Features.Events.Models
{
    public class EventDetailDto
    {
        public EventDetailDto()
        {
            Tags = new List<EventTagDto>();
            Audiences = new List<EventAudienceDto>();
            Features = new List<EventFeatureDto>();
            ContentSections = new List<EventContentSectionDto>();
            RelatedEvents = new List<EventListItemDto>();
        }

        public int EventId { get; set; }

        public int PortalId { get; set; }

        public int EventStatusId { get; set; }

        public string StatusName { get; set; }

        public int? EventCategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategorySlug { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string ShortSummary { get; set; }

        public string FullDescription { get; set; }

        public DateTime StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public string TimeZoneId { get; set; }

        public bool IsAllDay { get; set; }

        public string DisplayDateText { get; set; }

        public string DisplayTimeText { get; set; }

        public bool IsUpcoming => StartDateTimeUtc > DateTime.UtcNow;

        public string HeroImageUrl { get; set; }

        public string ListImageUrl { get; set; }

        public string ThumbnailImageUrl { get; set; }

        public string ImageAltText { get; set; }

        public string RegistrationUrl { get; set; }

        public string RegistrationButtonText { get; set; }

        public string SecondaryCtaUrl { get; set; }

        public string SecondaryCtaText { get; set; }

        public decimal? PriceAmount { get; set; }

        public string PriceCurrencyCode { get; set; }

        public string PriceLabel { get; set; }

        public bool IsFree { get; set; }

        public int? Capacity { get; set; }

        public string CapacitySummaryText { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string ContactUrl { get; set; }

        public string SeoTitle { get; set; }

        public string SeoDescription { get; set; }

        public string CanonicalUrl { get; set; }

        public string MetaRobots { get; set; }

        public int SortOrder { get; set; }

        public bool IsFeatured { get; set; }

        public bool AllowPublicDetailPage { get; set; }

        public DateTime? PublishStartUtc { get; set; }

        public DateTime? PublishEndUtc { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public DateTime LastModifiedOnDate { get; set; }

        public EventVenueDto Venue { get; set; }

        public EventOrganizerDto Organizer { get; set; }

        public EventCalendarMetadataDto CalendarMetadata { get; set; }

        public IList<EventTagDto> Tags { get; set; }

        public IList<EventAudienceDto> Audiences { get; set; }

        public IList<EventFeatureDto> Features { get; set; }

        public IList<EventContentSectionDto> ContentSections { get; set; }

        public IList<EventListItemDto> RelatedEvents { get; set; }
    }
}