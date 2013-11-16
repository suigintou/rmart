using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.Web.Helpers;
using RMArt.Core;
using RMArt.Web.Models;
using RMArt.Web.Resources;

namespace RMArt.Web.Controllers
{
	public class AccountController : Controller
	{
		private readonly IPicturesService _picturesService;
		private readonly IUsersService _usersService;
		private readonly IAuthenticationService _authenticationService;
		private readonly ITicketsService _ticketsService;

		public AccountController(
			IAuthenticationService authenticationService,
			IPicturesService picturesService,
			IUsersService usersService,
			ITicketsService ticketsService)
		{
			_usersService = usersService;
			_ticketsService = ticketsService;
			_picturesService = picturesService;
			_authenticationService = authenticationService;
		}

		public ActionResult Index()
		{
			return RedirectToAction("View");
		}

		public new ActionResult View(string login)
		{
			if (string.IsNullOrEmpty(login))
				return
					_authenticationService.CurrentUserID != null
						? RedirectToAction("View", new { login = _usersService.GetByID(_authenticationService.CurrentUserID.Value).Login })
						: RedirectToAction("LogIn", new { returnUrl = Request.RawUrl });

			var user = _usersService.GetByLogin(login);
			if (user == null)
				return HttpNotFound();

			var model =
				new AccountModel
					{
						Self = _authenticationService.CurrentUserID == user.ID,
						Login = user.Login,
						Name = user.Name,
						Email = user.Email,
						Role = user.Role,
						PicturesUploaded =
							_picturesService.Find(
								new PicturesQuery
								{
									AllowedStatuses = new SortedSet<ModerationStatus> { ModerationStatus.Accepted },
									UploadedBy = user.ID
								}).Count(),
						RatesGiven =
							_picturesService.Find(
								new PicturesQuery
								{
									AllowedStatuses = new SortedSet<ModerationStatus> { ModerationStatus.Accepted },
									RatedBy = user.ID
								}).Count(),
						PicturesFavorited =
							_picturesService.Find(
								new PicturesQuery
								{
									AllowedStatuses = new SortedSet<ModerationStatus> { ModerationStatus.Accepted },
									FavoritedBy = user.ID
								}).Count(),
						RegistrationDate = user.RegistrationDate
					};

			return View(model);
		}

		[Authorize]
		public ActionResult AccountSettings()
		{
			// ReSharper disable PossibleInvalidOperationException
			var user = _usersService.GetByID(_authenticationService.CurrentUserID.Value);
			// ReSharper restore PossibleInvalidOperationException
			return
				View(
					new AccauntSettingsModel
					{
						Login = user.Login,
						Email = user.Email
					});
		}

		[Authorize, HttpPost, ValidateAntiForgeryToken]
		public ActionResult AccountSettings(AccauntSettingsModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			// ReSharper disable PossibleInvalidOperationException
			var userID = _authenticationService.CurrentUserID.Value;
			// ReSharper restore PossibleInvalidOperationException

			if (!UsersValidation.ValidateLogin(model.Login))
			{
				ModelState.AddModelError("Login", AccountResources.InvalidLoginMessage);
				return View(model);
			}
			if (!_usersService.IsLoginAvaliable(userID, model.Login))
			{
				ModelState.AddModelError("Login", AccountResources.LoginTakenMessage);
				return View(model);
			}
			if (!_usersService.IsEmailAvaliable(userID, model.Email))
			{
				ModelState.AddModelError("Email", AccountResources.EmailTakenMessage);
				return View(model);
			}
			if (!_usersService.CheckPassword(userID, model.Password))
			{
				ModelState.AddModelError("Password", AccountResources.WrongPasswordMessage);
				return View(model);
			}

			_usersService.UpdateLogin(userID, model.Login);
			var isEmailChanged = _usersService.UpdateEmail(userID, model.Email);
			_usersService.UpdateIsPrivateProfile(userID, model.PrivateProfile);

			if (isEmailChanged)
			{
				_usersService.UpdateIsEmailConfirmed(userID, false);
				if (Config.Default.EmailConfirmationEnabled)
				{
					_authenticationService.LogOut();
					SendConfirmationEmail(userID);
					return View("EmailNotConfirmed", model: model.Email);
				}
			}

			return RedirectToAction("Index");
		}

		[Authorize]
		public ActionResult EditProfile()
		{
			// ReSharper disable PossibleInvalidOperationException
			var user = _usersService.GetByID(_authenticationService.CurrentUserID.Value);
			// ReSharper restore PossibleInvalidOperationException
			return View(new EditProfileModel { Name = user.Name, Email = user.Email });
		}

