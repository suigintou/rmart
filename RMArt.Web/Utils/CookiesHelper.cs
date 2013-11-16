using System;
using System.Web;

namespace RMArt.Web
{
	public static class CookiesHelper
	{
		public static int GetInt32(
			this HttpCookieCollection cookies,
			string name,
			int defaultValue = default(int),
			int minValue = int.MinValue,
			int maxValue = int.MaxValue)
		{
			int res;
			var cookie = cookies[name];
			if (cookie == null || !int.TryParse(cookie.Value, out res) || res < minValue || res > maxValue)
				return defaultValue;
			return res;
		}

		public static bool GetBoolean(
			this HttpCookieCollection cookies,
			string name,
			bool defaultValue = default(bool))
		{
			bool res;
			var cookie = cookies[name];
			if (cookie == null || !bool.TryParse(cookie.Value, out res))
				return defaultValue;
			return res;
		}

		public static T GetEnum<T>(
			this HttpCookieCollection cookies,
			string name,
			T defaultValue = default(T))
			where T : struct
		{
			T res;
			var cookie = cookies[name];
			if (cookie == null || !Enum.TryParse(cookie.Value, true, out res))
				return defaultValue;
			return res;
		}
	}
}