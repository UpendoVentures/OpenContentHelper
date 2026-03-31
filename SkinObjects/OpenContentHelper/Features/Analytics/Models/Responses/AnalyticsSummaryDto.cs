using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsSummaryDto
    {
        public int SiteId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int DaysInRange { get; set; }

        public int? Users { get; set; }
        public int? NewUsers { get; set; }
        public int? Sessions { get; set; }
        public int? EngagedSessions { get; set; }
        public int? PageViews { get; set; }
        public int? ScreenPageViews { get; set; }
        public int? EventCount { get; set; }
        public decimal? Conversions { get; set; }
        public decimal? Revenue { get; set; }
        public decimal? AvgSessionDurationSeconds { get; set; }
        public decimal? BounceRate { get; set; }
        public decimal? EngagementRate { get; set; }

        public DateTime? LatestMetricDate { get; set; }
    }
}