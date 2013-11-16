using System;
using System.Linq;

namespace RMArt.Core
{
	public class HistoryService : IHistoryService
	{
		private readonly IHistoryRepository _historyRepository;

		public HistoryService(IHistoryRepository historyRepository)
		{
			_historyRepository = historyRepository;
		}

		public IQueryable<HistoryEvent> Find(HistoryQuery query = null)
		{
			return _historyRepository.Find(query);
		}

		public void Log(ObjectReference target, HistoryField field, string oldValue, string newValue, Identity identity, string comment)
		{
			var prewEvent = GetPrewEvent(target.Type, target.ID, field, oldValue, newValue);
			var id =
				_historyRepository.Add(
					new HistoryEvent
					{
						TargetType = target.Type,
						TargetID = target.ID,
						Field = field,
						OldValue = oldValue,
						NewValue = newValue,
						UserID = identity.UserID,
						UserIP = identity.IPAddress.GetAddressBytes(),
						Date = DateTime.UtcNow,
						Comment = comment
					});
			if (prewEvent != null)
				_historyRepository.SetNextEventID(prewEvent.ID, id);
		}

		public void DeleteFor(ObjectReference target)
		{
			_historyRepository.Delete(new HistoryQuery { TargetType = target.Type, TargetID = target.ID });
		}

		public void Repair(IProgressIndicator progressIndicator)
		{
			var i = 0;
			var entries = Find().ToArray();
			foreach (var e in entries)
			{
				var prewEvent = GetPrewEvent(e.TargetType, e.TargetID, e.Field, e.OldValue, e.NewValue, e.ID, e.Date);
				if (prewEvent != null)
					_historyRepository.SetNextEventID(prewEvent.ID, e.ID);

				if (i % 1000 == 0)
					progressIndicator.ReportProgress(entries.Length, i);
				i++;
			}
		}

		private HistoryEvent GetPrewEvent(ObjectType targetType, int targetID, HistoryField field, string oldValue, string newValue, int? id = null, DateTime? date = null)
		{
			var events = Find(new HistoryQuery { TargetType = targetType, TargetID = targetID, Field = field, MaxDate = date }).AsEnumerable();

			if (id != null)
				events = events.Where(e => e.ID != id);
			else
				events = events.Where(e => e.NextEventID == null);

			if (field.IsCollection())
				events = events.Where(e => string.Equals(e.NewValue, oldValue ?? "") || string.Equals(e.OldValue, newValue ?? ""));
			return events.FirstOrDefault();
		}
	}
}