using System;
using System.Data;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class DiscussionService : IDiscussionService
	{
		private readonly IDataContextProvider _dataContextProvider;
		private readonly IHistoryService _historyService;

		public DiscussionService(IDataContextProvider dataContextProvider, IHistoryService historyService)
		{
			if (dataContextProvider == null)
				throw new ArgumentNullException("dataContextProvider");
			if (historyService == null)
				throw new ArgumentNullException("historyService");

			_dataContextProvider = dataContextProvider;
			_historyService = historyService;
		}

		public IQueryable<DiscussionMessage> All()
		{
			return _dataContextProvider.CreateDataContext().Discussions();
		}

		public void CreateMessage(ObjectReference parent, string body, Identity identity)
		{
			if (body == null)
				throw new ArgumentNullException("body");
			if (body.Length == 0 || body.Length > 500)
				throw new ArgumentOutOfRangeException("body");
			if (identity == null || identity.UserID == null)
				throw new ArgumentException("Identity can not be empty.", "identity");

			using (var context = _dataContextProvider.CreateDataContext())
				context.Discussions().InsertWithIdentity(
					() =>
					new DiscussionMessage
					{
						ParentType = parent.Type,
						ParentID = parent.ID,
						Body = body,
						CreatedBy = identity.UserID.Value,
						CreatedAt = DateTime.UtcNow,
						CreatorIP = identity.IPAddress.GetAddressBytes(),
						Status = ModerationStatus.Accepted
					});
		}

		public void SetStatus(int id, ModerationStatus status, Identity identity)
		{
			using (var dataContext = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(dataContext))
			{
				transaction.BeginTransaction(IsolationLevel.Serializable);

				var msgQuery = dataContext.Discussions().Where(_ => _.ID == id);
				var msgVal = msgQuery.Single();
				msgQuery.Set(_ => _.Status, status).Update();
				_historyService.LogValueChange(
					new ObjectReference(ObjectType.Message, msgVal.ID),
					HistoryField.Status,
					msgVal.Status,
					status,
					identity,
					msgVal.Body.RestrictLength(199)); //ограничение на размер поля комментария в бд

				transaction.CommitTransaction();
			}
		}

		public void Edit(int id, string newText)
		{
			if (newText == null)
				throw new ArgumentNullException("newText");
			if (newText.Length == 0 || newText.Length > 500)
				throw new ArgumentOutOfRangeException("newText");

			using (var dataContext = _dataContextProvider.CreateDataContext())
				dataContext.Discussions().Where(_ => _.ID == id).Set(_ => _.Body, newText).Update();
		}
	}
}