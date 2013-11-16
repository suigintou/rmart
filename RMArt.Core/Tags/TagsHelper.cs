using System;
using System.Collections.Generic;
using System.Linq;

namespace RMArt.Core
{
	public static class TagsHelper
	{
		public const string TagmeTagName = "tagme";
		public static readonly char[] TagSeparators = new[] { ';', ',' };
		public static readonly char[] SearchTagSeparators = TagSeparators.Concat(new[] { ' ' }).ToArray();
		public const int MaxTagNameLength = 200;
		public const string NotExistingTagName = "[deleted]";
		private const char _escapeCharacter = '"';

		public static TagsParsingResult ParseTagNames(string tags, bool allowTagme = true)
		{
			var res = new TagsParsingResult();

			if (string.IsNullOrEmpty(tags))
				return res;

			foreach (var t in tags.Split(TagSeparators).Select(t => t.Trim()).Where(t => t != ""))
				if (string.Equals(t, TagmeTagName, StringComparison.OrdinalIgnoreCase))
					if (allowTagme)
						res.TagmeIncluded = true;
					else
						res.InvalidNames.Add(t);
				else if (IsValidTagName(t))
					res.Names.Add(t);
				else
					res.InvalidNames.Add(t);

			return res;
		}

		public static string EncodeTagName(string tag)
		{
			if (tag == null)
				throw new ArgumentNullException("tag");

			return tag.IndexOfAny(SearchTagSeparators) > -1 ? _escapeCharacter + tag + _escapeCharacter : tag;
		}

		public static TagType? ParseTagType(string tagName, out string tagNameWithoutPrefix)
		{
			if (tagName == null)
				throw new ArgumentNullException("tagName");

			tagName = tagName.Trim();

			TagType? tagType = null;
			if (tagName.StartsWith(TagType.Copyright + ":", StringComparison.OrdinalIgnoreCase))
				tagType = TagType.Copyright;
			else if (tagName.StartsWith(TagType.Character + ":", StringComparison.OrdinalIgnoreCase))
				tagType = TagType.Character;
			else if (tagName.StartsWith(TagType.Artist + ":", StringComparison.OrdinalIgnoreCase))
				tagType = TagType.Artist;
			else if (tagName.StartsWith(TagType.General + ":", StringComparison.OrdinalIgnoreCase))
				tagType = TagType.General;

			tagNameWithoutPrefix = tagType != null ? tagName.Substring(tagName.IndexOf(':') + 1).TrimStart() : tagName;

			return tagType;
		}

		public static bool IsValidTagName(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag) || tag.Length > MaxTagNameLength)
				return false;

			if (tag.Equals(TagmeTagName, StringComparison.OrdinalIgnoreCase))
				return false;

