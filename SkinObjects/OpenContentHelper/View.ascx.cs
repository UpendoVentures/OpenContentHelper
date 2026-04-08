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
using DotNetNuke.Services.Exceptions;
using System;
using Upendo.OpenContentHelper.Features.Seo;
using Upendo.OpenContentHelper.Features.Seo.Helpers;
using Upendo.OpenContentHelper.Features.Seo.Models;
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
                if (Page?.Header == null || PortalSettings == null)
                {
                    return;
                }

                var seoService = SeoServiceFactory.Create(Config.GetConnectionString());

                var headData = seoService.GetOpenContentHeadData(PortalSettings.ActiveTab.TabID);
                SeoHeadWriter.InjectOpenContentTags(Page, headData.MetaTags, headData.LinkTags);

                var context = new SeoContext
                {
                    PortalSettings = PortalSettings,
                    Request = Request,
                    Authority = Request?.Url?.GetLeftPart(UriPartial.Authority),
                    CurrentPageTitle = Page.Title,
                    ActiveTabId = PortalSettings.ActiveTab.TabID,
                    PortalId = PortalSettings.PortalId
                };

                var overrides = seoService.BuildOverrides(context);
                SeoHeadWriter.Apply(Page, PortalSettings, overrides);
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
    }
}