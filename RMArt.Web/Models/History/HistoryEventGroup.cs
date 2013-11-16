using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class HistoryEventGroup
	{
		public ObjectType TargetType { get; set; }
		public int TargetID { get; set; }
		public IList<HistoryEvent> Changes { get; set; }
		public string Comment { get; set; }
	}
}