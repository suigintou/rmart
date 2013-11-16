using System.Collections.Generic;

namespace RMArt.Core
{
	public interface IFavoritesRepository
	{
		void Add(FavoritesItem item);
		int Delete(FavoritesQuery query);
		IEnumerable<FavoritesItem> Find(FavoritesQuery query = null);
		bool IsExists(FavoritesQuery query = null);
	}
}