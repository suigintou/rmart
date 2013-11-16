using RMArt.Core;
using Thorn;

namespace RMArt.ServiceTool
{
	[ThornExport]
	public class Tags
	{
		private readonly ITagsService _tagsService;

		public Tags(ITagsService tagsService)
		{
			_tagsService = tagsService;
		}
	}
}