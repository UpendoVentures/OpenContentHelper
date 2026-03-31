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