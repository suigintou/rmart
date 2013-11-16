using System;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class DatabaseContentRepository : IContentRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public DatabaseContentRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public bool IsExists(string name, int cultureID)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<ContentPage>().Any(p => p.Name == name && p.CultureID == cultureID);
		}

		public ContentPage GetPage(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<ContentPage>().SingleOrDefault(p => p.ID == id);
		}

		public ContentPage GetPage(string name, int cultureID)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.GetTable<ContentPage>().SingleOrDefault(p => p.Name == name && p.CultureID == cultureID);
		}

		public IQueryable<ContentPage> Find()
		{
			return _dataContextProvider.CreateDataContext().GetTable<ContentPage>();
		}

		public int Add(ContentPage page)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return
					Convert.ToInt32(
						context
							.GetTable<ContentPage>()
								.Value(p => p.Name, page.Name)
								.Value(p => p.CultureID, page.CultureID)
								.Value(p => p.Title, page.Title)
								.Value(p => p.Content, page.Content)
							.InsertWithIdentity());
		}

		public void Update(ContentPage page)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.GetTable<ContentPage>()
					.Where(p => p.ID == page.ID)
						.Set(p => p.Name, page.Name)
						.Set(p => p.CultureID, page.CultureID)
						.Set(p => p.Title, page.Title)
						.Set(p => p.Content, page.Content)
					.Update();
		}

		public void Delete(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context.GetTable<ContentPage>().Delete(p => p.ID == id);
		}
	}
}