using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class AccauntSettingsModel
	{
		[Display(Name = "AccountLogin", ResourceType = typeof(AccountResources))]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[StringLength(UsersValidation.MaximumLoginLength, MinimumLength = UsersValidation.MinimumLoginLength)]
		[RegularExpression(
			UsersValidation.LoginValidationPattern,
			ErrorMessageResourceName = "InvalidLoginMessage",
			ErrorMessageResourceType = typeof(AccountResources))]
		public string Login { get; set; }

		[Display(Name = "AccountEmail", ResourceType = typeof(AccountResources))]
		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[StringLength(UsersValidation.MaximumEmailLength)]
		[RegularExpression(
			UsersValidation.EmailValidationPattern,
			ErrorMessageResourceName = "InvalidEmailMessage",
			ErrorMessageResourceType = typeof(AccountResources))]
		public string Email { get; set; }

		[Display(Name = "PrivateProfile", ResourceType = typeof(AccountResources))]
		public bool PrivateProfile { get; set; }

		[Display(Name = "EnterPassword", ResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		public string Password { get; set; }
	}
}