using DotNetNuke.Web.Api;

namespace Upendo.SkinObjects.OpenContentHelper.WebApi
{
    /// <summary>
    /// Route Mapper
    /// </summary>
    public class RouteMapper : IServiceRouteMapper
    {
        private const string FolderName = "OpenContentHelper";
        private const string RouteName = "default";
        private const string Url = "{controller}/{action}";
        private const string Namespace = "Upendo.SkinObjects.OpenContentHelper.Controllers";

        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(FolderName, RouteName, Url, new string[] { Namespace });
        }
    }
}