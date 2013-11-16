using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;
using RMArt.Web.Resources;

namespace RMArt.Web.Controllers
{
	public class TagsController : Controller
	{
		private readonly ITagsService _tagsService;
		private readonly IPicturesService _picturesService;
		private readonly IAuthenticationService _authenticationService;

		public TagsController(ITagsService tagsService, IPicturesService picturesService, IAuthenticationService authenticationService)
		{
			_tagsService = tagsService;
			_picturesService = picturesService;
			_authenticationService = authenticationService;
		}

		public ActionResult Index(string q, TagType? type, ModerationStatus? status, TagsSortOrder? sortBy, int? p)
		{
			var isModerator = Request.IsUserInRole(UserRole.Moderator);
			var query =
				new TagsQuery
				{
					Term = q,
					Type = type,
					AllowedStatuses = isModerator ? status != null ? new SortedSet<ModerationStatus> { status.Value } : null : ModerationHelper.GetDefaultStatuses(false)
				};

			var totalCount = _tagsService.Find(query).Count();

			const int pageSize = 30;
			var pagesCount = (int)Math.Ceiling((float)totalCount / pageSize);
			var pageChecked = (p ?? 1).RestrictRange(1, pagesCount);

			query.SortBy = sortBy ?? Config.Default.DefaultTagsSortOrder;
			query.Skip = (pageChecked - 1) * pageSize;
			query.Take = pageSize;

			var tags =
				_tagsService
					.Find(query)
					.Select(t => _tagsService.LoadTag(t.ID))
					.ToArray();

			var model =
				new TagsListModel
				{
					Tags = tags,
					Query = q,
					TagType = type,
					SortBy = query.SortBy,
					CurrentPage = pageChecked,
					TotalPages = pagesCount,
					TotalCount = totalCount,
					CanEdit = Request.IsUserInRole(UserRole.Contributor),
					CanModerate = isModerator
				};

			return View(model);
		}

		public ActionResult Cloud(bool? logarithmic)
		{
			var tags =
				_tagsService
					.Find(
						new TagsQuery
						{
							AllowedStatuses = ModerationHelper.GetDefaultStatuses(Request.IsUserInRole(UserRole.Moderator)),
							SortBy = TagsSortOrder.MostUsed,
							Take = 100
						})
					.Select(t => _tagsService.LoadTag(t.ID))
					.OrderBy(t => t.Name)
					.ToArray();

			ViewBag.MinCount = tags.Min(t => t.UsageCount);
			ViewBag.MaxCount = tags.Max(t => t.UsageCount);
			ViewBag.Logarithmic = logarithmic ?? true;
			return View(tags);
		}

		public ActionResult AutoCompletionList(string term)
		{
			var query =
				new TagsQuery
				{
					Term = term,
					AllowedStatuses = ModerationHelper.GetDefaultStatuses(false),
					SortBy = TagsSortOrder.MostUsed,
					Take = 10
				};

			var list =
				_tagsService
					.Find(query)
					.Select(t => _tagsService.LoadTag(t.ID).GetDisplayName())
					.ToArray();

			return Json(list, JsonRequestBehavior.AllowGet);
		}

		[ChildActionOnly]
		public ActionResult ShowTag(int id, bool? linkToTagDetails)
		{
			ViewBag.LinkToTagDetails = linkToTagDetails;
			return PartialView(_tagsService.LoadTag(id));
		}

		[ChildActionOnly]
		public ActionResult ShowTags(IEnumerable<int> ids, bool? showTagme, bool? linkToTagDetails)
		{
			ViewBag.ShowTagme = showTagme == true;
			ViewBag.LinkToTagDetails = linkToTagDetails;
			return PartialView(ids ?? Enumerable.Empty<int>());
		}

