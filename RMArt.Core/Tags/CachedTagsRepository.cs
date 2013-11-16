using System.Collections.Concurrent;
using System.Linq;

namespace RMArt.Core
{
	public class CachedTagsRepository : ITagsRepository
	{
		private readonly ITagsRepository _tagsRepository;
		private readonly ConcurrentDictionary<int, Tag> _tagsCache = new ConcurrentDictionary<int, Tag>();

		public CachedTagsRepository(ITagsRepository tagsRepository)
		{
			_tagsRepository = tagsRepository;
		}

		public IQueryable<Tag> Find(TagsQuery query = null)
		{
			return _tagsRepository.Find(query);
		}

		public Tag LoadTag(int id)
		{
			return _tagsCache.GetOrAdd(id, _tagsRepository.LoadTag);
		}

		public int Add(Tag tag)
		{
			return _tagsRepository.Add(tag);
		}

		public Tag Update(int id, TagUpdate update)
		{
			var old = _tagsRepository.Update(id, update);

			DropCache(id);
			
			if (update.AddParents != null)
				foreach (var parentID in update.AddParents.Except(old.ParentIDs).Distinct())
					DropCache(parentID);
			
			if (update.RemoveParents != null)
				foreach (var parentID in update.RemoveParents.Intersect(old.ParentIDs).Distinct())
					DropCache(parentID);

			return old;
		}

		public void Remove(int id)
		{
			_tagsRepository.Remove(id);
			DropCache(id);
		}

		public void IncrementUsageCount(int id, int value)
		{
			_tagsRepository.IncrementUsageCount(id, value);
			DropCache(id);
		}

		public void SetUsageCount(int id, int value)
		{
			_tagsRepository.SetUsageCount(id, value);
			DropCache(id);
		}

		private void DropCache(int tagID)
		{
			Tag removed;
			_tagsCache.TryRemove(tagID, out removed);
		}
	}
}