using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;

namespace RMArt.Web.Controllers
{
	public class HistoryController : Controller
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly IHistoryService _historyService;
		private readonly ITagsService _tagsService;
		private readonly IUsersService _usersService;

		public HistoryController(
			IHistoryService historyService,
			ITagsService tagsService,
			IAuthenticationService authenticationService,
			IUsersService usersService)
		{
			_historyService = historyService;
			_authenticationService = authenticationService;
			_usersService = usersService;
			_tagsService = tagsService;
		}

		public ActionResult Index(ObjectType? targetType, int? targetID, HistoryField? actionType, string login, int? p)
		{
			var user = _usersService.GetByLogin(login);

			var currentPage = p ?? 1;
			const int pageSize = 200;

			var query =
				new HistoryQuery
				{
					TargetType = targetType,
					TargetID = targetID,
					Field = actionType,
					UserID = user != null ? user.ID : default(int?),
					Skip = (currentPage - 1) * pageSize,
					Take = pageSize + 1
				};

			var events = _historyService.Find(query).ToArray();
			var hasMorePages = events.Length > pageSize;

			var groupingPeriod = TimeSpan.FromMinutes(10);
			var groups =
				events
					.MergeNeighbors((l, r) => l.UserID == r.UserID && l.UserIP.SequenceEqual(r.UserIP) && l.Date.CompareDates(r.Date, groupingPeriod))
					.Select(
						groupedByUserAndDate => new HistoryActionGroup
						{
							UserID = groupedByUserAndDate.First().UserID,
							UserIP = new IPAddress(groupedByUserAndDate.First().UserIP),
							StartTime = groupedByUserAndDate.Last().Date,
							EndTime = groupedByUserAndDate.First().Date,
							Actions = groupedByUserAndDate
								.MergeNeighbors((l, r) => l.TargetType == r.TargetType && l.TargetID == r.TargetID && string.Equals(l.Comment, r.Comment, StringComparison.Ordinal))
								.Select(
									groupedByTargetAndComment => new HistoryEventGroup
									{
										TargetType = groupedByTargetAndComment.First().TargetType,
										TargetID = groupedByTargetAndComment.First().TargetID,
										Comment = groupedByTargetAndComment.First().Comment,
										Changes = groupedByTargetAndComment.Reverse().ToArray()
									})
								.ToArray()
						})
					.ToArray();

			var model =
				new HistoryModel
				{
					Groups = groups,
					CurrentPage = currentPage,
					HasMorePages = hasMorePages,
					TargetType = targetType,
					TargetID = targetID,
					ActionType = actionType,
					User = user != null ? user.Login : null,
					CanAdminister = Request.IsUserInRole(UserRole.Administrator)
				};

			return View(model);
		}

		[ChildActionOnly]
		public ActionResult Target(ObjectReference target)
		{
			string presentation;
			switch (target.Type)
			{
				case ObjectType.Picture:
					presentation = "#" + target.ID;
					break;
				case ObjectType.Tag:
					presentation = string.Concat("\"", _tagsService.LoadTag(target.ID).GetDisplayName(), "\"");
					break;
				case ObjectType.Message:
					presentation = ">>" + target.ID;
					break;
				default:
					throw new NotSupportedException();
			}
			return Content(string.Format("<a href=\"{0}\">{2} {1}</a>", Url.Object(target), presentation, target.Type.GetDisplayName()));
		}

		[ChildActionOnly]
		public ActionResult Event(HistoryEvent ev)
		{
			switch (ev.Field)
			{
				case HistoryField.Tags:
					var isTagAdded = !string.IsNullOrEmpty(ev.NewValue);
					var tag = _tagsService.LoadTag((isTagAdded ? ev.NewValue : ev.OldValue).ParseInt32());
					return Content(
						string.Format(
							"<span class=\"tag {0}\">{1}{2}</span>",
							tag != null ? tag.Type.ToString().ToLower() : "",
							isTagAdded ? "+" : "-",
							tag.GetDisplayName()));
				case HistoryField.RequiresTagging:
					return Content((bool.Parse(ev.NewValue) ? "+" : "-") + TagsHelper.TagmeTagName);
				case HistoryField.Status:
					return Content(string.Format("status:{0}←{1}", ev.NewValue.ParseEnum<ModerationStatus>().GetDisplayName(), ev.OldValue.ParseEnum<ModerationStatus>().GetDisplayName()));
				case HistoryField.Rating:
					return Content(string.Format("rating:{0}←{1}", ev.NewValue.ParseEnum<Rating>().GetDisplayName(), ev.OldValue.ParseEnum<Rating>().GetDisplayName()));
				case HistoryField.Source:
					return Content(FieldChange("source", ev.NewValue, ev.OldValue));
				case HistoryField.Name:
					return Content(string.Format("name:{0}←{1}", ev.NewValue, ev.OldValue));
				case HistoryField.Type:
					var newType = ev.NewValue.ParseEnum<TagType>();
					var oldType = ev.OldValue.ParseEnum<TagType>();
					return Content(
						string.Format(
							"type:<span class=\"tag {2}\">{0}</span>←<span class=\"tag {3}\">{1}</span>",
							newType.GetDisplayName(),
							oldType.GetDisplayName(),
							newType.ToString().ToLower(),
							oldType.ToString().ToLower()));
				case HistoryField.Aliases:
					var isAliasAdded = !string.IsNullOrEmpty(ev.NewValue);
					return Content(string.Format("{0}alias:{1}", isAliasAdded ? "+" : "-", isAliasAdded ? ev.NewValue : ev.OldValue));
				case HistoryField.Parents:
					var isParentAdded = !string.IsNullOrEmpty(ev.NewValue);
					var parentTag = _tagsService.LoadTag((isParentAdded ? ev.NewValue : ev.OldValue).ParseInt32());
					return Content(
						string.Format(
							"{0}parent:<span class=\"tag {1}\">{2}</span>",
							isParentAdded ? "+" : "-",
							parentTag != null ? parentTag.Type.ToString().ToLower() : "",
							parentTag.GetDisplayName()));
				default:
					return Content("[Unknown event]");
			}
		}

		private string FieldChange(string name, string newValue, string oldValue)
		{
			if (!string.IsNullOrEmpty(newValue) && string.IsNullOrEmpty(oldValue))
				return string.Format("+{0}:{1}", name, newValue);
			if (!string.IsNullOrEmpty(oldValue) && string.IsNullOrEmpty(newValue))
				return string.Format("-{0}:{1}", name, oldValue);

			return string.Format("{0}:{1}←{2}", name, newValue, oldValue);
		}
	}
}