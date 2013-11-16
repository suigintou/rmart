using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class RegistrationModel
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

		[Display(Name = "AccountName", ResourceType = typeof(AccountResources))]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[StringLength(UsersValidation.MaximumNameLength, MinimumLength = UsersValidation.MinimumNameLength)]
		public string Name { get; set; }

		[Display(Name = "Password", ResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[StringLength(UsersValidation.MaximumPasswordLength, MinimumLength = UsersValidation.MinimumPasswordLength)]
		public string Password { get; set; }

		[Display(Name = "ConfirmPassword", ResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		[Compare(
			"Password",
			ErrorMessageResourceName = "PasswordsDoNotMatchMessage",
			ErrorMessageResourceType = typeof(AccountResources))]
		public string ConfirmPassword { get; set; }

		[Display(Name = "CaptchaLabel", ResourceType = typeof(SharedResources))]
		public string Captcha { get; set; }
	}
}