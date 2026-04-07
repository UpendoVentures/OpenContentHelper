using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Upendo.OpenContentHelper.Features.Directory.Common;
using Upendo.OpenContentHelper.Features.Directory.Helpers;
using Upendo.OpenContentHelper.Features.Directory.Models;
using Upendo.SkinObjects.OpenContentHelper.Helpers;

namespace Upendo.OpenContentHelper.Features.Directory
{
    public sealed class ChamberMembershipController
    {
        // ----------------------------
        // 1) ParseRequest(Request)
        // ----------------------------
        public static DirectorySearchRequest ParseRequest(HttpRequest request)
        {
            // Query string keys are intentionally short to keep URLs clean.
            // q, cat, city, ms, featured, sort, page, pagesize
            var req = new DirectorySearchRequest
            {
                Query = NormalizeQuery(request.QueryString["q"]),
                CategorySlug = NormalizeToken(request.QueryString["cat"]),
                City = NormalizeToken(request.QueryString["city"]),
                Sort = NormalizeSort(request.QueryString["sort"]),
                FeaturedOnly = ToBool(request.QueryString["featured"]),
                Page = ClampInt(request.QueryString["page"], 1, 1, 100000),
                PageSize = ClampInt(request.QueryString["pagesize"], 24, 1, 50)
            };

            var msRaw = NormalizeToken(request.QueryString["ms"]);
            if (!string.IsNullOrWhiteSpace(msRaw) && byte.TryParse(msRaw, out var ms) && (ms == 1 || ms == 2 || ms == 3))
            {
                req.MembershipStatus = ms;
            }

            var rsRaw = NormalizeToken(request.QueryString["rs"]);
            int rs;
            if (!string.IsNullOrWhiteSpace(rsRaw) && int.TryParse(rsRaw, out rs))
            {
                // optional: clamp to keep it sane
                if (rs < 1) rs = 1;
                if (rs > 2147483647) rs = 2147483647;
                req.RandomSeed = rs;
            }

            return req;
        }

        // ----------------------------
        // 2) SearchCompanies(portalId, req)
        // ----------------------------
        public static PagedResult<CompanyDirectoryItem> SearchCompanies(int portalId, DirectorySearchRequest req)
        {
            if (portalId < 0) throw new ArgumentOutOfRangeException(nameof(portalId));
            if (req.Sort == "default" && (!req.RandomSeed.HasValue || req.RandomSeed.Value <= 0))
            {
                throw new InvalidOperationException(
                    "Default sort requires a random seed (rs) in the query string."
                );
            }

            // IMPORTANT: This assumes your view includes PortalId.
            // If your bcc_vwCompanyDirectory doesn't include PortalId yet, add it.
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@PortalId", SqlDbType.Int) { Value = portalId },
                new SqlParameter("@Offset", SqlDbType.Int) { Value = (req.Page - 1) * req.PageSize },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = req.PageSize }
            };

            var where = new StringBuilder();
            where.AppendLine("WHERE v.PortalId = @PortalId");

            // Optional: category filter
            // We only join when needed so the normal listing stays fast.
            var join = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(req.CategorySlug))
            {
                join.AppendLine("INNER JOIN dbo.bcc_CompanyCategory cc ON cc.CompanyId = v.CompanyId");
                join.AppendLine("INNER JOIN dbo.bcc_Category cat ON cat.CategoryId = cc.CategoryId AND cat.IsActive = 1");
                where.AppendLine("AND cat.Slug = @CategorySlug");
                parameters.Add(new SqlParameter("@CategorySlug", SqlDbType.NVarChar, 160) { Value = req.CategorySlug });
            }

            if (!string.IsNullOrWhiteSpace(req.City))
            {
                where.AppendLine("AND v.City = @City");
                parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar, 120) { Value = req.City });
            }

            if (req.MembershipStatus.HasValue)
            {
                where.AppendLine("AND v.MembershipStatus = @MembershipStatus");
                parameters.Add(new SqlParameter("@MembershipStatus", SqlDbType.TinyInt) { Value = req.MembershipStatus.Value });
            }

            if (req.FeaturedOnly)
            {
                where.AppendLine("AND v.IsFeatured = 1");
            }

            // Search (safe LIKE)
            if (!string.IsNullOrWhiteSpace(req.Query))
            {
                where.AppendLine("AND (v.CompanyName LIKE @Q OR v.ShortDescription LIKE @Q)");
                parameters.Add(new SqlParameter("@Q", SqlDbType.NVarChar, 210) { Value = "%" + EscapeLike(req.Query) + "%" });
            }

            var isRandomDefault = (req.Sort ?? string.Empty).Trim().Equals("default", StringComparison.OrdinalIgnoreCase);
            int? seed = 0;
            if (isRandomDefault)
            {
                seed = (req.RandomSeed > 0) ? req.RandomSeed : 0; // 0 means "not provided"
                parameters.Add(new SqlParameter("@Seed", SqlDbType.Int) { Value = seed });
            }

            var orderBy = GetOrderBy(req.Sort, isRandomDefault);
            
            var sqlCount = $@"
