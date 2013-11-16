using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;
using RMArt.Web.Resources;

namespace RMArt.Web
{
	public static class VisualizationHelper
	{
		public const string DateFormat = "yyyy-MM-dd";
		public const string TimeFormat = "HH\\:mm";
		public const string DateTimeFormat = DateFormat + " " + TimeFormat;
		public const string ISODateTimeFormat = "yyyy-MM-ddTHH\\:mm\\:ssZ";

		private static readonly string _pictureDescriptionTemplate =
			string.Concat(
				"{0}, {1}x{2} @ {3} \n",
				PicturesResources.RatingLabel, " {4} ", PicturesResources.MetadataScore, " {5} \n",
				PicturesResources.MetadataTags, " {6}");

		public static string TagSize(int count, int minCount, int maxCount, double minSize = 1, double maxSize = 5, bool logarithmic = true)
		{
			double size;
			if (logarithmic)
			{
				var constant = Math.Log(maxCount - (minCount - 1)) / ((maxSize - minSize) == 0 ? 1 : (maxSize - minSize));
				size = Math.Log(count - (minCount - 1)) / constant + minSize;
			}
			else
				size = minSize + (((maxSize - minSize) * (count - minCount)) / (maxCount - minCount));

			return size.ToString("0.##", CultureInfo.InvariantCulture) + "em";
		}

		public static string CultureName(int id)
		{
			return CultureInfo.GetCultureInfo(id).EnglishName;
		}

		public static string Score(int score, int ratesCount)
		{
			return ratesCount > 0 ? String.Format("{0} ({1})", score, ratesCount) : score.ToString();
		}

		public static string GetUploadResultMessage(this PictureAddingResult result)
		{
			switch (result)
			{
				case PictureAddingResult.Added:
				case PictureAddingResult.Resurrected:
					return UploadResources.SuccessUploadMessage;
				case PictureAddingResult.AlreadyExists:
					return UploadResources.AlreadyExistsMessage;
				case PictureAddingResult.FileTooBig:
					return UploadResources.FileTooBig;
				case PictureAddingResult.InvalidData:
					return UploadResources.InvalidDataMessage;
				case PictureAddingResult.NotSupportedFormat:
					return UploadResources.NotSupportedFormatMessage;
				default:
					return UploadResources.UnknownUploadResultMessage;
			}
		}

		public static string GetDisplayName(this PicturesSortOrder sortOrder)
		{
			switch (sortOrder)
			{
				case PicturesSortOrder.Newest:
					return PicturesResources.SortingNewest;
				case PicturesSortOrder.Oldest:
					return PicturesResources.SortingOldest;
				case PicturesSortOrder.HighestResolution:
					return PicturesResources.SortingHighestResolution;
				case PicturesSortOrder.LowestResolution:
					return PicturesResources.SortingLowestResolution;
				case PicturesSortOrder.TopRated:
					return PicturesResources.SortingTopRated;
				case PicturesSortOrder.Random:
					return PicturesResources.SortingRandom;
				default:
					throw new NotSupportedException();
			}
		}

		public static SelectList SortBySelectList(PicturesSortOrder selected)
		{
			var items =
				new[]
				{
					new SelectListItem { Text = PicturesSortOrder.Newest.GetDisplayName(), Value = PicturesSortOrder.Newest.ToString() },
					new SelectListItem { Text = PicturesSortOrder.Oldest.GetDisplayName(), Value = PicturesSortOrder.Oldest.ToString() },
					new SelectListItem { Text =PicturesSortOrder.TopRated.GetDisplayName(), Value = PicturesSortOrder.TopRated.ToString() },
					new SelectListItem { Text = PicturesSortOrder.HighestResolution.GetDisplayName(), Value = PicturesSortOrder.HighestResolution.ToString() },
					new SelectListItem { Text = PicturesSortOrder.LowestResolution.GetDisplayName(), Value = PicturesSortOrder.LowestResolution.ToString() },
					new SelectListItem { Text = PicturesSortOrder.Random.GetDisplayName(), Value = PicturesSortOrder.Random.ToString() }
				};

			return new SelectList(items, "Value", "Text", selected);
		}