		public ActionResult Details(int id)
		{
			var tag = _tagsService.LoadTag(id);
			if (tag == null)
				return HttpNotFound();

			var query =
				new PicturesQuery
				{
					ReqiredTagIDs = new SortedSet<int> { tag.ID },
					AllowedStatuses = ModerationHelper.GetDefaultStatuses(false),
					AllowedRatings = ModerationHelper.GetAllowedRatings(Request.GetMaxRating(), Request.GetShowUnrated()),
					SortBy = PicturesSortOrder.Random,
					Take = 5
				};
			var pictureModels =
				_picturesService
					.Find(query)
					.Select(VisualizationHelper.GetThumbModelProjector(_tagsService, _authenticationService))
					.ToArray();
			var thumbSizePreset = Config.Default.ThumbnailSize;
			var thumbSize = _picturesService.ThumbnailSizePresets[thumbSizePreset];


			var model =
				new TagDetailsModel
				{
					Tag = tag,
					CanEdit = Request.IsUserInRole(UserRole.Contributor),
					CanModerate = Request.IsUserInRole(UserRole.Moderator),
					CanAdminister = Request.IsUserInRole(UserRole.Administrator),
					SamplePictures =
						new ThumbnailsListModel
						{
							Pictures = pictureModels,
							ThumbSizePreset = thumbSizePreset,
							ThumbWidth = thumbSize.Width,
							ThumbHeight = thumbSize.Height
						},
				};

			return View(model);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult Edit(
			int[] ids,
			string name,
			TagType? type,
			ModerationStatus? status,
			//string[] aliasesToAdd,
			//string[] aliasesToRemove,
			//int[] parentsToAdd,
			//int[] parentsToRemove,
			string comment,
			string returnUrl)
		{
			if (status != null && !Request.IsUserInRole(UserRole.Moderator))
				return new HttpUnauthorizedResult();

			if (ids != null && ids.Any())
			{
				if ((name != null /*|| aliasesToAdd != null*/) && ids.Length > 1)
					throw new InvalidOperationException();

				var identity = Request.CreateIdentity(_authenticationService.CurrentUserID);
				var update = new TagUpdate
				{
					Name = name != null ? name.Trim() : null,
					Type = type,
					Status = status
				};

				foreach (var id in ids)
					_tagsService.Update(id, update, identity, comment);
			}
			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult SetName(int id, string name, string returnUrl)
		{
			_tagsService.Update(id, new TagUpdate { Name = name.Trim() }, Request.CreateIdentity(_authenticationService.CurrentUserID));
			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Moderator"), HttpPost]
		public ActionResult SetStatus(int[] ids, ModerationStatus status, string comment, string returnUrl)
		{
			if (ids != null && ids.Any())
			{
				var identity = Request.CreateIdentity(_authenticationService.CurrentUserID);
				foreach (var id in ids)
					_tagsService.Update(id, new TagUpdate { Status = status }, identity, comment);
			}
			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult SetType(int id, TagType type, string returnUrl)
		{
			_tagsService.Update(id, new TagUpdate { Type = type }, Request.CreateIdentity(_authenticationService.CurrentUserID));
			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Administrator"), HttpPost]
		public ActionResult Merge(int destantionID, string sourceTags, string returnUrl)
		{
			var parsedTags = TagsHelper.ParseTagNames(sourceTags, allowTagme: false);
			if (parsedTags.InvalidNames.Any())
				return new HttpStatusCodeResult(400, string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTags.InvalidNames)));

			var sourceTagIDs = new List<int>(parsedTags.Names.Count);
			foreach (var name in parsedTags.Names)
			{
				var tid = _tagsService.GetIDByName(name);
				if (tid == null)
					return new HttpStatusCodeResult(400, string.Format("Tag with name \"{0}\" not found.", name));
				if (tid != destantionID)
					sourceTagIDs.Add(tid.Value);
			}

			var identity = Request.CreateIdentity(_authenticationService.CurrentUserID);

			foreach (var sourceID in sourceTagIDs)
			{
				_picturesService.ReplaceTag(sourceID, destantionID, identity);
				_tagsService.Merge(sourceID, destantionID, identity);
			}

			return this.ReturnOrRedirect(returnUrl);
		}

		public ActionResult AliasesList(int id, bool showDeleteButtons)
		{
			var model =
				new TagAliasesListModel
				{
					TagID = id,
					Aliases = _tagsService.LoadTag(id).AliasNames,
					ShowDeleteButtons = showDeleteButtons
				};
			return PartialView(model);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult AddAlias(int tagID, string name, string returnUrl)
		{
			_tagsService.Update(
				tagID,
				new TagUpdate { AddAliases = new[] { name.Trim() } },
				Request.CreateIdentity(_authenticationService.CurrentUserID));

			return
				Request.IsAjaxRequest()
					? RedirectToAction("AliasesList", new { id = tagID, showDeleteButtons = true })
					: this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult DeleteAlias(int tagID, string name, string returnUrl)
		{
			_tagsService.Update(
				tagID,
				new TagUpdate { RemoveAliases = new[] { name } },
				Request.CreateIdentity(_authenticationService.CurrentUserID));

			return
				Request.IsAjaxRequest()
					? RedirectToAction("AliasesList", new { id = tagID, showDeleteButtons = true })
					: this.ReturnOrRedirect(returnUrl);
		}

		public ActionResult RelationsList(int id, bool showParents, bool showDeleteButtons, string returnUrl)
		{
			var relations =
				(showParents ? _tagsService.GetParentsOf(id, false) : _tagsService.GetChildsOf(id, false))
					.Select(_tagsService.LoadTag)
					.OrderBy(_ => _.Name)
					.ToList();
			var model =
				new TagRelationsListModel
				{
					Relations = relations,
					IsParents = showParents,
					ShowDeleteButtons = showDeleteButtons,
					TagID = id,
					ReturnUrl = returnUrl
				};
			return PartialView(model);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult AddRelations(int id, string tags, bool isParents, string returnUrl)
		{
			var parsedTags = TagsHelper.ParseTagNames(tags, allowTagme: false);
			if (parsedTags.InvalidNames.Any())
				return new HttpStatusCodeResult(400, string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTags.InvalidNames)));

			var identity = Request.CreateIdentity(_authenticationService.CurrentUserID);
			var status = Request.GetDefaultStatus();

			try
			{
				foreach (var tid in _tagsService.GetAssignTagIDs(parsedTags.Names, status, identity, false))
					if (isParents)
						_tagsService.Update(id, new TagUpdate { AddParents = new[] { tid } }, identity);
					else
						_tagsService.Update(tid, new TagUpdate { AddParents = new[] { id } }, identity);
			}
			catch (Exception ex)
			{
				return new HttpStatusCodeResult(500, ex.Message);
			}

			return
				Request.IsAjaxRequest()
					? RedirectToAction("RelationsList", new { id, showParents = isParents, returnUrl, showDeleteButtons = true })
					: this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Contributor"), HttpPost]
		public ActionResult DeleteRelation(int parentID, int childrenID, bool isParent, string returnUrl)
		{
			_tagsService.Update(
				childrenID,
				new TagUpdate { RemoveParents = new[] { parentID } },
				Request.CreateIdentity(_authenticationService.CurrentUserID));

			return
				Request.IsAjaxRequest()
					? RedirectToAction(
						"RelationsList",
						new { id = isParent ? childrenID : parentID, showParents = isParent, returnUrl, showDeleteButtons = true })
					: this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Administrator"), HttpPost]
		public ActionResult AssignParentTag(int tagID, string returnUrl)
		{
			_picturesService.AssignTagChilds(tagID, Request.CreateIdentity(_authenticationService.CurrentUserID));
			return this.ReturnOrRedirect(returnUrl);
		}
	}
}