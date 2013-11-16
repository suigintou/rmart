using System;

namespace RMArt.Core
{
	public static class UsersHelper
	{
		public static int? GetIDByLogin(this IUsersService usersService, string login)
		{
			var user = usersService.GetByLogin(login);
			return user != null ? user.ID : default(int?);
		}

		public static User GetByLoginOrEmail(this IUsersService usersService, string identity)
		{
			if (identity == null)
				throw new ArgumentNullException("identity");

			return usersService.GetByLogin(identity) ?? usersService.GetByEmail(identity);
		}

		public static bool IsLoginAvaliable(this IUsersService usersService, int? userID, string login)
		{
			var userWithSameLogin = usersService.GetByLogin(login);
			return userWithSameLogin == null || userWithSameLogin.ID == userID;
		}

		public static bool IsEmailAvaliable(this IUsersService usersService, int? userID, string email)
		{
			var userWithSameEmail = usersService.GetByEmail(email);
			return userWithSameEmail == null || userWithSameEmail.ID == userID;
		}

		public static bool CheckCredentials(this IUsersService usersService, string identity, string password, out User user)
		{
			if (identity == null)
				throw new ArgumentNullException("identity");
			if (password == null)
				throw new ArgumentNullException("password");

			user = usersService.GetByLoginOrEmail(identity);

			return user != null && usersService.CheckPassword(user, password);
		}

		public static bool CheckPassword(this IUsersService usersService, int userID, string password)
		{
			var user = usersService.GetByID(userID);
			if (user == null)
				throw new ArgumentException("User with same ID not found.", "userID");
			return usersService.CheckPassword(user, password);
		}
	}
}