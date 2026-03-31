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

using System.Collections.Generic;
using Upendo.OpenContentHelper.Features.Analytics.Models;
using Upendo.OpenContentHelper.Features.Analytics.Models.Import;
using Upendo.OpenContentHelper.Features.Analytics.Models.Requests;
using Upendo.OpenContentHelper.Features.Analytics.Models.Responses;

namespace Upendo.OpenContentHelper.Features.Analytics
{
    public interface IAnalyticsService
    {
        AnalyticsSiteDto GetSite(int siteId);
        IEnumerable<AnalyticsSiteDto> GetSites();
        int CreateSite(CreateAnalyticsSiteRequestDto request);
        void UpdateSite(UpdateAnalyticsSiteRequestDto request);
        void DeleteSite(int siteId);

        AnalyticsDataSourceDto GetDataSource(int dataSourceId);
        IEnumerable<AnalyticsDataSourceDto> GetDataSources();
        int CreateDataSource(CreateAnalyticsDataSourceRequestDto request);
        void UpdateDataSource(UpdateAnalyticsDataSourceRequestDto request);
        void DeleteDataSource(int dataSourceId);

        AnalyticsImportBatchDto GetImportBatch(int importBatchId);
        int CreateImportBatch(CreateAnalyticsImportBatchRequestDto request);

        AnalyticsImportBatchSummaryDto ValidateStageDaily(int importBatchId);
        IEnumerable<AnalyticsStageDailyValidationResultDto> GetStageDailyValidationResults(int importBatchId);
        AnalyticsImportBatchSummaryDto MergeFactDailyFromStage(int importBatchId);

        AnalyticsSummaryDto GetSummary(AnalyticsDateRangeRequestDto request);
        AnalyticsComparisonSummaryDto GetComparisonSummary(AnalyticsDateRangeRequestDto request);
        IEnumerable<AnalyticsTrendPointDto> GetTrend(AnalyticsDateRangeRequestDto request);
        IEnumerable<AnalyticsChannelBreakdownDto> GetChannelBreakdown(AnalyticsDateRangeRequestDto request);
        IEnumerable<AnalyticsEventBreakdownDto> GetEventBreakdown(AnalyticsTopNRequestDto request);
        IEnumerable<AnalyticsTopPageDto> GetTopPages(AnalyticsTopNRequestDto request);
    }
}