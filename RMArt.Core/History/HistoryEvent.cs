using System;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("History")]
	public class HistoryEvent
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column]
		public ObjectType TargetType { get; set; }

		[Column]
		public int TargetID { get; set; }

		[Column]
		public HistoryField Field { get; set; }

		[Column]
		public string OldValue { get; set; }

		[Column]
		public string NewValue { get; set; }

		[Column]
		public DateTime Date { get; set; }

		[Column]
		public int? UserID { get; set; }

		[Column, NotNull]
		public byte[] UserIP { get; set; }

		[Association(ThisKey = "UserID", OtherKey = "ID", CanBeNull = true)]
		public User User { get; set; }

		[Column]
		public string Comment { get; set; }

		[Column]
		public int? NextEventID { get; set; }
	}
}