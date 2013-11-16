using System;
using System.Web;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;

namespace RMArt.Web.Controllers
{
	public class ServiceController : Controller
	{
		public const string LocaleCookieName = "Locale";

		private readonly IAuthenticationService _authenticationService;
		private readonly IPicturesService _picturesService;
		private readonly ITagsService _tagsService;

		public ServiceController(
			IAuthenticationService authenticationService,
			IPicturesService picturesService,
			ITagsService tagsService)
		{
			_authenticationService = authenticationService;
			_picturesService = picturesService;
			_tagsService = tagsService;
		}

		public ActionResult SetLocale()
		{
			return View(new SetLocaleModel { Locale = Request.GetUICultureSetting() });
		}

		[HttpPost]
		public ActionResult SetLocale(SetLocaleModel model)
		{
			if (GlobalizationHelper.IsValidLocaleSetting(model.Locale))
				Response.Cookies.Set(
					new HttpCookie(LocaleCookieName, model.Locale)
					{
						Expires = string.IsNullOrEmpty(model.Locale) ? DateTime.UtcNow : DateTime.UtcNow.AddYears(1)
					});

			model.IsSaved = true;
			return View(model);
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Administer()
		{
			return View();
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult UpdatePictureRateAggregates()
		{
			_picturesService.UpdateRateAggregates();
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult UpdatePictureCachedTags()
		{
			_picturesService.RecalcCachedTags();
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult RecalcFileHashes()
		{
			_picturesService.RecalcFileHashes(null);
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult RecalcBitmapHashes()
		{
			_picturesService.RecalcBitmapHashes(null);
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult UpdateTagAggregates()
		{
			_picturesService.RecalcTagCount(null);
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult DeleteDeclinedPictures()
		{
			_picturesService.DeleteDeclined();
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult DeleteDeclinedTags()
		{
			_tagsService.DeleteDeclined(_picturesService, Request.CreateIdentity(_authenticationService.CurrentUserID));
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult RebuildThumbs(bool? overwriteExisting)
		{
			_picturesService.RebuildThumbs(overwriteExisting == true, null);
			return RedirectToAction("Administer");
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult UpdateSimilarPictures()
		{
			_picturesService.UpdateSimilarPictures(null);
			return RedirectToAction("Administer");
		}
	}
}