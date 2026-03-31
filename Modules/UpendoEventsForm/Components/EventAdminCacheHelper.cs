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