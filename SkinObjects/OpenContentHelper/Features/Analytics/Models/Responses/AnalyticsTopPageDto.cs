using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Responses
{
    [Serializable]
    public class AnalyticsTopPageDto
    {
        public string PagePath { get; set; }
        public string PageTitle { get; set; }
        public int? Entrances { get; set; }
        public int? Views { get; set; }
        public int? Users { get; set; }
        public decimal? Conversions { get; set; }
        public decimal? AvgTimeOnPageSeconds { get; set; }
    }
}