		[Authorize, HttpPost, ValidateAntiForgeryToken]
		public ActionResult EditProfile(EditProfileModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			_usersService.UpdateName(
				// ReSharper disable PossibleInvalidOperationException
					_authenticationService.CurrentUserID.Value,
				// ReSharper restore PossibleInvalidOperationException
					model.Name);

			return RedirectToAction("Index");
		}

		public ActionResult LogIn(string returnUrl)
		{
			if (Request.IsAuthenticated)
				return RedirectToAction("Index");

			return View(new LogInModel { ReturnUrl = returnUrl });
		}

		[HttpPost]
		public ActionResult LogIn(LogInModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			User user;
			var res = _authenticationService.LogIn(model.LoginOrEmail, model.Password, model.Remember, out user);

			if (!res)
			{
				ModelState.AddModelError("", AccountResources.IncorrectIdentityOrPasswordMessage);
				return View(model);
			}

			if (Config.Default.EmailConfirmationEnabled && !user.IsEmailConfirmed)
			{
				_authenticationService.LogOut();
				return View("EmailNotConfirmed", model: user.Email);
			}

			return this.ReturnOrRedirect(model.ReturnUrl);
		}

		[HttpPost]
		public ActionResult LogOut()
		{
			_authenticationService.LogOut();
			return Redirect("~");
		}

		public ActionResult Registration()
		{
			if (Request.IsAuthenticated)
				return RedirectToAction("Index");

			return View(new RegistrationModel());
		}

		[HttpPost]
		public ActionResult Registration(RegistrationModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			if (!UsersValidation.ValidateLogin(model.Login))
			{
				ModelState.AddModelError("Login", AccountResources.InvalidLoginMessage);
				return View(model);
			}
			if (!_usersService.IsLoginAvaliable(null, model.Login))
			{
				ModelState.AddModelError("Login", AccountResources.LoginTakenMessage);
				return View(model);
			}
			if (!_usersService.IsEmailAvaliable(null, model.Email))
			{
				ModelState.AddModelError("Email", AccountResources.EmailTakenMessage);
				return View(model);
			}

			if (!ReCaptcha.Validate())
			{
				ModelState.AddModelError("Captcha", SharedResources.InvalidCaptchaMessage);
				return View(model);
			}

			var id = _usersService.CreateUser(model.Login, model.Name, model.Email, false, model.Password, UserRole.User);

			if (Config.Default.EmailConfirmationEnabled)
			{
				SendConfirmationEmail(id);
				return View("EmailNotConfirmed", model: model.Email);
			}
			else
			{
				User user;
				_authenticationService.LogIn(model.Login, model.Password, false, out user);
				return RedirectToAction("View");
			}
		}

		public ActionResult ChangePassword(string key)
		{
			if (!string.IsNullOrEmpty(key))
			{
				if (_ticketsService.Check(TicketType.PasswordReset, key) < 0)
					return RedirectToAction("PasswordRecovery");
			}
			else
			{
				if (_authenticationService.CurrentUserID == null)
					return RedirectToAction("LogIn", new { returnUrl = Request.RawUrl });
			}

			return View(new ChangePasswordModel { Key = key });
		}

		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			if (!string.IsNullOrEmpty(model.Key))
			{
				var userID = _ticketsService.Check(TicketType.PasswordReset, model.Key);
				if (userID < 0)
					return RedirectToAction("PasswordRecovery");

				_ticketsService.Expire(model.Key);
				_usersService.UpdatePassword(userID, model.NewPassword);

				return RedirectToAction("LogIn");
			}
			else
			{
				var userID = _authenticationService.CurrentUserID;
				if (userID == null)
					return RedirectToAction("LogIn", new { returnUrl = Request.RawUrl });
				if (!_usersService.CheckPassword(userID.Value, model.OldPassword))
				{
					ModelState.AddModelError("OldPassword", AccountResources.WrongPasswordMessage);
					return View(model);
				}

				_usersService.UpdatePassword(userID.Value, model.NewPassword);

				return RedirectToAction("Index");
			}
		}

		public ActionResult PasswordRecovery()
		{
			if (Request.IsAuthenticated)
				return RedirectToAction("Index");

			return View(new PasswordRecoveryModel { LoginOrEmail = "" });
		}

		[HttpPost]
		public ActionResult PasswordRecovery(PasswordRecoveryModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			if (!ReCaptcha.Validate())
			{
				ModelState.AddModelError("Captcha", SharedResources.InvalidCaptchaMessage);
				return View(model);
			}

			var user = _usersService.GetByLoginOrEmail(model.LoginOrEmail);
			if (user != null)
			{
				_ticketsService.Expire(TicketType.PasswordReset, user.ID);
				var ticket =
					_ticketsService.Create(
						TicketType.PasswordReset,
						user.ID,
					// ReSharper disable AssignNullToNotNullAttribute
						IPAddress.Parse(Request.UserHostAddress));
				// ReSharper restore AssignNullToNotNullAttribute

				MailHelper.Send(
					user.Email,
					string.Format(AccountResources.PasswordRecoveryMessageSubject, Config.Default.SiteTitle),
					string.Format(
						AccountResources.PasswordRecoveryMessageBody,
						Url.ToAbsolute(Url.Action("ChangePassword", new { key = ticket.Token }))));
			}
			return base.View("PasswordRecoveryEmailSent");
		}