		public static SelectList SupportedCulturesList()
		{
			return new SelectList(GlobalizationHelper.AvailableCultures, "LCID", "EnglishName");
		}

		public static string GetDisplayName(this ModerationStatus status)
		{
			switch (status)
			{
				case ModerationStatus.Pending:
					return PicturesResources.StatusPending;
				case ModerationStatus.Accepted:
					return PicturesResources.StatusAccepted;
				case ModerationStatus.Declined:
					return PicturesResources.StatusDeclined;
				default:
					throw new NotSupportedException();
			}
		}

		public static IEnumerable<SelectListItem> ModerationStatusesList(bool showEmpty, string emptyTitle)
		{
			if (showEmpty)
				yield return new SelectListItem { Text = emptyTitle, Value = null };
			yield return new SelectListItem { Text = ModerationStatus.Accepted.GetDisplayName(), Value = ModerationStatus.Accepted.ToString() };
			yield return new SelectListItem { Text = ModerationStatus.Pending.GetDisplayName(), Value = ModerationStatus.Pending.ToString() };
			yield return new SelectListItem { Text = ModerationStatus.Declined.GetDisplayName(), Value = ModerationStatus.Declined.ToString() };
		}

		public static SelectList ModerationStatusesList(bool showEmpty, string emptyTitle, ModerationStatus? selected)
		{
			return new SelectList(ModerationStatusesList(showEmpty, emptyTitle), "Value", "Text", selected);
		}

		public static string GetDisplayName(this Rating rating)
		{
			switch (rating)
			{
				case Rating.Unrated:
					return PicturesResources.RatingUnrated;
				case Rating.SFW:
					return PicturesResources.RatingSFW;
				case Rating.R15:
					return PicturesResources.RatingR15;
				case Rating.R18:
					return PicturesResources.RatingR18;
				case Rating.R18G:
					return PicturesResources.RatingR18G;
				default:
					throw new NotSupportedException();
			}
		}

		public static IEnumerable<SelectListItem> RatingsList(bool showUnated, bool showEmpty, string emptyTitle)
		{
			if (showEmpty)
				yield return new SelectListItem { Text = emptyTitle, Value = null };
			if (showUnated)
				yield return new SelectListItem { Text = Rating.Unrated.GetDisplayName(), Value = Rating.Unrated.ToString() };
			yield return new SelectListItem { Text = Rating.SFW.GetDisplayName(), Value = Rating.SFW.ToString() };
			yield return new SelectListItem { Text = Rating.R15.GetDisplayName(), Value = Rating.R15.ToString() };
			yield return new SelectListItem { Text = Rating.R18.GetDisplayName(), Value = Rating.R18.ToString() };
			yield return new SelectListItem { Text = Rating.R18G.GetDisplayName(), Value = Rating.R18G.ToString() };
		}

		public static SelectList RatingsList(bool showUnated, bool showEmpty, string emptyTitle, Rating? selected)
		{
			return new SelectList(RatingsList(showUnated, showEmpty, emptyTitle), "Value", "Text", selected);
		}

		public static string GetDisplayName(this TagType tagType)
		{
			switch (tagType)
			{
				case TagType.General:
					return TagsResources.TypeGeneral;
				case TagType.Copyright:
					return TagsResources.TypeCopyright;
				case TagType.Character:
					return TagsResources.TypeCharacter;
				case TagType.Artist:
					return TagsResources.TypeArtist;
				default:
					throw new NotSupportedException();
			}
		}

		public static IEnumerable<SelectListItem> TagTypesList(string emptyTitle = null)
		{
			if (emptyTitle != null)
				yield return new SelectListItem { Text = emptyTitle, Value = "" };

			yield return new SelectListItem { Text = TagType.General.GetDisplayName(), Value = TagType.General.ToString() };
			yield return new SelectListItem { Text = TagType.Copyright.GetDisplayName(), Value = TagType.Copyright.ToString() };
			yield return new SelectListItem { Text = TagType.Character.GetDisplayName(), Value = TagType.Character.ToString() };
			yield return new SelectListItem { Text = TagType.Artist.GetDisplayName(), Value = TagType.Artist.ToString() };
		}

