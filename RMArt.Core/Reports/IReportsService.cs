using System.Collections.Generic;

namespace RMArt.Core
{
	public interface IReportsService
	{
		IEnumerable<Report> Find();
		void Create(ObjectReference? target, ReportType reportType, string message, Identity identity);
		void Resolve(int id, Identity identity);
	}
}