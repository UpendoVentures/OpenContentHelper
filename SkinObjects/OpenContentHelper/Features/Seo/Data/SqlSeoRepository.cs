using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using Newtonsoft.Json.Linq;
using System;
using System.Data.SqlClient;
using Upendo.OpenContentHelper.Features.Seo.Models;

namespace Upendo.OpenContentHelper.Features.Seo.Data
{
    public class SqlSeoRepository : ISeoRepository
    {
        private readonly string _connectionString;

        public SqlSeoRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("connectionString is required.");
            }

            _connectionString = connectionString;
        }

        public int GetMetaModuleIdOnTab(int tabId)
        {
            var modules = ModuleController.Instance.GetTabModules(tabId);
            foreach (ModuleInfo mod in modules.Values)
            {
                if (mod.DesktopModule?.ModuleName == "OpenContent" &&
                    !string.IsNullOrWhiteSpace(mod.ModuleTitle) &&
                    mod.ModuleTitle.IndexOf("Meta", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return mod.ModuleID;
                }
            }

            return Null.NullInteger;
        }

        public OpenContentHeadData GetLatestOpenContentHeadData(int moduleId)
        {
            var result = new OpenContentHeadData();

            if (moduleId <= 0)
            {
                return result;
            }

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 [Json]
                FROM [dbo].[OpenContent_Items]
                WHERE [ModuleId] = @ModuleId
                ORDER BY [LastModifiedOnDate] DESC", conn))
            {
                cmd.Parameters.AddWithValue("@ModuleId", moduleId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return result;
                    }

                    var json = reader["Json"]?.ToString();
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        return result;
                    }

                    var parsed = JObject.Parse(json);
                    result.MetaTags = parsed["MetaTags"]?.ToObject<System.Collections.Generic.List<HeadMetaTag>>()
                        ?? new System.Collections.Generic.List<HeadMetaTag>();
                    result.LinkTags = parsed["LinkTags"]?.ToObject<System.Collections.Generic.List<HeadLinkTag>>()
                        ?? new System.Collections.Generic.List<HeadLinkTag>();
                }
            }

            return result;
        }
    }
}