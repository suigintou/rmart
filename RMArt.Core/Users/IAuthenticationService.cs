namespace RMArt.Core
{
	public interface IAuthenticationService
	{
		bool LogIn(string identity, string password, bool remember, out User user);
		void LogOut();

		int? CurrentUserID { get; }
	}
}