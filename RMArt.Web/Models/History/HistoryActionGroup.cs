using System;
using System.Collections.Generic;
using System.Net;

namespace RMArt.Web.Models
{
	public class HistoryActionGroup
	{
		public int? UserID { get; set; }
		public IPAddress UserIP { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public IList<HistoryEventGroup> Actions { get; set; }
	}
}