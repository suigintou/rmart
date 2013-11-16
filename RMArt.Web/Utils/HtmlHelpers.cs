using System;
using System.Text;
using System.Web.Mvc;
using RMArt.Web.Resources;

namespace RMArt.Web
{
	public static class HtmlHelpers
	{
		public static MvcHtmlString PageLinks(
			this HtmlHelper html,
			Func<int, string> urlGetter,
			int currentPage,
			int totalPages,
			int linksAround = 3)
		{
			if (totalPages < 1)
				throw new ArgumentOutOfRangeException("totalPages");
			if (currentPage < 1 || currentPage > totalPages)
				throw new ArgumentOutOfRangeException("currentPage");
			if (linksAround < 0)
				throw new ArgumentOutOfRangeException("linksAround");

			linksAround += 2;
			var start = currentPage - linksAround;
			var end = currentPage + linksAround;

			var sb = new StringBuilder();

			//prev
			PageLink(sb, SharedResources.PagingPrev, currentPage > 1 ? urlGetter(currentPage - 1) : null);

			if (start > 1)
			{
				//first
				PageLink(sb, "1", urlGetter(1));
				start++;

				//left overflow
				if (start > 2)
				{
					PageLink(sb, "...");
					start++;
				}
			}

			//calculations
			else if (start < 1)
			{
				end += Math.Abs(start) + 1;
				start = 1;
			}
			if (end > totalPages)
			{
				if (start > 1)
					start = Math.Max(1, start - (end - totalPages));
				end = totalPages;
			}
			else if (end < totalPages)
			{
				end--;
				if (end <= totalPages - 1)
					end--;
			}

			//pages
			for (var i = start; i <= end; i++)
				PageLink(sb, i.ToString(), urlGetter(i), i == currentPage);

			if (end < totalPages)
			{
				//right overflow
				if (end < totalPages - 1)
					PageLink(sb, "...");

				//last
				PageLink(sb, totalPages.ToString(), urlGetter(totalPages));
			}

			//next
			PageLink(sb, SharedResources.PagingNext, currentPage < totalPages ? urlGetter(currentPage + 1) : null);

			return MvcHtmlString.Create(sb.ToString());
		}

		private static void PageLink(StringBuilder sb, string text, string url = null, bool selected = false)
		{
			if (url == null)
				sb.Append("<li class=\"disabled\"><span>").Append(text).Append("</span></li>");
			else if (selected)
				sb.Append("<li class=\"active\"><a href=\"").Append(url).Append("\">").Append(text).Append("</a></li>");
			else
				sb.Append("<li><a href=\"").Append(url).Append("\">").Append(text).Append("</a></li>");
		}
	}
}