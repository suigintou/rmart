using System;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Favorites")]
	public class FavoritesItem
	{
		[Column]
		public int UserID { get; set; }

		[Column]
		public int PictureID { get; set; }

		[Column]
		public DateTime Date { get; set; }

		[Association(ThisKey = "UserID", OtherKey = "ID", CanBeNull = false)]
		public User User { get; set; }

		[Association(ThisKey = "PictureID", OtherKey = "ID", CanBeNull = false)]
		public Picture Picture { get; set; }
	}
}