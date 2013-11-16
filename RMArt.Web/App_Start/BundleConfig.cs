using System.Web.Optimization;

namespace RMArt.Web
{
	public static class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new StyleBundle("~/css").Include("~/Styles/*.css"));
			bundles.Add(new ScriptBundle("~/js").Include("~/Scripts/*.js"));

			BundleTable.EnableOptimizations = true;
		}
	}
}