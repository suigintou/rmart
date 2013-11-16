using System;

namespace RMArt.Core
{
	public static class StringHelper
	{
		public static string ToInfoSizeString(this int size)
		{
			return ToInfoSizeString((long)size);
		}

		public static string ToInfoSizeString(this long size)
		{
			if (size == 0)
				return "0 B";
			string dimUnit;
			int k;
			if (size < 1024)
			{
				dimUnit = " B";
				k = 0;
			}
			else if (size < 1024 * 1024)
			{
				dimUnit = " KB";
				k = 10;
			}
			else
			{
				dimUnit = " MB";
				k = 20;
			}
			return ((double)size / (1 << k)).ToString("#.#") + dimUnit;
		}

		public static bool ParseBoolean(this string value)
		{
			return bool.Parse(value);
		}

		public static int ParseInt32(this string value)
		{
			return int.Parse(value);
		}

		public static T ParseEnum<T>(this string value, bool ignoreCase = false)
			where T : struct
		{
			return (T)Enum.Parse(typeof(T), value, ignoreCase);
		}

		public static string RestrictLength(this string s, int maxLength)
		{
			return s.Length > maxLength + 1 ? s.Substring(0, maxLength) + "…" : s;
		}
	}
}