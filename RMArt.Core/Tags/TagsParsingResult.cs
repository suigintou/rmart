using System.Collections.Generic;

namespace RMArt.Core
{
	public class TagsParsingResult
	{
		public IList<string> Names { get; private set; }
		public IList<string> InvalidNames { get; private set; }
		public bool TagmeIncluded { get; set; }

		public TagsParsingResult()
		{
			Names = new List<string>();
			InvalidNames = new List<string>();
		}
	}
}