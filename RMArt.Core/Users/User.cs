using LinqToDB.Mapping;
using System;

namespace RMArt.Core
{
	[Table("Users")]
	public class User : ICloneable
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column, NotNull]
		public string Login { get; set; }

		[Column, NotNull]
		public byte[] PasswordHash { get; set; }

		[Column, NotNull]
		public string Name { get; set; }

		[Column, NotNull]
		public string Email { get; set; }

		[Column]
		public bool IsEmailConfirmed { get; set; }

		[Column]
		public UserRole Role { get; set; }

		[Column]
		public DateTime RegistrationDate { get; set; }

		[Column, NotNull]
		public byte[] RegistrationIP { get; set; }

		public User Clone()
		{
			return
				new User
				{
					ID = ID,
					Login = Login,
					Name = Name,
					Email = Email,
					Role = Role,
					PasswordHash = PasswordHash,
					IsEmailConfirmed = IsEmailConfirmed,
					RegistrationDate = RegistrationDate,
					RegistrationIP = RegistrationIP
				};
		}

		object ICloneable.Clone()
		{
			return Clone();
		}
	}
}