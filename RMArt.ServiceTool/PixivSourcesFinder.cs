using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using RMArt.Core;

namespace RMArt.ServiceTool
{
	public static class PixivSourcesFinder
	{
		private static readonly Regex _saucenaoPixivUrlsRegex = new Regex(@"(?<!result hidden)http://www\.pixiv\.net/member_illust\.php\?mode=medium&amp;illust_id=\d+", RegexOptions.Compiled);
		private static readonly Regex _pixivAuthorNicknameRegex = new Regex("(?<=/img/)[^/]+", RegexOptions.Compiled);
		private static readonly Regex _pixivPictureUrlRegex = new Regex("(?<=<body><img src=\")[^\"]+", RegexOptions.Compiled);

		public static void FindPixivSources(IPicturesService picturesService, ITagsService tagsService, IProgressIndicator progressIndicator = null)
		{
			var ids = picturesService.Find(new PicturesQuery { SortBy = PicturesSortOrder.Oldest }).Select(_ => _.ID).ToArray();
			var authCookies = AuthPixiv("ginfag", "desu123");
			var interval = 10000;
			for (var i = 0; i < ids.Length; i++)
			{
				if (progressIndicator != null && i % 100 == 0)
					progressIndicator.ReportProgress(ids.Length, i);

				var id = ids[i];
				try
				{
					progressIndicator.Message("pic " + id);
					string[] pixivUrls;
					try
					{
						pixivUrls = GetPixivUrlFromSaucenao("http://rmart.org/" + id + "/Src/" + id);
					}
					catch (WebException sauceNaoWebException)
					{
						if (sauceNaoWebException.Message.Contains("Too Many Requests"))
						{
							interval *= 2;
							progressIndicator.Message("Too many requests. New interval: " + interval);
							Thread.Sleep(interval);
							i--;
							continue;
						}
						throw;
					}
					foreach (var pixivUrl in pixivUrls)
					{
						var pictureUrl = GetPixivPictureUrl(pixivUrl, authCookies);
						if (string.IsNullOrEmpty(pictureUrl))
							continue;
						progressIndicator.Message("pic url rcvd " + pixivUrl);

						var author = GetPixivAuthor(pictureUrl);
						progressIndicator.Message("author " + author);

						Stream pictureStream;
						var req = WebRequest.CreateHttp(pictureUrl);
						req.Referer = pixivUrl;
						req.CookieContainer = authCookies;
						using (var resp = req.GetResponse())
						using (var respStream = resp.GetResponseStream())
							pictureStream = respStream.CopyToMemoryStream();
						progressIndicator.Message("downloaded");

						if (picturesService.CheckExists(pictureStream) == null)
							continue;
						progressIndicator.Message("checked");
						pictureStream.Seek(0, SeekOrigin.Begin);

						int addedID;
						var res = picturesService.Add(pictureStream, ModerationStatus.Accepted, Identity.Empty, true, out addedID);
						if (res != PictureAddingResult.AlreadyExists)
							throw new ApplicationException("Picture not exists.");
						progressIndicator.Message("added " + addedID);
						picturesService.Update(
							addedID,
							new PictureUpdate
							{
								AddTags = tagsService.GetAssignTagIDs(new[] { TagType.Artist + ":" + author }, ModerationStatus.Accepted, Identity.Empty, true),
								Source = pixivUrl
							},
							Identity.Empty);
						progressIndicator.Message("updated");
					}
				}
				catch (Exception ex)
				{
					if (progressIndicator != null)
						progressIndicator.Message("Error processing picture " + id + ": " + ex);
				}

				Thread.Sleep(interval);
			}
		}

