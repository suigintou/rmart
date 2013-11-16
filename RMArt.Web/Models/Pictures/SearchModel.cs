using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class SearchModel
	{
		[Display(Name = "RequiredTags", ResourceType = typeof(SearchResources)), StringLength(200)]
		public string ReqiredTags { get; set; }

		[Display(Name = "ExcludedTags", ResourceType = typeof(SearchResources)), StringLength(200)]
		public string ExcldedTags { get; set; }

		[Display(Name = "StartDate", ResourceType = typeof(SearchResources)), DataType(DataType.DateTime)]
		public DateTime? StartDate { get; set; }

		[Display(Name = "EndDate", ResourceType = typeof(SearchResources)), DataType(DataType.DateTime)]
		public DateTime? EndDate { get; set; }

		public ICollection<ModerationStatus> AllowedStatuses { get; set; }

		public ICollection<Rating> AllowedRatings { get; set; }

		[Display(Name = "SortBy", ResourceType = typeof(SearchResources))]
		public PicturesSortOrder SortBy { get; set; }

		[Display(Name = "MinWidth", ResourceType = typeof(SearchResources)), Range(1, int.MaxValue)]
		public int? MinWidth { get; set; }

		[Display(Name = "MaxWidth", ResourceType = typeof(SearchResources)), Range(1, int.MaxValue)]
		public int? MaxWidth { get; set; }

		[Display(Name = "MinHeight", ResourceType = typeof(SearchResources)), Range(1, int.MaxValue)]
		public int? MinHeight { get; set; }

		[Display(Name = "MaxHeight", ResourceType = typeof(SearchResources)), Range(1, int.MaxValue)]
		public int? MaxHeight { get; set; }

		[Display(Name = "Orientation", ResourceType = typeof(SearchResources))]
		public PictureOrientation? Orientation { get; set; }

		[Display(Name = "Uploader", ResourceType = typeof(SearchResources))]
		public string Uploader { get; set; }

		[Display(Name = "FavoritedBy", ResourceType = typeof(SearchResources))]
		public string FavoritedBy { get; set; }
	}
}