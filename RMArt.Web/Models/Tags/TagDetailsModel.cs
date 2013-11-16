using RMArt.Core;

namespace RMArt.Web.Models
{
	public class TagDetailsModel
	{
		public Tag Tag { get; set; }
		public bool CanEdit { get; set; }
		public bool CanModerate { get; set; }
		public bool CanAdminister { get; set; }
		public ThumbnailsListModel SamplePictures { get; set; }
	}
}