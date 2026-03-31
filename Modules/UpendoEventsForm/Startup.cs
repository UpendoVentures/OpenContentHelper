using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Upendo.Modules.UpendoEventsForm.Components;
using Upendo.Modules.UpendoEventsForm.Data;
using Upendo.Modules.UpendoEventsForm.Repository;
using Upendo.Modules.UpendoEventsForm.Repository.Contract;

namespace Upendo.Modules.UpendoEventsForm
{
    public class Startup : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<DapperContext>();
            services.AddTransient<DnnDataHelper>();
            services.AddTransient<IEventAdminRepository, EventAdminRepository>();
        }
    }
}