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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Upendo.SkinObjects.OpenContentHelper.Common;

namespace Upendo.OpenContentHelper.Features.Directory.Helpers
{
    public class DirectoryHelper
    {
        private const string PageSizeDefault = "24";

        public static string CurrentQueryStrings()
        {
            string cleanedQueryString = string.Empty;

            if (HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Url.Query))
            {
                NameValueCollection qs = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query);

                // Keys we NEVER want to carry forward
                var excludedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    Constants.TabIdKey,
                    Constants.LanguageKey, 
                    Constants.CompanyKey
                };

                // Build a clean querystring
                NameValueCollection cleanQs = HttpUtility.ParseQueryString(string.Empty);

                foreach (string key in qs.AllKeys)
                {
                    if (string.IsNullOrEmpty(key)) continue;
                    if (excludedKeys.Contains(key)) continue;

                    cleanQs[key] = qs[key];
                }

                if (cleanQs.Count > 0)
                {
                    cleanedQueryString = string.Concat(Constants.QuestionMark, cleanQs.ToString());
                }
            }

            return cleanedQueryString;
        }
        
        public static string BuildPageUrl(int page)
        {
            HttpRequest request = HttpContext.Current.Request;

            // Start from current querystring, preserve q/cat/sort/pagesize, replace page
            NameValueCollection nvc = HttpUtility.ParseQueryString(request.QueryString.ToString());

            nvc.Set(Constants.PageKey, page.ToString());

            // Ensure pagesize is always present
            if (string.IsNullOrEmpty(nvc[Constants.PageSizeKey]))
            {
                nvc.Set(Constants.PageSizeKey, DirectoryHelper.PageSizeDefault);
            }

            string path = request.Url.AbsolutePath;
            string qs = nvc.ToString();

            if (string.IsNullOrEmpty(qs))
            {
                return path;
            }

            return string.Concat(path, Constants.QuestionMark, qs);
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) { return min; }
            if (value > max) { return max; }
            return value;
        }
    }
}