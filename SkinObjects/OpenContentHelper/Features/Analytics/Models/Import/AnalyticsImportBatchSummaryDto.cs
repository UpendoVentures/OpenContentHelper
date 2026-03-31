using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Import
{
    [Serializable]
    public class AnalyticsImportBatchSummaryDto
    {
        public int ImportBatchId { get; set; }
        public int TotalRows { get; set; }
        public int ValidRows { get; set; }
        public int InvalidRows { get; set; }
        public int InsertedRows { get; set; }
        public int UpdatedRows { get; set; }
    }
}