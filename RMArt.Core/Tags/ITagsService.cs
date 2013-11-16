using System.Linq;

namespace RMArt.Core
{
	public interface ITagsService
	{
		IQueryable<Tag> Find(TagsQuery query = null);
		Tag LoadTag(int id);
		int Create(string name, TagType type, ModerationStatus status, Identity identity);
		void Update(int id, TagUpdate update, Identity identity, string comment = null);
		void Delete(int id);
		void IncrementUsageCount(int id, int value);
		void SetUsageCount(int id, int value);
	}
}