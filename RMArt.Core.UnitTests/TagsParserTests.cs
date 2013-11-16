using System.Linq;
using NUnit.Framework;

namespace RMArt.Core.UnitTests
{
	[TestFixture]
	public class TagsParserTests
	{
		[Test]
		public void TagsParsing()
		{
			const string tags = "tag1, tag 2, tag3 ";
			var res = TagsHelper.ParseTagNames(tags);
			Assert.AreEqual(res.Names.SequenceEqual(new[] { "tag1", "tag 2", "tag3" }), true);
		}
	}
}