using System.Net;
using RMArt.Client.Core;
using RMArt.Core;

namespace RMArt.Client
{
	public static class Authenticator
	{
		public static AuthenticationCookie Authenticate(SiteConfig siteConfig, string login, string password, IProgressIndicator progressIndicator)
		{
			progressIndicator.Message("Authentication...");

			AuthenticationCookie authCookie;
			try
			{
				authCookie = AccountClient.Authenticate(siteConfig, login, password);
			}
			catch (WebException webException)
			{
				progressIndicator.Message("Authentication error: " + webException.Message);
				return null;
			}

			if (authCookie == null)
			{
				progressIndicator.Message("Authentication failed.");
				return null;
			}

			return authCookie;
		}
	}
}