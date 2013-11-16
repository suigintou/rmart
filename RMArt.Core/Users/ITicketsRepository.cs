namespace RMArt.Core
{
	public interface ITicketsRepository
	{
		void Add(Ticket ticket);
		bool Remove(string token);
		bool Remove(TicketType type, int userID);
		bool IsExists(string token);
		Ticket Get(string token);
	}
}