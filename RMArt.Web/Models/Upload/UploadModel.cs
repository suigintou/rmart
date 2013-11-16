using System.ComponentModel.DataAnnotations;
using System.Web;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class UploadModel
	{
		public HttpPostedFileBase[] Files { get; set; }

		[DataType(DataType.ImageUrl)]
		public string Url { get; set; }

		[StringLength(500, ErrorMessageResourceType = typeof(PicturesResources), ErrorMessageResourceName = "TagsStringTooLongMessage")]
		public string Tags { get; set; }

		public Rating Rating { get; set; }

		[DataType(DataType.Url)]
		public string Source { get; set; }

		public string Captcha { get; set; }
	}
}