using System.Linq;

namespace RMArt.Core
{
	public interface ITagsRepository
	{
		IQueryable<Tag> Find(TagsQuery query = null);
		Tag LoadTag(int id);
		int Add(Tag tag);
		Tag Update(int id, TagUpdate update);
		void Remove(int id);
		void IncrementUsageCount(int id, int value);
		void SetUsageCount(int id, int value);
	}
}