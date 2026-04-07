using System;
using System.Collections.Generic;
using System.Linq;
using Upendo.OpenContentHelper.Features.Directory;
using Upendo.OpenContentHelper.Features.Directory.Common;
using Upendo.OpenContentHelper.Features.Directory.Helpers;
using Upendo.OpenContentHelper.Features.Directory.Models;
using Upendo.OpenContentHelper.Features.Events;
using Upendo.OpenContentHelper.Features.Events.Helpers;
using Upendo.OpenContentHelper.Features.Events.Models;
using Upendo.OpenContentHelper.Features.Seo.Data;
using Upendo.OpenContentHelper.Features.Seo.Helpers;
using Upendo.OpenContentHelper.Features.Seo.Models;

namespace Upendo.OpenContentHelper.Features.Seo.Services
{
    public class SeoService : ISeoService
    {
        private readonly ISeoRepository _seoRepository;

        public SeoService(ISeoRepository seoRepository)
        {
            _seoRepository = seoRepository ?? throw new ArgumentNullException(nameof(seoRepository));
        }

        public OpenContentHeadData GetOpenContentHeadData(int activeTabId)
        {
            var moduleId = _seoRepository.GetMetaModuleIdOnTab(activeTabId);
            if (moduleId <= 0)
            {
                return new OpenContentHeadData();
            }

            return _seoRepository.GetLatestOpenContentHeadData(moduleId);
        }

        public SeoOverrideResult BuildOverrides(SeoContext context)
        {
            if (context?.Request == null || context.PortalSettings == null)
            {
                return null;
            }

            if (SeoRequestHelper.IsEventListRequest(context.Request))
            {
                return BuildEventListOverrides(context);
            }

            if (SeoRequestHelper.TryGetEventDetailSlug(context.Request, out var eventSlug))
            {
                return BuildEventDetailOverrides(context, eventSlug);
            }

            if (DirectoryRequestHelper.TryGetCompanyDetailSlug(context.Request, out var companySlug))
            {
                return BuildCompanyDetailOverrides(context, companySlug);
            }

            return null;
        }

        private SeoOverrideResult BuildEventListOverrides(SeoContext context)
        {
            var listPagePath = EventPageUrlHelper.BuildListPageUrl(EventConstants.EventListPageRoute);
            var canonicalUrl = SeoContentHelper.BuildAbsoluteUrl(context.Authority, listPagePath);

            var title = context.CurrentPageTitle;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = context.PortalSettings.ActiveTab?.Title ?? "Events";
            }

            var description = context.PortalSettings.ActiveTab?.Description;
            description = SeoContentHelper.NormalizeMetaText(description);

            var result = new SeoOverrideResult
            {
                Title = title
            };

