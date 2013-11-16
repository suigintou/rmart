using System;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Tickets")]
	public class Ticket
	{
		[Column]
		public int UserID { get; set; }

		[Association(ThisKey = "UserID", OtherKey = "ID", CanBeNull = false)]
		public User User { get; set; }

		[Column]
		public TicketType Type { get; set; }

		[Column, NotNull]
		public string Token { get; set; }

		[Column]
		public DateTime CreationDate { get; set; }

		[Column, NotNull]
		public byte[] RequestIP { get; set; }
	}
}