		public ActionResult ConfirmEmail(string key)
		{
			var userID = _ticketsService.Check(TicketType.EmailValidation, key);
			if (userID < 0)
				return new HttpStatusCodeResult(400, "Invalid comfirmation key.");

			_ticketsService.Expire(key);
			_usersService.UpdateIsEmailConfirmed(userID, true);

			return RedirectToAction("LogIn");
		}

		public ActionResult ResendConfirmation()
		{
			if (Request.IsAuthenticated)
				return RedirectToAction("Index");

			return View();
		}

		[HttpPost]
		public ActionResult ResendConfirmation(ResendConfirmationModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			if (!ReCaptcha.Validate())
			{
				ModelState.AddModelError("Captcha", SharedResources.InvalidCaptchaMessage);
				return View(model);
			}

			User user;
			if (!_usersService.CheckCredentials(model.LoginOrEmail, model.Password, out user))
			{
				ModelState.AddModelError("Password", AccountResources.WrongPasswordMessage);
				return View(model);
			}
			if (user.IsEmailConfirmed)
			{
				ModelState.AddModelError("", AccountResources.EmailAlreadyConfirmedMessage);
				return View(model);
			}

			if (model.NewEmail != null)
			{
				if (!_usersService.IsEmailAvaliable(user.ID, model.NewEmail))
				{
					ModelState.AddModelError("NewEmail", AccountResources.EmailTakenMessage);
					return View(model);
				}
				_usersService.UpdateEmail(user.ID, model.NewEmail);
			}

			SendConfirmationEmail(user.ID);

			return View("EmailNotConfirmed", model: model.NewEmail ?? user.Email);
		}

		[ChildActionOnly]
		public ActionResult ShowUserName(int? id)
		{
			var user = id != null ? _usersService.GetByID(id.Value) : null;
			ViewBag.Login = user != null ? user.Login : null;
			return PartialView();
		}

		[Authorize]
		public ActionResult Favorites()
		{
			return RedirectToAction(
				"Index",
				"Pictures",
				new
				{
					q =
						PicturesSearchQueryHelper.FavoritedByKey
							+ PicturesSearchQueryHelper.KeyValueDelimeter
						// ReSharper disable PossibleInvalidOperationException
							+ _usersService.GetByID(_authenticationService.CurrentUserID.Value).Login
					// ReSharper restore PossibleInvalidOperationException
				});
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult ManageUsers(int? p)
		{
			var users = _usersService.Find();
			var totalCount = users.Count();

			const int pageSize = 30;
			var pagesCount = (int)Math.Ceiling((float)totalCount / pageSize);
			var pageChecked = (p ?? 1).RestrictRange(1, pagesCount);

			users = users
				.OrderByDescending(u => u.ID)
				.Skip((pageChecked - 1) * pageSize)
				.Take(pageSize);

			return View(
				new ManageUsersModel
				{
					Users = users.ToArray(),
					TotalCount = totalCount,
					TotalPages = pagesCount,
					CurrentPage = pageChecked,
				});
		}

		[Authorize(Roles = "Administrator"), HttpPost, ValidateAntiForgeryToken]
		public ActionResult Edit(int[] ids, UserRole? role, string returnUrl)
		{
			if (ids != null && ids.Length > 0)
				foreach (var id in ids)
					if (role != null)
						_usersService.UpdateRole(id, role.Value);
			return this.ReturnOrRedirect(returnUrl);
		}

		private void SendConfirmationEmail(int userID)
		{
			var user = _usersService.GetByID(userID);

			_ticketsService.Expire(TicketType.EmailValidation, user.ID);
			var ticket =
				_ticketsService.Create(
					TicketType.EmailValidation,
					user.ID,
				// ReSharper disable AssignNullToNotNullAttribute
					IPAddress.Parse(Request.UserHostAddress));
			// ReSharper restore AssignNullToNotNullAttribute

			MailHelper.Send(
				user.Email,
				string.Format(AccountResources.EmailConfirmationMessageSubject, Config.Default.SiteTitle),
				string.Format(
					AccountResources.EmailConfirmationMessageBody,
					user.Name,
					Config.Default.SiteTitle,
					Url.ToAbsolute(Url.Action("ConfirmEmail", new { key = ticket.Token }))));
		}
	}
}