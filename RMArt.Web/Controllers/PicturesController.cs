using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;
using RMArt.Web.Resources;

namespace RMArt.Web.Controllers
{
	public class PicturesController : Controller
	{
		private readonly IRatesRepository _ratesRepository;
		private readonly IFavoritesRepository _favoritesRepository;
		private readonly IAuthenticationService _authenticationService;
		private readonly IPicturesService _picturesService;
		private readonly ITagsService _tagsService;
		private readonly IUsersService _usersService;
		private readonly Expression<Func<Picture, ThumbModel>> _thumbModelProjector;

		public PicturesController(
			IRatesRepository ratesRepository,
			IFavoritesRepository favoritesRepository,
			IAuthenticationService authenticationService,
			IPicturesService picturesService,
			ITagsService tagsService,
			IUsersService usersService)
		{
			_tagsService = tagsService;
			_usersService = usersService;
			_ratesRepository = ratesRepository;
			_favoritesRepository = favoritesRepository;
			_picturesService = picturesService;
			_authenticationService = authenticationService;
			_thumbModelProjector = VisualizationHelper.GetThumbModelProjector(_tagsService, _authenticationService);
		}

		public ActionResult Index(string q, PicturesSortOrder? sortBy, int? p)
		{
			var searchQuery = PicturesSearchQueryHelper.ParseSearchQuery(q);
			var isModerator = Request.IsUserInRole(UserRole.Moderator);
			var query = searchQuery.ToPredicate(_tagsService, _usersService);

			if (query.AllowedStatuses == null)
				query.AllowedStatuses = ModerationHelper.GetDefaultStatuses(isModerator);

			var maxRating = Request.GetMaxRating();
			var showUnrated = Request.GetShowUnrated();
			if (query.AllowedRatings == null)
				query.AllowedRatings = ModerationHelper.GetAllowedRatings(maxRating, showUnrated);

			query.SortBy = sortBy ?? Config.Default.DefaultPicturesSortOrder;

			var totalCount = _picturesService.Find(query).Count();
			var pageSize = Config.Default.ThmbnailsOnPageCount;
			var pagesCount = query.SortBy != PicturesSortOrder.Random ? (int)Math.Ceiling((float)totalCount / pageSize) : 1;
			var pageChecked = (p ?? 1).RestrictRange(1, pagesCount);

			query.Skip = (pageChecked - 1) * pageSize;
			query.Take = pageSize;

			var pictureModels =
				_picturesService
					.Find(query)
					.Select(_thumbModelProjector)
					.ToArray();

			var thumbSizePreset = Config.Default.ThumbnailSize;
			var thumbSize = _picturesService.ThumbnailSizePresets[thumbSizePreset];

			var canEdit = Request.IsUserInRole(UserRole.Contributor);

			return
				View(
					new GalleryModel
					{
						ThumbnailsListModel =
							new ThumbnailsListModel
							{
								Pictures = pictureModels,
								ThumbSizePreset = thumbSizePreset,
								ThumbWidth = thumbSize.Width,
								ThumbHeight = thumbSize.Height,
								ShowIcons = canEdit
							},
						RelatedTags = CreateTagModels(GetRelatedTags(pictureModels.SelectMany(_ => _.Tags), query.ReqiredTagIDs, 20), searchQuery, query.SortBy).ToArray(),
						Query = q,
						SortBy = query.SortBy,
						CurrentPage = pageChecked,
						TotalPages = pagesCount,
						MaxRating = maxRating,
						ShowUnrated = showUnrated,
						TotalCount = totalCount,
						CanEdit = canEdit,
						CanModerate = isModerator
					});
		}

		public ActionResult FindIDs(string q, PicturesSortOrder? sortBy)
		{
			if (!Request.IsUserInRole(UserRole.Moderator))
				return new HttpUnauthorizedResult();

			var searchQuery = PicturesSearchQueryHelper.ParseSearchQuery(q);
			var predicate = searchQuery.ToPredicate(_tagsService, _usersService);
			predicate.SortBy = sortBy ?? PicturesSortOrder.None;
			var res = string.Join(";", _picturesService.Find(predicate).Select(_ => _.ID));
			return Content(res);
		}

