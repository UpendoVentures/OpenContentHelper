using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models
{
    [Serializable]
    public class AnalyticsSiteDto
    {
        public int SiteId { get; set; }
        public int? PortalId { get; set; }
        public Guid SiteGuid { get; set; }
        public string ClientName { get; set; }
        public string SiteName { get; set; }
        public string DomainName { get; set; }
        public string TimeZoneId { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime? LastImportedOnDate { get; set; }
        public string Notes { get; set; }
    }
}