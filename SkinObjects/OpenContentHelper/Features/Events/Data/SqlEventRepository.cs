using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Upendo.OpenContentHelper.Features.Events.Data;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.OpenContentHelper.Features.Events.Data
{
    public class SqlEventRepository : IEventRepository
    {
        private readonly string _connectionString;

        public SqlEventRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("connectionString is required.");
            _connectionString = connectionString;
        }

        public PagedResult<EventListItemDto> ListPaged(EventListRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = new PagedResult<EventListItemDto>
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Event_ListPaged", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", request.PortalId);
                AddInt(cmd, "@PageNumber", request.PageNumber);
                AddInt(cmd, "@PageSize", request.PageSize);
                AddNVarChar(cmd, "@CategorySlug", 120, request.CategorySlug);
                AddNVarChar(cmd, "@OrganizerSlug", 220, request.OrganizerSlug);
                AddNVarChar(cmd, "@TagSlug", 120, request.TagSlug);
                AddNVarChar(cmd, "@Keyword", 200, request.Keyword);
                AddBit(cmd, "@IsUpcoming", request.IsUpcoming);
                AddBit(cmd, "@OnlyFeatured", request.OnlyFeatured);
                AddNVarChar(cmd, "@SortBy", 20, request.SortBy);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = MapEventListItem(reader);
                        result.Items.Add(item);

                        if (result.TotalRows == 0)
                        {
                            result.TotalRows = GetValue<int>(reader, "TotalRows");
                            result.PageNumber = GetValue<int>(reader, "PageNumber");
                            result.PageSize = GetValue<int>(reader, "PageSize");
                        }
                    }
                }
            }

            return result;
        }

        public EventDetailDto GetBySlug(int portalId, string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Event_GetBySlug", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", portalId);
                AddNVarChar(cmd, "@Slug", 220, slug);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read()) return null;
                    return MapEventDetailBase(reader);
                }
            }
        }

        public EventDetailDto GetDetail(int portalId, string slug, int relatedMaxResults)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            EventDetailDto detail = null;

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Event_GetDetail", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", portalId);
                AddNVarChar(cmd, "@Slug", 220, slug);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        detail = MapEventDetailBase(reader);
                    }

                    if (detail == null) return null;

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            detail.Tags.Add(new EventTagDto
                            {
                                TagId = GetValue<int>(reader, "TagId"),
                                TagName = GetValue<string>(reader, "TagName"),
                                Slug = GetValue<string>(reader, "Slug"),
                                SortOrder = GetValue<int>(reader, "SortOrder")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            detail.Audiences.Add(new EventAudienceDto
                            {
                                EventAudienceId = GetValue<int>(reader, "EventAudienceId"),
                                AudienceTypeId = GetNullableValue<int>(reader, "AudienceTypeId"),
                                AudienceName = GetValue<string>(reader, "AudienceName"),
                                AudienceSlug = GetValue<string>(reader, "AudienceSlug"),
                                AudienceTitle = GetValue<string>(reader, "AudienceTitle"),
                                AudienceDescription = GetValue<string>(reader, "AudienceDescription"),
                                SortOrder = GetValue<int>(reader, "SortOrder")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            detail.Features.Add(new EventFeatureDto
                            {
                                EventFeatureId = GetValue<int>(reader, "EventFeatureId"),
                                Title = GetValue<string>(reader, "Title"),
                                Description = GetValue<string>(reader, "Description"),
                                IconClass = GetValue<string>(reader, "IconClass"),
                                SortOrder = GetValue<int>(reader, "SortOrder")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            detail.ContentSections.Add(new EventContentSectionDto
                            {
                                EventContentSectionId = GetValue<int>(reader, "EventContentSectionId"),
                                SectionType = GetValue<string>(reader, "SectionType"),
                                SectionTitle = GetValue<string>(reader, "SectionTitle"),
                                BodyHtml = GetValue<string>(reader, "BodyHtml"),
                                BodyText = GetValue<string>(reader, "BodyText"),
                                IconClass = GetValue<string>(reader, "IconClass"),
                                SortOrder = GetValue<int>(reader, "SortOrder")
                            });
                        }
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            detail.CalendarMetadata = new EventCalendarMetadataDto
                            {
                                EventId = GetValue<int>(reader, "EventId"),
                                IcsUid = GetValue<string>(reader, "IcsUid"),
                                RecurrenceRule = GetValue<string>(reader, "RecurrenceRule"),
                                LocationText = GetValue<string>(reader, "LocationText"),
                                CalendarDescriptionText = GetValue<string>(reader, "CalendarDescriptionText")
                            };
                        }
                    }
                }
            }

            if (detail != null && relatedMaxResults > 0)
            {
                detail.RelatedEvents = ListRelated(portalId, detail.EventId, relatedMaxResults);
            }

            return detail;
        }

        public IList<EventCategoryDto> ListActiveCategories()
        {
            var results = new List<EventCategoryDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_EventCategory_ListActive", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new EventCategoryDto
                        {
                            EventCategoryId = GetValue<int>(reader, "EventCategoryId"),
                            CategoryName = GetValue<string>(reader, "CategoryName"),
                            CategorySlug = GetValue<string>(reader, "Slug"),
                            Description = GetValue<string>(reader, "Description"),
                            SortOrder = GetValue<int>(reader, "SortOrder")
                        });
                    }
                }
            }

            return results;
        }

        public IList<EventCategoryDto> ListCategoriesForPortal(int portalId, bool isUpcoming)
        {
            var results = new List<EventCategoryDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_EventCategory_ListForPortal", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", portalId);
                AddBit(cmd, "@IsUpcoming", isUpcoming);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new EventCategoryDto
                        {
                            EventCategoryId = GetValue<int>(reader, "EventCategoryId"),
                            CategoryName = GetValue<string>(reader, "CategoryName"),
                            CategorySlug = GetValue<string>(reader, "Slug"),
                            Description = GetValue<string>(reader, "Description"),
                            SortOrder = GetValue<int>(reader, "SortOrder"),
                            EventCount = Convert.ToInt32(reader["EventCount"])
                        });
                    }
                }
            }

            return results;
        }

        public IList<EventTagDto> ListActiveTags()
        {
            var results = new List<EventTagDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Tag_ListActive", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new EventTagDto
                        {
                            TagId = GetValue<int>(reader, "TagId"),
                            TagName = GetValue<string>(reader, "TagName"),
                            Slug = GetValue<string>(reader, "Slug")
                        });
                    }
                }
            }

            return results;
        }

        public IList<EventTagDto> ListTrendingTags(int portalId, bool isUpcoming, int maxResults)
        {
            var results = new List<EventTagDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Tag_ListTrendingForPortal", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", portalId);
                AddBit(cmd, "@IsUpcoming", isUpcoming);
                AddInt(cmd, "@MaxResults", maxResults);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new EventTagDto
                        {
                            TagId = GetValue<int>(reader, "TagId"),
                            TagName = GetValue<string>(reader, "TagName"),
                            Slug = GetValue<string>(reader, "Slug"),
                            EventCount = Convert.ToInt32(reader["EventCount"]),
                            SortOrder = GetValue<int>(reader, "SortOrder")
                        });
                    }
                }
            }

            return results;
        }

        public IList<EventOrganizerDto> ListTopOrganizers(int portalId, bool isUpcoming, int maxResults)
        {
            var results = new List<EventOrganizerDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Organizer_ListTopForPortal", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", portalId);
                AddBit(cmd, "@IsUpcoming", isUpcoming);
                AddInt(cmd, "@MaxResults", maxResults);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new EventOrganizerDto
                        {
                            OrganizerId = GetValue<int>(reader, "OrganizerId"),
                            OrganizerName = GetValue<string>(reader, "OrganizerName"),
                            ContactName = GetValue<string>(reader, "ContactName"),
                            EmailAddress = GetValue<string>(reader, "EmailAddress"),
                            PhoneNumber = GetValue<string>(reader, "PhoneNumber"),
                            WebsiteUrl = GetValue<string>(reader, "WebsiteUrl"),
                            Slug = GetValue<string>(reader, "Slug"),
                            HostedEventCount = Convert.ToInt32(reader["HostedEventCount"])
                        });
                    }
                }
            }

            return results;
        }

        private IList<EventListItemDto> ListRelated(int portalId, int eventId, int maxResults)
        {
            var results = new List<EventListItemDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("dbo.uv_Event_ListRelated", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                AddInt(cmd, "@PortalId", portalId);
                AddInt(cmd, "@EventId", eventId);
                AddInt(cmd, "@MaxResults", maxResults);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new EventListItemDto
                        {
                            EventId = GetValue<int>(reader, "EventId"),
                            Title = GetValue<string>(reader, "Title"),
                            Slug = GetValue<string>(reader, "Slug"),
                            ShortSummary = GetValue<string>(reader, "ShortSummary"),
                            StartDateTimeUtc = GetValue<DateTime>(reader, "StartDateTimeUtc"),
                            EndDateTimeUtc = GetNullableValue<DateTime>(reader, "EndDateTimeUtc"),
                            DisplayDateText = GetValue<string>(reader, "DisplayDateText"),
                            DisplayTimeText = GetValue<string>(reader, "DisplayTimeText"),
                            ListImageUrl = GetValue<string>(reader, "ListImageUrl"),
                            ThumbnailImageUrl = GetValue<string>(reader, "ThumbnailImageUrl"),
                            ImageAltText = GetValue<string>(reader, "ImageAltText"),
                            RegistrationUrl = GetValue<string>(reader, "RegistrationUrl"),
                            RegistrationButtonText = GetValue<string>(reader, "RegistrationButtonText"),
                            IsFeatured = GetValue<bool>(reader, "IsFeatured"),
                            SortOrder = GetValue<int>(reader, "SortOrder"),
                            CategoryName = GetValue<string>(reader, "CategoryName"),
                            CategorySlug = GetValue<string>(reader, "CategorySlug"),
                            VenueName = GetValue<string>(reader, "VenueName"),
                            PublicLocationText = GetValue<string>(reader, "PublicLocationText"),
                            City = GetValue<string>(reader, "City"),
                            Region = GetValue<string>(reader, "Region")
                        });
                    }
                }
            }

            return results;
        }

        private static EventListItemDto MapEventListItem(IDataRecord reader)
        {
            return new EventListItemDto
            {
                EventId = GetValue<int>(reader, "EventId"),
                PortalId = GetValue<int>(reader, "PortalId"),
                EventStatusId = GetValue<int>(reader, "EventStatusId"),
                EventCategoryId = GetNullableValue<int>(reader, "EventCategoryId"),
                CategoryName = GetValue<string>(reader, "CategoryName"),
                CategorySlug = GetValue<string>(reader, "CategorySlug"),
                VenueId = GetNullableValue<int>(reader, "VenueId"),
                VenueName = GetValue<string>(reader, "VenueName"),
                PublicLocationText = GetValue<string>(reader, "PublicLocationText"),
                City = GetValue<string>(reader, "City"),
                Region = GetValue<string>(reader, "Region"),
                Title = GetValue<string>(reader, "Title"),
                Slug = GetValue<string>(reader, "Slug"),
                ShortSummary = GetValue<string>(reader, "ShortSummary"),
                StartDateTimeUtc = GetValue<DateTime>(reader, "StartDateTimeUtc"),
                EndDateTimeUtc = GetNullableValue<DateTime>(reader, "EndDateTimeUtc"),
                TimeZoneId = GetValue<string>(reader, "TimeZoneId"),
                IsAllDay = GetValue<bool>(reader, "IsAllDay"),
                DisplayDateText = GetValue<string>(reader, "DisplayDateText"),
                DisplayTimeText = GetValue<string>(reader, "DisplayTimeText"),
                ListImageUrl = GetValue<string>(reader, "ListImageUrl"),
                ThumbnailImageUrl = GetValue<string>(reader, "ThumbnailImageUrl"),
                ImageAltText = GetValue<string>(reader, "ImageAltText"),
                RegistrationUrl = GetValue<string>(reader, "RegistrationUrl"),
                RegistrationButtonText = GetValue<string>(reader, "RegistrationButtonText"),
                PriceAmount = GetNullableValue<decimal>(reader, "PriceAmount"),
                PriceCurrencyCode = GetValue<string>(reader, "PriceCurrencyCode"),
                PriceLabel = GetValue<string>(reader, "PriceLabel"),
                IsFree = GetValue<bool>(reader, "IsFree"),
                SortOrder = GetValue<int>(reader, "SortOrder"),
                IsFeatured = GetValue<bool>(reader, "IsFeatured"),
                TotalRows = GetValue<int>(reader, "TotalRows"),
                PageNumber = GetValue<int>(reader, "PageNumber"),
                PageSize = GetValue<int>(reader, "PageSize")
            };
        }

        private static EventDetailDto MapEventDetailBase(IDataRecord reader)
        {
            var detail = new EventDetailDto
            {
                EventId = GetValue<int>(reader, "EventId"),
                PortalId = GetValue<int>(reader, "PortalId"),
                EventStatusId = GetValue<int>(reader, "EventStatusId"),
                StatusName = GetValue<string>(reader, "StatusName"),
                EventCategoryId = GetNullableValue<int>(reader, "EventCategoryId"),
                CategoryName = GetValue<string>(reader, "CategoryName"),
                CategorySlug = GetValue<string>(reader, "CategorySlug"),
                Title = GetValue<string>(reader, "Title"),
                Slug = GetValue<string>(reader, "Slug"),
                ShortSummary = GetValue<string>(reader, "ShortSummary"),
                FullDescription = GetValue<string>(reader, "FullDescription"),
                StartDateTimeUtc = GetValue<DateTime>(reader, "StartDateTimeUtc"),
                EndDateTimeUtc = GetNullableValue<DateTime>(reader, "EndDateTimeUtc"),
                TimeZoneId = GetValue<string>(reader, "TimeZoneId"),
                IsAllDay = GetValue<bool>(reader, "IsAllDay"),
                DisplayDateText = GetValue<string>(reader, "DisplayDateText"),
                DisplayTimeText = GetValue<string>(reader, "DisplayTimeText"),
                HeroImageUrl = GetValue<string>(reader, "HeroImageUrl"),
                ListImageUrl = GetValue<string>(reader, "ListImageUrl"),
                ThumbnailImageUrl = GetValue<string>(reader, "ThumbnailImageUrl"),
                ImageAltText = GetValue<string>(reader, "ImageAltText"),
                RegistrationUrl = GetValue<string>(reader, "RegistrationUrl"),
                RegistrationButtonText = GetValue<string>(reader, "RegistrationButtonText"),
                SecondaryCtaUrl = GetValue<string>(reader, "SecondaryCtaUrl"),
                SecondaryCtaText = GetValue<string>(reader, "SecondaryCtaText"),
                PriceAmount = GetNullableValue<decimal>(reader, "PriceAmount"),
                PriceCurrencyCode = GetValue<string>(reader, "PriceCurrencyCode"),
                PriceLabel = GetValue<string>(reader, "PriceLabel"),
                IsFree = GetValue<bool>(reader, "IsFree"),
                Capacity = GetNullableValue<int>(reader, "Capacity"),
                CapacitySummaryText = GetValue<string>(reader, "CapacitySummaryText"),
                ContactEmail = GetValue<string>(reader, "ContactEmail"),
                ContactPhone = GetValue<string>(reader, "ContactPhone"),
                ContactUrl = GetValue<string>(reader, "ContactUrl"),
                SeoTitle = GetValue<string>(reader, "SeoTitle"),
                SeoDescription = GetValue<string>(reader, "SeoDescription"),
                CanonicalUrl = GetValue<string>(reader, "CanonicalUrl"),
                MetaRobots = GetValue<string>(reader, "MetaRobots"),
                SortOrder = GetValue<int>(reader, "SortOrder"),
                IsFeatured = GetValue<bool>(reader, "IsFeatured"),
                AllowPublicDetailPage = GetValue<bool>(reader, "AllowPublicDetailPage"),
                PublishStartUtc = GetNullableValue<DateTime>(reader, "PublishStartUtc"),
                PublishEndUtc = GetNullableValue<DateTime>(reader, "PublishEndUtc"),
                CreatedOnDate = GetValue<DateTime>(reader, "CreatedOnDate"),
                LastModifiedOnDate = GetValue<DateTime>(reader, "LastModifiedOnDate"),
                Venue = new EventVenueDto
                {
                    VenueId = GetNullableValue<int>(reader, "VenueId") ?? 0,
                    VenueName = GetValue<string>(reader, "VenueName"),
                    AddressLine1 = GetValue<string>(reader, "AddressLine1"),
                    AddressLine2 = GetValue<string>(reader, "AddressLine2"),
                    City = GetValue<string>(reader, "City"),
                    Region = GetValue<string>(reader, "Region"),
                    PostalCode = GetValue<string>(reader, "PostalCode"),
                    CountryCode = GetValue<string>(reader, "CountryCode"),
                    Latitude = GetNullableValue<decimal>(reader, "Latitude"),
                    Longitude = GetNullableValue<decimal>(reader, "Longitude"),
                    TimeZoneId = GetValue<string>(reader, "VenueTimeZoneId"),
                    PublicLocationText = GetValue<string>(reader, "PublicLocationText"),
                    WebsiteUrl = GetValue<string>(reader, "VenueWebsiteUrl"),
                    PhoneNumber = GetValue<string>(reader, "VenuePhoneNumber")
                },
                Organizer = new EventOrganizerDto
                {
                    OrganizerId = GetNullableValue<int>(reader, "OrganizerId") ?? 0,
                    OrganizerName = GetValue<string>(reader, "OrganizerName"),
                    ContactName = GetValue<string>(reader, "OrganizerContactName"),
                    EmailAddress = GetValue<string>(reader, "OrganizerEmailAddress"),
                    PhoneNumber = GetValue<string>(reader, "OrganizerPhoneNumber"),
                    WebsiteUrl = GetValue<string>(reader, "OrganizerWebsiteUrl")
                }
            };

            return detail;
        }

        private static T GetValue<T>(IDataRecord reader, string fieldName)
        {
            var ordinal = reader.GetOrdinal(fieldName);
            if (reader.IsDBNull(ordinal))
            {
                if (typeof(T) == typeof(string)) return (T)(object)string.Empty;
                return default(T);
            }

            return (T)Convert.ChangeType(reader.GetValue(ordinal), typeof(T));
        }

        private static T? GetNullableValue<T>(IDataRecord reader, string fieldName) where T : struct
        {
            var ordinal = reader.GetOrdinal(fieldName);
            if (reader.IsDBNull(ordinal)) return null;
            return (T)Convert.ChangeType(reader.GetValue(ordinal), typeof(T));
        }

        private static SqlParameter AddInt(SqlCommand cmd, string name, int value)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.Int);
            p.Value = value;
            return p;
        }

        private static SqlParameter AddBit(SqlCommand cmd, string name, bool value)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.Bit);
            p.Value = value;
            return p;
        }

        private static SqlParameter AddNVarChar(SqlCommand cmd, string name, int size, string value)
        {
            var p = cmd.Parameters.Add(name, SqlDbType.NVarChar, size);
            p.Value = string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;
            return p;
        }
    }
}