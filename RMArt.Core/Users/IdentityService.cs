using System;
using System.Net;
using System.Web;
using System.Web.Security;
using RMArt.DataModel;

namespace RMArt.Core
{
	public class IdentityService : IIdentityService
	{
		private readonly IUsersService _usersService;

		public IdentityService(IUsersService usersService, HttpRequestBase request)
		{
			if (usersService == null)
				throw new ArgumentNullException("usersService");

			_usersService = usersService;

			CurrentUserID =
				request.IsAuthenticated
					? (int?)int.Parse(request.RequestContext.HttpContext.User.Identity.Name)
					: null;
			UserRole = CurrentUserID != null ? _usersService.GetRole(CurrentUserID.Value) ?? UserRole.Guest : UserRole.Guest;
			UserIPAddress =
				request.UserHostAddress != null
					? IPAddress.Parse(request.UserHostAddress)
					: IPAddress.None;
		}

		public int? CurrentUserID { get; private set; }
		public UserRole UserRole { get; private set; }
		public IPAddress UserIPAddress { get; private set; }

		public bool LogIn(string identity, string password, bool remember, out User user)
		{
			if (identity == null)
				throw new ArgumentNullException("identity");
			if (password == null)
				throw new ArgumentNullException("password");

			var res = _usersService.CheckCredentials(identity, password, out user);
			if (res)
			{
				FormsAuthentication.SetAuthCookie(user.ID.ToString(), remember);
				CurrentUserID = user.ID;
			}
			return res;
		}

		public void LogOut()
		{
			FormsAuthentication.SignOut();
			CurrentUserID = null;
		}
	}
}