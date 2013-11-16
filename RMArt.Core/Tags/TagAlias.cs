using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("TagAliases")]
	public class TagAlias
	{
		[Column, NotNull]
		public string Name { get; set; }

		[Column]
		public int TagID { get; set; }

		[Association(ThisKey = "TagID", OtherKey = "ID", CanBeNull = false)]
		public Tag Tag { get; set; }
	}
}