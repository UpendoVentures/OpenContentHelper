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