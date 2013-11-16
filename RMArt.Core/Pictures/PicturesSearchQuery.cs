using System;
using System.Collections.Generic;

namespace RMArt.Core
{
	public class PicturesSearchQuery
	{
		public IList<string> ReqiredTags { get; set; }
		public IList<string> ExcludedTags { get; set; }
		public bool TagmeRequired { get; set; }
		public bool TagmeExcuded { get; set; }
		public int? MinTagsCount { get; set; }
		public int? MaxTagsCount { get; set; }
		public ISet<ModerationStatus> AllowedStatuses { get; set; }
		public ISet<Rating> AllowedRatings { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public PictureOrientation? Orientation { get; set; }
		public int? MinWidth { get; set; }
		public int? MaxWidth { get; set; }
		public int? MinHeight { get; set; }
		public int? MaxHeight { get; set; }
		public string Uploader { get; set; }
		public string FavoritedBy { get; set; }
	}
}