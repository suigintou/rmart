using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RMArt.Core;

namespace RMArt.Web
{
	public class MyRoleProvider : RoleProvider
	{
		private readonly IUsersService _usersService = DependencyResolver.Current.GetService<IUsersService>();
		private readonly UserRole[] _roles = (UserRole[])Enum.GetValues(typeof(UserRole));
		private readonly string[] _rolesNames = Enum.GetNames(typeof(UserRole));

		public override bool IsUserInRole(string username, string roleName)
		{
			return GetRolesForUser(username).Contains(roleName);
		}

		public override string[] GetRolesForUser(string username)
		{
			int userID;
			UserRole? role;

			if (int.TryParse(username, out userID) && (role = _usersService.GetRole(userID)) != null)
				return _roles.Where(r => r <= role).Select(r => r.ToString()).ToArray();

			return new string[0];
		}

		public override void CreateRole(string roleName)
		{
			throw new NotSupportedException();
		}

		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			throw new NotSupportedException();
		}

		public override bool RoleExists(string roleName)
		{
			return GetAllRoles().Contains(roleName);
		}

		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			throw new NotSupportedException();
		}

		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			throw new NotSupportedException();
		}

		public override string[] GetUsersInRole(string roleName)
		{
			throw new NotSupportedException();
		}

		public override string[] GetAllRoles()
		{
			return _rolesNames;
		}

		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			throw new NotSupportedException();
		}

		public override string ApplicationName { get; set; }
	}
}