using System.ComponentModel.DataAnnotations;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class LogInModel
	{
		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		public string LoginOrEmail { get; set; }

		[Required(ErrorMessageResourceName = "FieldRequired", ErrorMessageResourceType = typeof(AccountResources))]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool Remember { get; set; }

		public string ReturnUrl { get; set; }
	}
}