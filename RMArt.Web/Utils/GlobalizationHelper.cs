using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RMArt.Web
{
	public static class GlobalizationHelper
	{
		private static readonly HashSet<string> _availableCultureNames;
		public const string AutodetectCultureSetting = "auto";
		public static readonly CultureInfo DefaultCulture;
		public static readonly ReadOnlyCollection<CultureInfo> AvailableCultures;

		static GlobalizationHelper()
		{
			DefaultCulture = CultureInfo.GetCultureInfo(Config.Default.DefaultCulture);

			var avaliableCultures = new List<CultureInfo>(1 + Config.Default.AdditionalCultures.Count);
			avaliableCultures.Add(DefaultCulture);
			avaliableCultures.AddRange(
				Config.Default.AdditionalCultures
					.Cast<CultureConfigurationElement>()
					.Select(c => CultureInfo.GetCultureInfo(c.Name)));
			AvailableCultures = new ReadOnlyCollection<CultureInfo>(avaliableCultures);

			_availableCultureNames =
				new HashSet<string>(
					AvailableCultures.Select(c => c.Name),
					StringComparer.OrdinalIgnoreCase);
		}

		public static CultureInfo GetUICulture(this HttpRequest request)
		{
			return GetUICulture(new HttpRequestWrapper(request));
		}

		public static CultureInfo GetUICulture(this HttpRequestBase request)
		{
			var localeSetting = request.GetUICultureSetting();
			
			if (IsAvailableLocaleName(localeSetting))
				return CultureInfo.GetCultureInfo(localeSetting);

			if (localeSetting.Equals(AutodetectCultureSetting, StringComparison.OrdinalIgnoreCase) && request.UserLanguages != null)
			{
				var autodetected =
					request.UserLanguages
						.Select(l => l.Split(';').First())
						.Where(_availableCultureNames.Contains)
						.Select(CultureInfo.GetCultureInfo)
						.FirstOrDefault();
				if (autodetected != null)
					return autodetected;
			}

			return DefaultCulture;
		}

		public static string GetUICultureSetting(this HttpRequestBase request)
		{
			var localeCookie = request.Cookies[Controllers.ServiceController.LocaleCookieName];
			if (localeCookie != null && IsValidLocaleSetting(localeCookie.Value))
				return localeCookie.Value;

			return Config.Default.AutodetectCulture ? AutodetectCultureSetting : DefaultCulture.Name;
		}

		public static bool IsValidLocaleSetting(string value)
		{
			return value.Equals(AutodetectCultureSetting, StringComparison.OrdinalIgnoreCase) || IsAvailableLocaleName(value);
		}

		public static bool IsAvailableLocaleName(string name)
		{
			return _availableCultureNames.Contains(name);
		}
	}
}