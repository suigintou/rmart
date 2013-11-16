using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class EditProfileModel
	{
		[Display(Name = "AccountName", ResourceType = typeof(AccountResources))]
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[StringLength(UsersValidation.MaximumNameLength, MinimumLength = UsersValidation.MinimumNameLength)]
		public string Name { get; set; }

		public string Email { get; set; }
	}
}