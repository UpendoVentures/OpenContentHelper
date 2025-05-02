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
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using Newtonsoft.Json.Linq;
using Upendo.SkinObjects.OpenContentHelper.Components;

namespace Upendo.SkinObjects.OpenContentHelper
{
    /// <summary>
    /// This module injects OpenContent meta and link tags into the page header.
    /// </summary>
    public partial class View : OpenContentHelperModuleBase
    {
        #region Events
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                var page = this.Page;
                var portalSettings = this.PortalSettings;
                if (page?.Header == null || portalSettings == null)
                    return;

                int tabId = portalSettings.ActiveTab.TabID;
                int moduleId = GetMetaModuleOnPage(tabId);

                if (moduleId <= 0) return;

                var data = LoadMetaData(moduleId);
                InjectMetaTags(data.MetaTags, page);
                InjectLinkTags(data.LinkTags, page);
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // do something? nothing? 😆
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        #region Helper Methods 
        private int GetMetaModuleOnPage(int tabId)
        {
            var modules = ModuleController.Instance.GetTabModules(tabId);
            foreach (ModuleInfo mod in modules.Values)
            {
                if (mod.DesktopModule?.ModuleName == "OpenContent" && mod.ModuleTitle.Contains("Meta"))
                    return mod.ModuleID;
            }
            return Null.NullInteger;
        }

        private (List<MetaTag> MetaTags, List<LinkTag> LinkTags) LoadMetaData(int moduleId)
        {
            var metaTags = new List<MetaTag>();
            var linkTags = new List<LinkTag>();

            using (var conn = new SqlConnection(Config.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"
                    SELECT TOP 1 [Json]
                    FROM [dbo].[OpenContent_Items]
                    WHERE [ModuleId] = @ModuleId
                    ORDER BY [LastModifiedOnDate] DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@ModuleId", moduleId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var json = reader["Json"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                var parsed = JObject.Parse(json);
                                metaTags = parsed["MetaTags"]?.ToObject<List<MetaTag>>() ?? new List<MetaTag>();
                                linkTags = parsed["LinkTags"]?.ToObject<List<LinkTag>>() ?? new List<LinkTag>();
                            }
                        }
                    }
                }
            }

            return (metaTags, linkTags);
        }

        private void InjectMetaTags(IEnumerable<MetaTag> metaTags, Page page)
        {
            if (metaTags == null || page?.Header == null)
                return;

            foreach (var tag in metaTags)
            {
                if ((MetaValidationHelper.IsValidMetaName(tag.Name) || MetaValidationHelper.IsValidMetaProperty(tag.Property)) &&
                    MetaValidationHelper.IsValidMetaContent(tag.Content))
                {
                    var meta = new HtmlMeta();

                    if (!string.IsNullOrEmpty(tag.Name) && !string.Equals(tag.Name.Trim(), "N/A", StringComparison.OrdinalIgnoreCase))
                    {
                        meta.Name = tag.Name.Trim();
                    }

                    if (!string.IsNullOrEmpty(tag.Property) && MetaValidationHelper.IsValidMetaProperty(tag.Property))
                    {
                        meta.Attributes.Add("property", tag.Property.Trim());
                    }

                    if (!string.IsNullOrEmpty(tag.Content))
                    {
                        meta.Content = tag.Content.Trim();
                    }

                    page.Header.Controls.Add(meta);
                }
            }
        }

        private void InjectLinkTags(IEnumerable<LinkTag> linkTags, Page page)
        {
            if (linkTags == null || page?.Header == null)
                return;

            foreach (var tag in linkTags)
            {
                if (MetaValidationHelper.IsValidHref(tag.Href))
                {
                    var link = new HtmlLink { Href = tag.Href.Trim() };

                    if (MetaValidationHelper.IsValidRel(tag.Rel)) link.Attributes["rel"] = tag.Rel.Trim();
                    if (!string.IsNullOrWhiteSpace(tag.Type)) link.Attributes["type"] = tag.Type.Trim();
                    if (!string.IsNullOrWhiteSpace(tag.Hreflang)) link.Attributes["hreflang"] = tag.Hreflang.Trim();
                    if (MetaValidationHelper.IsValidSizes(tag.Sizes)) link.Attributes["sizes"] = tag.Sizes.Trim();

                    page.Header.Controls.Add(link);
                }
            }
        }

        private bool IsSafeValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            input = input.Trim().ToLowerInvariant();

            return !input.Contains("<") &&
                   !input.Contains(">") &&
                   !input.Contains("\"") &&
                   !input.Contains("'") &&
                   !input.Contains("script") &&
                   !input.Contains("onerror") &&
                   !input.Contains("onload");
        }

        private bool IsSafeHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return false;

            href = href.Trim().ToLowerInvariant();

            return !href.StartsWith("javascript:") &&
                   !href.StartsWith("data:") &&
                   !href.Contains("<") &&
                   !href.Contains(">");
        }
        #endregion

        #region Classes 
        public class MetaTag
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public string Content { get; set; }
            public string Property { get; set; }
        }

        public class LinkTag
        {
            public string ID { get; set; }
            public string Rel { get; set; }
            public string Href { get; set; }
            public string Type { get; set; }
            public string Hreflang { get; set; }
            public string Sizes { get; set; }
        }
        #endregion
    }
}