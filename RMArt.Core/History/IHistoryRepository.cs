using System.Linq;

namespace RMArt.Core
{
	public interface IHistoryRepository
	{
		int Add(HistoryEvent historyEvent);
		void SetNextEventID(int id, int? nextID);
		void Delete(HistoryQuery query);
		IQueryable<HistoryEvent> Find(HistoryQuery query = null);
	}
}