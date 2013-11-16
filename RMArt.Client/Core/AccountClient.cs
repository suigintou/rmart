using System.Net;

namespace RMArt.Client.Core
{
	public static class AccountClient
	{
		public static AuthenticationCookie Authenticate(SiteConfig site, string login, string password)
		{
			var authReq = (HttpWebRequest)WebRequest.Create(site.AuthenticationUrl);
			authReq.Method = "POST";
			authReq.AllowAutoRedirect = false;
			var cookieContainer = new CookieContainer();
			authReq.CookieContainer = cookieContainer;
			using (var form = new MultiFormBuilder(authReq))
			{
				form.AppendFormData("LoginOrEmail", login);
				form.AppendFormData("Password", password);
				form.AppendFormData("Remember", "false");
			}
			Cookie cookie;
			using (authReq.GetResponse())
				cookie = cookieContainer.GetCookies(authReq.Address)[".ASPXAUTH"];
			return cookie != null ? new AuthenticationCookie(cookie) : null;
		}
	}
}