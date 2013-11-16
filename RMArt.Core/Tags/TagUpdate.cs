using System.Collections.Generic;

namespace RMArt.Core
{
	public class TagUpdate
	{
		public string Name { get; set; }
		public ModerationStatus? Status { get; set; }
		public TagType? Type { get; set; }
		public ICollection<int> AddParents { get; set; }
		public ICollection<int> RemoveParents { get; set; }
		public ICollection<string> AddAliases { get; set; }
		public ICollection<string> RemoveAliases { get; set; }
	}
}