using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class PictureViewerModel
	{
		public Picture Picture { get; set; }
		public IList<TagModel> Tags { get; set; }
		public ThumbnailsListModel SimilarPictureThumbs { get; set; }
		public string Keywords { get; set; }
		public int UserScore { get; set; }
		public bool IsFavorited { get; set; }
		public bool CanView { get; set; }
		public bool CanEdit { get; set; }
		public bool CanModerate { get; set; }
		public bool CanRate { get; set; }
		public bool CanFavorite { get; set; }
		public string PictureUrl { get; set; }
		public string ShareUrl { get; set; }
		public string ShareDirectUrl { get; set; }
	}
}