using ParsecSharp;

namespace RMArt.Core
{
	internal class QueryTagsParsers : CharParsers
	{
		public static readonly Parser<CharInput, char> Delimeter = Char(TagsHelper.SearchTagSeparators);
		public static readonly Parser<CharInput, char> Quote = Char('"');

		public static readonly Parser<CharInput, string> SimpleQueryTag =
			from chars in OneOrMany(NotChar(Delimeter))
			select new string(chars);

		public static readonly Parser<CharInput, string> QuotedQueryTag =
			from begin in Quote
			from chars in OneOrMany(NotChar(Quote))
			from end in Quote
			select new string(chars).Trim();

		public static readonly Parser<CharInput, string> QueryTag =
			from delimeter in ZeroOrMany(Delimeter)
			from tag in QuotedQueryTag.Or(SimpleQueryTag)
			select tag;

		public static readonly Parser<CharInput, string> ExcludedQueryTag =
			from delimeter in ZeroOrMany(Delimeter)
			from minusPrefix in Char('-')
			from tag in QueryTag
			select tag;

		public static readonly Parser<CharInput, object> RequiredTagmeTag =
			WsChar(String(TagsHelper.EncodeTagName(TagsHelper.TagmeTagName)));
		public static readonly Parser<CharInput, object> ExcludedTagmeTag =
			WsChar(String("-" + TagsHelper.EncodeTagName(TagsHelper.TagmeTagName)));
	}
}