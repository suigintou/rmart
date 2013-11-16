using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("TagRelations")]
	public class TagsRelation
	{
		[Column]
		public int ParentID { get; set; }
		
		[Column]
		public int ChildrenID { get; set; }

		[Association(ThisKey = "ParentID", OtherKey = "ID", CanBeNull = false)]
		public Tag Parent { get; set; }

		[Association(ThisKey = "ChildrenID", OtherKey = "ID", CanBeNull = false)]
		public Tag Children { get; set; }
	}
}