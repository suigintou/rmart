using System;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	internal static class PictureQueries
	{
		public static IQueryable<Picture> Query(this IQueryable<Picture> source, PicturesQuery query)
		{
			if (query == null)
				return source;

			var predicate = PredicateBuilder.True<Picture>();

			if (query.ID != null)
				predicate = predicate.And(p => p.ID == query.ID);
			if (query.FileHash != null)
				predicate = predicate.And(p => p.FileHash == query.FileHash);
			if (query.BitmapHash != null)
				predicate = predicate.And(p => p.BitmapHash == query.BitmapHash);

			#region Status
			if (query.AllowedStatuses != null)
				predicate = predicate.And(
					query.AllowedStatuses.Aggregate(
						PredicateBuilder.False<Picture>(),
						(pred, s) => pred.Or(p => p.Status == s)));
			#endregion

			#region Rating
			if (query.AllowedRatings != null)
				predicate = predicate.And(
					query.AllowedRatings.Aggregate(
						PredicateBuilder.False<Picture>(),
						(pred, r) => pred.Or(p => p.Rating == r)));
			#endregion

			#region Range filters
			if (query.MinDate != null)
				predicate = predicate.And(p => p.CreationDate >= query.MinDate);
			if (query.MaxDate != null)
				predicate = predicate.And(p => p.CreationDate <= query.MaxDate);

			if (query.MinWidth != null)
				predicate = predicate.And(p => p.Width >= query.MinWidth);
			if (query.MaxWidth != null)
				predicate = predicate.And(p => p.Width <= query.MaxWidth);

			if (query.MinHeight != null)
				predicate = predicate.And(p => p.Height >= query.MinHeight);
			if (query.MaxHeight != null)
				predicate = predicate.And(p => p.Height <= query.MaxHeight);
			#endregion

			#region Tags

			if (query.NotTagged == true)
				predicate = predicate.And(p => p.RequiresTagging || !p.TagRelations.Any());
			else if (query.NotTagged == false)
				predicate = predicate.And(p => !p.RequiresTagging && p.TagRelations.Any());

			if (query.MinTagsCount != null)
				predicate = predicate.And(p => p.TagRelations.Count() >= query.MinTagsCount);
			if (query.MaxTagsCount != null)
				predicate = predicate.And(p => p.TagRelations.Count() <= query.MaxTagsCount);

			if (query.ExcludedTagIDs != null && query.ExcludedTagIDs.Any())
			{
				var excluded = query.ExcludedTagIDs.ToArray();
				predicate = predicate.And(p => !p.TagRelations.Any(r => excluded.Contains(r.TagID)));
			}

			if (query.ReqiredTagIDs != null && query.ReqiredTagIDs.Any())
			{
				//var required = query.ReqiredTagIDs.ToArray();
				//predicate = predicate.And(p => p.TagRelations.Count(r => required.Contains(r.TagID)) == required.Count());
				predicate = query.ReqiredTagIDs.Aggregate(predicate, (pred, tid) => pred.And(p => p.TagRelations.Any(r => r.TagID == tid)));
			}

			#endregion

			#region Orientation
			if (query.Orientation != null)
				switch (query.Orientation)
				{
					case PictureOrientation.Landscape:
						predicate = predicate.And(p => p.Width > p.Height);
						break;
					case PictureOrientation.Portrait:
						predicate = predicate.And(p => p.Width < p.Height);
						break;
					case PictureOrientation.Square:
						predicate = predicate.And(p => p.Width == p.Height);
						break;
				}
			#endregion

			if (query.UploadedBy != null)
				predicate = predicate.And(p => p.CreatorID == query.UploadedBy);
			if (query.FavoritedBy != null)
				predicate = predicate.And(p => p.Favorites.Any(f => f.UserID == query.FavoritedBy));
			if (query.RatedBy != null)
				predicate = predicate.And(p => p.Rates.Any(r => r.UserID == query.RatedBy));

			source = source.Where(predicate);

			if (query.After != null)
				source = source.FilterNext(query.SortBy, query.After);

			source = source.Sort(query.SortBy);

			if (query.Skip != null && query.Skip > 0 && query.SortBy != PicturesSortOrder.Random)
				source = source.Skip(query.Skip.Value);
			if (query.Take != null)
				source = source.Take(query.Take.Value);

			return source;
		}

		private static IQueryable<Picture> FilterNext(this IQueryable<Picture> source, PicturesSortOrder sortBy, Picture current)
		{
			switch (sortBy)
			{
				case PicturesSortOrder.Newest:
					return source.Where(
						p => p.CreationDate < current.CreationDate
							|| (p.CreationDate == current.CreationDate && p.ID < current.ID));
				case PicturesSortOrder.Oldest:
					return source.Where(
						p => p.CreationDate > current.CreationDate
							|| (p.CreationDate == current.CreationDate && p.ID > current.ID));
				case PicturesSortOrder.TopRated:
					return source.Where(
						p => p.Score < current.Score
							|| (p.Score == current.Score && (p.RatesCount < current.RatesCount
							|| (p.RatesCount == current.RatesCount && (p.CreationDate < current.CreationDate
							|| (p.CreationDate == current.CreationDate && p.ID < current.ID))))));
				case PicturesSortOrder.LowRated:
					return source.Where(
						p => p.Score > current.Score
							|| (p.Score == current.Score && (p.RatesCount > current.RatesCount
							|| (p.RatesCount == current.RatesCount && (p.CreationDate > current.CreationDate
							|| (p.CreationDate == current.CreationDate && p.ID > current.ID))))));
				case PicturesSortOrder.HighestResolution:
					return source.Where(
						p => p.Resolution < current.Resolution
							|| (p.Resolution == current.Resolution && (p.CreationDate < current.CreationDate
							|| (p.CreationDate == current.CreationDate && p.ID < current.ID))));
				case PicturesSortOrder.LowestResolution:
					return source.Where(
						p => p.Resolution > current.Resolution
							|| (p.Resolution == current.Resolution && (p.CreationDate > current.CreationDate
							|| (p.CreationDate == current.CreationDate && p.ID > current.ID))));
				case PicturesSortOrder.Random:
					return source;
				default:
					throw new NotSupportedException();
			}
		}

		private static IQueryable<Picture> Sort(this IQueryable<Picture> source, PicturesSortOrder sortBy)
		{
			switch (sortBy)
			{
				case PicturesSortOrder.None:
					return source;

				case PicturesSortOrder.Newest:
					return
						source
							.OrderByDescending(p => p.CreationDate)
							.ThenByDescending(p => p.ID);

				case PicturesSortOrder.Oldest:
					return
						source
							.OrderBy(p => p.CreationDate)
							.ThenBy(p => p.ID);

				case PicturesSortOrder.TopRated:
					return
						source
							.OrderByDescending(p => p.Score)
							.ThenByDescending(p => p.RatesCount)
							.ThenByDescending(p => p.CreationDate)
							.ThenByDescending(p => p.ID);

				case PicturesSortOrder.LowRated:
					return
						source
							.OrderBy(p => p.Score)
							.ThenBy(p => p.RatesCount)
							.ThenBy(p => p.CreationDate)
							.ThenBy(p => p.ID);

				case PicturesSortOrder.HighestResolution:
					return
						source
							.OrderByDescending(p => p.Resolution)
							.ThenByDescending(p => p.CreationDate)
							.ThenByDescending(p => p.ID);

				case PicturesSortOrder.LowestResolution:
					return
						source
							.OrderBy(p => p.Resolution)
							.ThenBy(p => p.CreationDate)
							.ThenBy(p => p.ID);

				case PicturesSortOrder.Random:
					return source.OrderBy(p => Sql.NewGuid());

				default:
					throw new NotSupportedException();
			}
		}
	}
}