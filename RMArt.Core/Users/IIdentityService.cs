using System.Net;
using RMArt.DataModel;

namespace RMArt.Core
{
	public interface IIdentityService
	{
		int? CurrentUserID { get; }
		UserRole UserRole { get; }
		IPAddress UserIPAddress { get; }

		bool LogIn(string identity, string password, bool remember, out User user);
		void LogOut();
	}
}