		public static void FindPixivAuthors(IPicturesService picturesService, ITagsService tagsService, IProgressIndicator progressIndicator = null)
		{
			var authCookies = AuthPixiv("ginfag", "desu123");
			var pics = picturesService.Find().Where(p => p.Source.StartsWith("http://www.pixiv.net")).Select(p => new { p.ID, p.Source }).ToArray();
			var current = -1;
			foreach (var p in pics)
			{
				current++;
				if (progressIndicator != null && current % 100 == 0)
					progressIndicator.ReportProgress(pics.Length, current);

				var id = p.ID;
				var pixivUrl = p.Source;
				try
				{
					var pictureUrl = GetPixivPictureUrl(pixivUrl, authCookies);
					if (string.IsNullOrEmpty(pictureUrl))
						continue;

					var author = GetPixivAuthor(pictureUrl);

					//Stream pictureStream;
					//var req = WebRequest.CreateHttp(pictureUrl);
					//req.Referer = pixivUrl;
					//req.CookieContainer = authCookies;
					//using (var resp = req.GetResponse())
					//using (var respStream = resp.GetResponseStream())
					//	pictureStream = respStream.CopyToMemoryStream();

					//int addedID;
					//picturesService.Add(pictureStream, ModerationStatus.Accepted, Identity.Empty, true, out addedID);
					//if (addedID < 0)
					//	continue;

					picturesService.Update(
						p.ID,
						new PictureUpdate
						{
							AddTags = tagsService.GetAssignTagIDs(new[] { TagType.Artist + ":" + author }, ModerationStatus.Accepted, Identity.Empty, true),
							Source = pixivUrl
						},
						Identity.Empty);
				}
				catch (Exception ex)
				{
					if (progressIndicator != null)
						progressIndicator.Message("Error processing picture " + id + ": " + ex);
				}
			}
		}

		public static string[] GetPixivUrlFromSaucenao(string sourceUrl)
		{
			var sauceNaoRespText = Req("http://saucenao.com/search.php?db=999&url=" + Uri.EscapeDataString(sourceUrl));
			return _saucenaoPixivUrlsRegex.Matches(sauceNaoRespText).Cast<Match>().Select(_ => WebUtility.HtmlDecode(_.Value)).ToArray();
		}

		public static CookieContainer AuthPixiv(string login, string password)
		{
			var authCookie = new CookieContainer();
			var req = WebRequest.CreateHttp("http://www.pixiv.net/login.php");
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.CookieContainer = authCookie;
			req.AllowAutoRedirect = false;
			using (var reqStream = req.GetRequestStream())
			using (var reqStreamWriter = new StreamWriter(reqStream))
				reqStreamWriter.Write("mode=login&pixiv_id=" + login + "&pass=" + password + "&skip=1");
			using (var resp = (HttpWebResponse)req.GetResponse())
				if (resp.StatusCode != HttpStatusCode.Found || resp.Headers[HttpResponseHeader.Location] != "http://www.pixiv.net/mypage.php")
					return null;
			return authCookie;
		}

		public static string GetPixivPictureUrl(string pixivUrl, CookieContainer cookies)
		{
			var bigPicturePageText = Req(pixivUrl.Replace("medium", "big"), pixivUrl, cookies);
			if (bigPicturePageText.Contains("The following work is either deleted, or the ID does not exist."))
				return null;
			var pictureUrl = _pixivPictureUrlRegex.Match(bigPicturePageText).Value;
			return pictureUrl;
		}

		public static string GetPixivAuthor(string pixivPictureUrl)
		{
			var authorLogin = _pixivAuthorNicknameRegex.Match(pixivPictureUrl).Value;
			return authorLogin;
		}

		private static string Req(string url, string referer = null, CookieContainer cookies = null)
		{
			var req = WebRequest.CreateHttp(url);
			req.Referer = referer;
			req.CookieContainer = cookies;
			req.AllowAutoRedirect = false;
			using (var resp = req.GetResponse())
			using (var respStream = resp.GetResponseStream())
			using (var respStreamReader = new StreamReader(respStream))
				return respStreamReader.ReadToEnd();
		}
	}
}