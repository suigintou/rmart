using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class TicketsRepository : ITicketsRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public TicketsRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public void Add(Ticket ticket)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			{
				context
					.GetTable<Ticket>()
						.Value(_ => _.Type, ticket.Type)
						.Value(_ => _.UserID, ticket.UserID)
						.Value(_ => _.Token, ticket.Token)
						.Value(_ => _.CreationDate, ticket.CreationDate)
						.Value(_ => _.RequestIP, ticket.RequestIP)
					.Insert();
			}
		}

		public bool Remove(string token)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<Ticket>().Delete(_ => _.Token == token) > 0;
		}

		public bool Remove(TicketType type, int userID)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<Ticket>().Delete(_ => _.Type == type && _.UserID == userID) > 0;
		}

		public bool IsExists(string token)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<Ticket>().Any(_ => _.Token == token);
		}

		public Ticket Get(string token)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<Ticket>().SingleOrDefault(_ => _.Token == token);
		}
	}
}