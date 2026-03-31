using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Requests
{
    [Serializable]
    public class CreateAnalyticsImportBatchRequestDto
    {
        public int SiteId { get; set; }
        public int DataSourceId { get; set; }
        public string ImportType { get; set; }
        public string SourceFileName { get; set; }
        public string SourceSheetName { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? ImportedByUserId { get; set; }
        public string Notes { get; set; }
    }
}