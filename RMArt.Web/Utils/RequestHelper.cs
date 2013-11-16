using System;
using System.Globalization;
using System.Web;

namespace RMArt.Web
{
	public static class RequestHelper
	{
		public static bool IsClientCached(this HttpRequestBase request, DateTime lastModifiedTime)
		{
			var header = request.Headers["If-Modified-Since"];
			if (header != null)
			{
				lastModifiedTime = lastModifiedTime.ToUniversalTime();
				lastModifiedTime =
					new DateTime(
						lastModifiedTime.Year,
						lastModifiedTime.Month,
						lastModifiedTime.Day,
						lastModifiedTime.Hour,
						lastModifiedTime.Minute,
						lastModifiedTime.Second,
						lastModifiedTime.Kind);
				DateTime clientCachedTime;
				if (DateTime.TryParse(header, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out clientCachedTime))
					return clientCachedTime >= lastModifiedTime;
			}
			return false;
		}

		public static bool IsReffered(this HttpRequestBase request)
		{
			return request.UrlReferrer != null 
				&& !string.Equals(request.UrlReferrer.DnsSafeHost, request.Url.DnsSafeHost, StringComparison.OrdinalIgnoreCase);
		}
	}
}