using System.Collections.Generic;

namespace Upendo.Modules.UpendoEventsForm.Data
{
    public class EventFormEditDto
    {
        public int EventId { get; set; }

        public int PortalId { get; set; }

        public int EventStatusId { get; set; }

        public int? OrganizerUserId { get; set; }

        public int? VenueId { get; set; }

        public int? EventCategoryId { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string ShortSummary { get; set; }

        public string FullDescription { get; set; }

        public string StartDateTimeLocalText { get; set; }

        public string EndDateTimeLocalText { get; set; }

        public string TimeZoneId { get; set; }

        public bool IsAllDay { get; set; }

        public string DisplayDateText { get; set; }

        public string DisplayTimeText { get; set; }

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

        public string PublishStartLocalText { get; set; }

        public string PublishEndLocalText { get; set; }

        public int? DownloadFileId { get; set; }

        public string DownloadFileUrl { get; set; }

        public bool UseCustomLocationText { get; set; }

        public string VenueName { get; set; }

        public string PublicLocationText { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string CountryCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string VenueWebsiteUrl { get; set; }

        public string VenuePhoneNumber { get; set; }

        public List<int> TagIds { get; set; }

        public List<EventFeatureInputDto> Features { get; set; }

        public List<EventAudienceInputDto> Audiences { get; set; }
    }
}