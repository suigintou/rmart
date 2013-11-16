using System.Collections.Generic;

namespace RMArt.Core
{
	public class PictureUpdate
	{
		public ModerationStatus? Status { get; set; }
		public Rating? Rating { get; set; }
		public string Source { get; set; }
		public ICollection<int> AddTags { get; set; }
		public ICollection<int> RemoveTags { get; set; }
		public bool? RequiresTagging { get; set; }
	}
}