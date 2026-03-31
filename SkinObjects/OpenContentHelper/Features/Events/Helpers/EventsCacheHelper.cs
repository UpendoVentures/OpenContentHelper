using DotNetNuke.Common.Utilities;
using System;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    internal static class EventsCacheHelper
    {
        private const string Prefix = "UOCH:Events";

        private sealed class CachedNullMarker
        {
            internal static readonly CachedNullMarker Value = new CachedNullMarker();
        }

        internal static T GetOrAdd<T>(string cacheKey, TimeSpan duration, Func<T> factory)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentException("cacheKey is required.");
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var cached = DataCache.GetCache(cacheKey) as T;
            if (cached != null)
            {
                return cached;
            }

            var value = factory();

            if (value != null)
            {
                DataCache.SetCache(cacheKey, value, duration);
            }

            return value;
        }

        internal static T GetOrAddNullable<T>(string cacheKey, TimeSpan duration, Func<T> factory)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentException("cacheKey is required.");
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var cached = DataCache.GetCache(cacheKey);

            if (ReferenceEquals(cached, CachedNullMarker.Value))
            {
                return null;
            }

            var typed = cached as T;
            if (typed != null)
            {
                return typed;
            }

            var value = factory();

            DataCache.SetCache(
                cacheKey,
                value ?? (object)CachedNullMarker.Value,
                duration);

            return value;
        }

        internal static string List(EventListRequest request)
        {
            var version = GetPortalVersion(request.PortalId);

            return string.Format(
                "{0}:List:V:{1}:Portal:{2}:Page:{3}:Size:{4}:Category:{5}:Organizer:{6}:Tag:{7}:Keyword:{8}:Upcoming:{9}:Featured:{10}:Sort:{11}",
                Prefix,
                version,
                request.PortalId,
                request.PageNumber,
                request.PageSize,
                Tokenize(request.CategorySlug),
                Tokenize(request.OrganizerSlug),
                Tokenize(request.TagSlug),
                Tokenize(request.Keyword),
                request.IsUpcoming ? "1" : "0",
                request.OnlyFeatured ? "1" : "0",
                Tokenize(request.SortBy));
        }

        internal static string Detail(int portalId, string slug, int relatedMaxResults)
        {
            var version = GetPortalVersion(portalId);

            return string.Format(
                "{0}:Detail:V:{1}:Portal:{2}:Slug:{3}:Related:{4}",
                Prefix,
                version,
                portalId,
                Tokenize(slug),
                relatedMaxResults);
        }

        internal static string Summary(int portalId, string slug)
        {
            var version = GetPortalVersion(portalId);

            return string.Format(
                "{0}:Summary:V:{1}:Portal:{2}:Slug:{3}",
                Prefix,
                version,
                portalId,
                Tokenize(slug));
        }

        internal static string Categories(int portalId, bool isUpcoming)
        {
            var version = GetPortalVersion(portalId);

            return string.Format(
                "{0}:Categories:V:{1}:Portal:{2}:Upcoming:{3}",
                Prefix,
                version,
                portalId,
                isUpcoming ? "1" : "0");
        }

        internal static string TrendingTags(int portalId, bool isUpcoming, int maxResults)
        {
            var version = GetPortalVersion(portalId);

            return string.Format(
                "{0}:TrendingTags:V:{1}:Portal:{2}:Upcoming:{3}:Top:{4}",
                Prefix,
                version,
                portalId,
                isUpcoming ? "1" : "0",
                maxResults);
        }

        internal static string TopOrganizers(int portalId, bool isUpcoming, int maxResults)
        {
            var version = GetPortalVersion(portalId);

            return string.Format(
                "{0}:TopOrganizers:V:{1}:Portal:{2}:Upcoming:{3}:Top:{4}",
                Prefix,
                version,
                portalId,
                isUpcoming ? "1" : "0",
                maxResults);
        }

        internal static string PortalVersion(int portalId)
        {
            return string.Format("{0}:PortalVersion:{1}", Prefix, portalId);
        }

        internal static string GetPortalVersion(int portalId)
        {
            var cacheKey = PortalVersion(portalId);
            var version = DataCache.GetCache(cacheKey) as string;

            if (string.IsNullOrWhiteSpace(version))
            {
                version = DateTime.UtcNow.Ticks.ToString();
                DataCache.SetCache(cacheKey, version, TimeSpan.FromDays(30));
            }

            return version;
        }

        internal static void BumpPortalVersion(int portalId)
        {
            if (portalId <= 0)
            {
                return;
            }

            var cacheKey = PortalVersion(portalId);
            var version = DateTime.UtcNow.Ticks.ToString();
            DataCache.SetCache(cacheKey, version, TimeSpan.FromDays(30));
        }

        internal static void Remove(string cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                DataCache.RemoveCache(cacheKey);
            }
        }

        private static string Tokenize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "all";
            }

            return Uri.EscapeDataString(value.Trim().ToLowerInvariant());
        }
    }
}