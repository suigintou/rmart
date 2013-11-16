using System.Web.Mvc;
using Microsoft.Web.Helpers;
using RMArt.Core;
using RMArt.Web.Models;
using RMArt.Web.Resources;

namespace RMArt.Web.Controllers
{
	public class ReportsController : Controller
	{
		private readonly IReportsService _reportsService;
		private readonly IAuthenticationService _authenticationService;

		public ReportsController(IReportsService reportsService, IAuthenticationService authenticationService)
		{
			_reportsService = reportsService;
			_authenticationService = authenticationService;
		}

		[Authorize(Roles = "Moderator")]
		public ActionResult Index()
		{
			return View(_reportsService.Find());
		}

		public ActionResult Create(ObjectType? targetType, int? targetID)
		{
			return View(
				new CreateReportModel
				{
					TargetType = targetType,
					TargetID = targetID
				});
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Create(CreateReportModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			if (!Request.IsUserInRole(UserRole.User) && !ReCaptcha.Validate())
			{
				ModelState.AddModelError("Captcha", SharedResources.InvalidCaptchaMessage);
				return View(model);
			}

			_reportsService.Create(
				model.TargetType != null && model.TargetID != null 
					? new ObjectReference(model.TargetType.Value, model.TargetID.Value)
					: default(ObjectReference?),
				model.ReportType ?? ReportType.Feedback,
				model.Message,
				Request.CreateIdentity(_authenticationService.CurrentUserID));

			return View("ReportCreatedSuccess");
		}

		[Authorize(Roles = "Moderator"), HttpPost]
		public ActionResult Resolve(int id, string returnUrl)
		{
			_reportsService.Resolve(id, Request.CreateIdentity(_authenticationService.CurrentUserID));

			return this.ReturnOrRedirect(returnUrl);
		}
	}
}