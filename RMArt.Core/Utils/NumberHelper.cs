using System;

namespace RMArt.Core
{
	public static class NumberHelper
	{
		public static int RestrictRange(this int value, int min = int.MinValue, int max = int.MaxValue)
		{
			return Math.Max(min, Math.Min(value, max));
		}

		public static bool CompareDates(this DateTime x, DateTime y, TimeSpan delta)
		{
			return y >= x - delta && y <= x + delta;
		}
	}
}