SELECT COUNT_BIG(1)
FROM dbo.bcc_vwCompanyDirectory v
{join}
{where};
";

            var sqlPaged = $@"
SELECT
    v.CompanyId,
    v.CompanyName,
    v.Slug,
    ISNULL(v.ShortDescription, N'') AS ShortDescription,
    ISNULL(v.WebsiteUrl, N'') AS WebsiteUrl,
    ISNULL(v.Phone, N'') AS Phone,
    ISNULL(v.Address1, N'') AS Address1,
    ISNULL(v.City, N'') AS City,
    ISNULL(v.Region, N'') AS Region,
    ISNULL(v.PostalCode, N'') AS PostalCode,
    v.MembershipStatus,
    v.MemberSinceYear,
    v.IsFeatured,
    v.FeaturedSortOrder,
    v.LogoFileId,
    ISNULL(v.LogoUrl, N'') AS LogoUrl
FROM dbo.bcc_vwCompanyDirectory v
{join}
{where}
ORDER BY {orderBy}
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
";

            long totalLong;
            var items = new List<CompanyDirectoryItem>();

            using (var cn = ConfigBase.GetOpenConnection())
            {
                // total
                using (var cmd = new SqlCommand(sqlCount, cn))
                {
                    // IMPORTANT: new parameter instances per command
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        var p = parameters[i];
                        if (p != null)
                        {
                            if (!p.ParameterName.Equals("@Offset", StringComparison.OrdinalIgnoreCase) &&
                                !p.ParameterName.Equals("@PageSize", StringComparison.OrdinalIgnoreCase))
                            {
                                cmd.Parameters.Add(CloneParam(p));
                            }
                        }
                    }

                    totalLong = Convert.ToInt64(cmd.ExecuteScalar());
                }

                // page
                using (var cmd = new SqlCommand(sqlPaged, cn))
                {
                    // IMPORTANT: new parameter instances per command
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        var p = parameters[i];
                        if (p != null)
                        {
                            cmd.Parameters.Add(CloneParam(p));
                        }
                    }

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            var it = new CompanyDirectoryItem();

                            // safe string columns (your SQL already ISNULL's them, but keep defensive)
                            it.CompanyId = DataHandler.ToInt(rdr[0], 0);
                            it.CompanyName = DataHandler.ToStringSafe(rdr[1]);
                            it.Slug = DataHandler.ToStringSafe(rdr[2]);
                            it.ShortDescription = DataHandler.ToStringSafe(rdr[3]);
                            it.WebsiteUrl = DataHandler.ToStringSafe(rdr[4]);
                            it.Phone = DataHandler.ToStringSafe(rdr[5]);
                            it.Address1 = DataHandler.ToStringSafe(rdr[6]);
                            it.City = DataHandler.ToStringSafe(rdr[7]);
                            it.Region = DataHandler.ToStringSafe(rdr[8]);
                            it.PostalCode = DataHandler.ToStringSafe(rdr[9]);

                            // THESE are the common cast-crash fields
                            it.MembershipStatus = DataHandler.ToByte(rdr[10], (byte)0);

                            object msy = rdr[11];
                            it.MemberSinceYear = (msy == null || msy == DBNull.Value) ? (int?)null : (int?)DataHandler.ToInt(msy, 0);

                            it.IsFeatured = DataHandler.ToBool(rdr[12], false);
                            it.FeaturedSortOrder = DataHandler.ToInt(rdr[13], 0);

                            object lfi = rdr[14];
                            it.LogoFileId = (lfi == null || lfi == DBNull.Value) ? (int?)null : (int?)DataHandler.ToInt(lfi, 0);

                            it.LogoUrl = DataHandler.ToStringSafe(rdr[15]);

                            items.Add(it);
                        }

                    }
                }
            }

            var safeTotal = totalLong > int.MaxValue ? int.MaxValue : (int)totalLong;

            return new PagedResult<CompanyDirectoryItem>
            {
                Items = items,
                TotalCount = safeTotal,
                Page = req.Page,
                PageSize = req.PageSize
            };
        }

        // ----------------------------
        // 3) GetActiveCategories(portalId)
        // ----------------------------
        // If categories are NOT portal-scoped in your schema (most likely today),
        // portalId is ignored but kept in signature for future-proofing.
        public static List<CategoryDto> GetActiveCategories(int portalId)
        {
            // Cache key includes portalId so you can later scope categories by portal without changing callers.
            var cacheKey = $"BCC:Directory:Categories:Portal:{portalId}";
            var cached = DataCache.GetCache(cacheKey) as List<CategoryDto>;
            if (cached != null) return cached;

            var list = new List<CategoryDto>();

            const string sql = @"
SELECT CategoryId, CategoryName, Slug
FROM dbo.bcc_Category
WHERE IsActive = 1
ORDER BY CategoryName;
";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    list.Add(new CategoryDto
                    {
                        CategoryId = rdr.GetInt32(0),
                        CategoryName = rdr.GetString(1),
                        Slug = rdr.GetString(2)
                    });
                }
            }

            // 1 hour cache is safe for categories
            DataCache.SetCache(cacheKey, list, TimeSpan.FromHours(1));
            return list;
        }

        // ----------------------------
        // 4) GetCompanyBySlug(portalId, slug)
        // ----------------------------
        public static CompanyDetail GetCompanyBySlug(int portalId, string slug)
        {
            if (portalId < 0) throw new ArgumentOutOfRangeException(nameof(portalId));
            slug = NormalizeToken(slug);

            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            // Short cache: detail pages can be hit a lot.
            var cacheKey = $"BCC:Directory:CompanyDetail:{portalId}:{slug}";
            var cached = DataCache.GetCache(cacheKey) as CompanyDetail;
            if (cached != null) return cached;

            CompanyDetail company = null;

            const string sqlCompany = @"
SELECT TOP 1
    c.CompanyId,
    c.PortalId,
    c.CompanyName,
    c.Slug,
    ISNULL(c.ShortDescription, N''),
    ISNULL(c.LongDescription, N''),
    ISNULL(c.WebsiteUrl, N''),
    ISNULL(c.Phone, N''),
    ISNULL(c.Email, N''),
    ISNULL(c.PrimaryBusinessEmail, N''),
    ISNULL(c.Address1, N''),
    ISNULL(c.Address2, N''),
    ISNULL(c.City, N''),
    ISNULL(c.Region, N''),
    ISNULL(c.PostalCode, N''),
    c.Latitude,
    c.Longitude,
    c.MembershipStatus,
    c.MemberSinceYear,
    c.IsFeatured,
    c.FeaturedSortOrder,
    c.LogoFileId,
    ISNULL(c.LogoUrl, N''),
    ISNULL(c.LinkedInUrl, N''),
    ISNULL(c.FacebookUrl, N''),
    ISNULL(c.InstagramUrl, N''),
    ISNULL(c.TwitterUrl, N''),
    ISNULL(c.TikTokUrl, N''),
    c.IsPublic,
    c.IsActive,
    ISNULL(c.SeoTitle, N''),
    ISNULL(c.SeoDescription, N''),
    ISNULL(c.CanonicalUrl, N''),
    ISNULL(c.MetaRobots, N''),
    ISNULL(c.OgImageUrl, N'')
FROM dbo.bcc_Company c
WHERE c.PortalId = @PortalId
  AND c.Slug = @Slug
  AND c.IsActive = 1
  AND c.IsPublic = 1;
";

            const string sqlCats = @"
SELECT cat.CategoryId, cat.CategoryName, cat.Slug
FROM dbo.bcc_CompanyCategory cc
INNER JOIN dbo.bcc_Category cat ON cat.CategoryId = cc.CategoryId
WHERE cc.CompanyId = @CompanyId
  AND cat.IsActive = 1
ORDER BY cc.IsPrimary DESC, cat.CategoryName ASC;
";

            using (var cn = ConfigBase.GetOpenConnection())
            {
                using (var cmd = new SqlCommand(sqlCompany, cn))
                {
                    cmd.Parameters.Add("@PortalId", SqlDbType.Int).Value = portalId;
                    cmd.Parameters.Add("@Slug", SqlDbType.NVarChar, 220).Value = slug;

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.Read())
                        {
                            return null;
                        }

                        company = new CompanyDetail();
                        company.CompanyId = DataHandler.ToInt(rdr[0], 0);
                        company.PortalId = DataHandler.ToInt(rdr[1], 0);

                        company.CompanyName = DataHandler.ToStringSafe(rdr[2]);
                        company.Slug = DataHandler.ToStringSafe(rdr[3]);
                        company.ShortDescription = DataHandler.ToStringSafe(rdr[4]);
                        company.LongDescription = DataHandler.ToStringSafe(rdr[5]);

                        company.WebsiteUrl = DataHandler.ToStringSafe(rdr[6]);
                        company.Phone = DataHandler.ToStringSafe(rdr[7]);
                        company.Email = DataHandler.ToStringSafe(rdr[8]);
                        company.PrimaryBusinessEmail = DataHandler.ToStringSafe(rdr[9]);

                        company.Address1 = DataHandler.ToStringSafe(rdr[10]);
                        company.Address2 = DataHandler.ToStringSafe(rdr[11]);
                        company.City = DataHandler.ToStringSafe(rdr[12]);
                        company.Region = DataHandler.ToStringSafe(rdr[13]);
                        company.PostalCode = DataHandler.ToStringSafe(rdr[14]);

                        // Latitude / Longitude: database might be decimal, numeric, float, etc.
                        object lat = rdr[15];
                        company.Latitude = (lat == null || lat == DBNull.Value) ? (decimal?)null : (decimal?)Convert.ToDecimal(lat);

                        object lon = rdr[16];
                        company.Longitude = (lon == null || lon == DBNull.Value) ? (decimal?)null : (decimal?)Convert.ToDecimal(lon);

                        // MembershipStatus: tinyint, smallint, int… be defensive
                        company.MembershipStatus = DataHandler.ToByte(rdr[17], (byte)0);

                        // MemberSinceYear can come as smallint/int/null
                        object msy = rdr[18];
                        company.MemberSinceYear = (msy == null || msy == DBNull.Value) ? (int?)null : (int?)DataHandler.ToInt(msy, 0);

                        // bit should be fine, but still defensive
                        company.IsFeatured = DataHandler.ToBool(rdr[19], false);
                        company.FeaturedSortOrder = DataHandler.ToInt(rdr[20], 0);

                        // LogoFileId can be nullable
                        object lfi = rdr[21];
                        company.LogoFileId = (lfi == null || lfi == DBNull.Value) ? (int?)null : (int?)DataHandler.ToInt(lfi, 0);

                        company.LogoUrl = DataHandler.ToStringSafe(rdr[22]);

                        company.LinkedInUrl = DataHandler.ToStringSafe(rdr[23]);
                        company.FacebookUrl = DataHandler.ToStringSafe(rdr[24]);
                        company.InstagramUrl = DataHandler.ToStringSafe(rdr[25]);
                        company.TwitterUrl = DataHandler.ToStringSafe(rdr[26]);
                        company.TikTokUrl = DataHandler.ToStringSafe(rdr[27]);

                        company.IsPublic = DataHandler.ToBool(rdr[28], false);
                        company.IsActive = DataHandler.ToBool(rdr[29], false);
                        company.SeoTitle = DataHandler.ToStringSafe(rdr[30]);
                        company.SeoDescription = DataHandler.ToStringSafe(rdr[31]);
                        company.CanonicalUrl = DataHandler.ToStringSafe(rdr[32]);
                        company.MetaRobots = DataHandler.ToStringSafe(rdr[33]);
                        company.OgImageUrl = DataHandler.ToStringSafe(rdr[34]);
                    }
                }

                // Categories
                var cats = new List<CategoryDto>();
                using (var cmd = new SqlCommand(sqlCats, cn))
                {
                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = company.CompanyId;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            cats.Add(new CategoryDto
                            {
                                CategoryId = DataHandler.ToInt(rdr[0], 0),
                                CategoryName = DataHandler.ToStringSafe(rdr[1]),
                                Slug = DataHandler.ToStringSafe(rdr[2])
                            });
                        }
                    }
                }

                company.Categories = cats;
            }

            // Cache for 2 minutes (enough to help, short enough to reflect edits)
            DataCache.SetCache(cacheKey, company, TimeSpan.FromMinutes(2));
            return company;
        }

        public static DirectorySearchWithCategoriesResult GetRandomCompanySuggestions(int portalId, int count = 3, int? seed = null)
        {
            if (portalId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(portalId));
            }

            if (count < 1)
            {
                count = 3;
            }

            if (count > 12)
            {
                count = 12;
            }

            int effectiveSeed = seed.HasValue && seed.Value > 0
                ? seed.Value
                : Math.Abs(Guid.NewGuid().GetHashCode());

            if (effectiveSeed <= 0)
            {
                effectiveSeed = 1;
            }

            var req = new DirectorySearchRequest
            {
                Query = string.Empty,
                CategorySlug = string.Empty,
                City = null,
                MembershipStatus = null,
                FeaturedOnly = false,
                Sort = "default",
                RandomSeed = effectiveSeed,
                Page = 1,
                PageSize = count
            };

            var output = DirectorySearchWithCategoriesResult.SearchCompaniesWithCategoryNames(portalId, req);

            if (output == null)
            {
                output = new DirectorySearchWithCategoriesResult();
            }

            if (output.Results == null)
            {
                output.Results = new PagedResult<CompanyDirectoryItem>();
            }

            if (output.Results.Items == null)
            {
                output.Results.Items = new List<CompanyDirectoryItem>();
            }

            if (output.CategoriesByCompanyId == null)
            {
                output.CategoriesByCompanyId = new Dictionary<int, List<CategoryDto>>();
            }

            return output;
        }

        // ----------------------------
        // 5) ResolveLogoUrl(portalId, fileId, url)
        // ----------------------------
        public static string ResolveLogoUrl(int portalId, int? fileId, string fallbackUrl)
        {
            // Prefer DNN file id
            if (fileId.HasValue && fileId.Value > 0)
            {
                try
                {
                    var file = FileManager.Instance.GetFile(fileId.Value);
                    if (file != null)
                    {
                        // DNN will generate a correct, secured URL to the file
                        return FileManager.Instance.GetUrl(file);
                    }
                }
                catch
                {
                    // swallow; fallback below
                }
            }

            // Fallback to stored URL if present
            if (!string.IsNullOrWhiteSpace(fallbackUrl))
            {
                return fallbackUrl.Trim();
            }

            // Optional: return a placeholder (or empty)
            return "";
        }

        // ----------------------------
        // 6) GetDistinctCities(portalId)
        // ----------------------------
        public static IReadOnlyList<string> GetDistinctCities(int portalId)
        {
            if (portalId < 0) throw new ArgumentOutOfRangeException(nameof(portalId));

            var cacheKey = $"BCC:Directory:Cities:Portal:{portalId}";
            var cached = DataCache.GetCache(cacheKey) as List<string>;
            if (cached != null) return cached;

            var list = new List<string>();

            const string sql = @"
SELECT DISTINCT v.City
FROM dbo.bcc_vwCompanyDirectory v
WHERE v.PortalId = @PortalId
  AND v.City IS NOT NULL
  AND LTRIM(RTRIM(v.City)) <> N''
ORDER BY v.City ASC;
";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@PortalId", SqlDbType.Int).Value = portalId;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (!rdr.IsDBNull(0))
                        {
                            var city = rdr.GetString(0)?.Trim();
                            if (!string.IsNullOrWhiteSpace(city))
                            {
                                list.Add(city);
                            }
                        }
                    }
                }
            }

            // 60 minutes is a good balance - changes are rare but you want it to refresh occasionally
            DataCache.SetCache(cacheKey, list, TimeSpan.FromMinutes(60));
            return list;
        }

        // ----------------------------
        // 7) GetCompanyCategories(companyId)
        // ----------------------------
        public static List<CategoryDto> GetCompanyCategories(int companyId)
        {
            if (companyId <= 0) return new List<CategoryDto>();

            var cacheKey = $"BCC:Directory:CompanyCategories:{companyId}";
            var cached = DataCache.GetCache(cacheKey) as List<CategoryDto>;
            if (cached != null) return cached;

            var list = new List<CategoryDto>();

            const string sql = @"
SELECT cat.CategoryId, cat.CategoryName, cat.Slug
FROM dbo.bcc_CompanyCategory cc
INNER JOIN dbo.bcc_Category cat ON cat.CategoryId = cc.CategoryId
WHERE cc.CompanyId = @CompanyId
  AND cat.IsActive = 1
ORDER BY cc.IsPrimary DESC, cat.CategoryName ASC;
";

            using (var cn = ConfigBase.GetOpenConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;

                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new CategoryDto
                        {
                            CategoryId = rdr.GetInt32(0),
                            CategoryName = rdr.GetString(1),
                            Slug = rdr.GetString(2)
                        });
                    }
                }
            }

            // Categories per company also change rarely.
            DataCache.SetCache(cacheKey, list, TimeSpan.FromMinutes(30));
            return list;
        }

        public static IHtmlString BuildCompanyJsonLd(
            CompanyDetail company,
            string pageUrl,
            string logoUrl,
            IEnumerable<string> categoryNames = null)
        {
            if (company == null) return new HtmlString(string.Empty);

            // Prefer long description, fall back to short
            var description = !string.IsNullOrWhiteSpace(company.LongDescription)
                ? company.LongDescription
                : company.ShortDescription;

            // Prefer PrimaryBusinessEmail, fall back to Email
            var email = !string.IsNullOrWhiteSpace(company.PrimaryBusinessEmail)
                ? company.PrimaryBusinessEmail
                : company.Email;

            // Normalize URL inputs (defensive)
            pageUrl = (pageUrl ?? string.Empty).Trim();
            logoUrl = (logoUrl ?? string.Empty).Trim();

            // sameAs socials
            var sameAs = new List<string>();
            AddIfHttpUrl(sameAs, company.LinkedInUrl);
            AddIfHttpUrl(sameAs, company.FacebookUrl);
            AddIfHttpUrl(sameAs, company.InstagramUrl);
            AddIfHttpUrl(sameAs, company.TwitterUrl);
            AddIfHttpUrl(sameAs, company.TikTokUrl);

            // Categories -> keywords (optional)
            var keywords = string.Empty;
            if (categoryNames != null)
            {
                var cats = categoryNames
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (cats.Count > 0)
                {
                    keywords = string.Join(", ", cats);
                }
            }

            // Build street address
            var streetParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(company.Address1)) streetParts.Add(company.Address1.Trim());
            if (!string.IsNullOrWhiteSpace(company.Address2)) streetParts.Add(company.Address2.Trim());
            var streetAddress = streetParts.Count > 0 ? string.Join(", ", streetParts) : string.Empty;

            // @id: stable identifier for the entity
            var entityId = !string.IsNullOrWhiteSpace(pageUrl)
                ? (pageUrl.TrimEnd('/') + "#business")
                : ("#business-" + (company.CompanyId > 0 ? company.CompanyId.ToString() : company.Slug));

            // Root JSON-LD object
            var root = new Dictionary<string, object>
            {
                { "@context", "https://schema.org" },
                // Use LocalBusiness for directory listings, but include Organization for broad compatibility
                { "@type", new [] { "LocalBusiness", "Organization" } },
                { "@id", entityId },
                { "name", company.CompanyName ?? string.Empty },
                { "description", description ?? string.Empty }
            };

            if (!string.IsNullOrWhiteSpace(pageUrl))
            {
                root["mainEntityOfPage"] = pageUrl;
            }

            if (!string.IsNullOrWhiteSpace(company.WebsiteUrl))
            {
                root["url"] = company.WebsiteUrl;
                root["sameAs"] = sameAs.Count > 0 ? sameAs : null; // keep sameAs only if any exist
                                                                   // For LocalBusiness, "url" should be the page about the business; WebsiteUrl can be included as an additional URL.
                                                                   // We'll include it as "website" via "url" is already used; better: include under "sameAs" if it’s http/https.
                                                                   // But if you prefer "url" to be the business website, swap root["url"] assignments above.
                                                                   // We'll keep "url" as pageUrl and also include WebsiteUrl as an additional link.
                var web = company.WebsiteUrl.Trim();
                if (IsHttpUrl(web))
                {
                    sameAs.Insert(0, web);
                    root["sameAs"] = sameAs;
                }
            }
            else
            {
                if (sameAs.Count > 0) root["sameAs"] = sameAs;
            }

            if (!string.IsNullOrWhiteSpace(logoUrl))
            {
                root["logo"] = logoUrl;
                root["image"] = logoUrl;
            }

            if (!string.IsNullOrWhiteSpace(company.Phone))
            {
                root["telephone"] = company.Phone.Trim();
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                root["email"] = email.Trim();
            }

            // Address block (only if any address fields exist)
            if (!string.IsNullOrWhiteSpace(streetAddress) ||
                !string.IsNullOrWhiteSpace(company.City) ||
                !string.IsNullOrWhiteSpace(company.Region) ||
                !string.IsNullOrWhiteSpace(company.PostalCode))
            {
                var addr = new Dictionary<string, object>
                {
                    { "@type", "PostalAddress" }
                };

                if (!string.IsNullOrWhiteSpace(streetAddress)) addr["streetAddress"] = streetAddress;
                if (!string.IsNullOrWhiteSpace(company.City)) addr["addressLocality"] = company.City.Trim();
                if (!string.IsNullOrWhiteSpace(company.Region)) addr["addressRegion"] = company.Region.Trim();
                if (!string.IsNullOrWhiteSpace(company.PostalCode)) addr["postalCode"] = company.PostalCode.Trim();

                root["address"] = addr;
            }

            // Geo block (only if both lat/lon present)
            if (company.Latitude.HasValue && company.Longitude.HasValue)
            {
                root["geo"] = new Dictionary<string, object>
                {
                    { "@type", "GeoCoordinates" },
                    { "latitude", company.Latitude.Value },
                    { "longitude", company.Longitude.Value }
                };
            }

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                root["keywords"] = keywords;
            }

            // Clean out nulls we intentionally set above
            RemoveNullValues(root);

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            string json = JsonConvert.SerializeObject(root, Formatting.None, settings);

            // Return as a script tag (unencoded JSON)
            var script = "<script type=\"application/ld+json\">" + json + "</script>";
            return new HtmlString(script);
        }

        public static string GetPrimaryCategoryName(CompanyDetail company)
        {
            if (company?.Categories != null && company.Categories.Count > 0 && company.Categories[0] != null)
            {
                return company.Categories[0].CategoryName ?? string.Empty;
            }

            return string.Empty;
        }

        public static string GetPrimaryEmail(CompanyDetail company)
        {
            if (company == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(company.PrimaryBusinessEmail))
            {
                return company.PrimaryBusinessEmail.Trim();
            }

            return (company.Email ?? string.Empty).Trim();
        }

        public static List<string> GetCompanyAddressLines(CompanyDetail company)
        {
            var lines = new List<string>();

            if (company == null)
            {
                return lines;
            }

            if (!string.IsNullOrWhiteSpace(company.Address1))
            {
                lines.Add(company.Address1.Trim());
            }

            if (!string.IsNullOrWhiteSpace(company.Address2))
            {
                lines.Add(company.Address2.Trim());
            }

            var cityRegionPostalParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(company.City))
            {
                cityRegionPostalParts.Add(company.City.Trim());
            }

            if (!string.IsNullOrWhiteSpace(company.Region))
            {
                cityRegionPostalParts.Add(company.Region.Trim());
            }

            if (!string.IsNullOrWhiteSpace(company.PostalCode))
            {
                cityRegionPostalParts.Add(company.PostalCode.Trim());
            }

            if (cityRegionPostalParts.Count > 0)
            {
                lines.Add(string.Join(", ", cityRegionPostalParts));
            }

            return lines;
        }

        public static string GetCompanyAddressSingleLine(CompanyDetail company)
        {
            var lines = GetCompanyAddressLines(company);

            if (lines == null || lines.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(", ", lines);
        }

        public static bool HasCompanyAddress(CompanyDetail company)
        {
            return GetCompanyAddressLines(company).Count > 0;
        }

        public static string BuildCompanyGoogleMapsUrl(CompanyDetail company)
        {
            var address = GetCompanyAddressSingleLine(company);

            if (string.IsNullOrWhiteSpace(address))
            {
                return string.Empty;
            }

            return "https://www.google.com/maps/search/?api=1&query=" + HttpUtility.UrlEncode(address);
        }

        public static bool HasSocialLinks(CompanyDetail company)
        {
            if (company == null)
            {
                return false;
            }

            return
                !string.IsNullOrWhiteSpace(company.LinkedInUrl) ||
                !string.IsNullOrWhiteSpace(company.FacebookUrl) ||
                !string.IsNullOrWhiteSpace(company.InstagramUrl) ||
                !string.IsNullOrWhiteSpace(company.TwitterUrl) ||
                !string.IsNullOrWhiteSpace(company.TikTokUrl);
        }

        public static string BuildCompanyDetailUrl(int tabId, string slug, string queryString = "")
        {
            var baseTabUrl = DotNetNuke.Common.Globals.NavigateURL(tabId).TrimEnd('/');

            if (string.IsNullOrWhiteSpace(slug))
            {
                return baseTabUrl;
            }

            var encodedSlug = HttpUtility.UrlPathEncode(slug.Trim().Trim('/'));
            var url = baseTabUrl + "/company/" + encodedSlug;

            if (!string.IsNullOrWhiteSpace(queryString))
            {
                if (!queryString.StartsWith("?"))
                {
                    queryString = "?" + queryString.TrimStart('?');
                }

                url += queryString;
            }

            return url;
        }

        #region Private Helper Methods
        private static string GetOrderBy(string sort, bool isRandomDefault)
        {
            switch ((sort ?? "").Trim().ToLowerInvariant())
            {
                case "name":
                    // Add CompanyId as a tiebreaker so paging is stable
                    return "v.CompanyName ASC, v.CompanyId ASC";

                case "featured":
                    return "v.IsFeatured DESC, v.FeaturedSortOrder ASC, v.CompanyName ASC, v.CompanyId ASC";

                case "default":
                default:
                    if (isRandomDefault)
                    {
                        // Keeps featured items prioritized, but randomizes within those groups.
                        // CHECKSUM(@Seed, v.CompanyId) is deterministic for a given @Seed.
                        return "v.IsFeatured DESC, v.FeaturedSortOrder ASC, CHECKSUM(@Seed, v.CompanyId) ASC, v.CompanyId ASC";
                    }

                    // Fallback if needed
                    return "v.IsFeatured DESC, v.FeaturedSortOrder ASC, v.CompanyName ASC, v.CompanyId ASC";
            }
        }

        private static string NormalizeSort(string sort)
        {
            sort = NormalizeToken(sort);
            if (sort == "name" || sort == "featured" || sort == "default") return sort;
            return "default";
        }

        private static bool ToBool(string raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (raw.Equals("1", StringComparison.OrdinalIgnoreCase)) return true;
            if (raw.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
            if (raw.Equals("yes", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static int ClampInt(string raw, int defaultValue, int min, int max)
        {
            if (!int.TryParse(raw, out var v)) v = defaultValue;
            if (v < min) v = min;
            if (v > max) v = max;
            return v;
        }

        private static string NormalizeQuery(string q)
        {
            q = HttpUtility.UrlDecode(q ?? string.Empty);
            q = (q ?? string.Empty).Trim();
            q = Regex.Replace(q, @"\s+", " ");
            if (q.Length > 100) q = q.Substring(0, 100);
            return q;
        }

        private static string NormalizeToken(string s)
        {
            s = HttpUtility.UrlDecode(s ?? string.Empty);
            s = (s ?? string.Empty).Trim();
            if (s.Length > 220) s = s.Substring(0, 220);
            return s;
        }

        // Escapes LIKE wildcards so user input doesn't behave weirdly
        // (still parameterized, but this avoids treating user % _ as wildcards)
        private static string EscapeLike(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]");
        }

        private static SqlParameter CloneParam(SqlParameter p)
        {
            // Create a new parameter instance with the same metadata + value
            var cp = new SqlParameter(p.ParameterName, p.SqlDbType);

            // Preserve size when relevant (e.g., NVARCHAR)
            if (p.Size > 0)
            {
                cp.Size = p.Size;
            }

            // Preserve precision/scale if used
            cp.Precision = p.Precision;
            cp.Scale = p.Scale;

            // Preserve direction (usually Input)
            cp.Direction = p.Direction;

            // Preserve null handling
            cp.IsNullable = p.IsNullable;

            // Copy value
            cp.Value = (p.Value == null) ? DBNull.Value : p.Value;

            return cp;
        }

        private static void RemoveNullValues(Dictionary<string, object> dict)
        {
            if (dict == null) return;
            var keys = dict.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                if (dict[k] == null)
                {
                    dict.Remove(k);
                }
            }
        }

        private static void AddIfHttpUrl(List<string> list, string url)
        {
            url = (url ?? string.Empty).Trim();
            if (!IsHttpUrl(url)) return;
            list.Add(url);
        }

        private static bool IsHttpUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                   || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region Obsolete Methods
        /// <summary>
        /// Builds a legacy company detail URL using query string parameter format.
        /// </summary>
        /// <param name="tabId">The DNN tab/page ID where the company detail will be displayed.</param>
        /// <param name="slug">The company's URL-friendly slug identifier.</param>
        /// <param name="paramName">The query string parameter name (default: "member").</param>
        /// <returns>A fully qualified DNN-compatible URL with the company slug as a query parameter, or empty string if slug is invalid.</returns>
        /// <remarks>
        /// This method is obsolete. Use <see cref="BuildCompanyDetailUrl"/> instead for better URL structure and DNN compatibility.
        /// </remarks>
        [Obsolete("Use BuildCompanyDetailUrl instead for better URL structure and DNN compatibility.")]
        public static string BuildCompanyUrl(int tabId, string slug, string paramName = "member")
        {
            throw new NotImplementedException("This method is obsolete. Use BuildCompanyDetailUrl instead.");
            slug = NormalizeToken(slug);
            if (string.IsNullOrWhiteSpace(slug)) return string.Empty;

            // Prefer NavigateURL so it respects DNN tab settings, alias, etc.
            // e.g. /Membership/Directory?company=my-business
            try
            {
                // DotNetNuke.Common.Globals is available in a typical DNN web context
                return DotNetNuke.Common.Globals.NavigateURL(tabId, string.Empty, $"{paramName}={HttpUtility.UrlEncode(slug)}");
            }
            catch
            {
                // last resort - avoid throwing inside templates
                return string.Empty;
            }
        }
        #endregion
    }
}