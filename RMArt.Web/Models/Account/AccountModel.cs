using System;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class AccountModel
	{
		public bool Self { get; set; }

		public string Login { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public UserRole Role { get; set; }
		public bool PrivateProfile { get; set; }
		public DateTime RegistrationDate { get; set; }

		public int PicturesUploaded { get; set; }
		public int RatesGiven { get; set; }
		public int PicturesFavorited { get; set; }
	}
}