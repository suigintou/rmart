using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RMArt.Core
{
	public static class UsersValidation
	{
		public const int MinimumLoginLength = 3;
		public const int MaximumLoginLength = 20;
		public const string LoginValidationPattern = @"^[0-9a-zA-Z._\-]*$";
		public const int MinimumNameLength = 1;
		public const int MaximumNameLength = 50;
		public const int MinimumPasswordLength = 6;
		public const int MaximumPasswordLength = 30;
		public const int MaximumEmailLength = 50;
		public const string EmailValidationPattern =
			@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
			@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$";

		public static readonly HashSet<string> RestrictedLogins =
			new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "admin", "administrator", "moderator", "mod", "owner" };
		private static readonly Regex _loginValidationRegex = new Regex(LoginValidationPattern, RegexOptions.Compiled);
		private static readonly Regex _emailValidationRegex = new Regex(EmailValidationPattern, RegexOptions.Compiled);

		public static bool ValidateLogin(string login)
		{
			if (login == null)
				throw new ArgumentNullException("login");

			if (login.Length < MinimumLoginLength)
				return false;
			if (login.Length > MaximumLoginLength)
				return false;
			if (RestrictedLogins.Contains(login))
				return false;
			if (!_loginValidationRegex.IsMatch(login))
				return false;

			return true;
		}

		public static bool ValidatePassword(string password)
		{
			if (password == null)
				throw new ArgumentNullException("password");

			if (password.Length < MinimumPasswordLength)
				return false;
			if (password.Length > MaximumPasswordLength)
				return false;

			return true;
		}

		public static bool ValidateEmail(string email)
		{
			if (email == null)
				throw new ArgumentNullException("email");

			return email.Length <= MaximumEmailLength && _emailValidationRegex.IsMatch(email);
		}

		public static bool ValidateName(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			if (name.Length < MinimumNameLength)
				return false;
			if (name.Length > MaximumNameLength)
				return false;

			return true;
		}
	}
}