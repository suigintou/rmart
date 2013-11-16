using System;
using System.Net;

namespace RMArt.Core
{
	public class ReportResolutionStatus
	{
		public bool IsResolved { get; set; }
		public int? ResolvedBy { get; set; }
		public DateTime? ResolvedAt { get; set; }
		public IPAddress ResolverIP { get; set; }

		public ReportResolutionStatus()
		{
		}

		public ReportResolutionStatus(bool isResolved, int? resolvedBy, DateTime? resolvedAt, IPAddress resolverIP)
		{
			IsResolved = isResolved;
			ResolvedBy = resolvedBy;
			ResolvedAt = resolvedAt;
			ResolverIP = resolverIP;
		}
	}
}