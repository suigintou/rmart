using System.Collections.Generic;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class DatabaseFavoritesRepository : IFavoritesRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public DatabaseFavoritesRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public void Add(FavoritesItem item)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Favorites()
					.Value(_ => _.PictureID, item.PictureID)
					.Value(_ => _.UserID, item.UserID)
					.Value(_ => _.Date, item.Date)
					.Insert();
		}

		public int Delete(FavoritesQuery query)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Favorites(), query).Delete();
		}

		public IEnumerable<FavoritesItem> Find(FavoritesQuery query = null)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Favorites(), query).ToArray();
		}

		public bool IsExists(FavoritesQuery query)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Favorites(), query).Any();
		}

		private static IQueryable<FavoritesItem> Filter(IQueryable<FavoritesItem> source, FavoritesQuery query)
		{
			if (query == null)
				return source;

			var predicate = PredicateBuilder.True<FavoritesItem>();

			if (query.UserID != null)
				predicate = predicate.And(_ => _.UserID == query.UserID);
			if (query.PictureID != null)
				predicate = predicate.And(_ => _.PictureID == query.PictureID);

			return source.Where(predicate);
		}
	}
}