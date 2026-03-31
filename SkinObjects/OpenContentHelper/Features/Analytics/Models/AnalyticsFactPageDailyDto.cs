using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models
{
    [Serializable]
    public class AnalyticsFactPageDailyDto
    {
        public long FactPageDailyId { get; set; }
        public int SiteId { get; set; }
        public int DataSourceId { get; set; }
        public DateTime MetricDate { get; set; }
        public string PagePath { get; set; }
        public string PageTitle { get; set; }
        public int? Entrances { get; set; }
        public int? Views { get; set; }
        public int? Users { get; set; }
        public decimal? AvgTimeOnPageSeconds { get; set; }
        public decimal? Conversions { get; set; }
        public int ImportBatchId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime? LastModifiedOnDate { get; set; }
    }
}