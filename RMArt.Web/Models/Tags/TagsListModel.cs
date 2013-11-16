using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class TagsListModel
	{
		public IList<Tag> Tags { get; set; }
		public string Query { get; set; }
		public TagType? TagType { get; set; }
		public ModerationStatus? Status { get; set; }
		public TagsSortOrder SortBy { get; set; }
		public int TotalCount { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public bool CanEdit { get; set; }
		public bool CanModerate { get; set; }
	}
}