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
using Upendo.OpenContentHelper.Features.Events.Helpers;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events
{
    public static class EventFacade
    {
        public static EventListViewModel GetListViewModel(EventListRequest request, string listPagePath, string detailPagePath)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var normalizedRequest = EventRequestHelper.NormalizeListRequest(request);
            var service = EventServiceFactory.Create();

            var results = service.ListEvents(normalizedRequest);
            var categories = service.ListCategoriesForPortal(normalizedRequest.PortalId, normalizedRequest.IsUpcoming);
            var tags = service.ListTrendingTags(normalizedRequest.PortalId, normalizedRequest.IsUpcoming, 10);
            var organizers = service.ListTopOrganizers(normalizedRequest.PortalId, normalizedRequest.IsUpcoming, 5);

            return new EventListViewModel
            {
                Request = normalizedRequest,
                Results = results,
                ListPagePath = EventPageUrlHelper.BuildListPageUrl(listPagePath),
                DetailPagePath = EventPageUrlHelper.BuildDetailBasePageUrl(detailPagePath),
                EmptyStateTitle = "No Events Found",
                EmptyStateText = "There are no events matching the current filters.",
                ActiveCategories = categories,
                TrendingTags = tags,
                TopOrganizers = organizers,
                ActiveCategorySlug = normalizedRequest.CategorySlug,
                ActiveTagSlug = normalizedRequest.TagSlug,
                ActiveOrganizerSlug = normalizedRequest.OrganizerSlug
            };
        }

        public static EventDetailViewModel GetDetailViewModel(int portalId, string slug, string listPagePath, string detailPagePath, int relatedMaxResults)
        {
            if (portalId < 0)
            {
                throw new ArgumentOutOfRangeException("portalId");
            }

            var normalizedSlug = EventRequestHelper.NormalizeSlug(slug);
            var normalizedListPagePath = EventPageUrlHelper.BuildListPageUrl(listPagePath);
            var normalizedDetailPagePath = EventPageUrlHelper.BuildDetailBasePageUrl(detailPagePath);
            var normalizedRelatedMaxResults = EventRequestHelper.NormalizeRelatedMaxResults(relatedMaxResults);

            var vm = new EventDetailViewModel
            {
                ListPagePath = normalizedListPagePath,
                DetailPagePath = normalizedDetailPagePath,
                RequestedSlug = normalizedSlug,
                EventFound = false,
                FriendlyMessageTitle = "We Couldn’t Find That Event",
                FriendlyMessageBody = "The event you were looking for may have been removed, renamed, or the link may be incomplete."
            };

            if (string.IsNullOrWhiteSpace(normalizedSlug))
            {
                vm.FriendlyMessageBody = "The event link appears to be incomplete. Please return to the events page and choose an event.";
                return vm;
            }

            var service = EventServiceFactory.Create();
            var evt = service.GetEventDetail(portalId, normalizedSlug, normalizedRelatedMaxResults);

            if (evt == null)
            {
                return vm;
            }

            vm.Event = evt;
            vm.EventFound = true;
            vm.IcsDownloadUrl = EventPageUrlHelper.BuildIcsDownloadUrl(EventConstants.EndpointCalendar, normalizedSlug);

            return vm;
        }

        public static EventDetailViewModel GetDetailViewModel(int portalId, string slug, string listPagePath, string detailPagePath)
        {
            return GetDetailViewModel(portalId, slug, listPagePath, detailPagePath, 3);
        }

        public static EventDetailDto GetEventSummary(int portalId, string slug)
        {
            if (portalId < 0)
            {
                throw new ArgumentOutOfRangeException("portalId");
            }

            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentException("slug is required.", "slug");
            }

            var service = EventServiceFactory.Create();
            return service.GetEventSummary(portalId, EventRequestHelper.NormalizeSlug(slug));
        }
    }
}