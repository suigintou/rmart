using System.Collections.Generic;

namespace RMArt.Core
{
	public interface IReportsRepository
	{
		IEnumerable<Report> Find();
		int Add(Report report);
		void Update(int id, ReportUpdate update);
	}
}