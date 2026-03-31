using System;
using System.Collections.Generic;
using System.Linq;
using Upendo.OpenContentHelper.Features.Analytics.Models;
using Upendo.OpenContentHelper.Features.Analytics.Models.Import;
using Upendo.OpenContentHelper.Features.Analytics.Models.Requests;
using Upendo.OpenContentHelper.Features.Analytics.Models.Responses;

namespace Upendo.OpenContentHelper.Features.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _repository;

        public AnalyticsService()
            : this(new AnalyticsRepository())
        {
        }

        public AnalyticsService(IAnalyticsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public AnalyticsSiteDto GetSite(int siteId)
        {
            if (siteId <= 0) throw new ArgumentOutOfRangeException(nameof(siteId));

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.Site(siteId),
                TimeSpan.FromMinutes(60),
                () => _repository.GetSite(siteId));
        }

        public IEnumerable<AnalyticsSiteDto> GetSites()
        {
            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.Sites(),
                TimeSpan.FromMinutes(30),
                () => _repository.GetSites().ToList());
        }

        public int CreateSite(CreateAnalyticsSiteRequestDto request)
        {
            ValidateCreateSiteRequest(request);

            var siteId = _repository.CreateSite(request);

            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Sites());
            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Site(siteId));

            return siteId;
        }

        public void UpdateSite(UpdateAnalyticsSiteRequestDto request)
        {
            ValidateUpdateSiteRequest(request);
            _repository.UpdateSite(request);

            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Sites());
            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Site(request.SiteId));
        }

        public void DeleteSite(int siteId)
        {
            if (siteId <= 0) throw new ArgumentOutOfRangeException(nameof(siteId));
            _repository.DeleteSite(siteId);

            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Sites());
            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Site(siteId));
        }

        public AnalyticsDataSourceDto GetDataSource(int dataSourceId)
        {
            if (dataSourceId <= 0) throw new ArgumentOutOfRangeException(nameof(dataSourceId));

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.DataSource(dataSourceId),
                TimeSpan.FromMinutes(60),
                () => _repository.GetDataSource(dataSourceId));
        }

        public IEnumerable<AnalyticsDataSourceDto> GetDataSources()
        {
            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.DataSources(),
                TimeSpan.FromMinutes(30),
                () => _repository.GetDataSources().ToList());
        }

        public int CreateDataSource(CreateAnalyticsDataSourceRequestDto request)
        {
            ValidateCreateDataSourceRequest(request);

            var dataSourceId = _repository.CreateDataSource(request);

            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSources());
            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSource(dataSourceId));

            return dataSourceId;
        }

        public void UpdateDataSource(UpdateAnalyticsDataSourceRequestDto request)
        {
            ValidateUpdateDataSourceRequest(request);
            _repository.UpdateDataSource(request);

            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSources());
            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSource(request.DataSourceId));
        }

        public void DeleteDataSource(int dataSourceId)
        {
            if (dataSourceId <= 0) throw new ArgumentOutOfRangeException(nameof(dataSourceId));
            _repository.DeleteDataSource(dataSourceId);

            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSources());
            AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSource(dataSourceId));
        }

        public AnalyticsImportBatchDto GetImportBatch(int importBatchId)
        {
            if (importBatchId <= 0) throw new ArgumentOutOfRangeException(nameof(importBatchId));
            return _repository.GetImportBatch(importBatchId);
        }

        public int CreateImportBatch(CreateAnalyticsImportBatchRequestDto request)
        {
            ValidateCreateImportBatchRequest(request);
            return _repository.CreateImportBatch(request);
        }

        public AnalyticsImportBatchSummaryDto ValidateStageDaily(int importBatchId)
        {
            if (importBatchId <= 0) throw new ArgumentOutOfRangeException(nameof(importBatchId));
            return _repository.ValidateStageDaily(importBatchId);
        }

        public IEnumerable<AnalyticsStageDailyValidationResultDto> GetStageDailyValidationResults(int importBatchId)
        {
            if (importBatchId <= 0) throw new ArgumentOutOfRangeException(nameof(importBatchId));
            return _repository.GetStageDailyValidationResults(importBatchId);
        }

        public AnalyticsImportBatchSummaryDto MergeFactDailyFromStage(int importBatchId)
        {
            if (importBatchId <= 0) throw new ArgumentOutOfRangeException(nameof(importBatchId));

            var batch = _repository.GetImportBatch(importBatchId);
            var result = _repository.MergeFactDailyFromStage(importBatchId);

            if (batch != null)
            {
                AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.Site(batch.SiteId));

                if (batch.DataSourceId > 0)
                {
                    AnalyticsCacheHelper.Remove(AnalyticsCacheHelper.DataSource(batch.DataSourceId));
                }

                AnalyticsCacheHelper.BumpReportVersion(batch.SiteId);
            }

            return result;
        }

        public AnalyticsSummaryDto GetSummary(AnalyticsDateRangeRequestDto request)
        {
            ValidateDateRangeRequest(request);

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.Summary(request),
                TimeSpan.FromMinutes(5),
                () => _repository.GetSummary(request));
        }

        public AnalyticsComparisonSummaryDto GetComparisonSummary(AnalyticsDateRangeRequestDto request)
        {
            ValidateDateRangeRequest(request);

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.ComparisonSummary(request),
                TimeSpan.FromMinutes(5),
                () => _repository.GetComparisonSummary(request));
        }

        public IEnumerable<AnalyticsTrendPointDto> GetTrend(AnalyticsDateRangeRequestDto request)
        {
            ValidateDateRangeRequest(request);

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.Trend(request),
                TimeSpan.FromMinutes(5),
                () => _repository.GetTrend(request).ToList());
        }

        public IEnumerable<AnalyticsChannelBreakdownDto> GetChannelBreakdown(AnalyticsDateRangeRequestDto request)
        {
            ValidateDateRangeRequest(request);

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.ChannelBreakdown(request),
                TimeSpan.FromMinutes(5),
                () => _repository.GetChannelBreakdown(request).ToList());
        }

        public IEnumerable<AnalyticsEventBreakdownDto> GetEventBreakdown(AnalyticsTopNRequestDto request)
        {
            var normalizedRequest = ValidateTopNRequest(request);

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.EventBreakdown(normalizedRequest),
                TimeSpan.FromMinutes(5),
                () => _repository.GetEventBreakdown(normalizedRequest).ToList());
        }

        public IEnumerable<AnalyticsTopPageDto> GetTopPages(AnalyticsTopNRequestDto request)
        {
            var normalizedRequest = ValidateTopNRequest(request);

            return AnalyticsCacheHelper.GetOrAdd(
                AnalyticsCacheHelper.TopPages(normalizedRequest),
                TimeSpan.FromMinutes(5),
                () => _repository.GetTopPages(normalizedRequest).ToList());
        }

        private static void ValidateCreateSiteRequest(CreateAnalyticsSiteRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.ClientName)) throw new ArgumentException("ClientName is required.");
            if (string.IsNullOrWhiteSpace(request.SiteName)) throw new ArgumentException("SiteName is required.");
            if (string.IsNullOrWhiteSpace(request.DomainName)) throw new ArgumentException("DomainName is required.");
        }

        private static void ValidateUpdateSiteRequest(UpdateAnalyticsSiteRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.SiteId <= 0) throw new ArgumentOutOfRangeException(nameof(request.SiteId));
            if (string.IsNullOrWhiteSpace(request.ClientName)) throw new ArgumentException("ClientName is required.");
            if (string.IsNullOrWhiteSpace(request.SiteName)) throw new ArgumentException("SiteName is required.");
            if (string.IsNullOrWhiteSpace(request.DomainName)) throw new ArgumentException("DomainName is required.");
        }

        private static void ValidateCreateDataSourceRequest(CreateAnalyticsDataSourceRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.SourceName)) throw new ArgumentException("SourceName is required.");
            if (string.IsNullOrWhiteSpace(request.SourceType)) throw new ArgumentException("SourceType is required.");
        }

        private static void ValidateUpdateDataSourceRequest(UpdateAnalyticsDataSourceRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.DataSourceId <= 0) throw new ArgumentOutOfRangeException(nameof(request.DataSourceId));
            if (string.IsNullOrWhiteSpace(request.SourceName)) throw new ArgumentException("SourceName is required.");
            if (string.IsNullOrWhiteSpace(request.SourceType)) throw new ArgumentException("SourceType is required.");
        }

        private static void ValidateCreateImportBatchRequest(CreateAnalyticsImportBatchRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.SiteId <= 0) throw new ArgumentOutOfRangeException(nameof(request.SiteId));
            if (request.DataSourceId <= 0) throw new ArgumentOutOfRangeException(nameof(request.DataSourceId));
            if (string.IsNullOrWhiteSpace(request.ImportType)) throw new ArgumentException("ImportType is required.");

            if (request.DateStart.HasValue && request.DateEnd.HasValue && request.DateEnd.Value.Date < request.DateStart.Value.Date)
            {
                throw new ArgumentException("DateEnd must be greater than or equal to DateStart.");
            }
        }

        private static void ValidateDateRangeRequest(AnalyticsDateRangeRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.SiteId <= 0) throw new ArgumentOutOfRangeException(nameof(request.SiteId));
            if (request.DateEnd.Date < request.DateStart.Date) throw new ArgumentException("DateEnd must be greater than or equal to DateStart.");
            if (request.DataSourceId.HasValue && request.DataSourceId.Value <= 0) throw new ArgumentOutOfRangeException(nameof(request.DataSourceId));
        }

        private static AnalyticsTopNRequestDto ValidateTopNRequest(AnalyticsTopNRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            ValidateDateRangeRequest(request);

            return NormalizeTopNRequest(request);
        }

        private static AnalyticsTopNRequestDto NormalizeTopNRequest(AnalyticsTopNRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return new AnalyticsTopNRequestDto
            {
                SiteId = request.SiteId,
                DataSourceId = request.DataSourceId,
                DateStart = request.DateStart,
                DateEnd = request.DateEnd,
                TopN = request.TopN > 0 ? request.TopN : 25
            };
        }
    }
}