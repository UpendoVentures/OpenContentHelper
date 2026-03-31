using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Requests
{
    [Serializable]
    public class UpdateAnalyticsDataSourceRequestDto
    {
        public int DataSourceId { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public bool IsActive { get; set; }
    }
}