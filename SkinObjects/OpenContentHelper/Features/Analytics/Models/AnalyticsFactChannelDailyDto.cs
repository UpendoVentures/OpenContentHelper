using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models
{
    [Serializable]
    public class AnalyticsFactEventDailyDto
    {
        public long FactEventDailyId { get; set; }
        public int SiteId { get; set; }
        public int DataSourceId { get; set; }
        public DateTime MetricDate { get; set; }
        public string EventName { get; set; }
        public int? EventCount { get; set; }
        public int? Users { get; set; }
        public decimal? Conversions { get; set; }
        public int ImportBatchId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }
}