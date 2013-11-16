using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;

namespace RMArt.Web.Controllers
{
	public class DiscussionsController : Controller
	{
		private readonly IDiscussionService _discussionService;
		private readonly IAuthenticationService _authenticationService;

		public DiscussionsController(IDiscussionService discussionService, IAuthenticationService authenticationService)
		{
			_discussionService = discussionService;
			_authenticationService = authenticationService;
		}

		public ActionResult Index()
		{
			return View();
		}

		[ChildActionOnly]
		public ActionResult ShowMessages(ObjectType? parentType, int? parentID, bool? withPreview)
		{
			var canModerate = Request.IsUserInRole(UserRole.Moderator);

			var posts = _discussionService
				.All()
				.Where(m => !parentType.HasValue || m.ParentType == parentType)
				.Where(m => !parentID.HasValue || m.ParentID == parentID)
				.Where(m => m.Status == ModerationStatus.Accepted || canModerate)
				.OrderByDescending(m => m.ID)
				.Select(
					m => new DiscussionMessageModel
					{
						ID = m.ID,
						Body = m.Body,
						Parent = new ObjectReference(m.ParentType, m.ParentID),
						Status = m.Status,
						CreatedAt = m.CreatedAt,
						CreatorID = m.CreatedBy,
						CreatedFrom = new IPAddress(m.CreatorIP),
						CreatorLogin = m.Creator.Login,
						CreatorEmeil = m.Creator.Email,
						CanViewPrivateData = _authenticationService.CurrentUserID == m.CreatedBy || canModerate
					})
				.ToArray();

			return PartialView(
				new CommentsListModel
				{
					Messages = posts,
					WithPreview = withPreview == true,
					CanModerate = canModerate
				});
		}

		[ChildActionOnly]
		public ActionResult CommentForm(ObjectType parentType, int parentID)
		{
			return PartialView(new CommentFormModel { ParentType = parentType, ParentID = parentID });
		}

		[Authorize(Roles = "User"), HttpPost]
		public ActionResult CreateMessage(CommentFormModel model, string returnUrl)
		{
			if (ModelState.IsValid)
				_discussionService.CreateMessage(
					new ObjectReference(model.ParentType, model.ParentID),
					model.Body.Trim(),
					Request.CreateIdentity(_authenticationService.CurrentUserID));

			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Moderator"), HttpPost]
		public ActionResult Edit(int id, [Required, StringLength(500)] string newText, string returnUrl)
		{
			_discussionService.Edit(id, newText);
			return this.ReturnOrRedirect(returnUrl);
		}

		[Authorize(Roles = "Moderator"), HttpPost]
		public ActionResult SetStatus(int id, ModerationStatus status, string returnUrl)
		{
			_discussionService.SetStatus(id, status, Request.CreateIdentity(_authenticationService.CurrentUserID));
			return this.ReturnOrRedirect(returnUrl);
		}
	}
}