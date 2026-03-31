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

using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Upendo.Modules.UpendoEventsForm.Security
{
    internal static class EventRichTextSanitizer
    {
        private static readonly Lazy<HtmlSanitizer> Sanitizer = new Lazy<HtmlSanitizer>(CreateSanitizer);

        internal static string SanitizeFullDescription(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return null;
            }

            var sanitized = Sanitizer.Value.Sanitize(html);

            if (string.IsNullOrWhiteSpace(sanitized))
            {
                return null;
            }

            sanitized = sanitized.Trim();

            // Optional cleanup: collapse truly empty paragraphs left behind by pastes.
            sanitized = Regex.Replace(
                sanitized,
                @"<p>\s*(<br\s*/?>)?\s*</p>",
                string.Empty,
                RegexOptions.IgnoreCase);

            return string.IsNullOrWhiteSpace(sanitized)
                ? null
                : sanitized.Trim();
        }

        private static HtmlSanitizer CreateSanitizer()
        {
            var sanitizer = new HtmlSanitizer();

            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedSchemes.Clear();
            sanitizer.UriAttributes.Clear();

            foreach (var tag in new[]
            {
                "p",
                "br",
                "strong",
                "b",
                "em",
                "i",
                "ul",
                "ol",
                "li",
                "a"
            })
            {
                sanitizer.AllowedTags.Add(tag);
            }

            foreach (var attribute in new[]
            {
                "href",
                "target",
                "rel"
            })
            {
                sanitizer.AllowedAttributes.Add(attribute);
            }

            sanitizer.UriAttributes.Add("href");

            foreach (var scheme in new[]
            {
                "http",
                "https",
                "mailto",
                "tel"
            })
            {
                sanitizer.AllowedSchemes.Add(scheme);
            }

            sanitizer.AllowDataAttributes = false;

            // Strip any target value except _blank
            sanitizer.PostProcessNode += (sender, args) =>
            {
                var element = args.Node as AngleSharp.Dom.IElement;
                if (element == null || !element.TagName.Equals("A", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var target = element.GetAttribute("target");
                if (!string.IsNullOrWhiteSpace(target) &&
                    !target.Equals("_blank", StringComparison.OrdinalIgnoreCase))
                {
                    element.RemoveAttribute("target");
                }

                var rel = element.GetAttribute("rel");
                if (!string.IsNullOrWhiteSpace(rel))
                {
                    var cleanedRel = NormalizeRelTokens(rel);
                    if (string.IsNullOrWhiteSpace(cleanedRel))
                    {
                        element.RemoveAttribute("rel");
                    }
                    else
                    {
                        element.SetAttribute("rel", cleanedRel);
                    }
                }
            };

            return sanitizer;
        }

        private static string NormalizeRelTokens(string rel)
        {
            if (string.IsNullOrWhiteSpace(rel))
            {
                return null;
            }

            var tokens = rel
                .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToLowerInvariant())
                .Distinct()
                .ToList();

            return tokens.Count == 0
                ? null
                : string.Join(" ", tokens);
        }
    }
}