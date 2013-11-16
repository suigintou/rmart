using System;
using System.Linq;

namespace RMArt.Core
{
	public interface IUsersService
	{
		int CreateUser(
			string login,
			string name,
			string email,
			bool isEmailConfirmed,
			string password,
			UserRole role);

		void UpdateLogin(int id, string login);
		void UpdateName(int id, string name);
		void UpdatePassword(int id, string password);
		bool UpdateEmail(int id, string email);
		void UpdateIsPrivateProfile(int id, bool isPrivateProfile);
		void UpdateIsEmailConfirmed(int id, bool isEmailConfirmed);
		bool UpdateRole(int id, UserRole role);

		User GetByID(int id);
		User GetByLogin(string login);
		User GetByEmail(string email);
		UserRole? GetRole(int id);

		IQueryable<User> Find();

		bool CheckPassword(User user, string password);

		event EventHandler<RoleChangedEventArgs> RoleChanged;
	}
}