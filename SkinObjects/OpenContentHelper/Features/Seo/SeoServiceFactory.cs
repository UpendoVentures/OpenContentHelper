using Upendo.OpenContentHelper.Features.Seo.Data;
using Upendo.OpenContentHelper.Features.Seo.Services;

namespace Upendo.OpenContentHelper.Features.Seo
{
    public static class SeoServiceFactory
    {
        public static ISeoService Create(string connectionString)
        {
            var repository = new SqlSeoRepository(connectionString);
            return new SeoService(repository);
        }
    }
}