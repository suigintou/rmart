using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using RMArt.Core;
using RMArt.Web.Models;

namespace RMArt.Web.Controllers
{
	public class ContentController : Controller
	{
		private readonly IContentRepository _contentRepository;

		public ContentController(IContentRepository contentRepository)
		{
			_contentRepository = contentRepository;
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Index()
		{
			return View(_contentRepository.Find().ToArray());
		}

		public ActionResult Page(string name)
		{
			var page =
				Core.GlobalizationHelper.GetLocalized(
					c => _contentRepository.GetPage(name, c.LCID),
					() => null,
					CultureInfo.CurrentUICulture,
					GlobalizationHelper.DefaultCulture);
			return page != null ? View("Display", page) : (ActionResult)HttpNotFound();
		}

		public ActionResult Inline(string name)
		{
			var entry =
				Core.GlobalizationHelper.GetLocalized(
					c => _contentRepository.GetPage(name, c.LCID),
					() => null,
					CultureInfo.CurrentUICulture,
					GlobalizationHelper.DefaultCulture);
			return Content(entry != null ? entry.Content : "");
		}

		public ActionResult Display(int id)
		{
			var page = _contentRepository.GetPage(id);
			return page != null ? View(page) : (ActionResult)HttpNotFound();
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Create()
		{
			return View("Edit", new ContentPageEditModel { CultureID = GlobalizationHelper.DefaultCulture.LCID });
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Edit(int id)
		{
			var p = _contentRepository.GetPage(id);
			if (p == null)
				return HttpNotFound();

			return View(
				new ContentPageEditModel
				{
					ID = p.ID,
					Name = p.Name,
					CultureID = p.CultureID,
					Title = p.Title,
					Content = p.Content
				});
		}

		[Authorize(Roles = "Administrator"), HttpPost, ValidateInput(false)]
		public ActionResult Edit(ContentPageEditModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var page =
				new ContentPage
				{
					ID = model.ID ?? -1,
					Name = model.Name.Trim(),
					CultureID = model.CultureID,
					Title = model.Title.Trim(),
					Content = model.Content ?? ""
				};

			if (page.ID == -1)
				page.ID = _contentRepository.Add(page);
			else
				_contentRepository.Update(page);

			return RedirectToAction("Edit", new { id = page.ID });
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Delete(int id, string returnUrl)
		{
			var p = _contentRepository.GetPage(id);
			if (p == null)
				return HttpNotFound();

			var model =
				new ContentPageDeleteModel
				{
					ID = p.ID,
					Name = p.Name,
					CultureID = p.CultureID,
					ReturnUrl = returnUrl
				};

			return View(model);
		}

		[Authorize(Roles = "Administrator"), HttpPost]
		public ActionResult Delete(ContentPageDeleteModel model)
		{
			_contentRepository.Delete(model.ID);

			return this.ReturnOrRedirect(model.ReturnUrl);
		}
	}
}