            result.MetaTags.Add(new HeadMetaTag { Name = "description", Content = description });
            result.MetaTags.Add(new HeadMetaTag { Name = "keywords", Content = string.Empty });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:title", Content = title });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:description", Content = SeoContentHelper.TrimToLength(description, 170) });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:url", Content = canonicalUrl });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:title", Content = title });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:description", Content = SeoContentHelper.TrimToLength(description, 170) });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:url", Content = canonicalUrl });

            result.LinkTags.Add(new HeadLinkTag
            {
                Rel = "canonical",
                Href = canonicalUrl
            });

            return result;
        }

        private SeoOverrideResult BuildEventDetailOverrides(SeoContext context, string slug)
        {
            var detailPagePath = EventPageUrlHelper.BuildDetailBasePageUrl(EventConstants.EventDetailPageRoute);
            var evt = EventFacade.GetEventSummary(context.PortalId, slug);

            if (evt == null || !evt.AllowPublicDetailPage)
            {
                return null;
            }

            var title = GetEventMetaTitle(context, evt);
            var description = GetEventMetaDescription(evt);
            var keywords = GetEventMetaKeywords(evt);
            var canonicalUrl = GetEventCanonicalUrl(context, evt, detailPagePath);

            var result = new SeoOverrideResult
            {
                Title = title
            };

            result.MetaTags.Add(new HeadMetaTag { Name = "description", Content = description });
            result.MetaTags.Add(new HeadMetaTag { Name = "keywords", Content = keywords });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:title", Content = title });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:description", Content = SeoContentHelper.TrimToLength(description, 170) });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:url", Content = canonicalUrl });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:title", Content = title });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:description", Content = SeoContentHelper.TrimToLength(description, 170) });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:url", Content = canonicalUrl });

            result.LinkTags.Add(new HeadLinkTag
            {
                Rel = "canonical",
                Href = canonicalUrl
            });

            return result;
        }

        private SeoOverrideResult BuildCompanyDetailOverrides(SeoContext context, string slug)
        {
            var company = ChamberMembershipController.GetCompanyBySlug(context.PortalId, slug);
            if (company == null)
            {
                return null;
            }

            var title = !string.IsNullOrWhiteSpace(company.SeoTitle)
                ? company.SeoTitle.Trim()
                : GetCompanyMetaTitle(context, company);

            var description = !string.IsNullOrWhiteSpace(company.SeoDescription)
                ? SeoContentHelper.NormalizeMetaText(company.SeoDescription)
                : GetCompanyMetaDescription(company);

            var keywords = GetCompanyMetaKeywords(company);

            var canonicalUrl = !string.IsNullOrWhiteSpace(company.CanonicalUrl)
                ? company.CanonicalUrl.Trim()
                : GetCompanyCanonicalUrl(context, company);

            var result = new SeoOverrideResult
            {
                Title = title
            };

            result.MetaTags.Add(new HeadMetaTag { Name = "description", Content = description });
            result.MetaTags.Add(new HeadMetaTag { Name = "keywords", Content = keywords });

            if (!string.IsNullOrWhiteSpace(company.MetaRobots))
            {
                result.MetaTags.Add(new HeadMetaTag
                {
                    Name = "robots",
                    Content = company.MetaRobots.Trim()
                });
            }

            result.MetaTags.Add(new HeadMetaTag { Property = "og:title", Content = title });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:description", Content = SeoContentHelper.TrimToLength(description, 170) });
            result.MetaTags.Add(new HeadMetaTag { Property = "og:url", Content = canonicalUrl });

            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:title", Content = title });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:description", Content = SeoContentHelper.TrimToLength(description, 170) });
            result.MetaTags.Add(new HeadMetaTag { Name = "twitter:url", Content = canonicalUrl });

            if (!string.IsNullOrWhiteSpace(company.OgImageUrl))
            {
                result.MetaTags.Add(new HeadMetaTag
                {
                    Property = "og:image",
                    Content = company.OgImageUrl.Trim()
                });

                result.MetaTags.Add(new HeadMetaTag
                {
                    Name = "twitter:image",
                    Content = company.OgImageUrl.Trim()
                });
            }

            result.LinkTags.Add(new HeadLinkTag
            {
                Rel = "canonical",
                Href = canonicalUrl
            });

            return result;
        }

        private static string GetEventMetaTitle(SeoContext context, EventDetailDto evt)
        {
            if (!string.IsNullOrWhiteSpace(evt.SeoTitle))
            {
                return evt.SeoTitle.Trim();
            }

            if (!string.IsNullOrWhiteSpace(evt.Title))
            {
                return evt.Title.Trim();
            }

            return context.PortalSettings?.ActiveTab?.Title?.Trim() ?? "Event";
        }

        private static string GetEventMetaDescription(EventDetailDto evt)
        {
            var description = evt.SeoDescription;

            if (string.IsNullOrWhiteSpace(description))
            {
                description = evt.ShortSummary;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                description = SeoContentHelper.StripHtml(evt.FullDescription);
            }

            description = SeoContentHelper.NormalizeMetaText(description);

            if (string.IsNullOrWhiteSpace(description))
            {
                description = evt.Title;
            }

            return description;
        }

        private static string GetEventMetaKeywords(EventDetailDto evt)
        {
            var values = new List<string>();

            if (!string.IsNullOrWhiteSpace(evt.CategoryName))
            {
                values.Add(evt.CategoryName);
            }

            if (evt.Tags != null)
            {
                values.AddRange(evt.Tags
                    .Where(x => !string.IsNullOrWhiteSpace(x.TagName))
                    .Select(x => x.TagName));
            }

            if (evt.Organizer != null && !string.IsNullOrWhiteSpace(evt.Organizer.OrganizerName))
            {
                values.Add(evt.Organizer.OrganizerName);
            }

            if (evt.Venue != null)
            {
                if (!string.IsNullOrWhiteSpace(evt.Venue.VenueName))
                {
                    values.Add(evt.Venue.VenueName);
                }

                if (!string.IsNullOrWhiteSpace(evt.Venue.City))
                {
                    values.Add(evt.Venue.City);
                }
            }

            return string.Join(", ",
                values
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static string GetEventCanonicalUrl(SeoContext context, EventDetailDto evt, string detailPagePath)
        {
            if (!string.IsNullOrWhiteSpace(evt.CanonicalUrl))
            {
                var canonical = evt.CanonicalUrl.Trim();

                if (Uri.IsWellFormedUriString(canonical, UriKind.Absolute))
                {
                    return canonical;
                }

                if (canonical.StartsWith("/", StringComparison.Ordinal))
                {
                    return SeoContentHelper.BuildAbsoluteUrl(context.Authority, canonical);
                }
            }

            var relativeUrl = EventPageUrlHelper.BuildDetailPageUrl(detailPagePath, evt.Slug);
            return SeoContentHelper.BuildAbsoluteUrl(context.Authority, relativeUrl);
        }

        private static string GetCompanyMetaTitle(SeoContext context, CompanyDetail company)
        {
            if (company == null)
            {
                return context.PortalSettings?.ActiveTab?.Title ?? "Business Directory";
            }

            var companyName = (company.CompanyName ?? string.Empty).Trim();
            var tabTitle = context.PortalSettings?.ActiveTab?.Title ?? "Business Directory";

            if (string.IsNullOrWhiteSpace(companyName))
            {
                return tabTitle;
            }

            return companyName + " | " + tabTitle;
        }

        private static string GetCompanyMetaDescription(CompanyDetail company)
        {
            if (company == null)
            {
                return string.Empty;
            }

            var description = company.LongDescription;

            if (string.IsNullOrWhiteSpace(description))
            {
                description = company.ShortDescription;
            }

            description = SeoContentHelper.NormalizeMetaText(description);

            if (string.IsNullOrWhiteSpace(description))
            {
                description = company.CompanyName;
            }

            return description;
        }

        private static string GetCompanyMetaKeywords(CompanyDetail company)
        {
            if (company == null)
            {
                return string.Empty;
            }

            var values = new List<string>();

            if (company.Categories != null)
            {
                values.AddRange(company.Categories
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.CategoryName))
                    .Select(x => x.CategoryName));
            }

            if (!string.IsNullOrWhiteSpace(company.City))
            {
                values.Add(company.City);
            }

            if (!string.IsNullOrWhiteSpace(company.Region))
            {
                values.Add(company.Region);
            }

            if (!string.IsNullOrWhiteSpace(company.CompanyName))
            {
                values.Add(company.CompanyName);
            }

            return string.Join(", ",
                values
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static string GetCompanyCanonicalUrl(SeoContext context, CompanyDetail company)
        {
            var relativeUrl = DirectoryPageUrlHelper.BuildCompanyDetailUrl(
                DirectoryConstants.DirectoryListPageRoute,
                company.Slug);

            return SeoContentHelper.BuildAbsoluteUrl(context.Authority, relativeUrl);
        }
    }
}