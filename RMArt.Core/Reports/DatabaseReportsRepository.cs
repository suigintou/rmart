using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class ReportsRepository : IReportsRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public ReportsRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public IEnumerable<Report> Find()
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return
					context
						.Reports()
						.OrderByDescending(_ => _.CreatedAt);
		}

		public int Add(Report report)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return
					Convert.ToInt32(
						context
							.Reports()
							.Value(_ => _.TargetType, report.TargetType != null ? report.TargetType.Value : default(ObjectType?))
							.Value(_ => _.TargetID, report.TargetID != null ? report.TargetID.Value : default(int?))
							.Value(_ => _.ReportType, report.ReportType)
							.Value(_ => _.Message, report.Message)
							.Value(_ => _.CreatedBy, report.CreatedBy)
							.Value(_ => _.CreatedAt, report.CreatedAt)
							.Value(_ => _.CreatorIP, report.CreatorIP)
							.Value(_ => _.IsResolved, report.IsResolved)
							.Value(_ => _.ResolvedBy, report.ResolvedBy)
							.Value(_ => _.ResolvedAt, report.ResolvedAt)
							.Value(_ => _.ResolverIP, report.ResolverIP)
							.InsertWithIdentity());
		}

		public void Update(int id, ReportUpdate update)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			{
				var source = context.Reports().Where(_ => _.ID == id);
				if (update.ResolutionStatus != null)
					source
						.Set(_ => _.IsResolved, update.ResolutionStatus.IsResolved)
						.Set(_ => _.ResolvedBy, update.ResolutionStatus.ResolvedBy)
						.Set(_ => _.ResolvedAt, update.ResolutionStatus.ResolvedAt)
						.Set(_ => _.ResolverIP, update.ResolutionStatus.ResolverIP.GetAddressBytes())
						.Update();
			}
		}
	}
}