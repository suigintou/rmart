using System;

namespace RMArt.Core
{
	public class HistoryQuery
	{
		public ObjectType? TargetType { get; set; }
		public int? TargetID { get; set; }
		public HistoryField? Field { get; set; }
		public int? UserID { get; set; }
		public DateTime? MinDate { get; set; }
		public DateTime? MaxDate { get; set; }
		public int? Skip { get; set; }
		public int? Take { get; set; }
	}
}