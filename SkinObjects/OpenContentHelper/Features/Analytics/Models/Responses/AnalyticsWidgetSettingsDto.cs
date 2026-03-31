using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsWidgetSettingsDto
    {
        public int SiteId { get; set; }
        public int? DataSourceId { get; set; }
        public string DashboardTitle { get; set; }
        public string DefaultDateRangeType { get; set; }
        public string DefaultCompareMode { get; set; }
        public bool ShowRevenue { get; set; }
        public bool ShowEvents { get; set; }
        public bool ShowTopPages { get; set; }
        public bool ShowChannels { get; set; }
        public string CardStyle { get; set; }
        public string ChartHeight { get; set; }
    }
}