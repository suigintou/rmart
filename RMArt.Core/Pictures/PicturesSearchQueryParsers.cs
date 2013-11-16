using System;
using System.Collections.Generic;
using System.Linq;
using ParsecSharp;

namespace RMArt.Core
{
	internal class PicturesSearchQueryParsers : CharParsers
	{
		public static Parser<CharInput, object> KeyValue<TValue>(string key, Parser<CharInput, TValue> valueParser)
		{
			return
				from keyRes in WsChar(String(key))
				from delimeter in WsChar(String(PicturesSearchQueryHelper.KeyValueDelimeter))
				from value in WsChar(valueParser)
				select (object)value;
		}

		public static Parser<CharInput, T[]> List<T>(Parser<CharInput, T> itemParser)
		{
			return
				from head in WsChar(itemParser)
				from tail in
					ZeroOrMany(
						from delimeter in WsChar(String(PicturesSearchQueryHelper.ListItemsDelimeter))
						from item in WsChar(itemParser)
						select item)
				select new[] { head }.Concat(tail).ToArray();
		}

		public static readonly Parser<CharInput, object> Status = KeyValue(PicturesSearchQueryHelper.AcceptStatusKey, List(AnyFromEnum<ModerationStatus>()));
		public static readonly Parser<CharInput, object> Rating = KeyValue(PicturesSearchQueryHelper.RatingKey, List(AnyFromEnum<Rating>()));
		public static readonly Parser<CharInput, object> Uploader = KeyValue(PicturesSearchQueryHelper.UploaderKey, OneOrMany(NotChar(WhiteSpace)).Select(cs => new string(cs)));
		public static readonly Parser<CharInput, object> FavoritedBy = KeyValue(PicturesSearchQueryHelper.FavoritedByKey, OneOrMany(NotChar(WhiteSpace)).Select(cs => new string(cs)));
		public static readonly Parser<CharInput, object> Orientation = KeyValue(PicturesSearchQueryHelper.OrientationKey, AnyFromEnum<PictureOrientation>());
		public static readonly Parser<CharInput, object> MinWidth = KeyValue(PicturesSearchQueryHelper.MinWidthKey, Number());
		public static readonly Parser<CharInput, object> MaxWidth = KeyValue(PicturesSearchQueryHelper.MaxWidthKey, Number());
		public static readonly Parser<CharInput, object> MinHeight = KeyValue(PicturesSearchQueryHelper.MinHeightKey, Number());
		public static readonly Parser<CharInput, object> MaxHeight = KeyValue(PicturesSearchQueryHelper.MaxHeightKey, Number());
		public static readonly Parser<CharInput, object> StartDate = KeyValue(PicturesSearchQueryHelper.StartDateKey, DateTimeParsers.DateTime);
		public static readonly Parser<CharInput, object> EndDate = KeyValue(PicturesSearchQueryHelper.EndDateKey, DateTimeParsers.DateTime);
		public static readonly Parser<CharInput, object> MinTagsCount = KeyValue(PicturesSearchQueryHelper.MinTagsCountKey, Number());
		public static readonly Parser<CharInput, object> MaxTagsCount = KeyValue(PicturesSearchQueryHelper.MaxTagsCountKey, Number());

		public static readonly Parser<CharInput, PicturesSearchQuery> Query =
			from res in
				Unordered(
					Status,
					Rating,
					Uploader,
					FavoritedBy,
					Orientation,
					MinWidth,
					MaxWidth,
					MinHeight,
					MaxHeight,
					StartDate,
					EndDate,
					MinTagsCount,
					MaxTagsCount,
					QueryTagsParsers.ExcludedTagmeTag,
					QueryTagsParsers.RequiredTagmeTag,
					QueryTagsParsers.ExcludedQueryTag,
					QueryTagsParsers.QueryTag)
			let allowedStatuses = res[Status].Cast<ModerationStatus[]>().FirstOrDefault()
			let allowedRatings = res[Rating].Cast<Rating[]>().FirstOrDefault()
			select
				new PicturesSearchQuery
				{
					TagmeRequired = res[QueryTagsParsers.RequiredTagmeTag].Any(),
					TagmeExcuded = res[QueryTagsParsers.ExcludedTagmeTag].Any(),
					ReqiredTags = res[QueryTagsParsers.QueryTag].Cast<string>().ToList(),
					ExcludedTags = res[QueryTagsParsers.ExcludedQueryTag].Cast<string>().ToList(),
					AllowedStatuses = allowedStatuses != null ? new SortedSet<ModerationStatus>(allowedStatuses) : null,
					AllowedRatings = allowedRatings != null ? new SortedSet<Rating>(allowedRatings) : null,
					Uploader = res[Uploader].Cast<string>().DefaultIfEmpty().First(),
					FavoritedBy = res[FavoritedBy].Cast<string>().DefaultIfEmpty().First(),
					Orientation = res[Orientation].Cast<PictureOrientation?>().DefaultIfEmpty().First(),
					MinWidth = res[MinWidth].Cast<int?>().DefaultIfEmpty().First(),
					MaxWidth = res[MaxWidth].Cast<int?>().DefaultIfEmpty().First(),
					MinHeight = res[MinHeight].Cast<int?>().DefaultIfEmpty().First(),
					MaxHeight = res[MaxHeight].Cast<int?>().DefaultIfEmpty().First(),
					StartDate = res[StartDate].Cast<DateTime?>().DefaultIfEmpty().First(),
					EndDate = res[EndDate].Cast<DateTime?>().DefaultIfEmpty().First(),
					MinTagsCount = res[MinTagsCount].Cast<int?>().DefaultIfEmpty().First(),
					MaxTagsCount = res[MaxTagsCount].Cast<int?>().DefaultIfEmpty().First(),
				};
	}
}