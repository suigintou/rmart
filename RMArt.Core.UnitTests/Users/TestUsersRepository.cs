using System;
using System.Collections.Generic;
using System.Linq;

namespace RMArt.Core.UnitTests.Users
{
	public class TestUsersRepository : IUsersRepository
	{
		private readonly List<User> _users = new List<User>();

		public User GetByID(int id)
		{
			return _users.Where(u => u.ID == id).Select(u => u.Clone()).SingleOrDefault();
		}

		public User GetByLogin(string login)
		{
			return _users.Where(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase)).Select(u => u.Clone()).SingleOrDefault();
		}

		public User GetByEmail(string email)
		{
			return _users.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).Select(u => u.Clone()).SingleOrDefault();
		}

		public UserRole? GetRole(int id)
		{
			return GetByID(id).Role;
		}

		public IQueryable<User> Find()
		{
			return _users.Select(u => u.Clone()).AsQueryable();
		}

		public int Add(User user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			user = user.Clone();
			user.ID = _users.Select(u => u.ID).LastOrDefault() + 1;
			_users.Add(user);
			return user.ID;
		}

		public User Update(int id, UserUpdate update)
		{
			var user = _users.SingleOrDefault(u => u.ID == id);
			if (user == null)
				return null;
			var old = user.Clone();

			if (update.Login != null)
				user.Login = update.Login;
			if (update.Name != null)
				user.Name = update.Name;
			if (update.Email != null)
				user.Email = update.Email;
			if (update.PasswordHash != null)
				user.PasswordHash = update.PasswordHash;
			if (update.Role != null)
				user.Role = update.Role.Value;
			if (update.IsEmailChecked != null)
				user.IsEmailConfirmed = update.IsEmailChecked.Value;

			return old;
		}
	}
}