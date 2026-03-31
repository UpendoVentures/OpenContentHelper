using System;
using DotNetNuke.Common.Utilities;
using Upendo.OpenContentHelper.Features.Analytics.Models.Requests;

namespace Upendo.OpenContentHelper.Features.Analytics
{
    internal static class AnalyticsCacheHelper
    {
        private const string Prefix = "UOCH:Analytics";

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
            // IMPORTANT NOTE: 
            // Currently does not cache requests where `null` is returned.
            // This is okay for now, but it might be worth considering caching `null` values in the future to prevent repeated expensive operations.

            return value;
        }

        internal static string Site(int siteId)
        {
            return $"{Prefix}:Site:{siteId}";
        }

        internal static string Sites()
        {
            return $"{Prefix}:Sites:All";
        }

        internal static string DataSource(int dataSourceId)
        {
            return $"{Prefix}:DataSource:{dataSourceId}";
        }

        internal static string DataSources()
        {
            return $"{Prefix}:DataSources:All";
        }

        internal static string Summary(AnalyticsDateRangeRequestDto request)
        {
            var version = GetReportVersion(request.SiteId);
            return $"{Prefix}:Summary:V:{version}:{BuildRangeToken(request)}";
        }

        internal static string ComparisonSummary(AnalyticsDateRangeRequestDto request)
        {
            var version = GetReportVersion(request.SiteId);
            return $"{Prefix}:ComparisonSummary:V:{version}:{BuildRangeToken(request)}";
        }

        internal static string Trend(AnalyticsDateRangeRequestDto request)
        {
            var version = GetReportVersion(request.SiteId);
            return $"{Prefix}:Trend:V:{version}:{BuildRangeToken(request)}";
        }

        internal static string ChannelBreakdown(AnalyticsDateRangeRequestDto request)
        {
            var version = GetReportVersion(request.SiteId);
            return $"{Prefix}:Channels:V:{version}:{BuildRangeToken(request)}";
        }

        internal static string EventBreakdown(AnalyticsTopNRequestDto request)
        {
            var version = GetReportVersion(request.SiteId);
            return $"{Prefix}:Events:V:{version}:{BuildTopNToken(request)}";
        }

        internal static string TopPages(AnalyticsTopNRequestDto request)
        {
            var version = GetReportVersion(request.SiteId);
            return $"{Prefix}:TopPages:V:{version}:{BuildTopNToken(request)}";
        }

        private static string BuildRangeToken(AnalyticsDateRangeRequestDto request)
        {
            var source = request.DataSourceId.HasValue ? request.DataSourceId.Value.ToString() : "all";
            return $"Site:{request.SiteId}:Source:{source}:Start:{request.DateStart:yyyyMMdd}:End:{request.DateEnd:yyyyMMdd}";
        }

        private static string BuildTopNToken(AnalyticsTopNRequestDto request)
        {
            return $"{BuildRangeToken(request)}:Top:{request.TopN}";
        }

        internal static void Remove(string cacheKey)
        {
            if (!string.IsNullOrWhiteSpace(cacheKey))
            {
                DataCache.RemoveCache(cacheKey);
            }
        }

        internal static string GetReportVersion(int siteId)
        {
            var cacheKey = ReportVersion(siteId);
            var version = DataCache.GetCache(cacheKey) as string;

            if (string.IsNullOrWhiteSpace(version))
            {
                version = DateTime.UtcNow.Ticks.ToString();
                DataCache.SetCache(cacheKey, version, TimeSpan.FromDays(30));
            }

            return version;
        }

        internal static void BumpReportVersion(int siteId)
        {
            var cacheKey = ReportVersion(siteId);
            var version = DateTime.UtcNow.Ticks.ToString(); // makes accidental namespace collisions far less likely
            DataCache.SetCache(cacheKey, version, TimeSpan.FromDays(30));
        }

        internal static string ReportVersion(int siteId)
        {
            return $"{Prefix}:ReportVersion:Site:{siteId}";
        }
    }
}