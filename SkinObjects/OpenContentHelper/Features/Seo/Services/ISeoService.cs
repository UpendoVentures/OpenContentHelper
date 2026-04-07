using Upendo.OpenContentHelper.Features.Seo.Models;

namespace Upendo.OpenContentHelper.Features.Seo.Services
{
    public interface ISeoService
    {
        OpenContentHeadData GetOpenContentHeadData(int activeTabId);
        SeoOverrideResult BuildOverrides(SeoContext context);
    }
}