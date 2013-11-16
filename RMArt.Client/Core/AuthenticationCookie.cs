using System;
using System.Net;

namespace RMArt.Client.Core
{
	public class AuthenticationCookie
	{
		internal Cookie Cookie { get; set; }

		internal AuthenticationCookie(Cookie cookie)
		{
			if (cookie == null)
				throw new ArgumentNullException("cookie");

			Cookie = cookie;
		}
	}
}