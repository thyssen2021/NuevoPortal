using System.Web.Mvc;
using System.Web.Routing;

namespace IdentitySample
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "upgrade_revision", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute("404-PageNotFound", "{*url}", new { controller = "Error", action = "NotFound" });
        }
    }
}