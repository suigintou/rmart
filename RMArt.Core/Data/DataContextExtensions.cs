using LinqToDB;

namespace RMArt.Core
{
	internal static class DataContextExtensions
	{
		public static ITable<Picture> Pictures(this IDataContext context)
		{
			return context.GetTable<Picture>();
		}

		public static ITable<Tag> Tags(this IDataContext context)
		{
			return context.GetTable<Tag>();
		}

		public static ITable<TagAlias> TagAliases(this IDataContext context)
		{
			return context.GetTable<TagAlias>();
		}

		public static ITable<TagsRelation> TagRelations(this IDataContext context)
		{
			return context.GetTable<TagsRelation>();
		}

		public static ITable<PictureTagRelation> TagsAssignments(this IDataContext context)
		{
			return context.GetTable<PictureTagRelation>();
		}

		public static ITable<SimilarPictureRelation> SimilarPictures(this IDataContext context)
		{
			return context.GetTable<SimilarPictureRelation>();
		}

		public static ITable<User> Users(this IDataContext context)
		{
			return context.GetTable<User>();
		}

		public static ITable<Rate> Rates(this IDataContext context)
		{
			return context.GetTable<Rate>();
		}

		public static ITable<FavoritesItem> Favorites(this IDataContext context)
		{
			return context.GetTable<FavoritesItem>();
		}

		public static ITable<HistoryEvent> History(this IDataContext context)
		{
			return context.GetTable<HistoryEvent>();
		}

		public static ITable<Report> Reports(this IDataContext context)
		{
			return context.GetTable<Report>();
		}

		public static ITable<DiscussionMessage> Discussions(this IDataContext context)
		{
			return context.GetTable<DiscussionMessage>();
		}
	}
}