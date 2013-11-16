using System.Net;

namespace RMArt.Core
{
	public interface ITicketsService
	{
		Ticket Create(TicketType type, int userID, IPAddress requestIP);
		void Expire(string token);
		void Expire(TicketType type, int userID);
		int Check(TicketType type, string token);
	}
}