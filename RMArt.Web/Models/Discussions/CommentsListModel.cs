using System.Collections.Generic;

namespace RMArt.Web.Models
{
	public class CommentsListModel
	{
		public IList<DiscussionMessageModel> Messages { get; set; }
		public bool WithPreview { get; set; }
		public bool CanModerate { get; set; }
		public int TotalCount { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
	}
}