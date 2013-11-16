using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToDB;

namespace RMArt.Core
{
	public class DatabaseHistoryRepository : IHistoryRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public DatabaseHistoryRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public int Add(HistoryEvent historyEvent)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return
					Convert.ToInt32(
						context
							.History()
							.Value(_ => _.TargetType, historyEvent.TargetType)
							.Value(_ => _.TargetID, historyEvent.TargetID)
							.Value(_ => _.Field, historyEvent.Field)
							.Value(_ => _.OldValue, historyEvent.OldValue)
							.Value(_ => _.NewValue, historyEvent.NewValue)
							.Value(_ => _.Date, historyEvent.Date)
							.Value(_ => _.UserID, historyEvent.UserID)
							.Value(_ => _.UserIP, historyEvent.UserIP)
							.Value(_ => _.Comment, historyEvent.Comment)
							.Value(_ => _.NextEventID, historyEvent.NextEventID)
							.InsertWithIdentity());
		}

		public void SetNextEventID(int id, int? nextID)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context.History().Where(_ => _.ID == id).Set(_ => _.NextEventID, nextID).Update();
		}

		public void Delete(HistoryQuery query)
		{
			if (query != null && (query.Skip != null || query.Take != null))
				throw new NotSupportedException();
			using (var context = _dataContextProvider.CreateDataContext())
				context.History().Delete(MakePredicate(query));
		}

		public IQueryable<HistoryEvent> Find(HistoryQuery query)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			{
				var source = context.History().Where(MakePredicate(query));
				source = source.OrderByDescending(_ => _.Date).ThenByDescending(_ => _.ID);
				if (query != null)
				{
					if (query.Skip != null)
						source = source.Skip(query.Skip.Value);
					if (query.Take != null)
						source = source.Take(query.Take.Value);
				}
				return source;
			}
		}

		private static Expression<Func<HistoryEvent, bool>> MakePredicate(HistoryQuery query)
		{
			var predicate = PredicateBuilder.True<HistoryEvent>();

			if (query == null)
				return predicate;

			if (query.TargetType != null)
				predicate = predicate.And(_ => _.TargetType == query.TargetType);
			if (query.TargetID != null)
				predicate = predicate.And(_ => _.TargetID == query.TargetID);
			if (query.Field != null)
				predicate = predicate.And(_ => _.Field == query.Field);
			if (query.UserID != null)
				predicate = predicate.And(_ => _.UserID == query.UserID);
			if (query.MinDate != null)
				predicate = predicate.And(_ => _.Date >= query.MinDate);
			if (query.MaxDate != null)
				predicate = predicate.And(_ => _.Date <= query.MaxDate);

			return predicate;
		}
	}
}