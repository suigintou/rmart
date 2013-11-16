using System;
using System.Collections.Generic;
using System.Linq;

namespace RMArt.Core
{
	public static class ModerationHelper
	{
		public static bool CanView(ModerationStatus status, int? creatorID, int? currentUserID, Func<bool> isModerator)
		{
			switch (status)
			{
				case ModerationStatus.Accepted:
					return true;
				case ModerationStatus.Pending:
				case ModerationStatus.Declined:
					return (creatorID != null && creatorID == currentUserID) || isModerator();
				default:
					throw new NotSupportedException();
			}
		}

		public static ISet<ModerationStatus> GetDefaultStatuses(bool isModerator)
		{
			return
				isModerator
					? new SortedSet<ModerationStatus> { ModerationStatus.Accepted, ModerationStatus.Pending }
					: new SortedSet<ModerationStatus> { ModerationStatus.Accepted };
		}

		public static ISet<Rating> GetAllowedRatings(Rating maxRating, bool showUnrated)
		{
			return
				new SortedSet<Rating>(
					Enum
						.GetValues(typeof(Rating))
						.Cast<Rating>()
						.Where(r => r <= maxRating && (r != Rating.Unrated || showUnrated)));
		}
	}
}