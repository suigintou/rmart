using System;
using System.Net;
using System.Web;
using RMArt.Core;

namespace RMArt.Web
{
	public static class IdentityHelper
	{
		public static Identity CreateIdentity(this HttpRequestBase request, int? userID)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			IPAddress ipAddress;
			var cloudflareIP = request.Headers["CF-Connecting-IP"];
			if (cloudflareIP != null)
				ipAddress = IPAddress.Parse(cloudflareIP);
			else if (request.UserHostAddress != null)
				ipAddress = IPAddress.Parse(request.UserHostAddress);
			else
				ipAddress = IPAddress.None;

			return new Identity(userID, ipAddress);
		}

		public static bool IsUserInRole(this HttpRequestBase request, UserRole role)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			if (!request.IsAuthenticated)
				return false;

			return request.RequestContext.HttpContext.User.IsInRole(role.ToString());
		}

		public static ModerationStatus GetDefaultStatus(this HttpRequestBase request)
		{
			return
				!Config.Default.UploadsPremoderation || request.IsUserInRole(UserRole.Contributor)
					? ModerationStatus.Accepted
					: ModerationStatus.Pending;
		}
	}
}