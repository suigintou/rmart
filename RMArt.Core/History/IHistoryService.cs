using System.Linq;

namespace RMArt.Core
{
	public interface IHistoryService
	{
		IQueryable<HistoryEvent> Find(HistoryQuery query = null);
		void Log(ObjectReference target, HistoryField field, string oldValue, string newValue, Identity identity, string comment);
		void DeleteFor(ObjectReference target);
		void Repair(IProgressIndicator progressIndicator);
	}
}