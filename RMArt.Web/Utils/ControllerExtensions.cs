using System;
using System.Web.Mvc;

namespace RMArt.Web
{
	public static class ControllerExtensions
	{
		public static ActionResult ReturnOrRedirect(
			this Controller controller, string returnUrl, string redirectUrl = null)
		{
			if (controller == null)
				throw new ArgumentNullException("controller");

			return new RedirectResult(controller.Url.IsLocalUrl(returnUrl) ? returnUrl : (redirectUrl ?? "~"));
		}
	}
}