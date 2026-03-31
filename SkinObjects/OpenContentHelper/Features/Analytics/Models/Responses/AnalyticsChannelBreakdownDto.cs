using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsChannelBreakdownDto
    {
        public string ChannelName { get; set; }
        public int? Users { get; set; }
        public int? Sessions { get; set; }
        public int? EngagedSessions { get; set; }
        public decimal? Conversions { get; set; }
        public decimal? Revenue { get; set; }
    }
}