		public ActionResult Search(string q, PicturesSortOrder? sortBy, bool? construct)
		{
			var sortOrder = sortBy ?? Config.Default.DefaultPicturesSortOrder;

			if (construct == true)
			{
				var searchQuery = PicturesSearchQueryHelper.ParseSearchQuery(q);
				return View(
					new SearchModel
					{
						ReqiredTags = string.Join(", ", searchQuery.TagmeRequired ? searchQuery.ReqiredTags.Concat(new[] { TagsHelper.TagmeTagName }) : searchQuery.ReqiredTags),
						ExcldedTags = string.Join(", ", searchQuery.TagmeExcuded ? searchQuery.ExcludedTags.Concat(new[] { TagsHelper.TagmeTagName }) : searchQuery.ExcludedTags),
						Orientation = searchQuery.Orientation,
						MinWidth = searchQuery.MinWidth,
						MaxWidth = searchQuery.MaxWidth,
						MinHeight = searchQuery.MinHeight,
						MaxHeight = searchQuery.MaxHeight,
						StartDate = searchQuery.StartDate,
						EndDate = searchQuery.EndDate,
						Uploader = searchQuery.Uploader,
						FavoritedBy = searchQuery.FavoritedBy,
						AllowedStatuses = searchQuery.AllowedStatuses ?? ModerationHelper.GetDefaultStatuses(Request.IsUserInRole(UserRole.Moderator)),
						AllowedRatings = searchQuery.AllowedRatings ?? ModerationHelper.GetAllowedRatings(Request.GetMaxRating(), Request.GetShowUnrated()),
						SortBy = sortOrder
					});
			}

			return Redirect(Url.Gallery(q, sortOrder, addRnd: sortOrder == PicturesSortOrder.Random));
		}

		[HttpPost]
		public ActionResult Search(SearchModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var parsedReqTags = TagsHelper.ParseTagNames(model.ReqiredTags);
			if (parsedReqTags.InvalidNames.Any())
			{
				ModelState.AddModelError("ReqiredTags", string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedReqTags.InvalidNames)));
				return View(model);
			}

