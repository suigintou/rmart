using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using RMArt.Core;

namespace RMArt.Web
{
	public static class UrlHelperExtensions
	{
		public static string ToAbsolute(this UrlHelper url, string siteRelativeUrl)
		{
			return new Uri(GetBaseUrl(url), siteRelativeUrl).AbsoluteUri;
		}

		public static string PictureViewer(this UrlHelper url, int id)
		{
			return url.Action("Viewer", "Pictures", new { id });
		}

		public static string PictureFileNameFromHash(byte[] hash, ImageFormat format)
		{
			return hash.ToHexString() + "." + format.GetFileExtension();
		}

		public static string PictureFileNameFromTags(int id, IEnumerable<string> tags, ImageFormat format)
		{
			return string.Concat(id.ToString(), TagsUrl(tags, true), ".", format.GetFileExtension());
		}

		public static string TagsUrl(IEnumerable<string> tags, bool startWithDelimeter = false)
		{
			var sb = new StringBuilder();
			var isFirst = !startWithDelimeter;
			foreach (var tag in tags)
			{
				var encodedTag = TagUrlName(tag);
				if (encodedTag.Length == 0)
					continue;
				if (sb.Length + encodedTag.Length < 100)
				{
					if (!isFirst)
						sb.Append('-');
					else
						isFirst = false;
					sb.Append(encodedTag);
				}
				else
					break;
			}
			return Uri.EscapeDataString(sb.ToString());
		}

		public static string TagUrlName(string originalName, char wordDelimiter = '-')
		{
			var sb = new StringBuilder(originalName.Length);
			var delimeter = true;
			foreach (var c in originalName)
			{
				if (char.IsLetterOrDigit(c))
				{
					if (!char.IsDigit(c) && (c < 'A' || c > 'z'))
						return "";
					delimeter = false;
					sb.Append(c);
				}
				if (char.IsWhiteSpace(c) && !delimeter)
				{
					delimeter = true;
					sb.Append(wordDelimiter);
				}
			}
			return sb.ToString();
		}

		public static string PictureSrc(this UrlHelper url, int id, string fileName = null)
		{
			return url.Action("Src", "Pictures", new { id, fileName });
		}

		public static string ThumbSrc(this UrlHelper url, int id, int size)
		{
			return url.Action("Thumb", "Pictures", new { id, size });
		}

		public static string Gallery(
			this UrlHelper url,
			string q = null,
			PicturesSortOrder? sortBy = null,
			int? p = null,
			bool addRnd = false)
		{
			var routeValues = new RouteValueDictionary();
			if (q != null)
				routeValues.Add("q", q);
			if (sortBy.HasValue)
				routeValues.Add("sortBy", sortBy.Value.CreateQueryString(Config.Default.DefaultPicturesSortOrder));
			if (p.HasValue && p != 1)
				routeValues.Add("p", p.Value);
			if (addRnd)
				routeValues.Add("rnd", DateTime.Now.Millisecond);
			return url.Action("Index", "Pictures", routeValues);
		}

		public static string Tags(
			this UrlHelper url,
			string q = null,
			TagType? type = null,
			ModerationStatus? status = null,
			TagsSortOrder? sortBy = null,
			int? p = null)
		{
			var routeValues = new RouteValueDictionary();
			if (!string.IsNullOrEmpty(q))
				routeValues.Add("q", q);
			if (type != null)
				routeValues.Add("type", type);
			if (status != null)
				routeValues.Add("status", status);
			if (sortBy != null && sortBy != Config.Default.DefaultTagsSortOrder)
				routeValues.Add("sortBy", sortBy);
			if (p.HasValue && p != 1)
				routeValues.Add("p", p.Value);
			return url.Action("Index", "Tags", routeValues);
		}

		public static string History(
			this UrlHelper url,
			ObjectType? targetType = null,
			int? targetID = null,
			HistoryField? actionType = null,
			string user = null,
			int? p = null)
		{
			var routeValues = new RouteValueDictionary();
			if (targetType != null)
				routeValues.Add("targetType", targetType);
			if (targetID != null)
				routeValues.Add("targetID", targetID);
			if (actionType != null)
				routeValues.Add("actionType", actionType);
			if (!string.IsNullOrEmpty(user))
				routeValues.Add("user", user);
			if (p.HasValue && p != 1)
				routeValues.Add("p", p.Value);
			return url.Action("Index", "History", routeValues);
		}

		public static string Report(this UrlHelper url, ObjectReference target, string returnUrl = null)
		{
			var routeValues = new RouteValueDictionary();
			routeValues.Add("targetType", target.Type);
			routeValues.Add("targetID", target.ID);
			if (!string.IsNullOrEmpty(returnUrl))
				routeValues.Add("returnUrl", returnUrl);
			return url.Action("Create", "Reports", routeValues);
		}

		public static string Object(this UrlHelper url, ObjectReference target)
		{
			switch (target.Type)
			{
				case ObjectType.Picture:
					return url.PictureViewer(target.ID);
				case ObjectType.Tag:
					return url.Action("Details", "Tags", new { target.ID });
				case ObjectType.Message:
					return url.Action("Index", "Discussions") + "#message-" + target.ID;
				default:
					throw new NotSupportedException();
			}
		}

		private static Uri GetBaseUrl(this UrlHelper url)
		{
			var contextUri = new Uri(url.RequestContext.HttpContext.Request.Url, url.RequestContext.HttpContext.Request.RawUrl);
			var realmUri = new UriBuilder(contextUri) { Path = url.RequestContext.HttpContext.Request.ApplicationPath, Query = null, Fragment = null };
			return realmUri.Uri;
		}
	}
}