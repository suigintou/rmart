using System;
using System.Collections.Generic;
using System.Linq;

namespace RMArt.Core.UnitTests.Users
{
	public class TestTicketsRepository : ITicketsRepository
	{
		private readonly Dictionary<string, Ticket> _tickets = new Dictionary<string, Ticket>(StringComparer.Ordinal);

		public void Add(Ticket ticket)
		{
			_tickets.Add(ticket.Token, ticket);
		}

		public bool Remove(string token)
		{
			return _tickets.Remove(token);
		}

		public bool Remove(TicketType type, int userID)
		{
			return
				_tickets
					.Where(_ => _.Value.Type == type && _.Value.UserID == userID)
					.Select(_ => _.Key)
					.Aggregate(false, (isRemoved, token) => isRemoved | Remove(token));
		}

		public bool IsExists(string token)
		{
			return _tickets.ContainsKey(token);
		}

		public Ticket Get(string token)
		{
			Ticket res;
			return _tickets.TryGetValue(token, out res) ? res : null;
		}
	}
}