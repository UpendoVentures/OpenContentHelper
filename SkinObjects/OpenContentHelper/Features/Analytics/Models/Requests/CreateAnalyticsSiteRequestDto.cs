using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Requests
{
    [Serializable]
    public class CreateAnalyticsSiteRequestDto
    {
        public int? PortalId { get; set; }
        public string ClientName { get; set; }
        public string SiteName { get; set; }
        public string DomainName { get; set; }
        public string TimeZoneId { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
    }
}