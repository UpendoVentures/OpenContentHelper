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

using Upendo.SkinObjects.OpenContentHelper.Common;
using Upendo.OpenContentHelper.Features.Events.Models;
using System;

namespace Upendo.OpenContentHelper.Features.Events.Helpers
{
    public static class EventUrlHelper
    {
        public static string GetDetailUrl(EventListItemDto item, string detailPagePath)
        {
            if (item == null)
            {
                return EventPageUrlHelper.BuildDetailBasePageUrl(detailPagePath);
            }

            return EventPageUrlHelper.BuildDetailPageUrl(detailPagePath, item.Slug);
        }

        public static string GetEditUrl(int eventId)
        {
            return eventId <= 0 ? string.Empty : GetEditUrl(eventId, EventConstants.EventEditPageRoute);
        }

        public static string GetEditUrl(int eventId, string editPagePath)
        {
            if (eventId <= 0 || string.IsNullOrEmpty(editPagePath))
            {
                return string.Empty;
            }

            return EventPageUrlHelper.BuildEditPageUrl(editPagePath, eventId);
        }
    }
}