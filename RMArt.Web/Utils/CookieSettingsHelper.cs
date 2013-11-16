using System.Web;
using RMArt.Core;

namespace RMArt.Web
{
	public static class CookieSettingsHelper
	{
		//public const string PageSizeCookieName = "PageSize";
		//public const string ThumbSizeCookieName = "ThumbSize";
		public const string MaxRatingCookieName = "MaxRating";
		public const string ShowUnratedCookieName = "ShowUnrated";

		//public static int GetPageSize(this HttpRequestBase request)
		//{
		//    return
		//        request.Cookies.GetInt32(
		//            PageSizeCookieName,
		//            Config.Default.DefaltThmbnailsOnPage,
		//            1,
		//            Config.Default.MaxThmbnailsOnPage);
		//}

		//public static int GetThumbSize(this HttpRequestBase request)
		//{
		//    return
		//        request.Cookies.GetInt32(
		//            ThumbSizeCookieName,
		//            Config.Default.DefaultThumbnailSize,
		//            0,
		//            ThumbnailSizeHelper.PresetsCount - 1);
		//}

		public static Rating GetMaxRating(this HttpRequestBase request)
		{
			if (!request.IsAuthenticated)
				return Config.Default.DefaultMaxRating;

			return request.Cookies.GetEnum(MaxRatingCookieName, Config.Default.DefaultMaxRating);
		}

		public static bool GetShowUnrated(this HttpRequestBase request)
		{
			if (!request.IsAuthenticated)
				return Config.Default.DefaultShowUnrated;

			return request.Cookies.GetBoolean(ShowUnratedCookieName, Config.Default.DefaultShowUnrated);
		}
	}
}