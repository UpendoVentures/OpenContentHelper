using Upendo.OpenContentHelper.Features.Seo.Models;

namespace Upendo.OpenContentHelper.Features.Seo.Data
{
    public interface ISeoRepository
    {
        int GetMetaModuleIdOnTab(int tabId);
        OpenContentHeadData GetLatestOpenContentHeadData(int moduleId);
    }
}