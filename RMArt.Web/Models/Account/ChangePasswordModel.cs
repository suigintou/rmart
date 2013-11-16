using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class ChangePasswordModel
	{
		public string Key { get; set; }

		[Display(Name = "OldPassword", ResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; }

		[Display(Name = "NewPassword", ResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[StringLength(UsersValidation.MaximumPasswordLength, MinimumLength = UsersValidation.MinimumPasswordLength)]
		public string NewPassword { get; set; }

		[Display(Name = "ConfirmNewPassword", ResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		[Compare(
			"NewPassword",
			ErrorMessageResourceName = "PasswordsDoNotMatchMessage",
			ErrorMessageResourceType = typeof(AccountResources))]
		public string ConfirmPassword { get; set; }
	}
}