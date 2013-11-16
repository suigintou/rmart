using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Pictures")]
	public class Picture
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column, NotNull]
		public byte[] FileHash { get; set; }

		[Column, NotNull]
		public byte[] BitmapHash { get; set; }

		[Column]
		public ImageFormat Format { get; set; }

		[Column]
		public int Width { get; set; }

		[Column]
		public int Height { get; set; }

		[Column]
		public int Resolution { get; set; }

		[Column]
		public int FileSize { get; set; }

		[Column]
		public Rating Rating { get; set; }

		[Column]
		public DateTime CreationDate { get; set; }

		[Column]
		public int? CreatorID { get; set; }

		[Column, NotNull]
		public byte[] CreatorIP { get; set; }

		[Column]
		public ModerationStatus Status { get; set; }

		[Column, NotNull]
		public string CachedTagIDs { get; set; }

		[NotColumn]
		public ICollection<int> Tags
		{
			get { return CachedTagIDs.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(); }
		}

		[Column]
		public bool RequiresTagging { get; set; }

		[Column]
		public int Score { get; set; }

		[Column]
		public int RatesCount { get; set; }

		[Column]
		public string Source { get; set; }

		[Association(ThisKey = "CreatorID", OtherKey = "ID", CanBeNull = true)]
		public User Creator { get; set; }

		[Association(ThisKey = "ID", OtherKey = "PictureID")]
		public List<PictureTagRelation> TagRelations { get; set; }

		[Association(ThisKey = "ID", OtherKey = "PictureID")]
		public List<FavoritesItem> Favorites { get; set; }

		[Association(ThisKey = "ID", OtherKey = "PictureID")]
		public List<Rate> Rates { get; set; }

		[Association(ThisKey = "ID", OtherKey = "PictureID")]
		public List<SimilarPictureRelation> SimilarPictures { get; set; }

		[Association(ThisKey = "ID", OtherKey = "SimilarPictureID")]
		public List<SimilarPictureRelation> SimilarFor { get; set; }

		public PictureOrientation Orientation
		{
			get
			{
				if (Width == Height)
					return PictureOrientation.Square;
				else if (Width > Height)
					return PictureOrientation.Landscape;
				else
					return PictureOrientation.Portrait;
			}
		}
	}
}