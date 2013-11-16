using System.ComponentModel.DataAnnotations;

namespace RMArt.Web.Models
{
	public class ContentPageEditModel
	{
		public int? ID { get; set; }

		[Required]
		public string Name { get; set; }

		public int CultureID { get; set; }

		[Required]
		public string Title { get; set; }

		public string Content { get; set; }
	}
}