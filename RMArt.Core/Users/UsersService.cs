using System;
using System.Linq;
using System.Net;
using System.Web;

namespace RMArt.Core
{
	public class UsersService : IUsersService
	{
		private readonly IUsersRepository _usersRepository;

		public UsersService(IUsersRepository usersRepository)
		{
			if (usersRepository == null)
				throw new ArgumentNullException("usersRepository");

			_usersRepository = usersRepository;
		}

		public int CreateUser(
			string login,
			string name,
			string email,
			bool isEmailConfirmed,
			string password,
			UserRole role)
		{
			if (!UsersValidation.ValidateLogin(login))
				throw new ArgumentException("Invalid login.", "login");
			if (!UsersValidation.ValidatePassword(password))
				throw new ArgumentException("Invalid password.", "password");
			if (!UsersValidation.ValidateEmail(email))
				throw new ArgumentException("Invalid email.", "email");
			if (!UsersValidation.ValidateName(name))
				throw new ArgumentException("Invalid name.", "name");
			if (!this.IsLoginAvaliable(null, login))
				throw new ArgumentException("Login is alerady taken.", "login");
			if (!this.IsEmailAvaliable(null, email))
				throw new ArgumentException("Email is alerady taken.", "email");

			return _usersRepository.Add(
				new User
				{
					Login = login,
					PasswordHash = ComputePasswordHash(password),
					Name = name,
					Email = email,
					IsEmailConfirmed = isEmailConfirmed,
					Role = role,
					RegistrationDate = DateTime.UtcNow,
					RegistrationIP =
						HttpContext.Current != null && HttpContext.Current.Request.UserHostAddress != null
							? IPAddress.Parse(HttpContext.Current.Request.UserHostAddress).GetAddressBytes()
							: IPAddress.None.GetAddressBytes()
				});
		}

		public void UpdateLogin(int id, string login)
		{
			if (!UsersValidation.ValidateLogin(login))
				throw new ArgumentException("Invalid login.", "login");
			if (!this.IsLoginAvaliable(id, login))
				throw new ArgumentException("Login is alerady taken.", "login");

			_usersRepository.Update(id, new UserUpdate { Login = login });
		}

		public void UpdateName(int id, string name)
		{
			if (!UsersValidation.ValidateName(name))
				throw new ArgumentException("Invalid name.", "name");

			_usersRepository.Update(id, new UserUpdate { Name = name });
		}

		public void UpdatePassword(int id, string password)
		{
			if (!UsersValidation.ValidatePassword(password))
				throw new ArgumentException("Invalid password.", "password");

			_usersRepository.Update(id, new UserUpdate { PasswordHash = ComputePasswordHash(password) });
		}

		public bool UpdateEmail(int id, string email)
		{
			if (!UsersValidation.ValidateEmail(email))
				throw new ArgumentException("Invalid email.", "email");
			if (!this.IsEmailAvaliable(id, email))
				throw new ArgumentException("Email is alerady taken.", "email");

			var old = _usersRepository.Update(id, new UserUpdate { Email = email });
			return !old.Email.Equals(email, StringComparison.OrdinalIgnoreCase);
		}

		public void UpdateIsPrivateProfile(int id, bool isPrivateProfile)
		{
			_usersRepository.Update(id, new UserUpdate { IsPrivateProfile = isPrivateProfile });
		}

		public void UpdateIsEmailConfirmed(int id, bool isEmailConfirmed)
		{
			_usersRepository.Update(id, new UserUpdate { IsEmailChecked = isEmailConfirmed });
		}

		public bool UpdateRole(int id, UserRole role)
		{
			var old = _usersRepository.Update(id, new UserUpdate { Role = role });
			if (old.Role == role)
				return false;
			OnRoleChanged(new RoleChangedEventArgs(id, old.Role, role));
			return true;
		}

		public User GetByID(int id)
		{
			return _usersRepository.GetByID(id);
		}

		public User GetByLogin(string login)
		{
			return _usersRepository.GetByLogin(login);
		}

		public User GetByEmail(string email)
		{
			return _usersRepository.GetByEmail(email);
		}

		public UserRole? GetRole(int id)
		{
			return _usersRepository.GetRole(id);
		}

		public IQueryable<User> Find()
		{
			return _usersRepository.Find();
		}

		public bool CheckPassword(User user, string password)
		{
			return user.PasswordHash.SequenceEqual(ComputePasswordHash(password));
		}

		public event EventHandler<RoleChangedEventArgs> RoleChanged;

		private static byte[] ComputePasswordHash(string password)
		{
			return password.ComputeSHA1Hash();
		}

		private void OnRoleChanged(RoleChangedEventArgs e)
		{
			var handler = RoleChanged;
			if (handler != null)
				handler(this, e);
		}
	}
}