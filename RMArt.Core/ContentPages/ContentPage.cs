using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Content")]
	public class ContentPage
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column, NotNull]
		public string Name { get; set; }

		[Column]
		public int CultureID { get; set; }

		[Column, NotNull]
		public string Title { get; set; }

		[Column, NotNull]
		public string Content { get; set; }
	}
}