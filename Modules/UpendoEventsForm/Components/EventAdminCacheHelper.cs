using DotNetNuke.Common.Utilities;
using System;

namespace Upendo.Modules.UpendoEventsForm.Components
{
    internal static class EventAdminCacheHelper
    {
        private const string Prefix = "UOCH:Events:Admin";

        internal static T GetOrAdd<T>(string cacheKey, TimeSpan duration, Func<T> factory)
            where T : class
        {
            if (string.IsNullOrWhiteSpace(cacheKey)) throw new ArgumentException("cacheKey is required.");
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var cached = DataCache.GetCache(cacheKey) as T;
            if (cached != null)
            {
                return cached;
            }

            var value = factory();

            if (value != null)
            {
                DataCache.SetCache(cacheKey, value, duration);
            }

            return value;
        }

        internal static string Config(int portalId)
        {
            return string.Format("{0}:Config:Portal:{1}", Prefix, portalId);
        }

        internal static string FilesByFolder(int portalId, int folderId)
        {
            return string.Format("{0}:FilesByFolder:Portal:{1}:Folder:{2}", Prefix, portalId, folderId);
        }

        internal static string FileById(int portalId, int fileId)
        {
            return string.Format("{0}:FileById:Portal:{1}:File:{2}", Prefix, portalId, fileId);
        }
    }
}