using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Requests
{
    [Serializable]
    public class AnalyticsTopNRequestDto : AnalyticsDateRangeRequestDto
    {
        public int TopN { get; set; }
    }
}