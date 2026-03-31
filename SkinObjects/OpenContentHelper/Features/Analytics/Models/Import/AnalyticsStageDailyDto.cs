using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Import
{
    [Serializable]
    public class AnalyticsStageDailyDto
    {
        public long StageDailyId { get; set; }
        public int ImportBatchId { get; set; }

        public string SiteKey { get; set; }
        public string DataSourceKey { get; set; }
        public string MetricDateText { get; set; }

        public string UsersText { get; set; }
        public string NewUsersText { get; set; }
        public string SessionsText { get; set; }
        public string EngagedSessionsText { get; set; }
        public string PageViewsText { get; set; }
        public string ScreenPageViewsText { get; set; }
        public string EventCountText { get; set; }
        public string ConversionsText { get; set; }
        public string RevenueText { get; set; }
        public string AvgSessionDurationSecondsText { get; set; }
        public string BounceRateText { get; set; }
        public string EngagementRateText { get; set; }

        public string ValidationStatus { get; set; }
        public string ValidationMessage { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
}