			var parsedExcludesTags = TagsHelper.ParseTagNames(model.ExcldedTags);
			if (parsedExcludesTags.InvalidNames.Any())
			{
				ModelState.AddModelError("ReqiredTags", string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedExcludesTags.InvalidNames)));
				return View(model);
			}

			SortedSet<ModerationStatus> allowedStatuses = null;
			if (model.AllowedStatuses != null
					&& !ModerationHelper.GetDefaultStatuses(Request.IsUserInRole(UserRole.Moderator)).SetEquals(model.AllowedStatuses))
				allowedStatuses = new SortedSet<ModerationStatus>(model.AllowedStatuses);

			SortedSet<Rating> allowedRatings = null;
			if (model.AllowedRatings != null && !ModerationHelper.GetAllowedRatings(Request.GetMaxRating(), Request.GetShowUnrated()).SetEquals(model.AllowedRatings))
				allowedRatings = new SortedSet<Rating>(model.AllowedRatings);

			var q =
				new PicturesSearchQuery
				{
					ReqiredTags = parsedReqTags.Names,
					TagmeRequired = parsedReqTags.TagmeIncluded,
					ExcludedTags = parsedExcludesTags.Names,
					TagmeExcuded = parsedExcludesTags.TagmeIncluded,
					AllowedStatuses = allowedStatuses,
					AllowedRatings = allowedRatings,
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					Orientation = model.Orientation,
					MinWidth = model.MinWidth,
					MaxWidth = model.MaxWidth,
					MinHeight = model.MinHeight,
					MaxHeight = model.MaxHeight,
					Uploader = model.Uploader,
					FavoritedBy = model.FavoritedBy
				}.ToStringPresentation();

			return Redirect(Url.Gallery(q, model.SortBy, addRnd: model.SortBy == PicturesSortOrder.Random));
		}

		[Authorize]
		public ActionResult ChangeMaxRating(string returnUrl)
		{
			return View(
				new ChangeMaxRatingModel
				{
					MaxRating = Request.GetMaxRating(),
					ShowUnrated = Request.GetShowUnrated(),
					ReturnUrl = returnUrl
				});
		}

		[Authorize]
		public ActionResult SetOptions(Rating? maxRating, bool? showUnrated, string returnUrl)
		{
			if (maxRating != null)
			{
				var val = maxRating.Value.ToString();
				Response.Cookies.Set(new HttpCookie(CookieSettingsHelper.MaxRatingCookieName, val) { Expires = DateTime.UtcNow.AddYears(1) });
			}

			if (showUnrated != null)
			{
				var val = showUnrated.Value.ToString();
				Response.Cookies.Set(new HttpCookie(CookieSettingsHelper.ShowUnratedCookieName, val) { Expires = DateTime.UtcNow.AddYears(1) });
			}

			return this.ReturnOrRedirect(returnUrl);
		}

		public ActionResult Src(int id)
		{
			if (Request.IsReffered())
				return RedirectToAction("Viewer", new { id });

			var picture = _picturesService.Find(new PicturesQuery { ID = id }).SingleOrDefault();
			if (picture == null
					|| !ModerationHelper.CanView(
						picture.Status,
						picture.CreatorID,
						_authenticationService.CurrentUserID,
						() => Request.IsUserInRole(UserRole.Moderator)))
				return HttpNotFound();

			var pictureUrl = Url.ToAbsolute(Url.PictureSrc(picture.ID, UrlHelperExtensions.PictureFileNameFromHash(picture.FileHash, picture.Format)));
			if (!pictureUrl.Equals(Request.Url.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
				return RedirectPermanent(pictureUrl);

			var pictureFile = _picturesService.GetPictureFile(id);

			if (pictureFile == null)
				return HttpNotFound();

			if (Request.IsClientCached(pictureFile.LastModifiedTime))
				return new HttpStatusCodeResult(304);

			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(365));
			Response.Cache.SetLastModified(pictureFile.LastModifiedTime);

			if (pictureFile.LocalFilePath != null)
				return File(pictureFile.LocalFilePath, ImageFormat.Jpeg.GetMimeType());
			return File(pictureFile.OpenRead(), picture.Format.GetMimeType());
		}

		public ActionResult Thumb(int id, int size)
		{
			var thumbFile = _picturesService.GetThumbFile(id, size);

			if (thumbFile == null)
				return HttpNotFound();

			if (Request.IsClientCached(thumbFile.LastModifiedTime))
				return new HttpStatusCodeResult(304);

			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(365));
			Response.Cache.SetLastModified(thumbFile.LastModifiedTime);

			if (thumbFile.LocalFilePath != null)
				return File(thumbFile.LocalFilePath, ImageFormat.Jpeg.GetMimeType());
			return File(thumbFile.OpenRead(), ImageFormat.Jpeg.GetMimeType());
		}

		public ActionResult Viewer(int id)
		{
			var picture = _picturesService.Find(new PicturesQuery { ID = id }).SingleOrDefault();
			if (picture == null)
				return HttpNotFound();

			var viewerUrl = Url.ToAbsolute(Url.PictureViewer(picture.ID));
			if (!viewerUrl.Equals(Request.Url.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
				return RedirectPermanent(viewerUrl);

			var isModerator = Request.IsUserInRole(UserRole.Moderator);

			var userID = _authenticationService.CurrentUserID;
			var userRate = userID != null ? _ratesRepository.List(new RatesQuery { PictureID = picture.ID, UserID = userID.Value }).SingleOrDefault() : null;
			var isFavorited = userID != null && _favoritesRepository.IsExists(new FavoritesQuery(userID.Value, picture.ID));

			var tags =
				CreateTagModels(
						VisualizationHelper.MakeTagsList(picture.Tags, _tagsService, _authenticationService),
						new PicturesSearchQuery(),
						Config.Default.DefaultPicturesSortOrder)
					.ToArray();

			var similarPictureThumbs =
				_picturesService
					.GetSimilar(picture.ID)
					.Select(_thumbModelProjector)
					.ToArray();
			var thumbSizePreset = Config.Default.ThumbnailSize;
			var thumbSize = _picturesService.ThumbnailSizePresets[thumbSizePreset];
			var similarPictureThumbsListModel = new ThumbnailsListModel
			{
				Pictures = similarPictureThumbs,
				ThumbSizePreset = thumbSizePreset,
				ThumbWidth = thumbSize.Width,
				ThumbHeight = thumbSize.Height,
				ShowIcons = false
			};

			var model =
				new PictureViewerModel
				{
					Picture = picture,
					Tags = tags,
					SimilarPictureThumbs = similarPictureThumbsListModel,
					Keywords = string.Join(", ", tags.Take(20).Select(t => t.Tag.Name)),
					UserScore = userRate != null ? userRate.Score : 0,
					IsFavorited = isFavorited,
					CanView = ModerationHelper.CanView(picture.Status, picture.CreatorID, _authenticationService.CurrentUserID, () => isModerator),
					CanEdit = Request.IsUserInRole(UserRole.Contributor),
					CanModerate = isModerator,
					CanRate = Request.IsAuthenticated,
					CanFavorite = Request.IsAuthenticated,
					PictureUrl = Url.PictureSrc(picture.ID, UrlHelperExtensions.PictureFileNameFromHash(picture.FileHash, picture.Format)),
					ShareUrl = viewerUrl,
					ShareDirectUrl = Url.ToAbsolute(Url.PictureSrc(picture.ID, UrlHelperExtensions.PictureFileNameFromHash(picture.FileHash, picture.Format)))
				};

			return View(model);
		}

		//public ActionResult Next(int id, bool forward, string q, PicturesSortOrder? sortBy, int? p)
		//{
		//	var current = _picturesService.Find().GetByID(id);
		//	if (current == null)
		//		return HttpNotFound();

		//	var searchQuery = PicturesSearchQueryHelper.ParseSearchQuery(q);
		//	var isModerator = Request.IsUserInRole(UserRole.Moderator);
		//	var query = searchQuery.ToPredicate(_tagsService, _usersService, _authenticationService.CurrentUserID, isModerator);

		//	if (query.AllowedStatuses == null)
		//		query.AllowedStatuses = ModerationHelper.GetDefaultStatuses(isModerator);

		//	var maxRating = Request.GetMaxRating();
		//	var showUnrated = Request.GetShowUnrated();
		//	if (query.AllowedRatings == null)
		//		query.AllowedRatings = ModerationHelper.GetAllowedRatings(maxRating, showUnrated);

		//	var sortOrder = sortBy ?? Config.Default.DefaultPicturesSortOrder;
		//	if (!forward)
		//		sortOrder = sortOrder.Invert();
		//	query.SortBy = sortOrder;

		//	query.After = current;
		//	query.Take = 1;

		//	var next = _picturesService.Find(query).FirstOrDefault();

		//	return RedirectToAction("Viewer", new { id = next != null ? next.ID : current.ID, sq = q, so = sortBy, sp = p });
		//}

		public ActionResult Rates(string login, int? pictureID, int? score, int? p)
		{
			var user =_usersService.GetByLogin(login);
			var userID = user != null ? user.ID : default(int?);

			const int pageSize = 20;
			var predicate = new RatesQuery { PictureID = pictureID, UserID = userID, Score = score };
			if (user == null)
				predicate.IsActive = true;
			var totalCount = _ratesRepository.Count(predicate);
			var pagesCount = (int)Math.Ceiling((float)totalCount / pageSize);
			var pageChecked = (p ?? 1).RestrictRange(1, pagesCount);
			var rates = _ratesRepository.List(predicate, (pageChecked - 1) * pageSize, pageSize);

			return View(
				new RatesListModel
				{
					Rates = rates,
					CurrentPage = pageChecked,
					TotalPages = pagesCount,
					TotalCount = totalCount,
					PictureID = pictureID,
					User = user != null ? user.Login : null,
					Score = score,
					ShowIPAddresses = Request.IsUserInRole(UserRole.Administrator)
				});
		}

		[Authorize, HttpPost]
		public ActionResult Rate(int pictureID, int userScore, string returnUrl)
		{
			if (!_picturesService.Find(new PicturesQuery { ID = pictureID }).Any())
				return new HttpStatusCodeResult(400, "Picture not found.");
			if (userScore < 0 || userScore > 3)
				return new HttpStatusCodeResult(400, "Score is out of range.");

			_picturesService.Rate(pictureID, userScore, Request.CreateIdentity(_authenticationService.CurrentUserID));

			if (Request.IsAjaxRequest())
			{
				var p = _picturesService.Find(new PicturesQuery { ID = pictureID }).Single();
				return Content(VisualizationHelper.Score(p.Score, p.RatesCount));
			}

			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "User"), HttpPost]
		public ActionResult SetFavorited(int pictureID, bool favorited, string returnUrl)
		{
			if (!_picturesService.Find(new PicturesQuery { ID = pictureID }).Any())
				return new HttpStatusCodeResult(400, "Picture not found.");

			_picturesService.SetFavorited(
				pictureID,
				favorited,
				Request.CreateIdentity(_authenticationService.CurrentUserID));

			return Request.IsAjaxRequest() ? new EmptyResult() : this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult Edit(
			int[] ids,
			string tagsToAdd,
			string tagsToRemove,
			Rating? rating,
			ModerationStatus? status,
			string source,
			bool? deleteSource,
			string comment,
			string returnUrl)
		{
			if (ids != null && ids.Length > 0)
			{
				if (!Request.IsUserInRole(UserRole.Moderator) && (ids.Length > 1 || status != null))
					return new HttpUnauthorizedResult();
				if ((tagsToAdd != null && tagsToAdd.Length > 200) || (tagsToRemove != null && tagsToRemove.Length > 200))
					return new HttpStatusCodeResult(400, PicturesResources.TagsStringTooLongMessage);

				var parsedTagsToAdd = TagsHelper.ParseTagNames(tagsToAdd);
				if (parsedTagsToAdd.InvalidNames.Any())
					return new HttpStatusCodeResult(400, string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTagsToAdd.InvalidNames)));
				var parsedTagsToRemove = TagsHelper.ParseTagNames(tagsToRemove);
				if (parsedTagsToRemove.InvalidNames.Any())
					return new HttpStatusCodeResult(400, string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTagsToRemove.InvalidNames)));

				if (deleteSource != true && !string.IsNullOrEmpty(source) && !PicturesHelper.IsValidSource(source))
					return new HttpStatusCodeResult(400, UploadResources.InvalidSourceMessage);

				var identity = Request.CreateIdentity(_authenticationService.CurrentUserID);

				_picturesService.UpdateMany(
					ids,
					new PictureUpdate
					{
						Status = status,
						AddTags = _tagsService.GetAssignTagIDs(parsedTagsToAdd.Names, Request.GetDefaultStatus(), identity, true),
						RemoveTags = _tagsService.GetSearchTagIDs(parsedTagsToRemove.Names),
						RequiresTagging = parsedTagsToRemove.TagmeIncluded ? false : (parsedTagsToAdd.TagmeIncluded ? true : default(bool?)),
						Rating = rating,
						Source = deleteSource == true ? "" : (string.IsNullOrWhiteSpace(source) ? null : source.Trim())
					},
					identity,
					comment);
			}
			return this.ReturnOrRedirect(returnUrl);
		}

		private static IEnumerable<Tag> GetRelatedTags(IEnumerable<Tag> tags, ICollection<int> exclude, int maxCount)
		{
			return
				tags
					.Where(t => exclude == null || !exclude.Contains(t.ID))
					.GroupBy(_ => _.ID, (id, ts) => new { Tag = ts.First(), Count = ts.Count() })
					.OrderByDescending(_ => _.Count)
					.ThenBy(_ => _.Tag.Name)
					.Select(_ => _.Tag)
					.Take(maxCount);
		}

		private IEnumerable<TagModel> CreateTagModels(IEnumerable<Tag> tags, PicturesSearchQuery query, PicturesSortOrder sortBy)
		{
			return tags.Select(t =>
				new TagModel
				{
					Tag = t,
					Url = Url.Gallery(TagsHelper.EncodeTagName(t.Name), sortBy),
					IncludeUrl = Url.Gallery(query.ToStringPresentation(includeTag: t.Name), sortBy),
					ExcludeUrl = Url.Gallery(query.ToStringPresentation(excludeTag: t.Name), sortBy),
					Description = t.TagDescription(_tagsService)
				});
		}
	}
}