using RMArt.Core;

namespace RMArt.Web.Models
{
	public class FileUploadResult
	{
		public string Name { get; set; }
		public PictureAddingResult Status { get; set; }
		public int? PictureID { get; set; }
	}
}