			return tag.IndexOf(_escapeCharacter) < 0;
		}

		public static string GetDisplayName(this Tag tag)
		{
			return tag == null ? NotExistingTagName : tag.Name;
		}

		public static int? GetIDByName(this ITagsService tagsService, string name)
		{
			return
				tagsService
					.Find(new TagsQuery { Name = name })
					.Select(t => (int?)t.ID)
					.SingleOrDefault();
		}

		public static ISet<int> GetSearchTagIDs(this ITagsService tagsService, IEnumerable<string> names)
		{
			bool dummy;
			return GetSearchTagIDs(tagsService, names, out dummy);
		}

		public static ISet<int> GetSearchTagIDs(this ITagsService tagsService, IEnumerable<string> names, out bool notFound)
		{
			var res = new HashSet<int>();
			notFound = false;
			foreach (var tagID in names.Select(tagsService.GetIDByName))
			{
				if (tagID != null)
					res.Add(tagID.Value);
				else
					notFound = true;
			}
			res.TrimExcess();
			return res;
		}

		public static ISet<int> GetAssignTagIDs(this ITagsService tagsService, IEnumerable<string> tagNames, ModerationStatus status, Identity identity, bool withParents)
		{
			if (tagsService == null)
				throw new ArgumentNullException("tagsService");
			if (tagNames == null)
				throw new ArgumentNullException("tagNames");
			if (identity == null)
				throw new ArgumentNullException("identity");

			var res = new HashSet<int>();
			foreach (var name in tagNames)
			{
				string nameWithoutPrefix;
				var type = ParseTagType(name, out nameWithoutPrefix);

				int tagID;
				var exsistingTagID = tagsService.GetIDByName(nameWithoutPrefix);
				if (exsistingTagID != null)
				{
					var exsisting = tagsService.LoadTag(exsistingTagID.Value);
					if (exsisting.Status == ModerationStatus.Declined || (type != null && exsisting.Type != type.Value))
						continue;
					tagID = exsisting.ID;
				}
				else
				{
					tagID = tagsService.Create(nameWithoutPrefix, type ?? TagType.General, status, identity);
					if (status == ModerationStatus.Declined)
						continue;
				}
				if (withParents)
					GetParentsRecursive(tagsService, tagID, res);
				res.Add(tagID);
			}
			res.TrimExcess();
			return res;
		}

		public static void Merge(this ITagsService tagsService, int sourceID, int destantionID, Identity identity)
		{
			if (tagsService == null)
				throw new ArgumentNullException("tagsService");
			if (sourceID == destantionID)
				throw new InvalidOperationException("Source and destantion are equls.");

			var source = tagsService.LoadTag(sourceID);
			var destantion = tagsService.LoadTag(destantionID);

			if (source == null)
				throw new ArgumentException("Source tag does non exists.", "sourceID");
			if (destantion == null)
				throw new ArgumentException("Destantion tag does non exists.", "destantionID");

			tagsService.Delete(source.ID);

			tagsService.Update(
				destantionID,
				new TagUpdate
				{
					AddAliases = new[] { source.Name }.Concat(source.AliasNames).ToArray(),
					AddParents = source.ParentIDs
				},
				identity);

			foreach (var childrenID in source.ChildrenIDs)
				tagsService.Update(childrenID, new TagUpdate { AddParents = new[] { destantionID } }, identity);
		}

		public static void DeleteDeclined(this ITagsService tagsService, IPicturesService picturesService, Identity identity)
		{
			var declinedTags = tagsService.Find(new TagsQuery { AllowedStatuses = new SortedSet<ModerationStatus> { ModerationStatus.Declined } }).Select(t => t.ID);
			foreach (var tid in declinedTags)
			{
				picturesService.RemoveTag(tid, identity);
				tagsService.Delete(tid);
			}
		}

		public static bool IsChildren(this ITagsService tagsService, int parentTagID, int childrenTagID, bool recursive)
		{
			var childs = tagsService.GetChildsOf(parentTagID, false);
			return
				childs.Contains(childrenTagID)
					|| (recursive && childs.Any(tid => tagsService.IsChildren(tid, childrenTagID, true)));
		}

		public static ICollection<int> GetChildsOf(this ITagsService tagsService, int tagID, bool recursive)
		{
			if (recursive)
			{
				var res = new HashSet<int>();
				GetChildrensRecursive(tagsService, tagID, res);
				res.TrimExcess();
				return res;
			}
			return tagsService.LoadTag(tagID).ChildrenIDs;
		}

		public static ICollection<int> GetParentsOf(this ITagsService tagsService, int tagID, bool recursive)
		{
			if (recursive)
			{
				var res = new HashSet<int>();
				GetParentsRecursive(tagsService, tagID, res);
				res.TrimExcess();
				return res;
			}
			return tagsService.LoadTag(tagID).ParentIDs;
		}

		private static void GetChildrensRecursive(ITagsService tagsService, int id, ISet<int> output)
		{
			foreach (var c in tagsService.LoadTag(id).ChildrenIDs)
				if (output.Add(c))
					GetChildrensRecursive(tagsService, c, output);
		}

		private static void GetParentsRecursive(ITagsService tagsService, int id, ISet<int> output)
		{
			foreach (var p in tagsService.LoadTag(id).ParentIDs)
				if (output.Add(p))
					GetParentsRecursive(tagsService, p, output);
		}
	}
}