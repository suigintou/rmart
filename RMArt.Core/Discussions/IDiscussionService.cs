using System.Linq;

namespace RMArt.Core
{
	public interface IDiscussionService
	{
		IQueryable<DiscussionMessage> All();
		void CreateMessage(ObjectReference parent, string body, Identity identity);
		void SetStatus(int id, ModerationStatus status, Identity identity);
		void Edit(int id, string newText);
	}
}