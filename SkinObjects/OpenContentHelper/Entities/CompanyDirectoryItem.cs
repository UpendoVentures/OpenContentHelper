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


namespace Upendo.SkinObjects.OpenContentHelper.Entities
{
    public sealed class CompanyDirectoryItem
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public string ShortDescription { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        
        public string Address1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        public byte MembershipStatus { get; set; }
        public int? MemberSinceYear { get; set; }

        public bool IsFeatured { get; set; }
        public int FeaturedSortOrder { get; set; }

        public int? LogoFileId { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
    }
}