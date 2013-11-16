using RMArt.Core;
using Thorn;

namespace RMArt.ServiceTool
{
	[ThornExport]
	public class History
	{
		private readonly IHistoryService _historyService;

		public History(IHistoryService historyService)
		{
			_historyService = historyService;
		}

		public void Repair()
		{
			_historyService.Repair(new ConsoleProgressIndicator());
		}
	}
}