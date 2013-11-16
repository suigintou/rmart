using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("SimilarPictures")]
	public class SimilarPictureRelation
	{
		[Column]
		public int PictureID { get; set; }

		[Column]
		public int SimilarPictureID { get; set; }

		[Association(ThisKey = "PictureID", OtherKey = "ID", CanBeNull = false)]
		public Picture Picture { get; set; }

		[Association(ThisKey = "SimilarPictureID", OtherKey = "ID", CanBeNull = false)]
		public Picture SimilarPicture { get; set; }
	}
}