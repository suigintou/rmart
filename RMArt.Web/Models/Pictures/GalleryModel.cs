using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class GalleryModel
	{
		public ThumbnailsListModel ThumbnailsListModel { get; set; }
		public IList<TagModel> RelatedTags { get; set; }
		public string Query { get; set; }
		public PicturesSortOrder SortBy { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public Rating MaxRating { get; set; }
		public bool ShowUnrated { get; set; }
		public int TotalCount { get; set; }
		public bool CanEdit { get; set; }
		public bool CanModerate { get; set; }
	}
}