using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using RMArt.Core;

namespace RMArt.Client.Core
{
	public static class PicturesClient
	{
		public static readonly string[] Ratings = { "Unrated", "SFW", "R15", "R18", "R18G" };

		public static string Upload(
			SiteConfig site,
			AuthenticationCookie authenticationCookie,
			byte[] imageData,
			string tags = null,
			string rating = null,
			string source = null,
			bool? replaceFile = null)
		{
			if (authenticationCookie == null)
				throw new ArgumentNullException("authenticationCookie");
			if (imageData == null)
				throw new ArgumentNullException("imageData");

			var uploadReq = WebRequest.CreateHttp(site.UploadUrl);
			uploadReq.Method = "POST";
			uploadReq.Timeout = 1000000;
			uploadReq.CookieContainer = new CookieContainer();
			uploadReq.CookieContainer.Add(authenticationCookie.Cookie);
			using (var form = new MultiFormBuilder(uploadReq))
			{
				form.AppendFormData("file", imageData, "image", new KeyValuePair<string, string>("filename", "image"));
				if (tags != null)
					form.AppendFormData("tags", tags);
				if (rating != null)
					form.AppendFormData("rating", rating);
				if (source != null)
					form.AppendFormData("source", source);
				if (replaceFile != null)
					form.AppendFormData("replaceFile", replaceFile.Value.ToString());
			}
			using (var uploadResp = uploadReq.GetResponse())
			using (var uploadRespStream = uploadResp.GetResponseStream())
			using (var uploadRespStreamReader = new StreamReader(uploadRespStream))
				return uploadRespStreamReader.ReadToEnd();
		}

		public static int? CheckByHash(SiteConfig site, byte[] hash, AuthenticationCookie authenticationCookie)
		{
			if (site == null)
				throw new ArgumentNullException("site");
			if (hash == null)
				throw new ArgumentNullException("hash");
			if (authenticationCookie == null)
				throw new ArgumentNullException("authenticationCookie");

			using (var webClient = new WebClient())
			{
				webClient.Headers.Add(HttpRequestHeader.Cookie, authenticationCookie.Cookie.ToString());

				var respContent = webClient.DownloadString(site.CheckByHashUrl.Replace("{hash}", hash.ToHexString()));

				if (respContent.Equals(site.NotExistsCheckResponse, StringComparison.OrdinalIgnoreCase))
					return null;

				int id;
				if (int.TryParse(respContent, out id))
					return id;

				throw new NotSupportedException("Unknown responce from server.");
			}
		}

		public static int[] Search(SiteConfig site, AuthenticationCookie authenticationCookie, string query = "", string sortBy = "")
		{
			var req = WebRequest.CreateHttp(site.SearchUrl.Replace("{query}", query).Replace("{sortBy}", sortBy));
			req.CookieContainer = new CookieContainer();
			req.CookieContainer.Add(authenticationCookie.Cookie);

			using (var resp = req.GetResponse())
			using (var responseStream = resp.GetResponseStream())
			using (var respStreamReader = new StreamReader(responseStream))
			{
				var res = respStreamReader.ReadToEnd();
				if (res.StartsWith(site.SearchErrorPrefix))
					throw new PicturesSearchException(res);
				return res.Split(';').Select(int.Parse).ToArray();
			}
		}

		public static void Edit(SiteConfig site, AuthenticationCookie authenticationCookie, int[] ids, string rating = null, string source = null)
		{
			if (site == null)
				throw new ArgumentNullException("site");
			if (authenticationCookie == null)
				throw new ArgumentNullException("authenticationCookie");
			if (ids == null)
				throw new ArgumentNullException("ids");

			if (ids.Length == 0)
				return;

			var req = WebRequest.CreateHttp(site.EditUrl);
			req.Method = "POST";
			req.CookieContainer = new CookieContainer();
			req.CookieContainer.Add(authenticationCookie.Cookie);
			using (var form = new MultiFormBuilder(req))
			{
				foreach (var id in ids)
					form.AppendFormData("ids", id.ToString());
				if (rating != null)
					form.AppendFormData("rating", rating);
				if (source != null)
					form.AppendFormData("source", source);
				form.AppendFormData("tagsToAdd", "");
				form.AppendFormData("tagsToRemove", "");
			}
			req.GetResponse().Close();
		}

		public static string GetPictureUrl(SiteConfig siteConfig, int pictureID)
		{
			return siteConfig.PictureUrl.Replace("{id}", pictureID.ToString());
		}
	}
}