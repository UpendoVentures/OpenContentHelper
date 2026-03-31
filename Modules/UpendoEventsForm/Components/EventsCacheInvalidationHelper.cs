using DotNetNuke.Common.Utilities;
using System;

namespace Upendo.Modules.UpendoEventsForm.Components
{
    internal static class EventsCacheInvalidationHelper
    {
        private const string Prefix = "UOCH:Events";

        internal static void BumpPortalVersion(int portalId)
        {
            if (portalId <= 0)
            {
                return;
            }

            var cacheKey = string.Format("{0}:PortalVersion:{1}", Prefix, portalId);
            var version = DateTime.UtcNow.Ticks.ToString();

            DataCache.SetCache(cacheKey, version, TimeSpan.FromDays(30));
        }
    }
}