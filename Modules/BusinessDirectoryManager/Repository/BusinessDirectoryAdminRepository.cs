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

using Dapper;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Upendo.Modules.BusinessDirectoryManager.Data;
using Upendo.Modules.BusinessDirectoryManager.Repository.Contract;

namespace Upendo.Modules.BusinessDirectoryManager.Repository
{
    internal class BusinessDirectoryAdminRepository : IBusinessDirectoryAdminRepository
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(BusinessDirectoryAdminRepository));

        private readonly DapperContext _context;

        public BusinessDirectoryAdminRepository(DapperContext context)
        {
            _context = context;
        }

        public BusinessDirectoryConfigDto GetConfig(int portalId, bool includeCategories = true)
        {
            try
            {
                var result = new BusinessDirectoryConfigDto
                {
                    Portals = GetPortals(),
                    Categories = new List<CategoryOptionDto>(),
                    IsSuperUser = false
                };

                if (includeCategories && portalId > 0)
                {
                    using (var connection = _context.CreateConnection())
                    {
                        result.Categories = connection.Query<CategoryOptionDto>(
                            "dbo.bcc_Category_ListForPortal",
                            new { PortalId = portalId },
                            commandType: CommandType.StoredProcedure).ToList();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        public BusinessDirectoryListResponseDto GetBusinessesPage(BusinessDirectoryListRequestDto request)
        {
            try
            {
                if (request == null || request.PortalId <= 0)
                {
                    return new BusinessDirectoryListResponseDto();
                }

                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

                using (var connection = _context.CreateConnection())
                using (var multi = connection.QueryMultiple(
                           "dbo.bcc_Company_ListForAdminPaged",
                           new
                           {
                               PortalId = request.PortalId,
                               SearchText = string.IsNullOrWhiteSpace(request.SearchText) ? null : request.SearchText.Trim(),
                               StatusFilter = string.IsNullOrWhiteSpace(request.StatusFilter) ? null : request.StatusFilter.Trim(),
                               VisibilityFilter = string.IsNullOrWhiteSpace(request.VisibilityFilter) ? null : request.VisibilityFilter.Trim(),
                               FeaturedOnly = request.FeaturedOnly,
                               PageNumber = pageNumber,
                               PageSize = pageSize
                           },
                           commandType: CommandType.StoredProcedure))
                {
                    var items = multi.Read<BusinessDirectoryListItemDto>().ToList();
                    var summary = multi.Read<BusinessDirectoryListSummaryDto>().FirstOrDefault() ?? new BusinessDirectoryListSummaryDto();

                    return new BusinessDirectoryListResponseDto
                    {
                        Items = items,
                        TotalItems = summary.TotalItems,
                        ActiveCount = summary.ActiveCount,
                        PublicCount = summary.PublicCount,
                        FeaturedCount = summary.FeaturedCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        public BusinessDirectoryEditDto GetBusinessForEdit(int portalId, int companyId)
        {
            try
            {
                if (portalId <= 0 || companyId <= 0)
                {
                    return null;
                }

                using (var connection = _context.CreateConnection())
                using (var multi = connection.QueryMultiple(
                    "dbo.bcc_Company_GetForEdit",
                    new
                    {
                        PortalId = portalId,
                        CompanyId = companyId
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    var item = multi.Read<BusinessDirectoryEditDto>().FirstOrDefault();
                    if (item == null)
                    {
                        return null;
                    }

                    item.CategoryIds = multi.Read<int>().ToList();

                    return item;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        public SlugAvailabilityDto CheckSlugAvailability(int portalId, string slug, int? companyId)
        {
            try
            {
                slug = NormalizeSlug(slug);

                if (portalId <= 0 || string.IsNullOrWhiteSpace(slug))
                {
                    return new SlugAvailabilityDto
                    {
                        IsAvailable = false
                    };
                }

                using (var connection = _context.CreateConnection())
                {
                    return connection.QueryFirstOrDefault<SlugAvailabilityDto>(
                        "dbo.bcc_Company_IsSlugAvailable",
                        new
                        {
                            PortalId = portalId,
                            Slug = slug,
                            CompanyId = companyId
                        },
                        commandType: CommandType.StoredProcedure) ?? new SlugAvailabilityDto
                        {
                            IsAvailable = false
                        };
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        public int SaveBusiness(BusinessDirectorySaveRequest request, int currentUserId)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                NormalizeRequest(request);
                ValidateSaveRequest(request);

                var slugAvailability = CheckSlugAvailability(request.PortalId, request.Slug, request.CompanyId);
                if (slugAvailability == null || !slugAvailability.IsAvailable)
                {
                    throw new ArgumentException("Slug is already in use.");
                }

                using (var connection = _context.CreateConnection())
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var companyId = connection.QuerySingle<int>(
                                "dbo.bcc_Company_Save",
                                new
                                {
                                    CompanyId = request.CompanyId,
                                    PortalId = request.PortalId,
                                    LegacySystem = request.LegacySystem,
                                    LegacyCompanyId = request.LegacyCompanyId,
                                    CompanyName = request.CompanyName,
                                    Slug = request.Slug,
                                    ShortDescription = request.ShortDescription,
                                    LongDescription = request.LongDescription,
                                    WebsiteUrl = request.WebsiteUrl,
                                    Phone = request.Phone,
                                    Email = request.Email,
                                    PrimaryBusinessEmail = request.PrimaryBusinessEmail,
                                    Address1 = request.Address1,
                                    Address2 = request.Address2,
                                    City = request.City,
                                    Region = request.Region,
                                    PostalCode = request.PostalCode,
                                    Latitude = request.Latitude,
                                    Longitude = request.Longitude,
                                    MembershipStatus = request.MembershipStatus,
                                    MemberSinceYear = request.MemberSinceYear,
                                    PaidThroughDate = request.PaidThroughDate,
                                    RenewalDate = request.RenewalDate,
                                    IsFeatured = request.IsFeatured,
                                    FeaturedSortOrder = request.FeaturedSortOrder,
                                    LogoFileId = request.LogoFileId,
                                    LogoUrl = request.LogoUrl,
                                    LinkedInUrl = request.LinkedInUrl,
                                    FacebookUrl = request.FacebookUrl,
                                    InstagramUrl = request.InstagramUrl,
                                    TwitterUrl = request.TwitterUrl,
                                    TikTokUrl = request.TikTokUrl,
                                    IsPublic = request.IsPublic,
                                    IsActive = request.IsActive,
                                    SeoTitle = request.SeoTitle,
                                    SeoDescription = request.SeoDescription,
                                    CanonicalUrl = request.CanonicalUrl,
                                    MetaRobots = request.MetaRobots,
                                    OgImageUrl = request.OgImageUrl,
                                    UpdatedByUserId = currentUserId
                                },
                                transaction,
                                commandType: CommandType.StoredProcedure);

                            connection.Execute(
                                "dbo.bcc_CompanyCategory_ReplaceForCompany",
                                new
                                {
                                    CompanyId = companyId,
                                    CategoryIdsCsv = string.Join(",", request.CategoryIds ?? new List<int>())
                                },
                                transaction,
                                commandType: CommandType.StoredProcedure);

                            transaction.Commit();

                            return companyId;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private static IList<PortalOptionDto> GetPortals()
        {
            // TODO: Only return all portals if the current user is a superuser. Otherwise, return only the current portal.
            // TODO: (Longer-term, integrate portal groups and permissions to determine which portals the user should see.)  
            return PortalController.Instance.GetPortals()
                .Cast<PortalInfo>()
                .OrderBy(x => x.PortalName)
                .Select(x => new PortalOptionDto
                {
                    PortalId = x.PortalID,
                    PortalName = x.PortalName
                })
                .ToList();
        }

        private static void NormalizeRequest(BusinessDirectorySaveRequest request)
        {
            request.CompanyName = NullIfWhiteSpace(request.CompanyName);
            request.Slug = NormalizeSlug(request.Slug);

            request.LegacySystem = NullIfWhiteSpace(request.LegacySystem);
            request.LegacyCompanyId = NullIfWhiteSpace(request.LegacyCompanyId);

            request.ShortDescription = NullIfWhiteSpace(request.ShortDescription);
            request.LongDescription = NullIfWhiteSpace(request.LongDescription);

            request.WebsiteUrl = NullIfWhiteSpace(request.WebsiteUrl);
            request.Phone = NullIfWhiteSpace(request.Phone);
            request.Email = NullIfWhiteSpace(request.Email);
            request.PrimaryBusinessEmail = NullIfWhiteSpace(request.PrimaryBusinessEmail);

            request.Address1 = NullIfWhiteSpace(request.Address1);
            request.Address2 = NullIfWhiteSpace(request.Address2);
            request.City = NullIfWhiteSpace(request.City);
            request.Region = NullIfWhiteSpace(request.Region);
            request.PostalCode = NullIfWhiteSpace(request.PostalCode);

            request.LogoUrl = NullIfWhiteSpace(request.LogoUrl);

            request.LinkedInUrl = NullIfWhiteSpace(request.LinkedInUrl);
            request.FacebookUrl = NullIfWhiteSpace(request.FacebookUrl);
            request.InstagramUrl = NullIfWhiteSpace(request.InstagramUrl);
            request.TwitterUrl = NullIfWhiteSpace(request.TwitterUrl);
            request.TikTokUrl = NullIfWhiteSpace(request.TikTokUrl);

            request.SeoTitle = NullIfWhiteSpace(request.SeoTitle);
            request.SeoDescription = NullIfWhiteSpace(request.SeoDescription);
            request.CanonicalUrl = NullIfWhiteSpace(request.CanonicalUrl);
            request.MetaRobots = NullIfWhiteSpace(request.MetaRobots);
            request.OgImageUrl = NullIfWhiteSpace(request.OgImageUrl);

            request.CategoryIds = (request.CategoryIds ?? new List<int>())
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (string.IsNullOrWhiteSpace(request.Slug) && !string.IsNullOrWhiteSpace(request.CompanyName))
            {
                request.Slug = NormalizeSlug(request.CompanyName);
            }
        }

        private static void ValidateSaveRequest(BusinessDirectorySaveRequest request)
        {
            if (request.PortalId <= 0)
            {
                throw new ArgumentException("A valid portal is required.");
            }

            if (string.IsNullOrWhiteSpace(request.CompanyName))
            {
                throw new ArgumentException("Company name is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                throw new ArgumentException("Slug is required.");
            }

            if (request.SeoTitle != null && request.SeoTitle.Length > 255)
            {
                throw new ArgumentException("SEO title cannot exceed 255 characters.");
            }

            if (request.SeoDescription != null && request.SeoDescription.Length > 500)
            {
                throw new ArgumentException("SEO description cannot exceed 500 characters.");
            }

            if (request.MetaRobots != null && request.MetaRobots.Length > 100)
            {
                throw new ArgumentException("Meta robots cannot exceed 100 characters.");
            }

            if (!IsValidOptionalHttpUrl(request.WebsiteUrl))
            {
                throw new ArgumentException("Website URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.LogoUrl))
            {
                throw new ArgumentException("Logo URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.CanonicalUrl))
            {
                throw new ArgumentException("Canonical URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.OgImageUrl))
            {
                throw new ArgumentException("Open Graph Image URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.LinkedInUrl))
            {
                throw new ArgumentException("LinkedIn URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.FacebookUrl))
            {
                throw new ArgumentException("Facebook URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.InstagramUrl))
            {
                throw new ArgumentException("Instagram URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.TwitterUrl))
            {
                throw new ArgumentException("Twitter/X URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalHttpUrl(request.TikTokUrl))
            {
                throw new ArgumentException("TikTok URL must be a valid http or https URL.");
            }

            if (!IsValidOptionalEmail(request.Email))
            {
                throw new ArgumentException("Email must be a valid email address.");
            }

            if (!IsValidOptionalEmail(request.PrimaryBusinessEmail))
            {
                throw new ArgumentException("Primary business email must be a valid email address.");
            }

            if (request.MemberSinceYear.HasValue)
            {
                var maxYear = DateTime.UtcNow.Year + 1;

                if (request.MemberSinceYear.Value < 1900 || request.MemberSinceYear.Value > maxYear)
                {
                    throw new ArgumentException(string.Format("Member since year must be between 1900 and {0}.", maxYear));
                }
            }

            if (request.Latitude.HasValue && (request.Latitude.Value < -90m || request.Latitude.Value > 90m))
            {
                throw new ArgumentException("Latitude must be between -90 and 90.");
            }

            if (request.Longitude.HasValue && (request.Longitude.Value < -180m || request.Longitude.Value > 180m))
            {
                throw new ArgumentException("Longitude must be between -180 and 180.");
            }
        }

        private static string NormalizeSlug(string value)
        {
            value = NullIfWhiteSpace(value);

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var slug = value.Trim().ToLowerInvariant();
            slug = Regex.Replace(slug, @"[^a-z0-9]+", "-");
            slug = Regex.Replace(slug, @"-+", "-").Trim('-');

            if (slug.Length > 220)
            {
                slug = slug.Substring(0, 220).Trim('-');
            }

            return slug;
        }

        private static string NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static bool IsValidOptionalHttpUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            Uri uri;
            return Uri.TryCreate(value, UriKind.Absolute, out uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static bool IsValidOptionalEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            return Regex.IsMatch(value, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
        }

        private static void LogError(Exception ex)
        {
            Logger.Error(ex);
        }
    }
}