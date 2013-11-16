using System.Linq;

namespace RMArt.Core
{
	public interface IContentRepository
	{
		bool IsExists(string name, int cultureID);
		ContentPage GetPage(int id);
		ContentPage GetPage(string name, int cultureID);
		IQueryable<ContentPage> Find();
		int Add(ContentPage page);
		void Update(ContentPage page);
		void Delete(int id);
	}
}