using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Upendo.OpenContentHelper.Features.Analytics.Models;
using Upendo.OpenContentHelper.Features.Analytics.Models.Import;
using Upendo.OpenContentHelper.Features.Analytics.Models.Requests;
using Upendo.OpenContentHelper.Features.Analytics.Models.Responses;
using Upendo.SkinObjects.OpenContentHelper.Helpers;

namespace Upendo.OpenContentHelper.Features.Analytics
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        #region Sites

        public AnalyticsSiteDto GetSite(int siteId)
        {
            const string sql = @"
SELECT
    SiteId,
    PortalId,
    SiteGuid,
    ClientName,
    SiteName,
    DomainName,
    TimeZoneId,
    CurrencyCode,
    IsActive,
    CreatedOnDate,
    LastImportedOnDate,
    Notes
FROM dbo.uv_AnalyticsSite
WHERE SiteId = @SiteId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@SiteId", SqlDbType.Int).Value = siteId;

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return MapAnalyticsSite(rdr);
                }
            }
        }

        public IEnumerable<AnalyticsSiteDto> GetSites()
        {
            const string sql = @"
SELECT
    SiteId,
    PortalId,
    SiteGuid,
    ClientName,
    SiteName,
    DomainName,
    TimeZoneId,
    CurrencyCode,
    IsActive,
    CreatedOnDate,
    LastImportedOnDate,
    Notes
FROM dbo.uv_AnalyticsSite
ORDER BY ClientName ASC, SiteName ASC;";

            var results = new List<AnalyticsSiteDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    results.Add(MapAnalyticsSite(rdr));
                }
            }

            return results;
        }

        public int CreateSite(CreateAnalyticsSiteRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            const string sql = @"
INSERT INTO dbo.uv_AnalyticsSite
(
    PortalId,
    ClientName,
    SiteName,
    DomainName,
    TimeZoneId,
    CurrencyCode,
    IsActive,
    Notes
)
VALUES
(
    @PortalId,
    @ClientName,
    @SiteName,
    @DomainName,
    @TimeZoneId,
    @CurrencyCode,
    @IsActive,
    @Notes
);

SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@PortalId", SqlDbType.Int).Value = (object)request.PortalId ?? DBNull.Value;
                cmd.Parameters.Add("@ClientName", SqlDbType.NVarChar, 200).Value = DbValue(request.ClientName);
                cmd.Parameters.Add("@SiteName", SqlDbType.NVarChar, 200).Value = DbValue(request.SiteName);
                cmd.Parameters.Add("@DomainName", SqlDbType.NVarChar, 255).Value = DbValue(request.DomainName);
                cmd.Parameters.Add("@TimeZoneId", SqlDbType.NVarChar, 100).Value = DbValue(request.TimeZoneId);
                cmd.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar, 10).Value = DbValue(request.CurrencyCode);
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = request.IsActive;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = DbValue(request.Notes);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdateSite(UpdateAnalyticsSiteRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            const string sql = @"
UPDATE dbo.uv_AnalyticsSite
SET
    PortalId = @PortalId,
    ClientName = @ClientName,
    SiteName = @SiteName,
    DomainName = @DomainName,
    TimeZoneId = @TimeZoneId,
    CurrencyCode = @CurrencyCode,
    IsActive = @IsActive,
    Notes = @Notes
WHERE SiteId = @SiteId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@SiteId", SqlDbType.Int).Value = request.SiteId;
                cmd.Parameters.Add("@PortalId", SqlDbType.Int).Value = (object)request.PortalId ?? DBNull.Value;
                cmd.Parameters.Add("@ClientName", SqlDbType.NVarChar, 200).Value = DbValue(request.ClientName);
                cmd.Parameters.Add("@SiteName", SqlDbType.NVarChar, 200).Value = DbValue(request.SiteName);
                cmd.Parameters.Add("@DomainName", SqlDbType.NVarChar, 255).Value = DbValue(request.DomainName);
                cmd.Parameters.Add("@TimeZoneId", SqlDbType.NVarChar, 100).Value = DbValue(request.TimeZoneId);
                cmd.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar, 10).Value = DbValue(request.CurrencyCode);
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = request.IsActive;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = DbValue(request.Notes);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteSite(int siteId)
        {
            const string sql = @"
DELETE FROM dbo.uv_AnalyticsSite
WHERE SiteId = @SiteId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@SiteId", SqlDbType.Int).Value = siteId;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Data Sources

        public AnalyticsDataSourceDto GetDataSource(int dataSourceId)
        {
            const string sql = @"
SELECT
    DataSourceId,
    SourceName,
    SourceType,
    IsActive,
    CreatedOnDate
FROM dbo.uv_AnalyticsDataSource
WHERE DataSourceId = @DataSourceId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@DataSourceId", SqlDbType.Int).Value = dataSourceId;

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return MapAnalyticsDataSource(rdr);
                }
            }
        }

        public IEnumerable<AnalyticsDataSourceDto> GetDataSources()
        {
            const string sql = @"
SELECT
    DataSourceId,
    SourceName,
    SourceType,
    IsActive,
    CreatedOnDate
FROM dbo.uv_AnalyticsDataSource
ORDER BY SourceName ASC;";

            var results = new List<AnalyticsDataSourceDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    results.Add(MapAnalyticsDataSource(rdr));
                }
            }

            return results;
        }

        public int CreateDataSource(CreateAnalyticsDataSourceRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            const string sql = @"
INSERT INTO dbo.uv_AnalyticsDataSource
(
    SourceName,
    SourceType,
    IsActive
)
VALUES
(
    @SourceName,
    @SourceType,
    @IsActive
);

SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@SourceName", SqlDbType.NVarChar, 100).Value = DbValue(request.SourceName);
                cmd.Parameters.Add("@SourceType", SqlDbType.NVarChar, 100).Value = DbValue(request.SourceType);
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = request.IsActive;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void UpdateDataSource(UpdateAnalyticsDataSourceRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            const string sql = @"
UPDATE dbo.uv_AnalyticsDataSource
SET
    SourceName = @SourceName,
    SourceType = @SourceType,
    IsActive = @IsActive
WHERE DataSourceId = @DataSourceId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@DataSourceId", SqlDbType.Int).Value = request.DataSourceId;
                cmd.Parameters.Add("@SourceName", SqlDbType.NVarChar, 100).Value = DbValue(request.SourceName);
                cmd.Parameters.Add("@SourceType", SqlDbType.NVarChar, 100).Value = DbValue(request.SourceType);
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = request.IsActive;

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteDataSource(int dataSourceId)
        {
            const string sql = @"
DELETE FROM dbo.uv_AnalyticsDataSource
WHERE DataSourceId = @DataSourceId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@DataSourceId", SqlDbType.Int).Value = dataSourceId;
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Import Batches

        public AnalyticsImportBatchDto GetImportBatch(int importBatchId)
        {
            const string sql = @"
SELECT
    ImportBatchId,
    SiteId,
    DataSourceId,
    BatchGuid,
    SourceFileName,
    SourceSheetName,
    ImportType,
    DateStart,
    DateEnd,
    Status,
    ImportedByUserId,
    ImportedOnDate,
    RecordCountRaw,
    RecordCountInserted,
    RecordCountUpdated,
    RecordCountRejected,
    Notes
FROM dbo.uv_AnalyticsImportBatch
WHERE ImportBatchId = @ImportBatchId;";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ImportBatchId", SqlDbType.Int).Value = importBatchId;

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return MapAnalyticsImportBatch(rdr);
                }
            }
        }

        public int CreateImportBatch(CreateAnalyticsImportBatchRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsImportBatch_Create", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SiteId", SqlDbType.Int).Value = request.SiteId;
                cmd.Parameters.Add("@DataSourceId", SqlDbType.Int).Value = request.DataSourceId;
                cmd.Parameters.Add("@ImportType", SqlDbType.NVarChar, 50).Value = DbValue(request.ImportType);
                cmd.Parameters.Add("@SourceFileName", SqlDbType.NVarChar, 255).Value = DbValue(request.SourceFileName);
                cmd.Parameters.Add("@SourceSheetName", SqlDbType.NVarChar, 255).Value = DbValue(request.SourceSheetName);
                cmd.Parameters.Add("@DateStart", SqlDbType.Date).Value = (object)request.DateStart ?? DBNull.Value;
                cmd.Parameters.Add("@DateEnd", SqlDbType.Date).Value = (object)request.DateEnd ?? DBNull.Value;
                cmd.Parameters.Add("@ImportedByUserId", SqlDbType.Int).Value = (object)request.ImportedByUserId ?? DBNull.Value;
                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = DbValue(request.Notes);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public AnalyticsImportBatchSummaryDto ValidateStageDaily(int importBatchId)
        {
            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsStageDaily_Validate", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ImportBatchId", SqlDbType.Int).Value = importBatchId;

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return new AnalyticsImportBatchSummaryDto
                    {
                        ImportBatchId = GetInt32(rdr, "ImportBatchId"),
                        TotalRows = GetInt32(rdr, "TotalRows"),
                        ValidRows = GetInt32(rdr, "ValidRows"),
                        InvalidRows = GetInt32(rdr, "InvalidRows")
                    };
                }
            }
        }

        public IEnumerable<AnalyticsStageDailyValidationResultDto> GetStageDailyValidationResults(int importBatchId)
        {
            var results = new List<AnalyticsStageDailyValidationResultDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsStageDaily_GetValidationResults", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@ImportBatchId", SqlDbType.Int).Value = importBatchId;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        results.Add(new AnalyticsStageDailyValidationResultDto
                        {
                            StageDailyId = GetInt64(rdr, "StageDailyId"),
                            SiteKey = GetString(rdr, "SiteKey"),
                            DataSourceKey = GetString(rdr, "DataSourceKey"),
                            MetricDateText = GetString(rdr, "MetricDateText"),
                            UsersText = GetString(rdr, "UsersText"),
                            NewUsersText = GetString(rdr, "NewUsersText"),
                            SessionsText = GetString(rdr, "SessionsText"),
                            EngagedSessionsText = GetString(rdr, "EngagedSessionsText"),
                            PageViewsText = GetString(rdr, "PageViewsText"),
                            ScreenPageViewsText = GetString(rdr, "ScreenPageViewsText"),
                            EventCountText = GetString(rdr, "EventCountText"),
                            ConversionsText = GetString(rdr, "ConversionsText"),
                            RevenueText = GetString(rdr, "RevenueText"),
                            AvgSessionDurationSecondsText = GetString(rdr, "AvgSessionDurationSecondsText"),
                            BounceRateText = GetString(rdr, "BounceRateText"),
                            EngagementRateText = GetString(rdr, "EngagementRateText"),
                            ValidationStatus = GetString(rdr, "ValidationStatus"),
                            ValidationMessage = GetString(rdr, "ValidationMessage")
                        });
                    }
                }
            }

            return results;
        }

        public AnalyticsImportBatchSummaryDto MergeFactDailyFromStage(int importBatchId)
        {
            if (importBatchId <= 0) throw new ArgumentOutOfRangeException(nameof(importBatchId));

            var batch = GetImportBatch(importBatchId);
            var result = MergeFactDailyFromStage(importBatchId);

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

        #endregion

        #region Dashboard Reads

        public AnalyticsSummaryDto GetSummary(AnalyticsDateRangeRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsDashboard_GetSummary", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddDateRangeParameters(cmd, request);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return MapAnalyticsSummary(rdr);
                }
            }
        }

        public AnalyticsComparisonSummaryDto GetComparisonSummary(AnalyticsDateRangeRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsDashboard_GetComparisonSummary", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddDateRangeParameters(cmd, request);

                using (var rdr = cmd.ExecuteReader())
                {
                    if (!rdr.Read()) return null;
                    return MapAnalyticsComparisonSummary(rdr);
                }
            }
        }

        public IEnumerable<AnalyticsTrendPointDto> GetTrend(AnalyticsDateRangeRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var results = new List<AnalyticsTrendPointDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsDashboard_GetTrend", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddDateRangeParameters(cmd, request);

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        results.Add(new AnalyticsTrendPointDto
                        {
                            MetricDate = GetDateTime(rdr, "MetricDate"),
                            Users = GetNullableInt32(rdr, "Users"),
                            NewUsers = GetNullableInt32(rdr, "NewUsers"),
                            Sessions = GetNullableInt32(rdr, "Sessions"),
                            EngagedSessions = GetNullableInt32(rdr, "EngagedSessions"),
                            PageViews = GetNullableInt32(rdr, "PageViews"),
                            EventCount = GetNullableInt32(rdr, "EventCount"),
                            Conversions = GetNullableDecimal(rdr, "Conversions"),
                            Revenue = GetNullableDecimal(rdr, "Revenue"),
                            AvgSessionDurationSeconds = GetNullableDecimal(rdr, "AvgSessionDurationSeconds"),
                            BounceRate = GetNullableDecimal(rdr, "BounceRate"),
                            EngagementRate = GetNullableDecimal(rdr, "EngagementRate")
                        });
                    }
                }
            }

            return results;
        }

        public IEnumerable<AnalyticsChannelBreakdownDto> GetChannelBreakdown(AnalyticsDateRangeRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var results = new List<AnalyticsChannelBreakdownDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsDashboard_GetChannelBreakdown", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddDateRangeParameters(cmd, request);

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        results.Add(new AnalyticsChannelBreakdownDto
                        {
                            ChannelName = GetString(rdr, "ChannelName"),
                            Users = GetNullableInt32(rdr, "Users"),
                            Sessions = GetNullableInt32(rdr, "Sessions"),
                            EngagedSessions = GetNullableInt32(rdr, "EngagedSessions"),
                            Conversions = GetNullableDecimal(rdr, "Conversions"),
                            Revenue = GetNullableDecimal(rdr, "Revenue")
                        });
                    }
                }
            }

            return results;
        }

        public IEnumerable<AnalyticsEventBreakdownDto> GetEventBreakdown(AnalyticsTopNRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var results = new List<AnalyticsEventBreakdownDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsDashboard_GetEventBreakdown", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddDateRangeParameters(cmd, request);
                cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = request.TopN;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        results.Add(new AnalyticsEventBreakdownDto
                        {
                            EventName = GetString(rdr, "EventName"),
                            EventCount = GetNullableInt32(rdr, "EventCount"),
                            Users = GetNullableInt32(rdr, "Users"),
                            Conversions = GetNullableDecimal(rdr, "Conversions")
                        });
                    }
                }
            }

            return results;
        }

        public IEnumerable<AnalyticsTopPageDto> GetTopPages(AnalyticsTopNRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var results = new List<AnalyticsTopPageDto>();

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand("dbo.uv_AnalyticsDashboard_GetTopPages", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                AddDateRangeParameters(cmd, request);
                cmd.Parameters.Add("@TopN", SqlDbType.Int).Value = request.TopN;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        results.Add(new AnalyticsTopPageDto
                        {
                            PagePath = GetString(rdr, "PagePath"),
                            PageTitle = GetString(rdr, "PageTitle"),
                            Entrances = GetNullableInt32(rdr, "Entrances"),
                            Views = GetNullableInt32(rdr, "Views"),
                            Users = GetNullableInt32(rdr, "Users"),
                            Conversions = GetNullableDecimal(rdr, "Conversions"),
                            AvgTimeOnPageSeconds = GetNullableDecimal(rdr, "AvgTimeOnPageSeconds")
                        });
                    }
                }
            }

            return results;
        }

        #endregion

        #region Mapping Helpers

        private static AnalyticsSiteDto MapAnalyticsSite(IDataRecord rdr)
        {
            return new AnalyticsSiteDto
            {
                SiteId = GetInt32(rdr, "SiteId"),
                PortalId = GetNullableInt32(rdr, "PortalId"),
                SiteGuid = GetGuid(rdr, "SiteGuid"),
                ClientName = GetString(rdr, "ClientName"),
                SiteName = GetString(rdr, "SiteName"),
                DomainName = GetString(rdr, "DomainName"),
                TimeZoneId = GetString(rdr, "TimeZoneId"),
                CurrencyCode = GetString(rdr, "CurrencyCode"),
                IsActive = GetBoolean(rdr, "IsActive"),
                CreatedOnDate = GetDateTime(rdr, "CreatedOnDate"),
                LastImportedOnDate = GetNullableDateTime(rdr, "LastImportedOnDate"),
                Notes = GetString(rdr, "Notes")
            };
        }

        private static AnalyticsDataSourceDto MapAnalyticsDataSource(IDataRecord rdr)
        {
            return new AnalyticsDataSourceDto
            {
                DataSourceId = GetInt32(rdr, "DataSourceId"),
                SourceName = GetString(rdr, "SourceName"),
                SourceType = GetString(rdr, "SourceType"),
                IsActive = GetBoolean(rdr, "IsActive"),
                CreatedOnDate = GetDateTime(rdr, "CreatedOnDate")
            };
        }

        private static AnalyticsImportBatchDto MapAnalyticsImportBatch(IDataRecord rdr)
        {
            return new AnalyticsImportBatchDto
            {
                ImportBatchId = GetInt32(rdr, "ImportBatchId"),
                SiteId = GetInt32(rdr, "SiteId"),
                DataSourceId = GetInt32(rdr, "DataSourceId"),
                BatchGuid = GetGuid(rdr, "BatchGuid"),
                SourceFileName = GetString(rdr, "SourceFileName"),
                SourceSheetName = GetString(rdr, "SourceSheetName"),
                ImportType = GetString(rdr, "ImportType"),
                DateStart = GetNullableDateTime(rdr, "DateStart"),
                DateEnd = GetNullableDateTime(rdr, "DateEnd"),
                Status = GetString(rdr, "Status"),
                ImportedByUserId = GetNullableInt32(rdr, "ImportedByUserId"),
                ImportedOnDate = GetDateTime(rdr, "ImportedOnDate"),
                RecordCountRaw = GetNullableInt32(rdr, "RecordCountRaw"),
                RecordCountInserted = GetNullableInt32(rdr, "RecordCountInserted"),
                RecordCountUpdated = GetNullableInt32(rdr, "RecordCountUpdated"),
                RecordCountRejected = GetNullableInt32(rdr, "RecordCountRejected"),
                Notes = GetString(rdr, "Notes")
            };
        }

        private static AnalyticsSummaryDto MapAnalyticsSummary(IDataRecord rdr)
        {
            return new AnalyticsSummaryDto
            {
                SiteId = GetInt32(rdr, "SiteId"),
                DateStart = GetDateTime(rdr, "DateStart"),
                DateEnd = GetDateTime(rdr, "DateEnd"),
                DaysInRange = GetInt32(rdr, "DaysInRange"),
                Users = GetNullableInt32(rdr, "Users"),
                NewUsers = GetNullableInt32(rdr, "NewUsers"),
                Sessions = GetNullableInt32(rdr, "Sessions"),
                EngagedSessions = GetNullableInt32(rdr, "EngagedSessions"),
                PageViews = GetNullableInt32(rdr, "PageViews"),
                ScreenPageViews = GetNullableInt32(rdr, "ScreenPageViews"),
                EventCount = GetNullableInt32(rdr, "EventCount"),
                Conversions = GetNullableDecimal(rdr, "Conversions"),
                Revenue = GetNullableDecimal(rdr, "Revenue"),
                AvgSessionDurationSeconds = GetNullableDecimal(rdr, "AvgSessionDurationSeconds"),
                BounceRate = GetNullableDecimal(rdr, "BounceRate"),
                EngagementRate = GetNullableDecimal(rdr, "EngagementRate"),
                LatestMetricDate = GetNullableDateTime(rdr, "LatestMetricDate")
            };
        }

        private static AnalyticsComparisonSummaryDto MapAnalyticsComparisonSummary(IDataRecord rdr)
        {
            return new AnalyticsComparisonSummaryDto
            {
                SiteId = GetInt32(rdr, "SiteId"),

                CurrentDateStart = GetDateTime(rdr, "CurrentDateStart"),
                CurrentDateEnd = GetDateTime(rdr, "CurrentDateEnd"),
                PreviousDateStart = GetDateTime(rdr, "PreviousDateStart"),
                PreviousDateEnd = GetDateTime(rdr, "PreviousDateEnd"),
                LastYearDateStart = GetDateTime(rdr, "LastYearDateStart"),
                LastYearDateEnd = GetDateTime(rdr, "LastYearDateEnd"),

                CurrentUsers = GetNullableInt32(rdr, "CurrentUsers"),
                PreviousUsers = GetNullableInt32(rdr, "PreviousUsers"),
                LastYearUsers = GetNullableInt32(rdr, "LastYearUsers"),
                UsersDeltaVsPrevious = GetNullableInt32(rdr, "UsersDeltaVsPrevious"),
                UsersDeltaPctVsPrevious = GetNullableDecimal(rdr, "UsersDeltaPctVsPrevious"),
                UsersDeltaVsLastYear = GetNullableInt32(rdr, "UsersDeltaVsLastYear"),
                UsersDeltaPctVsLastYear = GetNullableDecimal(rdr, "UsersDeltaPctVsLastYear"),

                CurrentSessions = GetNullableInt32(rdr, "CurrentSessions"),
                PreviousSessions = GetNullableInt32(rdr, "PreviousSessions"),
                LastYearSessions = GetNullableInt32(rdr, "LastYearSessions"),
                SessionsDeltaVsPrevious = GetNullableInt32(rdr, "SessionsDeltaVsPrevious"),
                SessionsDeltaPctVsPrevious = GetNullableDecimal(rdr, "SessionsDeltaPctVsPrevious"),
                SessionsDeltaVsLastYear = GetNullableInt32(rdr, "SessionsDeltaVsLastYear"),
                SessionsDeltaPctVsLastYear = GetNullableDecimal(rdr, "SessionsDeltaPctVsLastYear"),

                CurrentPageViews = GetNullableInt32(rdr, "CurrentPageViews"),
                PreviousPageViews = GetNullableInt32(rdr, "PreviousPageViews"),
                LastYearPageViews = GetNullableInt32(rdr, "LastYearPageViews"),
                PageViewsDeltaVsPrevious = GetNullableInt32(rdr, "PageViewsDeltaVsPrevious"),
                PageViewsDeltaPctVsPrevious = GetNullableDecimal(rdr, "PageViewsDeltaPctVsPrevious"),
                PageViewsDeltaVsLastYear = GetNullableInt32(rdr, "PageViewsDeltaVsLastYear"),
                PageViewsDeltaPctVsLastYear = GetNullableDecimal(rdr, "PageViewsDeltaPctVsLastYear"),

                CurrentConversions = GetNullableDecimal(rdr, "CurrentConversions"),
                PreviousConversions = GetNullableDecimal(rdr, "PreviousConversions"),
                LastYearConversions = GetNullableDecimal(rdr, "LastYearConversions"),
                ConversionsDeltaVsPrevious = GetNullableDecimal(rdr, "ConversionsDeltaVsPrevious"),
                ConversionsDeltaPctVsPrevious = GetNullableDecimal(rdr, "ConversionsDeltaPctVsPrevious"),
                ConversionsDeltaVsLastYear = GetNullableDecimal(rdr, "ConversionsDeltaVsLastYear"),
                ConversionsDeltaPctVsLastYear = GetNullableDecimal(rdr, "ConversionsDeltaPctVsLastYear"),

                CurrentRevenue = GetNullableDecimal(rdr, "CurrentRevenue"),
                PreviousRevenue = GetNullableDecimal(rdr, "PreviousRevenue"),
                LastYearRevenue = GetNullableDecimal(rdr, "LastYearRevenue"),
                RevenueDeltaVsPrevious = GetNullableDecimal(rdr, "RevenueDeltaVsPrevious"),
                RevenueDeltaPctVsPrevious = GetNullableDecimal(rdr, "RevenueDeltaPctVsPrevious"),
                RevenueDeltaVsLastYear = GetNullableDecimal(rdr, "RevenueDeltaVsLastYear"),
                RevenueDeltaPctVsLastYear = GetNullableDecimal(rdr, "RevenueDeltaPctVsLastYear"),

                CurrentEngagementRate = GetNullableDecimal(rdr, "CurrentEngagementRate"),
                PreviousEngagementRate = GetNullableDecimal(rdr, "PreviousEngagementRate"),
                LastYearEngagementRate = GetNullableDecimal(rdr, "LastYearEngagementRate"),
                EngagementRateDeltaVsPrevious = GetNullableDecimal(rdr, "EngagementRateDeltaVsPrevious"),
                EngagementRateDeltaVsLastYear = GetNullableDecimal(rdr, "EngagementRateDeltaVsLastYear"),

                CurrentBounceRate = GetNullableDecimal(rdr, "CurrentBounceRate"),
                PreviousBounceRate = GetNullableDecimal(rdr, "PreviousBounceRate"),
                LastYearBounceRate = GetNullableDecimal(rdr, "LastYearBounceRate"),
                BounceRateDeltaVsPrevious = GetNullableDecimal(rdr, "BounceRateDeltaVsPrevious"),
                BounceRateDeltaVsLastYear = GetNullableDecimal(rdr, "BounceRateDeltaVsLastYear"),

                CurrentAvgSessionDurationSeconds = GetNullableDecimal(rdr, "CurrentAvgSessionDurationSeconds"),
                PreviousAvgSessionDurationSeconds = GetNullableDecimal(rdr, "PreviousAvgSessionDurationSeconds"),
                LastYearAvgSessionDurationSeconds = GetNullableDecimal(rdr, "LastYearAvgSessionDurationSeconds"),
                AvgSessionDurationDeltaVsPrevious = GetNullableDecimal(rdr, "AvgSessionDurationDeltaVsPrevious"),
                AvgSessionDurationDeltaVsLastYear = GetNullableDecimal(rdr, "AvgSessionDurationDeltaVsLastYear"),

                CurrentDaysWithData = GetNullableInt32(rdr, "CurrentDaysWithData"),
                PreviousDaysWithData = GetNullableInt32(rdr, "PreviousDaysWithData"),
                LastYearDaysWithData = GetNullableInt32(rdr, "LastYearDaysWithData"),

                HasPreviousPeriodData = GetBoolean(rdr, "HasPreviousPeriodData"),
                HasLastYearData = GetBoolean(rdr, "HasLastYearData")
            };
        }

        #endregion

        #region Low-Level Helpers

        private static void AddDateRangeParameters(SqlCommand cmd, AnalyticsDateRangeRequestDto request)
        {
            cmd.Parameters.Add("@SiteId", SqlDbType.Int).Value = request.SiteId;
            cmd.Parameters.Add("@DateStart", SqlDbType.Date).Value = request.DateStart.Date;
            cmd.Parameters.Add("@DateEnd", SqlDbType.Date).Value = request.DateEnd.Date;
            cmd.Parameters.Add("@DataSourceId", SqlDbType.Int).Value = (object)request.DataSourceId ?? DBNull.Value;
        }

        private static object DbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        private static int GetOrdinal(IDataRecord rdr, string columnName)
        {
            return rdr.GetOrdinal(columnName);
        }

        private static string GetString(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return rdr.IsDBNull(ordinal) ? null : Convert.ToString(rdr.GetValue(ordinal));
        }

        private static int GetInt32(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return Convert.ToInt32(rdr.GetValue(ordinal));
        }

        private static int? GetNullableInt32(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return rdr.IsDBNull(ordinal) ? (int?)null : Convert.ToInt32(rdr.GetValue(ordinal));
        }

        private static long GetInt64(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return Convert.ToInt64(rdr.GetValue(ordinal));
        }

        private static decimal? GetNullableDecimal(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return rdr.IsDBNull(ordinal) ? (decimal?)null : Convert.ToDecimal(rdr.GetValue(ordinal));
        }

        private static bool GetBoolean(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return Convert.ToBoolean(rdr.GetValue(ordinal));
        }

        private static DateTime GetDateTime(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return Convert.ToDateTime(rdr.GetValue(ordinal));
        }

        private static DateTime? GetNullableDateTime(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return rdr.IsDBNull(ordinal) ? (DateTime?)null : Convert.ToDateTime(rdr.GetValue(ordinal));
        }

        private static Guid GetGuid(IDataRecord rdr, string columnName)
        {
            var ordinal = GetOrdinal(rdr, columnName);
            return (Guid)rdr.GetValue(ordinal);
        }

        #endregion
    }
}