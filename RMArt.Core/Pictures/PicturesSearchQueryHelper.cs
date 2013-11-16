using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ParsecSharp;

namespace RMArt.Core
{
	public static class PicturesSearchQueryHelper
	{
		public const string AcceptStatusKey = "status";
		public const string RatingKey = "rating";
		public const string UploaderKey = "uploader";
		public const string FavoritedByKey = "favoritedby";
		public const string StartDateKey = "startdate";
		public const string EndDateKey = "enddate";
		public const string OrientationKey = "orientation";
		public const string MinWidthKey = "minwidth";
		public const string MaxWidthKey = "maxwidth";
		public const string MinHeightKey = "minheight";
		public const string MaxHeightKey = "maxheight";
		public const string MinTagsCountKey = "mintagscount";
		public const string MaxTagsCountKey = "maxtagscount";
		public const string KeyValueDelimeter = ":";
		public const string ListItemsDelimeter = "|";
		public const string ExcludePrefix = "-";

		public static PicturesSearchQuery ParseSearchQuery(string text)
		{
			var res = PicturesSearchQueryParsers.Query(new StringInput(text ?? string.Empty));
			if (res == null)
				throw new ArgumentException("Invalid query.", "text");
			return res.Value;
		}

		public static string ToStringPresentation(this PicturesSearchQuery query, string includeTag = null, string excludeTag = null)
		{
			var sb = new StringBuilder();

			if (query.ReqiredTags != null && query.ReqiredTags.Count > 0)
				foreach (var t in query.ReqiredTags)
					sb.Append(TagsHelper.EncodeTagName(t)).Append(" ");
			if (!string.IsNullOrEmpty(includeTag))
				sb.Append(TagsHelper.EncodeTagName(includeTag)).Append(" ");
			if (query.TagmeRequired)
				sb.Append(TagsHelper.EncodeTagName(TagsHelper.TagmeTagName)).Append(" ");

			if (query.ExcludedTags != null && query.ExcludedTags.Count > 0)
				foreach (var t in query.ExcludedTags)
					sb.Append(ExcludePrefix).Append(TagsHelper.EncodeTagName(t)).Append(" ");
			if (!string.IsNullOrEmpty(excludeTag))
				sb.Append(ExcludePrefix).Append(TagsHelper.EncodeTagName(excludeTag)).Append(" ");
			if (query.TagmeExcuded)
				sb.Append(ExcludePrefix).Append(TagsHelper.EncodeTagName(TagsHelper.TagmeTagName)).Append(" ");

			if (query.MinTagsCount != null)
				sb.Append(MinTagsCountKey).Append(KeyValueDelimeter).Append(query.MinTagsCount).Append(' ');
			if (query.MinTagsCount != null)
				sb.Append(MaxTagsCountKey).Append(KeyValueDelimeter).Append(query.MaxTagsCount).Append(' ');

			if (query.AllowedStatuses != null && query.AllowedStatuses.Any())
				sb.Append(AcceptStatusKey).Append(KeyValueDelimeter).Append(string.Join(ListItemsDelimeter, query.AllowedStatuses)).Append(' ');

			if (query.AllowedRatings != null && query.AllowedRatings.Any())
				sb.Append(RatingKey).Append(KeyValueDelimeter).Append(string.Join(ListItemsDelimeter, query.AllowedRatings)).Append(' ');

			if (query.MinWidth != null)
				sb.Append(MinWidthKey).Append(KeyValueDelimeter).Append(query.MinWidth).Append(' ');
			if (query.MaxWidth != null)
				sb.Append(MaxWidthKey).Append(KeyValueDelimeter).Append(query.MaxWidth).Append(' ');
			if (query.MinHeight != null)
				sb.Append(MinHeightKey).Append(KeyValueDelimeter).Append(query.MinHeight).Append(' ');
			if (query.MaxHeight != null)
				sb.Append(MaxHeightKey).Append(KeyValueDelimeter).Append(query.MaxHeight).Append(' ');

			if (query.Orientation != null)
				sb.Append(OrientationKey).Append(KeyValueDelimeter).Append(query.Orientation).Append(' ');

			if (query.StartDate != null)
				sb.Append(StartDateKey).Append(KeyValueDelimeter).Append(FormatDate(query.StartDate.Value)).Append(' ');
			if (query.EndDate != null)
				sb.Append(EndDateKey).Append(KeyValueDelimeter).Append(FormatDate(query.EndDate.Value)).Append(' ');

			if (!string.IsNullOrEmpty(query.Uploader))
				sb.Append(UploaderKey).Append(KeyValueDelimeter).Append(query.Uploader).Append(' ');
			if (!string.IsNullOrEmpty(query.FavoritedBy))
				sb.Append(FavoritedByKey).Append(KeyValueDelimeter).Append(query.FavoritedBy).Append(' ');

			return sb.ToString().Trim();
		}

		public static PicturesQuery ToPredicate(
			this PicturesSearchQuery query,
			ITagsService tagsService,
			IUsersService usersService)
		{
			if (query.TagmeRequired && query.TagmeExcuded)
				return PicturesQuery.None;

			ISet<int> requiredTagIDs = null;
			if (query.ReqiredTags != null && query.ReqiredTags.Any())
			{
				bool requiredTagNotExists;
				requiredTagIDs = tagsService.GetSearchTagIDs(query.ReqiredTags, out requiredTagNotExists);
				if (requiredTagNotExists)
					return PicturesQuery.None;
			}

			ISet<int> excludedTagIDs = null;
			if (query.ExcludedTags != null && query.ExcludedTags.Any())
			{
				bool excludedTagNotExists;
				excludedTagIDs = tagsService.GetSearchTagIDs(query.ExcludedTags, out excludedTagNotExists);
			}

			if (requiredTagIDs != null && excludedTagIDs != null && requiredTagIDs.Intersect(excludedTagIDs).Any())
				return PicturesQuery.None;

			return
				new PicturesQuery
				{
					ReqiredTagIDs = requiredTagIDs,
					ExcludedTagIDs = excludedTagIDs,
					NotTagged = query.TagmeRequired ? true : query.TagmeExcuded ? false : default(bool?),
					MinTagsCount = query.MinTagsCount,
					MaxTagsCount = query.MaxTagsCount,
					AllowedStatuses = query.AllowedStatuses,
					AllowedRatings = query.AllowedRatings,
					Orientation = query.Orientation,
					MinDate = query.StartDate,
					MaxDate = query.EndDate,
					MinWidth = query.MinWidth,
					MaxWidth = query.MaxWidth,
					MinHeight = query.MinHeight,
					MaxHeight = query.MaxHeight,
					UploadedBy = usersService.GetIDByLogin(query.Uploader),
					FavoritedBy = usersService.GetIDByLogin(query.FavoritedBy)
				};
		}

		public static string FormatDate(DateTime dateTime)
		{
			return dateTime.ToString("dd.MM.yyyy");
		}
	}
}