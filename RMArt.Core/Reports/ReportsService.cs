using System;
using System.Collections.Generic;

namespace RMArt.Core
{
	public class ReportsService : IReportsService
	{
		private readonly IReportsRepository _reportsRepository;

		public ReportsService(IReportsRepository reportsRepository)
		{
			_reportsRepository = reportsRepository;
		}

		public IEnumerable<Report> Find()
		{
			return _reportsRepository.Find();
		}

		public void Create(ObjectReference? target, ReportType reportType, string message, Identity identity)
		{
			_reportsRepository.Add(
				new Report
				{
					TargetType = target.HasValue ? target.Value.Type : default(ObjectType?),
					TargetID = target.HasValue ? target.Value.ID : default(int?),
					ReportType = reportType,
					Message = message ?? "",
					CreatedAt = DateTime.UtcNow,
					CreatedBy = identity.UserID,
					CreatorIP = identity.IPAddress.GetAddressBytes()
				});
		}

		public void Resolve(int id, Identity identity)
		{
			_reportsRepository.Update(
				id,
				new ReportUpdate
				{
					ResolutionStatus =
						new ReportResolutionStatus
						{
							IsResolved = true,
							ResolvedBy = identity.UserID,
							ResolvedAt = DateTime.UtcNow,
							ResolverIP = identity.IPAddress
						}
				});
		}
	}
}