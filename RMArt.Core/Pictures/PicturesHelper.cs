using System;
using System.Collections.Generic;
using System.Linq;

namespace RMArt.Core
{
	public static class PicturesHelper
	{
		public static void UpdateMany(
			this IPicturesService picturesService,
			IEnumerable<int> ids,
			PictureUpdate update,
			Identity identity,
			string comment = null)
		{
			foreach (var pid in ids)
				picturesService.Update(pid, update, identity, comment);
		}

		public static void UpdateMany(
			this IPicturesService picturesService,
			PicturesQuery query,
			PictureUpdate update,
			Identity identity,
			string comment = null)
		{
			picturesService.UpdateMany(
				picturesService.Find(query).Select(p => p.ID),
				update,
				identity,
				comment);
		}

		public static void RemoveTag(this IPicturesService picturesService, int tagID, Identity identity)
		{
			picturesService.UpdateMany(
				new PicturesQuery { ReqiredTagIDs = new SortedSet<int> { tagID } },
				new PictureUpdate { RemoveTags = new[] { tagID } },
				identity);
		}

		public static void ReplaceTag(this IPicturesService picturesService, int oldTagID, int newTagID, Identity identity)
		{
			picturesService.UpdateMany(
				new PicturesQuery { ReqiredTagIDs = new SortedSet<int> { oldTagID } },
				new PictureUpdate { RemoveTags = new[] { oldTagID }, AddTags = new[] { newTagID } },
				identity);
		}

		public static void DeleteDeclined(this IPicturesService picturesService)
		{
			var declinedPictures =
				picturesService
					.Find(new PicturesQuery { AllowedStatuses = new SortedSet<ModerationStatus> { ModerationStatus.Declined } })
					.Select(p => p.ID);
			foreach (var pid in declinedPictures)
				picturesService.Delete(pid);
		}

		public static bool IsValidSource(string source)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			Uri sourceUri;
			return source == "" ||
				Uri.TryCreate(source, UriKind.Absolute, out sourceUri)
					&& (sourceUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)
						|| sourceUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsSourcesEquals(string a, string b)
		{
			return string.Equals(a, b, StringComparison.Ordinal);
		}
	}
}