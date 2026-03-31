using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsDashboardDto
    {
        public AnalyticsSummaryDto Summary { get; set; }
        public AnalyticsComparisonSummaryDto ComparisonSummary { get; set; }
        public List<AnalyticsTrendPointDto> Trend { get; set; }
        public List<AnalyticsChannelBreakdownDto> Channels { get; set; }
        public List<AnalyticsEventBreakdownDto> Events { get; set; }
        public List<AnalyticsTopPageDto> TopPages { get; set; }

        public DateTime? LastImportedOnDate { get; set; }
        public DateTime? LatestMetricDate { get; set; }

        public AnalyticsDashboardDto()
        {
            Trend = new List<AnalyticsTrendPointDto>();
            Channels = new List<AnalyticsChannelBreakdownDto>();
            Events = new List<AnalyticsEventBreakdownDto>();
            TopPages = new List<AnalyticsTopPageDto>();
        }
    }
}