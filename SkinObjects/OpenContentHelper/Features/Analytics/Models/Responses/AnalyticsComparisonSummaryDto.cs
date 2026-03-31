using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsComparisonSummaryDto
    {
        public int SiteId { get; set; }

        public DateTime CurrentDateStart { get; set; }
        public DateTime CurrentDateEnd { get; set; }
        public DateTime PreviousDateStart { get; set; }
        public DateTime PreviousDateEnd { get; set; }
        public DateTime LastYearDateStart { get; set; }
        public DateTime LastYearDateEnd { get; set; }

        public int? CurrentUsers { get; set; }
        public int? PreviousUsers { get; set; }
        public int? LastYearUsers { get; set; }
        public int? UsersDeltaVsPrevious { get; set; }
        public decimal? UsersDeltaPctVsPrevious { get; set; }
        public int? UsersDeltaVsLastYear { get; set; }
        public decimal? UsersDeltaPctVsLastYear { get; set; }

        public int? CurrentSessions { get; set; }
        public int? PreviousSessions { get; set; }
        public int? LastYearSessions { get; set; }
        public int? SessionsDeltaVsPrevious { get; set; }
        public decimal? SessionsDeltaPctVsPrevious { get; set; }
        public int? SessionsDeltaVsLastYear { get; set; }
        public decimal? SessionsDeltaPctVsLastYear { get; set; }

        public int? CurrentPageViews { get; set; }
        public int? PreviousPageViews { get; set; }
        public int? LastYearPageViews { get; set; }
        public int? PageViewsDeltaVsPrevious { get; set; }
        public decimal? PageViewsDeltaPctVsPrevious { get; set; }
        public int? PageViewsDeltaVsLastYear { get; set; }
        public decimal? PageViewsDeltaPctVsLastYear { get; set; }

        public decimal? CurrentConversions { get; set; }
        public decimal? PreviousConversions { get; set; }
        public decimal? LastYearConversions { get; set; }
        public decimal? ConversionsDeltaVsPrevious { get; set; }
        public decimal? ConversionsDeltaPctVsPrevious { get; set; }
        public decimal? ConversionsDeltaVsLastYear { get; set; }
        public decimal? ConversionsDeltaPctVsLastYear { get; set; }

        public decimal? CurrentRevenue { get; set; }
        public decimal? PreviousRevenue { get; set; }
        public decimal? LastYearRevenue { get; set; }
        public decimal? RevenueDeltaVsPrevious { get; set; }
        public decimal? RevenueDeltaPctVsPrevious { get; set; }
        public decimal? RevenueDeltaVsLastYear { get; set; }
        public decimal? RevenueDeltaPctVsLastYear { get; set; }

        public decimal? CurrentEngagementRate { get; set; }
        public decimal? PreviousEngagementRate { get; set; }
        public decimal? LastYearEngagementRate { get; set; }
        public decimal? EngagementRateDeltaVsPrevious { get; set; }
        public decimal? EngagementRateDeltaVsLastYear { get; set; }

        public decimal? CurrentBounceRate { get; set; }
        public decimal? PreviousBounceRate { get; set; }
        public decimal? LastYearBounceRate { get; set; }
        public decimal? BounceRateDeltaVsPrevious { get; set; }
        public decimal? BounceRateDeltaVsLastYear { get; set; }

        public decimal? CurrentAvgSessionDurationSeconds { get; set; }
        public decimal? PreviousAvgSessionDurationSeconds { get; set; }
        public decimal? LastYearAvgSessionDurationSeconds { get; set; }
        public decimal? AvgSessionDurationDeltaVsPrevious { get; set; }
        public decimal? AvgSessionDurationDeltaVsLastYear { get; set; }

        public int? CurrentDaysWithData { get; set; }
        public int? PreviousDaysWithData { get; set; }
        public int? LastYearDaysWithData { get; set; }

        public bool HasPreviousPeriodData { get; set; }
        public bool HasLastYearData { get; set; }
    }
}