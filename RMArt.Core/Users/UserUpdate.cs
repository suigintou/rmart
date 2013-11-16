namespace RMArt.Core
{
	public class UserUpdate
	{
		public string Login { get; set; }
		public byte[] PasswordHash { get; set; }
		public string Email { get; set; }
		public UserRole? Role { get; set; }
		public string Name { get; set; }
		public bool? IsEmailChecked { get; set; }
		public bool? IsPrivateProfile { get; set; }
	}
}