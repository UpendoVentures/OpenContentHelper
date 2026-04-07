using System.Web;
using DotNetNuke.Entities.Portals;

namespace Upendo.OpenContentHelper.Features.Seo.Models
{
    public class SeoContext
    {
        public PortalSettings PortalSettings { get; set; }
        public HttpRequest Request { get; set; }
        public string Authority { get; set; }
        public string CurrentPageTitle { get; set; }
        public int ActiveTabId { get; set; }
        public int PortalId { get; set; }
    }
}