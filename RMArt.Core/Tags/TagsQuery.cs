using System.Collections.Generic;

namespace RMArt.Core
{
	public class TagsQuery
	{
		public TagsSortOrder SortBy { get; set; }
		public int? Skip { get; set; }
		public int? Take { get; set; }
		public ISet<ModerationStatus> AllowedStatuses { get; set; }
		public string Name { get; set; }
		public string Term { get; set; }
		public TagType? Type { get; set; }
	}
}