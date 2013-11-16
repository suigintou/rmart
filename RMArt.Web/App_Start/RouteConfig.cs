using System.Web.Mvc;
using System.Web.Routing;

namespace RMArt.Web
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Viewer",
				"{id}",
				new { controller = "Pictures", action = "Viewer" },
				new { id = @"\d+" });

			routes.MapRoute(
				"Src",
				"{id}/Src/{fileName}",
				new { controller = "Pictures", action = "Src", fileName = UrlParameter.Optional },
				new { id = @"\d+" });

			routes.MapRoute(
				"Thumb",
				"{id}/Thumb/{size}.jpg",
				new { controller = "Pictures", action = "Thumb" },
				new { id = @"\d+", size = @"\d+" });

			routes.MapRoute(
				"Search",
				"Search",
				new { controller = "Pictures", action = "Search" });

			routes.MapRoute(
				"UserDetails",
				"User/{login}",
				new { controller = "Account", action = "View" });

			routes.MapRoute(
				"TagDetails",
				"Tags/{id}",
				new { controller = "Tags", action = "Details" },
				new { id = @"\d+" });

			routes.MapRoute(
				"ContentPage",
				"Pages/{name}",
				new { controller = "Content", action = "Page" });

			routes.MapRoute(
				"Default",
				"{controller}/{action}/{id}",
				new { controller = "Pictures", action = "Index", id = UrlParameter.Optional });
		}
	}
}