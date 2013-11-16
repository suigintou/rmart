namespace RMArt.Core
{
	public class FavoritesQuery
	{
		public int? UserID { get; set; }
		public int? PictureID { get; set; }

		public FavoritesQuery(int? userID = null, int? pictureID = null)
		{
			UserID = userID;
			PictureID = pictureID;
		}
	}
}