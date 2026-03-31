using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models
{
    [Serializable]
    public class AnalyticsImportBatchDto
    {
        public int ImportBatchId { get; set; }
        public int SiteId { get; set; }
        public int DataSourceId { get; set; }
        public Guid BatchGuid { get; set; }
        public string SourceFileName { get; set; }
        public string SourceSheetName { get; set; }
        public string ImportType { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Status { get; set; }
        public int? ImportedByUserId { get; set; }
        public DateTime ImportedOnDate { get; set; }
        public int? RecordCountRaw { get; set; }
        public int? RecordCountInserted { get; set; }
        public int? RecordCountUpdated { get; set; }
        public int? RecordCountRejected { get; set; }
        public string Notes { get; set; }
    }
}