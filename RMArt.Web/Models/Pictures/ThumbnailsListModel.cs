using System.Collections.Generic;

namespace RMArt.Web.Models
{
	public class ThumbnailsListModel
	{
		public IList<ThumbModel> Pictures { get; set; }
		public int ThumbSizePreset { get; set; }
		public int ThumbWidth { get; set; }
		public int ThumbHeight { get; set; }
		public bool ShowIcons { get; set; }
	}
}