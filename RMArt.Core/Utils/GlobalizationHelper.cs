using System;
using System.Globalization;

namespace RMArt.Core
{
	public static class GlobalizationHelper
	{
		public static T GetLocalized<T>(
			Func<CultureInfo, T> resultSelector,
			Func<T> fallbackResultSelector,
			CultureInfo culture = null)
			where T : class
		{
			if (culture == null)
				culture = CultureInfo.CurrentUICulture;

			do
			{
				var res = resultSelector(culture);
				if (res != null)
					return res;
				culture = culture.Parent;
			} while (!culture.Equals(CultureInfo.InvariantCulture));

			return fallbackResultSelector();
		}

		public static T GetLocalized<T>(
			Func<CultureInfo, T> resultSelector,
			Func<T> fallbackResultSelector,
			CultureInfo culture = null,
			CultureInfo fallbackCulture = null)
			where T : class
		{
			if (fallbackCulture == null)
				fallbackCulture = CultureInfo.InvariantCulture;

			return
				GetLocalized(
					resultSelector,
					() => GetLocalized(resultSelector, fallbackResultSelector, fallbackCulture),
					culture);
		}
	}
}