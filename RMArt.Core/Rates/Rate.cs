using System;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Rates")]
	public class Rate
	{
		[Column]
		public int PictureID { get; set; }

		[Column]
		public int UserID { get; set; }

		[Column]
		public int Score { get; set; }

		[Column]
		public DateTime Date { get; set; }

		[Column, NotNull]
		public byte[] IPAddress { get; set; }

		[Column]
		public bool IsActive { get; set; }

		[Association(ThisKey = "PictureID", OtherKey = "ID", CanBeNull = false)]
		public Picture Picture { get; set; }

		[Association(ThisKey = "UserID", OtherKey = "ID", CanBeNull = false)]
		public User User { get; set; }
	}
}