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
using System.Linq;
using Upendo.OpenContentHelper.Features.Events.Data;
using Upendo.OpenContentHelper.Features.Events.Helpers;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            _repository = repository;
        }

        public PagedResult<EventListItemDto> ListEvents(EventListRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                request.SortBy = "date_asc";
            }

            return EventsCacheHelper.GetOrAdd(
                EventsCacheHelper.List(request),
                TimeSpan.FromMinutes(5),
                () => _repository.ListPaged(request));
        }

        public EventDetailDto GetEventDetail(int portalId, string slug, int relatedMaxResults)
        {
            if (portalId < 0) return null;
            if (string.IsNullOrWhiteSpace(slug)) return null;

            if (relatedMaxResults < 0) relatedMaxResults = 0;
            if (relatedMaxResults > 24) relatedMaxResults = 24;

            var normalizedSlug = slug.Trim();

            return EventsCacheHelper.GetOrAddNullable(
                EventsCacheHelper.Detail(portalId, normalizedSlug, relatedMaxResults),
                TimeSpan.FromMinutes(5),
                () => _repository.GetDetail(portalId, normalizedSlug, relatedMaxResults));
        }

        public EventDetailDto GetEventSummary(int portalId, string slug)
        {
            if (portalId < 0) return null;
            if (string.IsNullOrWhiteSpace(slug)) return null;

            var normalizedSlug = slug.Trim();

            return EventsCacheHelper.GetOrAddNullable(
                EventsCacheHelper.Summary(portalId, normalizedSlug),
                TimeSpan.FromMinutes(5),
                () => _repository.GetBySlug(portalId, normalizedSlug));
        }

        public IList<EventCategoryDto> ListCategoriesForPortal(int portalId, bool isUpcoming)
        {
            if (portalId < 0) return new List<EventCategoryDto>();

            return EventsCacheHelper.GetOrAdd(
                EventsCacheHelper.Categories(portalId, isUpcoming),
                TimeSpan.FromMinutes(15),
                () => _repository.ListCategoriesForPortal(portalId, isUpcoming).ToList());
        }

        public IList<EventTagDto> ListTrendingTags(int portalId, bool isUpcoming, int maxResults)
        {
            if (portalId < 0) return new List<EventTagDto>();
            if (maxResults < 1) maxResults = 10;
            if (maxResults > 100) maxResults = 100;

            return EventsCacheHelper.GetOrAdd(
                EventsCacheHelper.TrendingTags(portalId, isUpcoming, maxResults),
                TimeSpan.FromMinutes(10),
                () => _repository.ListTrendingTags(portalId, isUpcoming, maxResults).ToList());
        }

        public IList<EventOrganizerDto> ListTopOrganizers(int portalId, bool isUpcoming, int maxResults)
        {
            if (portalId < 0) return new List<EventOrganizerDto>();
            if (maxResults < 1) maxResults = 5;
            if (maxResults > 100) maxResults = 100;

            return EventsCacheHelper.GetOrAdd(
                EventsCacheHelper.TopOrganizers(portalId, isUpcoming, maxResults),
                TimeSpan.FromMinutes(10),
                () => _repository.ListTopOrganizers(portalId, isUpcoming, maxResults).ToList());
        }
    }
}