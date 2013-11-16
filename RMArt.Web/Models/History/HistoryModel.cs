using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class HistoryModel
	{
		public IList<HistoryActionGroup> Groups { get; set; }
		public int CurrentPage { get; set; }
		public bool HasMorePages { get; set; }
		public ObjectType? TargetType { get; set; }
		public int? TargetID { get; set; }
		public HistoryField? ActionType { get; set; }
		public string User { get; set; }
		public bool CanAdminister { get; set; }
	}
}