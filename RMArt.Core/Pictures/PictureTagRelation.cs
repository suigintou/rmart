using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("PictureTagRelations")]
	public class PictureTagRelation
	{
		[Column]
		public int PictureID { get; set; }

		[Column]
		public int TagID { get; set; }

		[Association(ThisKey = "PictureID", OtherKey = "ID", CanBeNull = false)]
		public Picture Picture { get; set; }

		[Association(ThisKey = "TagID", OtherKey = "ID", CanBeNull = false)]
		public Tag Tag { get; set; }
	}
}