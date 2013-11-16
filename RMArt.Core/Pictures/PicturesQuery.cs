using System;
using System.Collections.Generic;

namespace RMArt.Core
{
	public class PicturesQuery
	{
		public static readonly PicturesQuery None = new PicturesQuery { AllowedStatuses = new SortedSet<ModerationStatus>() };

		public PicturesSortOrder SortBy { get; set; }
		public int? Skip { get; set; }
		public int? Take { get; set; }
		public Picture After { get; set; }
		public ISet<int> ReqiredTagIDs { get; set; }
		public ISet<int> ExcludedTagIDs { get; set; }
		public int? MinTagsCount { get; set; }
		public int? MaxTagsCount { get; set; }
		public bool? NotTagged { get; set; }
		public ISet<ModerationStatus> AllowedStatuses { get; set; }
		public ISet<Rating> AllowedRatings { get; set; }
		public DateTime? MinDate { get; set; }
		public DateTime? MaxDate { get; set; }
		public PictureOrientation? Orientation { get; set; }
		public int? MinWidth { get; set; }
		public int? MaxWidth { get; set; }
		public int? MinHeight { get; set; }
		public int? MaxHeight { get; set; }
		public int? UploadedBy { get; set; }
		public int? FavoritedBy { get; set; }
		public int? RatedBy { get; set; }
		public byte[] FileHash { get; set; }
		public byte[] BitmapHash { get; set; }
		public int? ID { get; set; }
	}
}