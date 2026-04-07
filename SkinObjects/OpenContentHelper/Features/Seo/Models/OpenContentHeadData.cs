using System.Collections.Generic;

namespace Upendo.OpenContentHelper.Features.Seo.Models
{
    public class OpenContentHeadData
    {
        public List<HeadMetaTag> MetaTags { get; set; } = new List<HeadMetaTag>();
        public List<HeadLinkTag> LinkTags { get; set; } = new List<HeadLinkTag>();
    }
}