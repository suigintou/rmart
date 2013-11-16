using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Helpers;
using RMArt.Core;
using RMArt.Web.Models;
using RMArt.Web.Resources;

namespace RMArt.Web.Controllers
{
	public class UploadController : Controller
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly IPicturesService _picturesService;
		private readonly ITagsService _tagsService;

		public UploadController(
			IAuthenticationService authenticationService,
			IPicturesService picturesService,
			ITagsService tagsService)
		{
			_tagsService = tagsService;
			_picturesService = picturesService;
			_authenticationService = authenticationService;
		}

		public ActionResult Index()
		{
			return View(new UploadModel { Rating = Config.Default.DefaultRating });
		}

		[HttpPost]
		public ActionResult Index(UploadModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			if (!Request.IsUserInRole(UserRole.User) && !ReCaptcha.Validate())
			{
				ModelState.AddModelError("Captcha", SharedResources.InvalidCaptchaMessage);
				return View(model);
			}

			var parsedTags = TagsHelper.ParseTagNames(model.Tags);
			if (parsedTags.InvalidNames.Any())
			{
				ModelState.AddModelError("Tags", string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTags.InvalidNames)));
				return View(model);
			}

			if (!string.IsNullOrEmpty(model.Source) && !PicturesHelper.IsValidSource(model.Source))
			{
				ModelState.AddModelError("Source", UploadResources.InvalidSourceMessage);
				return View(model);
			}

			var results = new List<FileUploadResult>(model.Files.Length);

			if (model.Files.Length > 0 && model.Files[0] != null)
				foreach (var file in model.Files)
				{
					int pictureID;
					var res = AddPicture(file.InputStream, parsedTags.Names, parsedTags.TagmeIncluded, model.Rating, model.Source, false, out pictureID);
					results.Add(new FileUploadResult { Name = file.FileName, Status = res, PictureID = pictureID });
				}
			else if (!string.IsNullOrEmpty(model.Url))
				using (var webClient = new WebClient())
					try
					{
						int pictureID;
						var res = AddPicture(webClient.OpenRead(model.Url), parsedTags.Names, parsedTags.TagmeIncluded, model.Rating, model.Source, false, out pictureID);
						results.Add(new FileUploadResult { Name = model.Url, Status = res, PictureID = pictureID });
					}
					catch (WebException webException)
					{
						ModelState.AddModelError("Url", UploadResources.DownloadFailedMessage + " " + webException.Message);
						return View(model);
					}
			else
			{
				ModelState.AddModelError("", UploadResources.PictureNotSelectedMessage);
				return View(model);
			}

			if (results.Count > 1)
				return View("MultiUploadResult", results);

			var result = results[0];
			switch (result.Status)
			{
				case PictureAddingResult.Added:
				case PictureAddingResult.Resurrected:
				case PictureAddingResult.AlreadyExists:
					return RedirectToAction("Viewer", "Pictures", new { id = result.PictureID });
				default:
					ModelState.AddModelError("", result.Status.GetUploadResultMessage());
					return View(model);
			}
		}

		[Authorize(Roles = "Contributor")]
		public ActionResult FromUrl(string url, string tags, Rating? rating, string source, bool? replaceFile, string referer)
		{
			if (string.IsNullOrEmpty(url))
				return new HttpStatusCodeResult(400, UploadResources.EmptyUrlMessage);

			var parsedTags = TagsHelper.ParseTagNames(tags);
			if (parsedTags.InvalidNames.Any())
				return new HttpStatusCodeResult(400, string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTags.InvalidNames)));

			if (!string.IsNullOrEmpty(source) && !PicturesHelper.IsValidSource(source))
				return new HttpStatusCodeResult(400, UploadResources.InvalidSourceMessage);

			using (var webClient = new WebClient())
			{
				if (!string.IsNullOrEmpty(referer))
					webClient.Headers.Add(HttpRequestHeader.Referer, referer);

				try
				{
					int pictureID;
					var result =
						AddPicture(
							webClient.OpenRead(url),
							parsedTags.Names,
							parsedTags.TagmeIncluded,
							rating ?? Config.Default.DefaultRating,
							source,
							replaceFile == true,
							out pictureID);

					switch (result)
					{
						case PictureAddingResult.Added:
						case PictureAddingResult.AlreadyExists:
							return RedirectToAction("Viewer", "Pictures", new { id = pictureID });
						default:
							return Content(result.GetUploadResultMessage());
					}
				}
				catch (WebException webException)
				{
					return new HttpStatusCodeResult(500, UploadResources.DownloadFailedMessage + " " + webException.Message);
				}
			}
		}

		[HttpPost]
		public ActionResult Batch(HttpPostedFileBase file, string tags, Rating? rating, string source, bool? replaceFile)
		{
			if (file == null)
				return new HttpStatusCodeResult(400, UploadResources.EmptyDataMessage);
			if (!Request.IsUserInRole(UserRole.Contributor))
				return new HttpUnauthorizedResult(UploadResources.AuthentificationRequiredMessage);

			try
			{
				var parsedTags = TagsHelper.ParseTagNames(tags);
				if (parsedTags.InvalidNames.Any())
					return new HttpStatusCodeResult(400, string.Format(TagsResources.InvalidTagsMessage, string.Join(", ", parsedTags.InvalidNames)));

				if (!string.IsNullOrEmpty(source) && !PicturesHelper.IsValidSource(source))
					return new HttpStatusCodeResult(400, UploadResources.InvalidSourceMessage);

				int pictureID;
				var res =
					AddPicture(
						file.InputStream,
						parsedTags.Names,
						parsedTags.TagmeIncluded,
						rating ?? Config.Default.DefaultRating,
						source,
						replaceFile == true,
						out pictureID);

				return Content(res.GetUploadResultMessage());
			}
			catch (Exception exception)
			{
				return new HttpStatusCodeResult(500, exception.Message);
			}
		}

		[Authorize(Roles = "Contributor")]
		public ActionResult CheckExists(string hash, HttpPostedFileBase file)
		{
			Picture picture;
			if (!string.IsNullOrEmpty(hash))
				picture = _picturesService.Find(new PicturesQuery { BitmapHash = ByteHelper.FromHexString(hash) }).SingleOrDefault();
			else if (file != null)
				picture = _picturesService.CheckExists(file.InputStream);
			else
				return new HttpStatusCodeResult(400, "Hash or file is not specified.");

			return picture == null ? Content("NOT_FOUND") : Content(picture.ID.ToString());
		}

		private PictureAddingResult AddPicture(
			Stream inputStream,
			IEnumerable<string> tags,
			bool requiresTagging,
			Rating rating,
			string source,
			bool replaceFile,
			out int pictureID)
		{
			var identity = Request.CreateIdentity(_authenticationService.CurrentUserID);
			var status = Request.GetDefaultStatus();

			var result = _picturesService.Add(inputStream, status, identity, replaceFile, out pictureID);

			if (result == PictureAddingResult.Added
				|| result == PictureAddingResult.Resurrected
				|| (result == PictureAddingResult.AlreadyExists && identity.UserID != null))
			{
				var update = new PictureUpdate();
				if (tags != null)
					update.AddTags = _tagsService.GetAssignTagIDs(tags, status, identity, true);
				if (requiresTagging && result == PictureAddingResult.Added)
					update.RequiresTagging = true;
				if (rating != Rating.Unrated)
					update.Rating = rating;
				if (!string.IsNullOrWhiteSpace(source))
					update.Source = source;
				_picturesService.Update(pictureID, update, identity);
			}

			return result;
		}
	}
}