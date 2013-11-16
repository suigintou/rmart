using Microsoft.Web.Helpers;
using System;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RMArt.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			ContainerConfig.InitContainer(Server);

			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());

			ReCaptcha.PublicKey = Config.Default.ReCapthchaPublicKey;
			ReCaptcha.PrivateKey = Config.Default.ReCapthchaPrivateKey;
		}

		protected void Application_AcquireRequestState(object sender, EventArgs e)
		{
			var locale = Request.GetUICulture();
			Thread.CurrentThread.CurrentUICulture = locale;
			Thread.CurrentThread.CurrentCulture = locale;
		}
	}
}