		public static SelectList TagTypesList(TagType? selected, string emptyTitle = null)
		{
			return new SelectList(TagTypesList(emptyTitle), "Value", "Text", selected);
		}

		public static string GetDisplayName(this TagsSortOrder sortOrder)
		{
			switch (sortOrder)
			{
				case TagsSortOrder.NameAsc:
					return TagsResources.SortOrderNameAsc;
				case TagsSortOrder.NameDesc:
					return TagsResources.SortOrderNameDesc;
				case TagsSortOrder.MostUsed:
					return TagsResources.SortOrderMostUsed;
				case TagsSortOrder.LeastUsed:
					return TagsResources.SortOrderLeastUsed;
				case TagsSortOrder.Newest:
					return TagsResources.SortOrderNewest;
				case TagsSortOrder.Oldest:
					return TagsResources.SortOrderOldest;
				default:
					throw new NotSupportedException();
			}
		}

		public static SelectList TagsSortOrderList(TagsSortOrder selected)
		{
			var items =
				new[]
				{
					new SelectListItem { Text = TagsSortOrder.NameAsc.GetDisplayName(), Value = TagsSortOrder.NameAsc.ToString() },
					new SelectListItem { Text = TagsSortOrder.NameDesc.GetDisplayName(), Value = TagsSortOrder.NameDesc.ToString() },
					new SelectListItem { Text = TagsSortOrder.MostUsed.GetDisplayName(), Value = TagsSortOrder.MostUsed.ToString() },
					new SelectListItem { Text = TagsSortOrder.LeastUsed.GetDisplayName(), Value = TagsSortOrder.LeastUsed.ToString() },
					new SelectListItem { Text = TagsSortOrder.Newest.GetDisplayName(), Value = TagsSortOrder.Newest.ToString() },
					new SelectListItem { Text = TagsSortOrder.Oldest.GetDisplayName(), Value = TagsSortOrder.Oldest.ToString() }
				};

			return new SelectList(items, "Value", "Text", selected);
		}

		public static string GetDisplayName(this UserRole role)
		{
			return role.ToString();
		}

		public static IEnumerable<SelectListItem> UserRolesList(bool showEmpty, string emptyTitle)
		{
			if (showEmpty)
				yield return new SelectListItem { Text = emptyTitle, Value = null };
			yield return new SelectListItem { Text = UserRole.Guest.GetDisplayName(), Value = UserRole.Guest.ToString() };
			yield return new SelectListItem { Text = UserRole.User.GetDisplayName(), Value = UserRole.User.ToString() };
			yield return new SelectListItem { Text = UserRole.Contributor.GetDisplayName(), Value = UserRole.Contributor.ToString() };
			yield return new SelectListItem { Text = UserRole.Moderator.GetDisplayName(), Value = UserRole.Moderator.ToString() };
			yield return new SelectListItem { Text = UserRole.Administrator.GetDisplayName(), Value = UserRole.Administrator.ToString() };
		}

		public static string GetDisplayName(this ReportType reportType)
		{
			switch (reportType)
			{
				case ReportType.Feedback:
					return ReportsResources.TypeFeedback;
				case ReportType.IncorrectData:
					return ReportsResources.TypeInvalidData;
				case ReportType.RulesViolation:
					return ReportsResources.TypeRulesViolation;
				case ReportType.Duplicate:
					return ReportsResources.TypeDuplicate;
				case ReportType.Other:
					return ReportsResources.TypeOther;
				default:
					throw new NotSupportedException();
			}
		}

		public static IEnumerable<SelectListItem> ReportTypesList()
		{
			return new[]
			{
				new SelectListItem { Text = ReportType.IncorrectData.GetDisplayName(), Value = ReportType.IncorrectData.ToString() },
				new SelectListItem { Text = ReportType.RulesViolation.GetDisplayName(), Value = ReportType.RulesViolation.ToString() },
				new SelectListItem { Text = ReportType.Duplicate.GetDisplayName(), Value = ReportType.Duplicate.ToString() },
				new SelectListItem { Text = ReportType.Other.GetDisplayName(), Value = ReportType.Other.ToString() }
			};
		}

