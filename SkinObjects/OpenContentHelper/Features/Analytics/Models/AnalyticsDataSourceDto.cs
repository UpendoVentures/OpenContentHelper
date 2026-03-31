using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models
{
    [Serializable]
    public class AnalyticsDataSourceDto
    {
        public int DataSourceId { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
}