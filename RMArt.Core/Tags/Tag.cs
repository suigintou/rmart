using System;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Tags")]
	public class Tag : IEquatable<Tag>
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column, NotNull]
		public string Name { get; set; }

		[Column]
		public ModerationStatus Status { get; set; }

		[Column]
		public TagType Type { get; set; }

		[Column]
		public DateTime CreationDate { get; set; }

		[Column]
		public int? CreatorID { get; set; }

		[Column]
		public byte[] CreatorIP { get; set; }

		[Column]
		public int UsageCount { get; set; }

		[Association(ThisKey = "ID", OtherKey = "TagID")]
		public IList<TagAlias> Aliases { get; set; }

		[Association(ThisKey = "ID", OtherKey = "ChildrenID")]
		public IList<TagsRelation> Parents { get; set; }

		[Association(ThisKey = "ID", OtherKey = "ParentID")]
		public IList<TagsRelation> Childrens { get; set; }

		[Association(ThisKey = "ID", OtherKey = "TagID")]
		public IList<PictureTagRelation> PictureRelations { get; set; }

		[NotColumn]
		public ICollection<string> AliasNames { get; set; }

		[NotColumn]
		public ICollection<int> ParentIDs { get; set; }

		[NotColumn]
		public ICollection<int> ChildrenIDs { get; set; }

		public bool Equals(Tag other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			return other.ID == ID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof (Tag))
				return false;
			return Equals((Tag) obj);
		}

		public override int GetHashCode()
		{
			return ID;
		}
	}
}