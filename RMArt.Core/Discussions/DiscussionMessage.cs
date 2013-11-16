using System;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Discussions")]
	public class DiscussionMessage
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column]
		public ObjectType ParentType { get; set; }

		[Column]
		public int ParentID { get; set; }

		[Column, NotNull]
		public string Body { get; set; }

		[Column]
		public int CreatedBy { get; set; }

		[Column, NotNull]
		public byte[] CreatorIP { get; set; }

		[Column]
		public DateTime CreatedAt { get; set; }

		[Column]
		public ModerationStatus Status { get; set; }

		[Association(ThisKey = "CreatedBy", OtherKey = "ID", CanBeNull = false)]
		public User Creator { get; set; }
	}
}