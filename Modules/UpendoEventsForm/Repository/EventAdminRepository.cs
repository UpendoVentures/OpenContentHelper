using Dapper;
using DotNetNuke.Instrumentation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Upendo.Modules.UpendoEventsForm.Components;
using Upendo.Modules.UpendoEventsForm.Data;
using Upendo.Modules.UpendoEventsForm.Repository.Contract;
using Upendo.Modules.UpendoEventsForm.Security;

namespace Upendo.Modules.UpendoEventsForm.Repository
{
    internal class EventAdminRepository : IEventAdminRepository
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(EventAdminRepository));
        private static readonly HttpClient VenueHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
        private readonly DapperContext _context;
        private readonly DnnDataHelper _dnnDataHelper;

        public EventAdminRepository(DapperContext context, DnnDataHelper dnnDataHelper)
        {
            _context = context;
            _dnnDataHelper = dnnDataHelper;
        }

        public EventFormConfigDto GetConfig(int portalId)
        {
            try
            {
                if (portalId <= 0)
                {
                    return new EventFormConfigDto
                    {
                        Portals = _dnnDataHelper.GetPortals(),
                        Categories = new List<CategoryOptionDto>(),
                        Tags = new List<TagOptionDto>(),
                        Organizers = new List<UserOptionDto>(),
                        Folders = new List<FolderOptionDto>(),
                        Files = new List<EventFileOptionDto>(),
                        TimeZoneOptions = GetTimeZoneOptions(),
                        DefaultTimeZoneId = TimeZoneInfo.Local.Id
                    };
                }

                return EventAdminCacheHelper.GetOrAdd(
                    EventAdminCacheHelper.Config(portalId),
                    TimeSpan.FromMinutes(10),
                    () =>
                    {
                        var config = new EventFormConfigDto
                        {
                            Portals = _dnnDataHelper.GetPortals(),
                            Categories = new List<CategoryOptionDto>(),
                            Tags = new List<TagOptionDto>(),
                            Organizers = new List<UserOptionDto>(),
                            Folders = new List<FolderOptionDto>(),
                            Files = new List<EventFileOptionDto>(),
                            TimeZoneOptions = GetTimeZoneOptions(),
                            DefaultTimeZoneId = TimeZoneInfo.Local.Id
                        };

                        config.Folders = _dnnDataHelper.GetFolders(portalId);
                        config.Organizers = _dnnDataHelper.GetPortalUsers(portalId);

                        using (var connection = _context.CreateConnection())
                        {
                            config.Categories = connection.Query<CategoryOptionDto>(
                                "dbo.uv_EventCategory_ListByPortal",
                                new { PortalId = portalId },
                                commandType: CommandType.StoredProcedure).ToList();

                            config.Tags = connection.Query<TagOptionDto>(
                                "dbo.uv_Tag_ListByPortal",
                                new { PortalId = portalId },
                                commandType: CommandType.StoredProcedure).ToList();
                        }

                        return config;
                    });
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        public EventFormEditDto GetEventForEdit(int portalId, int eventId)
        {
            try
            {
                if (portalId <= 0 || eventId <= 0)
                {
                    return null;
                }

                using (var connection = _context.CreateConnection())
                using (var multi = connection.QueryMultiple(
                    "dbo.uv_Event_GetForEdit",
                    new
                    {
                        PortalId = portalId,
                        EventId = eventId
                    },
                    commandType: CommandType.StoredProcedure))
                {
                    var row = multi.Read<EventEditRow>().FirstOrDefault();
                    if (row == null)
                    {
                        return null;
                    }

                    var timeZone = ResolveTimeZone(row.TimeZoneId);

                    var result = new EventFormEditDto
                    {
                        EventId = row.EventId,
                        PortalId = row.PortalId,
                        EventStatusId = row.EventStatusId,
                        OrganizerUserId = row.OrganizerUserId,
                        VenueId = row.VenueId,
                        EventCategoryId = row.EventCategoryId,
                        Title = row.Title,
                        Slug = row.Slug,
                        ShortSummary = row.ShortSummary,
                        FullDescription = row.FullDescription,
                        StartDateTimeLocalText = ToDateTimeLocalInputText(row.StartDateTimeUtc, timeZone),
                        EndDateTimeLocalText = ToDateTimeLocalInputText(row.EndDateTimeUtc, timeZone),
                        TimeZoneId = string.IsNullOrWhiteSpace(row.TimeZoneId) ? timeZone.Id : row.TimeZoneId,
                        IsAllDay = row.IsAllDay,
                        DisplayDateText = row.DisplayDateText,
                        DisplayTimeText = row.DisplayTimeText,
                        HeroImageUrl = row.HeroImageUrl,
                        ListImageUrl = row.ListImageUrl,
                        ThumbnailImageUrl = row.ThumbnailImageUrl,
                        ImageAltText = row.ImageAltText,
                        RegistrationUrl = row.RegistrationUrl,
                        RegistrationButtonText = row.RegistrationButtonText,
                        SecondaryCtaUrl = row.SecondaryCtaUrl,
                        SecondaryCtaText = row.SecondaryCtaText,
                        PriceAmount = row.PriceAmount,
                        PriceCurrencyCode = row.PriceCurrencyCode,
                        PriceLabel = row.PriceLabel,
                        IsFree = row.IsFree,
                        Capacity = row.Capacity,
                        CapacitySummaryText = row.CapacitySummaryText,
                        ContactEmail = row.ContactEmail,
                        ContactPhone = row.ContactPhone,
                        ContactUrl = row.ContactUrl,
                        SeoTitle = row.SeoTitle,
                        SeoDescription = row.SeoDescription,
                        CanonicalUrl = row.CanonicalUrl,
                        MetaRobots = row.MetaRobots,
                        SortOrder = row.SortOrder,
                        IsFeatured = row.IsFeatured,
                        AllowPublicDetailPage = row.AllowPublicDetailPage,
                        PublishStartLocalText = ToDateTimeLocalInputText(row.PublishStartUtc, timeZone),
                        PublishEndLocalText = ToDateTimeLocalInputText(row.PublishEndUtc, timeZone),
                        DownloadFileId = row.DownloadFileId,
                        DownloadFileUrl = row.DownloadFileUrl,
                        UseCustomLocationText = row.UseCustomLocationText,
                        VenueName = row.VenueName,
                        PublicLocationText = row.PublicLocationText,
                        AddressLine1 = row.AddressLine1,
                        AddressLine2 = row.AddressLine2,
                        City = row.City,
                        Region = row.Region,
                        PostalCode = row.PostalCode,
                        CountryCode = row.CountryCode,
                        Latitude = row.Latitude,
                        Longitude = row.Longitude,
                        VenueWebsiteUrl = row.VenueWebsiteUrl,
                        VenuePhoneNumber = row.VenuePhoneNumber,
                        TagIds = multi.Read<int>().ToList(),
                        Features = multi.Read<EventFeatureInputDto>().ToList(),
                        Audiences = multi.Read<EventAudienceInputDto>().ToList()
                    };

                    if (result.TagIds == null)
                    {
                        result.TagIds = new List<int>();
                    }

                    if (result.Features == null)
                    {
                        result.Features = new List<EventFeatureInputDto>();
                    }

                    if (result.Audiences == null)
                    {
                        result.Audiences = new List<EventAudienceInputDto>();
                    }

                    return result;
                }
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        public bool IsSlugAvailable(int portalId, string slug, int? eventId)
        {
            if (portalId <= 0 || string.IsNullOrWhiteSpace(slug))
            {
                return false;
            }

            var normalizedSlug = NormalizeEventSlug(slug);
            if (string.IsNullOrWhiteSpace(normalizedSlug))
            {
                return false;
            }

            using (var connection = _context.CreateConnection())
            {
                return connection.QuerySingle<bool>(
                    "dbo.uv_Event_IsSlugAvailable",
                    new
                    {
                        PortalId = portalId,
                        Slug = normalizedSlug,
                        EventId = eventId
                    },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public int SaveEvent(EventFormSaveRequest request, int currentUserId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            NormalizeRequest(request);

            if (string.IsNullOrWhiteSpace(request.Slug) && !string.IsNullOrWhiteSpace(request.Title))
            {
                request.Slug = BuildEventSlug(request.Title);
            }

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var existingPortalId = GetExistingEventPortalId(connection, transaction, request.EventId);

                        ValidateSaveRequest(connection, transaction, request);

                        var timeZone = ResolveTimeZone(request.TimeZoneId);
                        var startDateTimeUtc = ConvertLocalInputToUtc(request.StartDateTimeLocalText, timeZone);
                        var endDateTimeUtc = ConvertLocalInputToUtcNullable(request.EndDateTimeLocalText, timeZone);
                        var publishStartUtc = ConvertLocalInputToUtcNullable(request.PublishStartLocalText, timeZone);
                        var publishEndUtc = ConvertLocalInputToUtcNullable(request.PublishEndLocalText, timeZone);

                        int? organizerId = null;
                        if (request.OrganizerUserId.HasValue && request.OrganizerUserId.Value > 0)
                        {
                            organizerId = connection.QuerySingle<int>(
                                "dbo.uv_Organizer_UpsertFromPortalUser",
                                new
                                {
                                    PortalId = request.PortalId,
                                    DnnUserId = request.OrganizerUserId.Value
                                },
                                transaction,
                                commandType: CommandType.StoredProcedure);
                        }

                        int? venueId = SaveVenueIfNeeded(connection, transaction, request);

                        var eventId = connection.QuerySingle<int>(
                            "dbo.uv_Event_Save",
                            new
                            {
                                EventId = request.EventId,
                                PortalId = request.PortalId,
                                EventStatusId = request.EventStatusId,
                                EventCategoryId = request.EventCategoryId,
                                OrganizerId = organizerId,
                                VenueId = venueId,
                                Title = request.Title,
                                Slug = request.Slug,
                                ShortSummary = request.ShortSummary,
                                FullDescription = request.FullDescription,
                                StartDateTimeUtc = startDateTimeUtc,
                                EndDateTimeUtc = endDateTimeUtc,
                                TimeZoneId = request.TimeZoneId,
                                IsAllDay = request.IsAllDay,
                                DisplayDateText = request.DisplayDateText,
                                DisplayTimeText = request.DisplayTimeText,
                                HeroImageUrl = request.HeroImageUrl,
                                ListImageUrl = request.ListImageUrl,
                                ThumbnailImageUrl = request.ThumbnailImageUrl,
                                ImageAltText = request.ImageAltText,
                                RegistrationUrl = request.RegistrationUrl,
                                RegistrationButtonText = request.RegistrationButtonText,
                                SecondaryCtaUrl = request.SecondaryCtaUrl,
                                SecondaryCtaText = request.SecondaryCtaText,
                                PriceAmount = request.IsFree ? null : request.PriceAmount,
                                PriceCurrencyCode = request.IsFree ? null : request.PriceCurrencyCode,
                                PriceLabel = request.PriceLabel,
                                IsFree = request.IsFree,
                                Capacity = request.Capacity,
                                CapacitySummaryText = request.CapacitySummaryText,
                                ContactEmail = request.ContactEmail,
                                ContactPhone = request.ContactPhone,
                                ContactUrl = request.ContactUrl,
                                SeoTitle = request.SeoTitle,
                                SeoDescription = request.SeoDescription,
                                CanonicalUrl = request.CanonicalUrl,
                                MetaRobots = request.MetaRobots,
                                SortOrder = request.SortOrder,
                                IsFeatured = request.IsFeatured,
                                AllowPublicDetailPage = request.AllowPublicDetailPage,
                                PublishStartUtc = publishStartUtc,
                                PublishEndUtc = publishEndUtc,
                                CreatedByUserId = currentUserId,
                                LastModifiedByUserId = currentUserId,
                                DownloadFileId = request.DownloadFileId,
                                DownloadFileUrl = request.DownloadFileUrl
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure);

                        ReplaceEventTags(connection, transaction, eventId, request.TagIds);
                        ReplaceEventFeatures(connection, transaction, eventId, request.Features);
                        ReplaceEventAudiences(connection, transaction, eventId, request.Audiences);

                        transaction.Commit();

                        EventsCacheInvalidationHelper.BumpPortalVersion(request.PortalId);

                        if (existingPortalId.HasValue && existingPortalId.Value > 0 && existingPortalId.Value != request.PortalId)
                        {
                            EventsCacheInvalidationHelper.BumpPortalVersion(existingPortalId.Value);
                        }

                        return eventId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public CategoryOptionDto CreateCategory(int portalId, string name)
        {
            if (portalId <= 0)
            {
                throw new ArgumentException("A valid portal is required.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name is required.");
            }

            var trimmedName = name.Trim();

            using (var connection = _context.CreateConnection())
            {
                var existing = connection.QueryFirstOrDefault<CategoryOptionDto>(
                    @"
            SELECT TOP 1
                EventCategoryId,
                PortalId,
                CategoryName,
                Slug,
                Description,
                SortOrder,
                IsActive
            FROM dbo.uv_EventCategory
            WHERE PortalId = @PortalId
              AND CategoryName = @CategoryName",
                    new
                    {
                        PortalId = portalId,
                        CategoryName = trimmedName
                    });

                if (existing != null)
                {
                    EventsCacheInvalidationHelper.BumpPortalVersion(portalId);
                    return existing;
                }

                var slug = GetUniqueCategorySlug(connection, portalId, trimmedName);

                var result = connection.QuerySingle<CategoryOptionDto>(
                    "dbo.uv_EventCategory_Create",
                    new
                    {
                        PortalId = portalId,
                        CategoryName = trimmedName,
                        Slug = slug,
                        Description = (string)null
                    },
                    commandType: CommandType.StoredProcedure);

                EventsCacheInvalidationHelper.BumpPortalVersion(portalId);

                return result;
            }
        }

        public TagOptionDto CreateTag(int portalId, string name)
        {
            if (portalId <= 0)
            {
                throw new ArgumentException("A valid portal is required.");
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tag name is required.");
            }

            var trimmedName = name.Trim();

            using (var connection = _context.CreateConnection())
            {
                var existing = connection.QueryFirstOrDefault<TagOptionDto>(
                    @"
            SELECT TOP 1
                TagId,
                TagName,
                Slug
            FROM dbo.uv_Tag
            WHERE PortalId = @PortalId
              AND TagName = @TagName",
                    new
                    {
                        PortalId = portalId,
                        TagName = trimmedName
                    });

                if (existing != null)
                {
                    EventsCacheInvalidationHelper.BumpPortalVersion(portalId);
                    return existing;
                }

                var slug = GetUniqueTagSlug(connection, portalId, trimmedName);

                var result = connection.QuerySingle<TagOptionDto>(
                    "dbo.uv_Tag_Create",
                    new
                    {
                        PortalId = portalId,
                        TagName = trimmedName,
                        Slug = slug
                    },
                    commandType: CommandType.StoredProcedure);

                EventsCacheInvalidationHelper.BumpPortalVersion(portalId);

                return result;
            }
        }

        public IList<LocationSuggestionDto> SearchVenueSuggestions(int portalId, string query, int limit)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<LocationSuggestionDto>();
            }

            var trimmedQuery = query.Trim();
            if (trimmedQuery.Length < 3)
            {
                return new List<LocationSuggestionDto>();
            }

            if (limit <= 0)
            {
                limit = 8;
            }

            if (limit > 20)
            {
                limit = 20;
            }

            var apiKey = ConfigurationManager.AppSettings["LocationIqApiKey"];
            var countryCodes = ConfigurationManager.AppSettings["LocationIqCountryCodes"];
            var acceptLanguage = ConfigurationManager.AppSettings["LocationIqAcceptLanguage"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("The LocationIQ API key is not configured.");
            }

            var requestUrl =
                "https://api.locationiq.com/v1/autocomplete" +
                "?key=" + Uri.EscapeDataString(apiKey.Trim()) +
                "&q=" + Uri.EscapeDataString(trimmedQuery) +
                "&limit=" + limit.ToString(CultureInfo.InvariantCulture) +
                "&normalizecity=1";

            if (!string.IsNullOrWhiteSpace(countryCodes))
            {
                requestUrl += "&countrycodes=" + Uri.EscapeDataString(countryCodes.Trim());
            }

            if (!string.IsNullOrWhiteSpace(acceptLanguage))
            {
                requestUrl += "&accept-language=" + Uri.EscapeDataString(acceptLanguage.Trim());
            }

            var json = VenueHttpClient.GetStringAsync(requestUrl).GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<LocationSuggestionDto>();
            }

            var results = JsonConvert.DeserializeObject<List<LocationIqAutocompleteResponseDto>>(json)
                          ?? new List<LocationIqAutocompleteResponseDto>();

            return results
                .Select(MapLocationSuggestion)
                .Where(x => x != null)
                .ToList();
        }

        public IList<EventFileOptionDto> GetFilesByFolder(int portalId, int folderId)
        {
            if (portalId <= 0 || folderId <= 0)
            {
                return new List<EventFileOptionDto>();
            }

            return EventAdminCacheHelper.GetOrAdd(
                EventAdminCacheHelper.FilesByFolder(portalId, folderId),
                TimeSpan.FromMinutes(10),
                () => _dnnDataHelper.GetFilesByFolder(portalId, folderId).ToList());
        }

        public EventFileOptionDto GetFileById(int portalId, int fileId)
        {
            if (portalId <= 0 || fileId <= 0)
            {
                return null;
            }

            return EventAdminCacheHelper.GetOrAdd(
                EventAdminCacheHelper.FileById(portalId, fileId),
                TimeSpan.FromMinutes(10),
                () => _dnnDataHelper.GetFileById(portalId, fileId));
        }

        #region Private Helper Methods

        private static IList<TimeZoneOptionDto> GetTimeZoneOptions()
        {
            return TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(tz => tz.BaseUtcOffset)
                .ThenBy(tz => tz.DisplayName)
                .Select(tz => new TimeZoneOptionDto
                {
                    TimeZoneId = tz.Id,
                    DisplayName = tz.DisplayName
                })
                .ToList();
        }

        private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
            {
                return TimeZoneInfo.Local;
            }

            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch
            {
                return TimeZoneInfo.Local;
            }
        }

        private static string ToDateTimeLocalInputText(DateTime? utcValue, TimeZoneInfo timeZone)
        {
            if (!utcValue.HasValue)
            {
                return string.Empty;
            }

            var utc = utcValue.Value;

            if (utc.Kind == DateTimeKind.Unspecified)
            {
                utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            }
            else if (utc.Kind == DateTimeKind.Local)
            {
                utc = utc.ToUniversalTime();
            }

            var localValue = TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone);
            return localValue.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
        }

        private const int EventSlugMaxLength = 220;
        private static readonly Regex SlugCleanupRegex = new Regex(@"[^a-z0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex MultiHyphenRegex = new Regex(@"-+", RegexOptions.Compiled);

        private static string BuildEventSlug(string title)
        {
            var year = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            var suffix = "-" + year;
            var maxBaseLength = EventSlugMaxLength - suffix.Length;

            var baseSlug = NormalizeEventSlug(title, maxBaseLength);
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                return string.Empty;
            }

            return baseSlug.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)
                ? baseSlug
                : string.Concat(baseSlug, suffix);
        }

        private static string NormalizeEventSlug(string value, int maxLength = EventSlugMaxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var lower = value.Trim().ToLowerInvariant();
            var slug = SlugCleanupRegex.Replace(lower, "-");
            slug = MultiHyphenRegex.Replace(slug, "-").Trim('-');

            if (slug.Length > maxLength)
            {
                slug = slug.Substring(0, maxLength).TrimEnd('-');
            }

            return slug;
        }

        private static void NormalizeRequest(EventFormSaveRequest request)
        {
            request.Title = NullIfWhiteSpace(request.Title);
            request.Slug = NullIfWhiteSpace(request.Slug);
            if (!string.IsNullOrWhiteSpace(request.Slug))
            {
                request.Slug = NormalizeEventSlug(request.Slug);
            }
            request.ShortSummary = NullIfWhiteSpace(request.ShortSummary);
            request.FullDescription = EventRichTextSanitizer.SanitizeFullDescription(request.FullDescription);
            request.TimeZoneId = NullIfWhiteSpace(request.TimeZoneId);
            request.DisplayDateText = NullIfWhiteSpace(request.DisplayDateText);
            request.DisplayTimeText = NullIfWhiteSpace(request.DisplayTimeText);
            request.HeroImageUrl = NullIfWhiteSpace(request.HeroImageUrl);
            request.ListImageUrl = NullIfWhiteSpace(request.ListImageUrl);
            request.ThumbnailImageUrl = NullIfWhiteSpace(request.ThumbnailImageUrl);
            request.ImageAltText = NullIfWhiteSpace(request.ImageAltText);
            request.RegistrationUrl = NullIfWhiteSpace(request.RegistrationUrl);
            request.RegistrationButtonText = NullIfWhiteSpace(request.RegistrationButtonText);
            request.SecondaryCtaUrl = NullIfWhiteSpace(request.SecondaryCtaUrl);
            request.SecondaryCtaText = NullIfWhiteSpace(request.SecondaryCtaText);
            request.PriceCurrencyCode = NullIfWhiteSpace(request.PriceCurrencyCode);
            request.PriceLabel = NullIfWhiteSpace(request.PriceLabel);
            request.CapacitySummaryText = NullIfWhiteSpace(request.CapacitySummaryText);
            request.ContactEmail = NullIfWhiteSpace(request.ContactEmail);
            request.ContactPhone = NullIfWhiteSpace(request.ContactPhone);
            request.ContactUrl = NullIfWhiteSpace(request.ContactUrl);
            request.SeoTitle = NullIfWhiteSpace(request.SeoTitle);
            request.SeoDescription = NullIfWhiteSpace(request.SeoDescription);
            request.CanonicalUrl = NullIfWhiteSpace(request.CanonicalUrl);
            request.MetaRobots = NullIfWhiteSpace(request.MetaRobots);
            request.PublishStartLocalText = NullIfWhiteSpace(request.PublishStartLocalText);
            request.PublishEndLocalText = NullIfWhiteSpace(request.PublishEndLocalText);
            request.DownloadFileUrl = NullIfWhiteSpace(request.DownloadFileUrl);
            request.VenueName = NullIfWhiteSpace(request.VenueName);
            request.PublicLocationText = NullIfWhiteSpace(request.PublicLocationText);
            request.AddressLine1 = NullIfWhiteSpace(request.AddressLine1);
            request.AddressLine2 = NullIfWhiteSpace(request.AddressLine2);
            request.City = NullIfWhiteSpace(request.City);
            request.Region = NullIfWhiteSpace(request.Region);
            request.PostalCode = NullIfWhiteSpace(request.PostalCode);
            request.CountryCode = NullIfWhiteSpace(request.CountryCode);
            request.VenueWebsiteUrl = NullIfWhiteSpace(request.VenueWebsiteUrl);
            request.VenuePhoneNumber = NullIfWhiteSpace(request.VenuePhoneNumber);
        }

        private void ValidateSaveRequest(IDbConnection connection, IDbTransaction transaction, EventFormSaveRequest request)
        {
            var errors = new List<string>();

            if (request.PortalId <= 0)
            {
                errors.Add("Portal is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                errors.Add("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                errors.Add("Slug is required.");
            }

            if (string.IsNullOrWhiteSpace(request.TimeZoneId))
            {
                errors.Add("Time zone is required.");
            }

            DateTime parsedStartLocal;
            if (!TryParseLocalInput(request.StartDateTimeLocalText, out parsedStartLocal))
            {
                errors.Add("Start date and time is required.");
            }

            DateTime parsedEndLocal;
            if (!string.IsNullOrWhiteSpace(request.EndDateTimeLocalText) && !TryParseLocalInput(request.EndDateTimeLocalText, out parsedEndLocal))
            {
                errors.Add("End date and time is invalid.");
            }

            if (TryParseLocalInput(request.StartDateTimeLocalText, out parsedStartLocal) &&
                TryParseLocalInput(request.EndDateTimeLocalText, out parsedEndLocal) &&
                parsedEndLocal < parsedStartLocal)
            {
                errors.Add("End date and time cannot be earlier than the start date and time.");
            }

            DateTime parsedPublishStartLocal;
            DateTime parsedPublishEndLocal;

            if (!string.IsNullOrWhiteSpace(request.PublishStartLocalText) &&
                !TryParseLocalInput(request.PublishStartLocalText, out parsedPublishStartLocal))
            {
                errors.Add("Publish start is invalid.");
            }

            if (!string.IsNullOrWhiteSpace(request.PublishEndLocalText) &&
                !TryParseLocalInput(request.PublishEndLocalText, out parsedPublishEndLocal))
            {
                errors.Add("Publish end is invalid.");
            }

            if (TryParseLocalInput(request.PublishStartLocalText, out parsedPublishStartLocal) &&
                TryParseLocalInput(request.PublishEndLocalText, out parsedPublishEndLocal) &&
                parsedPublishEndLocal < parsedPublishStartLocal)
            {
                errors.Add("Publish end cannot be earlier than publish start.");
            }

            if (request.PriceAmount.HasValue && request.PriceAmount.Value < 0)
            {
                errors.Add("Price amount cannot be negative.");
            }

            if (request.Capacity.HasValue && request.Capacity.Value < 0)
            {
                errors.Add("Capacity cannot be negative.");
            }

            if (!string.IsNullOrWhiteSpace(request.ContactEmail) && !IsValidEmail(request.ContactEmail))
            {
                errors.Add("Contact email is invalid.");
            }

            if (!IsValidOptionalUrl(request.RegistrationUrl))
            {
                errors.Add("Registration URL is invalid.");
            }

            if (!IsValidOptionalUrl(request.SecondaryCtaUrl))
            {
                errors.Add("Secondary CTA URL is invalid.");
            }

            if (!IsValidOptionalUrl(request.ContactUrl))
            {
                errors.Add("Contact URL is invalid.");
            }

            if (!IsValidOptionalUrl(request.CanonicalUrl))
            {
                errors.Add("Canonical URL is invalid.");
            }

            if (!IsValidOptionalUrl(request.VenueWebsiteUrl))
            {
                errors.Add("Venue website URL is invalid.");
            }

            if (request.DownloadFileId.HasValue &&
                request.DownloadFileId.Value > 0 &&
                !_dnnDataHelper.FileBelongsToPortal(request.PortalId, request.DownloadFileId))
            {
                errors.Add("The selected download file does not belong to the selected portal.");
            }

            if (request.OrganizerUserId.HasValue &&
                request.OrganizerUserId.Value > 0 &&
                !_dnnDataHelper.GetPortalUsers(request.PortalId).Any(x => x.UserId == request.OrganizerUserId.Value))
            {
                errors.Add("The selected organizer is not valid for the selected portal.");
            }

            if (request.EventCategoryId.HasValue && request.EventCategoryId.Value > 0)
            {
                var categoryCount = connection.ExecuteScalar<int>(
                    @"
            SELECT COUNT(1)
            FROM dbo.uv_EventCategory
            WHERE EventCategoryId = @EventCategoryId
              AND PortalId = @PortalId
              AND IsActive = 1",
                    new
                    {
                        EventCategoryId = request.EventCategoryId.Value,
                        PortalId = request.PortalId
                    },
                    transaction);

                if (categoryCount <= 0)
                {
                    errors.Add("The selected category is not valid for the selected portal.");
                }
            }

            var tagIds = (request.TagIds ?? new List<int>())
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            if (tagIds.Any())
            {
                var tagCount = connection.ExecuteScalar<int>(
                    @"
            SELECT COUNT(1)
            FROM dbo.uv_Tag
            WHERE PortalId = @PortalId
              AND TagId IN @TagIds
              AND IsActive = 1",
                    new
                    {
                        PortalId = request.PortalId,
                        TagIds = tagIds
                    },
                    transaction);

                if (tagCount != tagIds.Count)
                {
                    errors.Add("One or more selected tags are not valid for the selected portal.");
                }
            }

            if (!IsSlugAvailableInternal(connection, transaction, request.PortalId, request.Slug, request.EventId))
            {
                errors.Add("The slug must be unique within the selected portal.");
            }

            if (request.EventId.HasValue && request.EventId.Value > 0)
            {
                var eventCount = connection.ExecuteScalar<int>(
                    @"
            SELECT COUNT(1)
            FROM dbo.uv_Event
            WHERE EventId = @EventId
              AND PortalId = @PortalId
              AND IsDeleted = 0",
                    new
                    {
                        EventId = request.EventId.Value,
                        PortalId = request.PortalId
                    },
                    transaction);

                if (eventCount <= 0)
                {
                    errors.Add("The event being edited is not valid for the selected portal.");
                }
            }

            ValidateFeatures(request.Features, errors);
            ValidateAudiences(request.Audiences, errors);

            if (errors.Any())
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }
        }

        private static void ValidateFeatures(IList<EventFeatureInputDto> features, IList<string> errors)
        {
            if (features == null)
            {
                return;
            }

            for (var i = 0; i < features.Count; i++)
            {
                var feature = features[i];
                if (feature == null)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(feature.Description) || !string.IsNullOrWhiteSpace(feature.IconClass))
                {
                    if (string.IsNullOrWhiteSpace(feature.Title))
                    {
                        errors.Add(string.Format(CultureInfo.InvariantCulture, "Feature {0} must have a title.", i + 1));
                    }
                }
            }
        }

        private static void ValidateAudiences(IList<EventAudienceInputDto> audiences, IList<string> errors)
        {
            if (audiences == null)
            {
                return;
            }

            for (var i = 0; i < audiences.Count; i++)
            {
                var audience = audiences[i];
                if (audience == null)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(audience.Description) && string.IsNullOrWhiteSpace(audience.Title))
                {
                    errors.Add(string.Format(CultureInfo.InvariantCulture, "Audience {0} must have a title.", i + 1));
                }
            }
        }

        private int? SaveVenueIfNeeded(IDbConnection connection, IDbTransaction transaction, EventFormSaveRequest request)
        {
            if (!request.UseCustomLocationText)
            {
                return null;
            }

            if (!HasVenueData(request))
            {
                return null;
            }

            var venueName = !string.IsNullOrWhiteSpace(request.VenueName)
                ? request.VenueName
                : request.PublicLocationText;

            return connection.QuerySingle<int>(
                "dbo.uv_Venue_Save",
                new
                {
                    VenueId = request.VenueId,
                    VenueName = venueName,
                    AddressLine1 = request.AddressLine1,
                    AddressLine2 = request.AddressLine2,
                    City = request.City,
                    Region = request.Region,
                    PostalCode = request.PostalCode,
                    CountryCode = request.CountryCode,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    TimeZoneId = request.TimeZoneId,
                    PublicLocationText = request.PublicLocationText,
                    WebsiteUrl = request.VenueWebsiteUrl,
                    PhoneNumber = request.VenuePhoneNumber
                },
                transaction,
                commandType: CommandType.StoredProcedure);
        }

        private static bool HasVenueData(EventFormSaveRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.VenueName) ||
                   !string.IsNullOrWhiteSpace(request.PublicLocationText) ||
                   !string.IsNullOrWhiteSpace(request.AddressLine1) ||
                   !string.IsNullOrWhiteSpace(request.AddressLine2) ||
                   !string.IsNullOrWhiteSpace(request.City) ||
                   !string.IsNullOrWhiteSpace(request.Region) ||
                   !string.IsNullOrWhiteSpace(request.PostalCode) ||
                   !string.IsNullOrWhiteSpace(request.CountryCode) ||
                   request.Latitude.HasValue ||
                   request.Longitude.HasValue ||
                   !string.IsNullOrWhiteSpace(request.VenueWebsiteUrl) ||
                   !string.IsNullOrWhiteSpace(request.VenuePhoneNumber);
        }

        private static void ReplaceEventTags(IDbConnection connection, IDbTransaction transaction, int eventId, IList<int> tagIds)
        {
            connection.Execute(
                "DELETE FROM dbo.uv_EventTag WHERE EventId = @EventId",
                new { EventId = eventId },
                transaction);

            var distinctTagIds = (tagIds ?? new List<int>())
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            for (var i = 0; i < distinctTagIds.Count; i++)
            {
                connection.Execute(
                    @"
            INSERT INTO dbo.uv_EventTag
            (
                EventId,
                TagId,
                SortOrder
            )
            VALUES
            (
                @EventId,
                @TagId,
                @SortOrder
            )",
                    new
                    {
                        EventId = eventId,
                        TagId = distinctTagIds[i],
                        SortOrder = i
                    },
                    transaction);
            }
        }

        private static void ReplaceEventFeatures(IDbConnection connection, IDbTransaction transaction, int eventId, IList<EventFeatureInputDto> features)
        {
            connection.Execute(
                "DELETE FROM dbo.uv_EventFeature WHERE EventId = @EventId",
                new { EventId = eventId },
                transaction);

            var validFeatures = (features ?? new List<EventFeatureInputDto>())
                .Where(x => x != null)
                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                .ToList();

            for (var i = 0; i < validFeatures.Count; i++)
            {
                connection.Execute(
                    @"
            INSERT INTO dbo.uv_EventFeature
            (
                EventId,
                Title,
                Description,
                IconClass,
                SortOrder
            )
            VALUES
            (
                @EventId,
                @Title,
                @Description,
                @IconClass,
                @SortOrder
            )",
                    new
                    {
                        EventId = eventId,
                        Title = validFeatures[i].Title.Trim(),
                        Description = NullIfWhiteSpace(validFeatures[i].Description),
                        IconClass = NullIfWhiteSpace(validFeatures[i].IconClass),
                        SortOrder = i
                    },
                    transaction);
            }
        }

        private static void ReplaceEventAudiences(IDbConnection connection, IDbTransaction transaction, int eventId, IList<EventAudienceInputDto> audiences)
        {
            connection.Execute(
                "DELETE FROM dbo.uv_EventAudience WHERE EventId = @EventId",
                new { EventId = eventId },
                transaction);

            var validAudiences = (audiences ?? new List<EventAudienceInputDto>())
                .Where(x => x != null)
                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                .ToList();

            for (var i = 0; i < validAudiences.Count; i++)
            {
                connection.Execute(
                    @"
            INSERT INTO dbo.uv_EventAudience
            (
                EventId,
                AudienceTypeId,
                AudienceTitle,
                AudienceDescription,
                SortOrder
            )
            VALUES
            (
                @EventId,
                NULL,
                @AudienceTitle,
                @AudienceDescription,
                @SortOrder
            )",
                    new
                    {
                        EventId = eventId,
                        AudienceTitle = validAudiences[i].Title.Trim(),
                        AudienceDescription = NullIfWhiteSpace(validAudiences[i].Description),
                        SortOrder = i
                    },
                    transaction);
            }
        }

        private string GetUniqueCategorySlug(IDbConnection connection, int portalId, string name)
        {
            return GetUniqueLookupSlug(
                connection,
                portalId,
                name,
                "dbo.uv_EventCategory",
                "EventCategoryId");
        }

        private string GetUniqueTagSlug(IDbConnection connection, int portalId, string name)
        {
            return GetUniqueLookupSlug(
                connection,
                portalId,
                name,
                "dbo.uv_Tag",
                "TagId");
        }

        private static string GetUniqueLookupSlug(IDbConnection connection, int portalId, string name, string tableName, string keyName)
        {
            var baseSlug = NormalizeEventSlug(name);
            if (string.IsNullOrWhiteSpace(baseSlug))
            {
                baseSlug = "item";
            }

            var candidate = baseSlug;
            var suffix = 2;

            while (true)
            {
                var sql = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT COUNT(1) FROM {0} WHERE PortalId = @PortalId AND Slug = @Slug",
                    tableName);

                var count = connection.ExecuteScalar<int>(
                    sql,
                    new
                    {
                        PortalId = portalId,
                        Slug = candidate
                    });

                if (count <= 0)
                {
                    return candidate;
                }

                candidate = string.Concat(baseSlug, "-", suffix.ToString(CultureInfo.InvariantCulture));
                suffix++;
            }
        }

        private bool IsSlugAvailableInternal(IDbConnection connection, IDbTransaction transaction, int portalId, string slug, int? eventId)
        {
            return connection.QuerySingle<bool>(
                "dbo.uv_Event_IsSlugAvailable",
                new
                {
                    PortalId = portalId,
                    Slug = slug,
                    EventId = eventId
                },
                transaction,
                commandType: CommandType.StoredProcedure);
        }

        private static DateTime ConvertLocalInputToUtc(string value, TimeZoneInfo timeZone)
        {
            DateTime localValue;
            if (!TryParseLocalInput(value, out localValue))
            {
                throw new ArgumentException("A valid local date/time value is required.");
            }

            var unspecified = DateTime.SpecifyKind(localValue, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZone);
        }

        private static DateTime? ConvertLocalInputToUtcNullable(string value, TimeZoneInfo timeZone)
        {
            DateTime localValue;
            if (!TryParseLocalInput(value, out localValue))
            {
                return null;
            }

            var unspecified = DateTime.SpecifyKind(localValue, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZone);
        }

        private static bool TryParseLocalInput(string value, out DateTime result)
        {
            return DateTime.TryParseExact(
                value,
                "yyyy-MM-ddTHH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result);
        }

        private static bool IsValidEmail(string value)
        {
            try
            {
                var _ = new MailAddress(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidOptionalUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            Uri uri;
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
            {
                return false;
            }

            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }

        private static string NullIfWhiteSpace(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private int? GetExistingEventPortalId(IDbConnection connection, IDbTransaction transaction, int? eventId)
        {
            if (!eventId.HasValue || eventId.Value <= 0)
            {
                return null;
            }

            return connection.QueryFirstOrDefault<int?>(
                @"SELECT TOP 1 PortalId
          FROM dbo.uv_Event
          WHERE EventId = @EventId",
                new { EventId = eventId.Value },
                transaction);
        }

        private static LocationSuggestionDto MapLocationSuggestion(LocationIqAutocompleteResponseDto item)
        {
            if (item == null)
            {
                return null;
            }

            var address = item.Address ?? new LocationIqAddressDto();

            decimal latitude;
            decimal longitude;

            decimal? parsedLatitude = decimal.TryParse(item.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out latitude)
                ? latitude
                : (decimal?)null;

            decimal? parsedLongitude = decimal.TryParse(item.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out longitude)
                ? longitude
                : (decimal?)null;

            var venueName = FirstNonBlank(
                address.Name,
                item.DisplayPlace);

            var city = FirstNonBlank(
                address.City,
                address.Town,
                address.Village,
                address.Hamlet,
                address.Municipality,
                address.County);

            return new LocationSuggestionDto
            {
                DisplayName = NullIfWhiteSpace(item.DisplayName),
                VenueName = NullIfWhiteSpace(venueName),
                PublicLocationText = NullIfWhiteSpace(item.DisplayName),
                AddressLine1 = BuildAddressLine1(address.HouseNumber, address.Road, item.DisplayAddress),
                AddressLine2 = string.Empty,
                City = NullIfWhiteSpace(city),
                Region = NullIfWhiteSpace(address.State),
                PostalCode = NullIfWhiteSpace(address.Postcode),
                CountryCode = NormalizeCountryCode(address.CountryCode),
                Latitude = parsedLatitude,
                Longitude = parsedLongitude,
                WebsiteUrl = null,
                PhoneNumber = null
            };
        }

        private static string BuildAddressLine1(string houseNumber, string road, string displayAddress)
        {
            var line1 = string.Join(" ", new[] { houseNumber, road }
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim()));

            if (!string.IsNullOrWhiteSpace(line1))
            {
                return line1;
            }

            if (!string.IsNullOrWhiteSpace(displayAddress))
            {
                var firstPart = displayAddress
                    .Split(',')
                    .Select(x => x.Trim())
                    .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                return NullIfWhiteSpace(firstPart);
            }

            return null;
        }

        private static string NormalizeCountryCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return null;
            }

            return countryCode.Trim().ToUpperInvariant();
        }

        private static string FirstNonBlank(params string[] values)
        {
            return values == null
                ? null
                : values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        internal static void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);

                var ctlDnn = new DnnDataHelper();
                ctlDnn.LogErrorToAdminLog(ex);

                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }

        private class EventEditRow
        {
            public int EventId { get; set; }

            public int PortalId { get; set; }

            public int EventStatusId { get; set; }

            public int? OrganizerUserId { get; set; }

            public int? VenueId { get; set; }

            public int? EventCategoryId { get; set; }

            public string Title { get; set; }

            public string Slug { get; set; }

            public string ShortSummary { get; set; }

            public string FullDescription { get; set; }

            public DateTime? StartDateTimeUtc { get; set; }

            public DateTime? EndDateTimeUtc { get; set; }

            public string TimeZoneId { get; set; }

            public bool IsAllDay { get; set; }

            public string DisplayDateText { get; set; }

            public string DisplayTimeText { get; set; }

            public string HeroImageUrl { get; set; }

            public string ListImageUrl { get; set; }

            public string ThumbnailImageUrl { get; set; }

            public string ImageAltText { get; set; }

            public string RegistrationUrl { get; set; }

            public string RegistrationButtonText { get; set; }

            public string SecondaryCtaUrl { get; set; }

            public string SecondaryCtaText { get; set; }

            public decimal? PriceAmount { get; set; }

            public string PriceCurrencyCode { get; set; }

            public string PriceLabel { get; set; }

            public bool IsFree { get; set; }

            public int? Capacity { get; set; }

            public string CapacitySummaryText { get; set; }

            public string ContactEmail { get; set; }

            public string ContactPhone { get; set; }

            public string ContactUrl { get; set; }

            public string SeoTitle { get; set; }

            public string SeoDescription { get; set; }

            public string CanonicalUrl { get; set; }

            public string MetaRobots { get; set; }

            public int SortOrder { get; set; }

            public bool IsFeatured { get; set; }

            public bool AllowPublicDetailPage { get; set; }

            public DateTime? PublishStartUtc { get; set; }

            public DateTime? PublishEndUtc { get; set; }

            public int? DownloadFileId { get; set; }

            public string DownloadFileUrl { get; set; }

            public bool UseCustomLocationText { get; set; }

            public string VenueName { get; set; }

            public string PublicLocationText { get; set; }

            public string AddressLine1 { get; set; }

            public string AddressLine2 { get; set; }

            public string City { get; set; }

            public string Region { get; set; }

            public string PostalCode { get; set; }

            public string CountryCode { get; set; }

            public decimal? Latitude { get; set; }

            public decimal? Longitude { get; set; }

            public string VenueWebsiteUrl { get; set; }

            public string VenuePhoneNumber { get; set; }
        }

        #endregion
    }
}