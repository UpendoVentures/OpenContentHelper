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
using System.Globalization;
using System.Web;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventRequestHelper
    {
        public static EventListRequest NormalizeListRequest(EventListRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var normalized = new EventListRequest
            {
                PortalId = request.PortalId,
                PageNumber = NormalizePageNumber(request.PageNumber),
                PageSize = NormalizePageSize(request.PageSize),
                CategorySlug = NormalizeNullableSlug(request.CategorySlug),
                OrganizerSlug = NormalizeNullableSlug(request.OrganizerSlug),
                TagSlug = NormalizeNullableSlug(request.TagSlug),
                Keyword = NormalizeKeyword(request.Keyword),
                IsUpcoming = request.IsUpcoming,
                OnlyFeatured = request.OnlyFeatured,
                SortBy = NormalizeSortBy(request.SortBy)
            };

            return normalized;
        }

        public static int NormalizePageNumber(int pageNumber)
        {
            return pageNumber < 1 ? 1 : pageNumber;
        }

        public static int NormalizePageSize(int pageSize)
        {
            if (pageSize < 1) return 10;
            if (pageSize > 100) return 100;
            return pageSize;
        }

        public static int NormalizeRelatedMaxResults(int relatedMaxResults)
        {
            if (relatedMaxResults < 0) return 0;
            if (relatedMaxResults > 24) return 24;
            return relatedMaxResults;
        }

        public static string NormalizeSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return string.Empty;
            return slug.Trim();
        }

        public static string NormalizeNullableSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;
            return slug.Trim();
        }

        public static string NormalizeKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return null;

            var trimmed = keyword.Trim();

            if (trimmed.Length > 200)
            {
                trimmed = trimmed.Substring(0, 200);
            }

            return trimmed;
        }

        public static string NormalizeSortBy(string sortBy)
        {
            if (string.IsNullOrWhiteSpace(sortBy)) return "date_asc";

            var value = sortBy.Trim().ToLowerInvariant();

            switch (value)
            {
                case "date_desc":
                    return "date_desc";

                case "date_asc":
                default:
                    return "date_asc";
            }
        }

        public static string BuildFilterUrl(EventListRequest request, string categorySlug)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            bool isUpcoming = request == null || request.IsUpcoming;
            query["upcoming"] = isUpcoming ? "true" : "false";

            if (request != null && !string.IsNullOrWhiteSpace(request.TagSlug))
            {
                query["tag"] = request.TagSlug;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.OrganizerSlug))
            {
                query["organizer"] = request.OrganizerSlug;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.Keyword))
            {
                query["q"] = request.Keyword;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.SortBy))
            {
                query["sort"] = request.SortBy;
            }

            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                query["category"] = categorySlug;
            }

            string qs = query.ToString();
            return string.IsNullOrWhiteSpace(qs) ? "?" : "?" + qs;
        }

        public static string BuildPageUrl(EventListRequest request, int pageNumber)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            bool isUpcoming = request == null || request.IsUpcoming;
            query["upcoming"] = isUpcoming ? "true" : "false";

            if (request != null && !string.IsNullOrWhiteSpace(request.CategorySlug))
            {
                query["category"] = request.CategorySlug;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.TagSlug))
            {
                query["tag"] = request.TagSlug;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.OrganizerSlug))
            {
                query["organizer"] = request.OrganizerSlug;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.Keyword))
            {
                query["q"] = request.Keyword;
            }

            if (request != null && !string.IsNullOrWhiteSpace(request.SortBy))
            {
                query["sort"] = request.SortBy;
            }

            if (pageNumber > 1)
            {
                query["page"] = pageNumber.ToString(CultureInfo.InvariantCulture);
            }

            string qs = query.ToString();
            return string.IsNullOrWhiteSpace(qs) ? "?" : "?" + qs;
        }
    }
}