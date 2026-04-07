using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Upendo.Modules.BusinessDirectoryManager.Data;
using Upendo.Modules.BusinessDirectoryManager.Repository;
using Upendo.Modules.BusinessDirectoryManager.Repository.Contract;
using Upendo.Modules.BusinessDirectoryManager.Services;

namespace Upendo.Modules.BusinessDirectoryManager
{
    public class Startup : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<DapperContext>();
            services.AddTransient<IBusinessDirectoryAdminRepository, BusinessDirectoryAdminRepository>();
        }
    }
}
