using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upendo.OpenContentHelper.Features.Events
{
    public static class EventConstants
    {
        public const string EventListPageRoute = "/events";
        public const string EventDetailPageRoute = "/events/detail";

        public const string EventEditPageRoute = "/manage/events";

        public const string EndpointCalendar = "/DesktopModules/OpenContentHelper/API/Events/Calendar";
        public const string EndpointSitemap = "/DesktopModules/OpenContentHelper/API/Events/Sitemap";

        public const string SlugIsRequiredMessage = "slug is required.";
        public const string EventNotFoundMessage = "The specified event could not be found.";
        public const string ContentTypeCalendar = "text/calendar";
        public const string ContentTypeXml = "application/xml";
        public const string AttachmentDispositionType = "attachment";
        public const string CalendarFileExtension = ".ics";
    }
}