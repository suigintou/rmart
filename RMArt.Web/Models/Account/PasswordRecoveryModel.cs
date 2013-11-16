using System.ComponentModel.DataAnnotations;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class PasswordRecoveryModel
	{
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[Display(Name = "LoginOrEmail", ResourceType = typeof(AccountResources))]
		public string LoginOrEmail { get; set; }

		[Display(Name = "CaptchaLabel", ResourceType = typeof(SharedResources))]
		public string Captcha { get; set; }
	}
}