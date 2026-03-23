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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Log.EventLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Upendo.SkinObjects.OpenContentHelper.Components
{
    public static class MetaValidationHelper
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(MetaValidationHelper));
        private const int MaxContentLength = 500;
        private const int MaxHrefLength = 2048;
        private const int MaxAttrLength = 128;
        private static readonly Regex ValidMetaPropertyPattern = new Regex(@"^[a-z0-9]+:[a-z0-9:_-]+$", RegexOptions.IgnoreCase);

        private static readonly HashSet<string> AllowedMetaNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "author", "generator", "application-name", "robots", "name", "designer", "theme-color", "msapplication-TileColor", "msapplication-TileImage", "N/A"
        };

        private static readonly HashSet<string> AllowedRelValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "canonical", "icon", "stylesheet", "author", "alternate", "help", "license", "next", "prev", "search", "apple-touch-icon"
        };

        private static readonly Regex ValidSizesPattern = new Regex(@"^(\d+x\d+|any)$", RegexOptions.IgnoreCase);

        private static readonly string RejectionLoggedKey = "OpenContent.MetaTagRejectionLogged";

        public static bool IsValidMetaName(string name)
        {
            var valid = !string.IsNullOrWhiteSpace(name) &&
                        name.Length <= MaxAttrLength &&
                        AllowedMetaNames.Contains(name.Trim());

            if (!valid) LogRejection("MetaTag.Name", name);
            return valid;
        }

        public static bool IsValidMetaProperty(string property)
        {
            var trimmedProperty = property?.Trim();
            var valid = !string.IsNullOrWhiteSpace(property) &&
                        property.Length <= MaxAttrLength &&
                        (ValidMetaPropertyPattern.IsMatch(trimmedProperty) || AllowedMetaNames.Contains(trimmedProperty));

            if (!valid) LogRejection("MetaTag.Property", property);
            return valid;
        }

        public static bool IsValidMetaContent(string content)
        {
            var valid = !string.IsNullOrWhiteSpace(content) &&
                        content.Length <= MaxContentLength &&
                        !content.Contains("<") &&
                        !content.Contains(">");

            if (!valid) LogRejection("MetaTag.Content", content);
            return valid;
        }

        public static bool IsValidRel(string rel)
        {
            var valid = !string.IsNullOrWhiteSpace(rel) &&
                        rel.Length <= MaxAttrLength &&
                        AllowedRelValues.Contains(rel.Trim());

            if (!valid) LogRejection("LinkTag.Rel", rel);
            return valid;
        }

        public static bool IsValidHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href) || href.Length > MaxHrefLength)
            {
                LogRejection("LinkTag.Href", href);
                return false;
            }

            var normalized = href.Trim().ToLowerInvariant();
            var valid = !normalized.StartsWith("javascript:") && !normalized.StartsWith("data:");

            if (!valid) LogRejection("LinkTag.Href", href);
            return valid;
        }

        public static bool IsValidSizes(string sizes)
        {
            if (string.IsNullOrEmpty(sizes)) return false;

            var valid = ValidSizesPattern.IsMatch(sizes.Trim());

            if (!valid) LogRejection("LinkTag.Sizes", sizes);
            return valid;
        }

        private static void LogRejection(string field, string value)
        {
            var context = HttpContext.Current;
            if (context == null || context.Items[RejectionLoggedKey] != null)
                return;

            try
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat($"Rejected OpenContent Head Tag (Field: {field}, Value: {value})");
                }
                
                EventLogController.Instance.AddLog(
                    "Rejected OpenContent Head Tag",
                    $"Field: {field}, Value: {value}",
                    PortalSettings.Current,
                    PortalSettings.Current?.UserId ?? -1,
                    EventLogController.EventLogType.HOST_ALERT
                );

                context.Items[RejectionLoggedKey] = true; // prevent further logs this request to prevent log flooding
            }
            catch (Exception ex)
            {
                // suppress any log failures
                LogError(ex);
            }
        }
        
        private static void LogError(Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex.Message, ex);
                if (ex.InnerException != null)
                {
                    Logger.Error(ex.InnerException.Message, ex.InnerException);
                }
            }
        }
    }
}