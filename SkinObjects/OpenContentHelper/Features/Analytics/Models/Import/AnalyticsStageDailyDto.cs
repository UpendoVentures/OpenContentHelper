/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Analytics.Models.Import
{
    [Serializable]
    public class AnalyticsStageDailyDto
    {
        public long StageDailyId { get; set; }
        public int ImportBatchId { get; set; }

        public string SiteKey { get; set; }
        public string DataSourceKey { get; set; }
        public string MetricDateText { get; set; }

        public string UsersText { get; set; }
        public string NewUsersText { get; set; }
        public string SessionsText { get; set; }
        public string EngagedSessionsText { get; set; }
        public string PageViewsText { get; set; }
        public string ScreenPageViewsText { get; set; }
        public string EventCountText { get; set; }
        public string ConversionsText { get; set; }
        public string RevenueText { get; set; }
        public string AvgSessionDurationSecondsText { get; set; }
        public string BounceRateText { get; set; }
        public string EngagementRateText { get; set; }

        public string ValidationStatus { get; set; }
        public string ValidationMessage { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
}