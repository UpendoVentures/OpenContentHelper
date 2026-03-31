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