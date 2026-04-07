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