using System;
using System.Web;
using System.Web.Security;

namespace RMArt.Core
{
	public class FormsAuthenticationService : IAuthenticationService
	{
		private readonly IUsersService _usersService;

		public FormsAuthenticationService(IUsersService usersService)
		{
			if (usersService == null)
				throw new ArgumentNullException("usersService");

			_usersService = usersService;
		}

		public bool LogIn(string identity, string password, bool remember, out User user)
		{
			if (identity == null)
				throw new ArgumentNullException("identity");
			if (password == null)
				throw new ArgumentNullException("password");

			var res = _usersService.CheckCredentials(identity, password, out user);
			if (res)
				FormsAuthentication.SetAuthCookie(user.ID.ToString(), remember);
			return res;
		}

		public void LogOut()
		{
			FormsAuthentication.SignOut();
		}

		public int? CurrentUserID
		{
			get
			{
				return
					HttpContext.Current.Request.IsAuthenticated
						? (int?)int.Parse(HttpContext.Current.User.Identity.Name)
						: null;
			}
		}
	}
}