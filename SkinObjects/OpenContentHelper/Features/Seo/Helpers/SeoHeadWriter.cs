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
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Upendo.OpenContentHelper.Features.Seo.Models;
using Upendo.SkinObjects.OpenContentHelper.Components;

namespace Upendo.OpenContentHelper.Features.Seo.Helpers
{
    public static class SeoHeadWriter
    {
        public static void Apply(Page page, PortalSettings portalSettings, SeoOverrideResult result)
        {
            if (page?.Header == null || result == null)
            {
                return;
            }

            SetPageTitle(page, portalSettings, result.Title);

            if (result.MetaTags != null)
            {
                foreach (var meta in result.MetaTags)
                {
                    if (!string.IsNullOrWhiteSpace(meta.Name))
                    {
                        UpsertMetaName(page, meta.Name, meta.Content);
                    }
                    else if (!string.IsNullOrWhiteSpace(meta.Property))
                    {
                        UpsertMetaProperty(page, meta.Property, meta.Content);
                    }
                }
            }

            if (result.LinkTags != null)
            {
                foreach (var link in result.LinkTags)
                {
                    if (!string.IsNullOrWhiteSpace(link.Rel) &&
                        link.Rel.Equals("canonical", StringComparison.OrdinalIgnoreCase))
                    {
                        UpsertCanonicalLink(page, link.Href);
                    }
                    else
                    {
                        UpsertLinkTag(page, link);
                    }
                }
            }
        }

        public static void InjectOpenContentTags(Page page, IEnumerable<HeadMetaTag> metaTags, IEnumerable<HeadLinkTag> linkTags)
        {
            if (page?.Header == null)
            {
                return;
            }

            if (metaTags != null)
            {
                foreach (var tag in metaTags)
                {
                    if ((MetaValidationHelper.IsValidMetaName(tag.Name) || MetaValidationHelper.IsValidMetaProperty(tag.Property)) &&
                        MetaValidationHelper.IsValidMetaContent(tag.Content))
                    {
                        var meta = new HtmlMeta();

                        if (!string.IsNullOrWhiteSpace(tag.Name) &&
                            !string.Equals(tag.Name.Trim(), "N/A", StringComparison.OrdinalIgnoreCase))
                        {
                            meta.Name = tag.Name.Trim();
                        }

                        if (!string.IsNullOrWhiteSpace(tag.Property) &&
                            MetaValidationHelper.IsValidMetaProperty(tag.Property))
                        {
                            meta.Attributes["property"] = tag.Property.Trim();
                        }

                        meta.Content = tag.Content.Trim();
                        page.Header.Controls.Add(meta);
                    }
                }
            }

            if (linkTags != null)
            {
                foreach (var tag in linkTags)
                {
                    if (!MetaValidationHelper.IsValidHref(tag.Href))
                    {
                        continue;
                    }

                    var link = new HtmlLink { Href = tag.Href.Trim() };

                    if (MetaValidationHelper.IsValidRel(tag.Rel)) link.Attributes["rel"] = tag.Rel.Trim();
                    if (!string.IsNullOrWhiteSpace(tag.Type)) link.Attributes["type"] = tag.Type.Trim();
                    if (!string.IsNullOrWhiteSpace(tag.Hreflang)) link.Attributes["hreflang"] = tag.Hreflang.Trim();
                    if (MetaValidationHelper.IsValidSizes(tag.Sizes)) link.Attributes["sizes"] = tag.Sizes.Trim();

                    page.Header.Controls.Add(link);
                }
            }
        }

        private static void SetPageTitle(Page page, PortalSettings portalSettings, string title)
        {
            if (page == null || string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            var cleanTitle = title.Trim();

            if (portalSettings?.ActiveTab != null)
            {
                portalSettings.ActiveTab.Title = cleanTitle;
            }

            page.Title = cleanTitle;

            if (page is DotNetNuke.Framework.CDefault dnnPage)
            {
                dnnPage.Title = cleanTitle;
            }
        }

        private static void UpsertMetaName(Page page, string name, string content)
        {
            if (page?.Header == null || string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            RemoveMetaTagsByName(page, name);

            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var meta = new HtmlMeta
            {
                Name = name.Trim(),
                Content = content.Trim()
            };

            page.Header.Controls.Add(meta);
        }

        private static void UpsertMetaProperty(Page page, string propertyName, string content)
        {
            if (page?.Header == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            RemoveMetaTagsByProperty(page, propertyName);

            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var meta = new HtmlMeta
            {
                Content = content.Trim()
            };

            meta.Attributes["property"] = propertyName.Trim();
            page.Header.Controls.Add(meta);
        }

        private static void UpsertCanonicalLink(Page page, string href)
        {
            if (page?.Header == null)
            {
                return;
            }

            RemoveLinkTagsByRel(page, "canonical");

            if (string.IsNullOrWhiteSpace(href))
            {
                return;
            }

            var link = new HtmlLink
            {
                Href = href.Trim()
            };

            link.Attributes["rel"] = "canonical";
            page.Header.Controls.Add(link);
        }

        private static void UpsertLinkTag(Page page, HeadLinkTag tag)
        {
            if (page?.Header == null || tag == null || string.IsNullOrWhiteSpace(tag.Href))
            {
                return;
            }

            var link = new HtmlLink
            {
                Href = tag.Href.Trim()
            };

            if (!string.IsNullOrWhiteSpace(tag.Rel)) link.Attributes["rel"] = tag.Rel.Trim();
            if (!string.IsNullOrWhiteSpace(tag.Type)) link.Attributes["type"] = tag.Type.Trim();
            if (!string.IsNullOrWhiteSpace(tag.Hreflang)) link.Attributes["hreflang"] = tag.Hreflang.Trim();
            if (!string.IsNullOrWhiteSpace(tag.Sizes)) link.Attributes["sizes"] = tag.Sizes.Trim();

            page.Header.Controls.Add(link);
        }

        private static void RemoveMetaTagsByName(Page page, string name)
        {
            for (int i = page.Header.Controls.Count - 1; i >= 0; i--)
            {
                var meta = page.Header.Controls[i] as HtmlMeta;
                if (meta != null &&
                    !string.IsNullOrWhiteSpace(meta.Name) &&
                    meta.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    page.Header.Controls.RemoveAt(i);
                }
            }
        }

        private static void RemoveMetaTagsByProperty(Page page, string propertyName)
        {
            for (int i = page.Header.Controls.Count - 1; i >= 0; i--)
            {
                var meta = page.Header.Controls[i] as HtmlMeta;
                if (meta != null &&
                    meta.Attributes["property"] != null &&
                    meta.Attributes["property"].Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    page.Header.Controls.RemoveAt(i);
                }
            }
        }

        private static void RemoveLinkTagsByRel(Page page, string rel)
        {
            for (int i = page.Header.Controls.Count - 1; i >= 0; i--)
            {
                var link = page.Header.Controls[i] as HtmlLink;
                if (link != null &&
                    link.Attributes["rel"] != null &&
                    link.Attributes["rel"].Equals(rel, StringComparison.OrdinalIgnoreCase))
                {
                    page.Header.Controls.RemoveAt(i);
                }
            }
        }
    }
}