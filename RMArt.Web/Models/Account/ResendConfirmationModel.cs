using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class ResendConfirmationModel
	{
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[Display(Name = "LoginOrEmail", ResourceType = typeof(AccountResources))]
		public string LoginOrEmail { get; set; }

		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		[Display(Name = "Password", ResourceType = typeof(AccountResources))]
		public string Password { get; set; }

		[Display(Name = "CaptchaLabel", ResourceType = typeof(SharedResources))]
		public string Captcha { get; set; }

		[DataType(DataType.EmailAddress)]
		[StringLength(UsersValidation.MaximumEmailLength)]
		[RegularExpression(
			UsersValidation.EmailValidationPattern,
			ErrorMessageResourceName = "InvalidEmailMessage",
			ErrorMessageResourceType = typeof(AccountResources))]
		[Display(Name = "NewEmail", ResourceType = typeof(AccountResources))]
		public string NewEmail { get; set; }
	}
}