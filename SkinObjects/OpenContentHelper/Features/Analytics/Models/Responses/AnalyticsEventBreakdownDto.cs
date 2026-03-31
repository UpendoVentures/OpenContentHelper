using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsEventBreakdownDto
    {
        public string EventName { get; set; }
        public int? EventCount { get; set; }
        public int? Users { get; set; }
        public decimal? Conversions { get; set; }
    }
}