		public static string GetDisplayName(this ObjectType objectType)
		{
			switch (objectType)
			{
				case ObjectType.Picture:
					return SharedResources.ObjectPicture;
				case ObjectType.Tag:
					return SharedResources.ObjectTag;
				case ObjectType.Message:
					return SharedResources.ObjectMessage;
				default:
					throw new NotSupportedException();
			}
		}

		public static string GetDisplayName(this PictureOrientation orientation)
		{
			switch (orientation)
			{
				case PictureOrientation.Landscape:
					return PicturesResources.OrientationLandscape;
				case PictureOrientation.Portrait:
					return PicturesResources.OrientationPortrait;
				case PictureOrientation.Square:
					return PicturesResources.OrientationSquare;
				default:
					throw new NotSupportedException();
			}
		}

		public static IOrderedEnumerable<Tag> DefaultOrder(this IEnumerable<Tag> source)
		{
			return
				source
					.OrderBy(
						t =>
						{
							switch (t.Type)
							{
								case TagType.Copyright:
									return 0;
								case TagType.Character:
									return 1;
								case TagType.Artist:
									return 2;
								case TagType.General:
									return 3;
								default:
									throw new NotSupportedException();
							}
						})
					.ThenBy(t => t.Name);
		}

		public static string TagDescription(this Tag tag, ITagsService tagsService)
		{
			const string keyValueSeparator = ": ";
			const string parametersSeparator = " \n";
			return
				string.Concat(
					"\"", tag.Name, "\"", parametersSeparator,
					TagsResources.TypeField, keyValueSeparator, tag.Type.GetDisplayName(), parametersSeparator,
					tag.AliasNames.Any() ? string.Concat(TagsResources.AliasesField, keyValueSeparator, string.Join(", ", tag.AliasNames), parametersSeparator) : "",
					tag.ParentIDs.Any() ? string.Concat(TagsResources.ParentsField, keyValueSeparator, string.Join(", ", tag.ParentIDs.Select(p => tagsService.LoadTag(p).Name)), parametersSeparator) : "",
					TagsResources.CountField, keyValueSeparator, tag.UsageCount);
		}

		public static string PictureTitle(this Picture picture, string tagsString)
		{
			var sb = new StringBuilder();

			if (picture.Rating != Rating.SFW)
				sb.Append("[").Append(picture.Rating.GetDisplayName()).Append("] ");

			sb.Append("#").Append(picture.ID);

			if (tagsString.Length > 0)
				sb.Append(": ").Append(tagsString);

			return sb.ToString();
		}

		public static string PictureDescription(ImageFormat format, int width, int height, long fileSize, Rating rating, int score, int ratesCount, string tagsString)
		{
			return
				string.Format(
					_pictureDescriptionTemplate,
					format.GetName(),
					width,
					height,
					fileSize.ToInfoSizeString(),
					rating.GetDisplayName(),
					Score(score, ratesCount),
					tagsString.Length > 0 ? tagsString : TagsHelper.TagmeTagName);
		}

		public static Expression<Func<Picture, ThumbModel>> GetThumbModelProjector(ITagsService tagsService, IAuthenticationService authenticationService)
		{
			return p =>
				new ThumbModel
				{
					ID = p.ID,
					Status = p.Status,
					Tags = MakeTagsList(p.Tags, tagsService, authenticationService).ToArray(),
					RequiresTagging = p.RequiresTagging,
					Rating = p.Rating,
					Format = p.Format,
					Width = p.Width,
					Height = p.Height,
					FileSize = p.FileSize,
					Score = p.Score,
					RatesCount = p.RatesCount
				};
		}

		public static IEnumerable<Tag> MakeTagsList(IEnumerable<int> tagIDs, ITagsService tagsService, IAuthenticationService authenticationService)
		{
			return
				tagIDs
					.Select(tagsService.LoadTag)
					.Where(t => ModerationHelper.CanView(t.Status, t.CreatorID, authenticationService.CurrentUserID, () => false))
					.DefaultOrder();
		}
	}
}