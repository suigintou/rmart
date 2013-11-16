using System;

namespace RMArt.Core
{
	public static class PicturesSortingHelper
	{
		public static string CreateQueryString(this PicturesSortOrder sortBy, PicturesSortOrder defaultOrder)
		{
			return sortBy == defaultOrder ? string.Empty : sortBy.ToString();
		}

		public static PicturesSortOrder Invert(this PicturesSortOrder sortBy)
		{
			switch (sortBy)
			{
				case PicturesSortOrder.Newest:
					return PicturesSortOrder.Oldest;
				case PicturesSortOrder.Oldest:
					return PicturesSortOrder.Newest;
				case PicturesSortOrder.TopRated:
					return PicturesSortOrder.LowRated;
				case PicturesSortOrder.LowRated:
					return PicturesSortOrder.TopRated;
				case PicturesSortOrder.HighestResolution:
					return PicturesSortOrder.LowestResolution;
				case PicturesSortOrder.LowestResolution:
					return PicturesSortOrder.HighestResolution;
				case PicturesSortOrder.Random:
					return PicturesSortOrder.Random;
				case PicturesSortOrder.None:
					return PicturesSortOrder.None;
				default:
					throw new NotSupportedException();
			}
		}
	}
}