using RMArt.Core;

namespace RMArt.Web.Models
{
	public class ChangeMaxRatingModel
	{
		public Rating MaxRating { get; set; }
		public bool ShowUnrated { get; set; }
		public string ReturnUrl { get; set; }
	}
}