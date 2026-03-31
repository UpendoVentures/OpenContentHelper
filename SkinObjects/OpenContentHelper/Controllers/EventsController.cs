using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using Upendo.OpenContentHelper.Features.Events;
using Upendo.OpenContentHelper.Features.Events.Helpers;
using Upendo.OpenContentHelper.Features.Events.Models;

namespace Upendo.SkinObjects.OpenContentHelper.Controllers
{
    [AllowAnonymous]
    public class EventsController : DnnApiController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(EventsController));
        private static readonly XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        [HttpGet]
        public HttpResponseMessage Calendar(string slug)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug("Calender method called.");
            }

            if (string.IsNullOrWhiteSpace(slug))
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, EventConstants.SlugIsRequiredMessage);
            }

            try
            {
                var portalId = PortalSettings.PortalId;
                var vm = EventFacade.GetDetailViewModel(portalId, slug, EventConstants.EventListPageRoute, EventConstants.EventDetailPageRoute, 0);

                if (vm == null || vm.Event == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, EventConstants.EventNotFoundMessage);
                }

                var ics = EventCalendarHelper.BuildIcs(vm.Event);
                var bytes = Encoding.UTF8.GetBytes(ics);
                var fileName = (vm.Event.Slug ?? "event") + EventConstants.CalendarFileExtension;

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(EventConstants.ContentTypeCalendar);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue(EventConstants.AttachmentDispositionType)
                {
                    FileName = fileName
                };

                return response;
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Generates a sitemap for all upcoming events.
        /// </summary>
        /// <param name="detailPagePath">The path to the detail page for events.</param>
        /// <returns>An HttpResponseMessage containing the sitemap XML.</returns>
        /// <remarks>
        /// The sitemap is generated in XML format and includes all upcoming events with their respective detail page URLs.
        /// Calling URL Pattern:
        /// - `/DesktopModules/OpenContentHelper/API/Events/Sitemap` 
        /// - `/DesktopModules/OpenContentHelper/API/Events/Sitemap?detailPagePath=/events`
        /// </remarks>
        [HttpGet]
        public HttpResponseMessage Sitemap(string detailPagePath = null)
        {
            try
            {
                var portalId = PortalSettings.PortalId;
                var normalizedDetailPagePath = EventPageUrlHelper.NormalizePagePath(detailPagePath, EventConstants.EventDetailPageRoute);
                var events = GetAllUpcomingEvents(portalId);

                var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);

                var urlElements = events
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Slug))
                    .Select(x => new XElement(ns + "url",
                        new XElement(ns + "loc", BuildAbsoluteEventUrl(baseUrl, normalizedDetailPagePath, x.Slug)),
                        new XElement(ns + "lastmod", GetSitemapLastModifiedDate(x).ToString("yyyy-MM-dd")),
                        new XElement(ns + "priority", "0.7")
                    ));

                var document = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement(ns + "urlset", urlElements)
                );

                var xml = document.ToString(SaveOptions.DisableFormatting);
                var bytes = Encoding.UTF8.GetBytes(xml);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(bytes);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(EventConstants.ContentTypeXml);

                return response;
            }
            catch (Exception ex)
            {
                LogError(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unable to generate the events sitemap.");
            }
        }

        private static string BuildAbsoluteEventUrl(string baseUrl, string detailPagePath, string slug)
        {
            var relativeUrl = EventPageUrlHelper.BuildDetailPageUrl(detailPagePath, slug);
            var absoluteUri = new Uri(new Uri(baseUrl.TrimEnd('/') + "/"), relativeUrl.TrimStart('/'));
            return absoluteUri.AbsoluteUri;
        }

        private static IList<EventListItemDto> GetAllUpcomingEvents(int portalId)
        {
            const int pageSize = 100;

            var service = EventServiceFactory.Create();
            var results = new List<EventListItemDto>();
            var pageNumber = 1;
            var totalRows = int.MaxValue;

            while (results.Count < totalRows)
            {
                var request = new EventListRequest
                {
                    PortalId = portalId,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    IsUpcoming = true,
                    OnlyFeatured = false,
                    SortBy = "date_asc"
                };

                var page = service.ListEvents(request);

                if (page == null || page.Items == null || page.Items.Count == 0)
                {
                    break;
                }

                results.AddRange(page.Items);

                totalRows = page.TotalRows > 0 ? page.TotalRows : results.Count;
                pageNumber++;
            }

            return results
                .GroupBy(x => x.EventId)
                .Select(x => x.First())
                .ToList();
        }

        private static DateTime GetSitemapLastModifiedDate(EventListItemDto item)
        {
            if (item == null)
            {
                return DateTime.UtcNow.Date;
            }

            var candidate = item.StartDateTimeUtc;

            if (candidate.Kind == DateTimeKind.Unspecified)
            {
                candidate = DateTime.SpecifyKind(candidate, DateTimeKind.Utc);
            }
            else
            {
                candidate = candidate.ToUniversalTime();
            }

            return candidate.Date;
        }

        private void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);

                if (ex.InnerException != null)
                {
                    LogError(ex.InnerException);
                }
            }
        }
    }
}