using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace RMArt.Core.UnitTests
{
	[TestFixture]
	public class SearchQueryParserTests
	{
		[Test]
		public void TagsParsing()
		{
			const string tags = "  -tag1 \"tag 2\", tag3;-tag4  ";
			var res = PicturesSearchQueryHelper.ParseSearchQuery(tags);
			Assert.AreEqual(res.ReqiredTags.SequenceEqual(new[] { "tag 2", "tag3" }), true);
			Assert.AreEqual(res.ExcludedTags.SequenceEqual(new[] { "tag1", "tag4" }), true);
		}

		[Test]
		public void RatingParsing()
		{
			var rs = new SortedSet<Rating> { Rating.R18, Rating.R18G };
			var q =
				PicturesSearchQueryHelper.RatingKey
					+ PicturesSearchQueryHelper.KeyValueDelimeter
					+ string.Join(PicturesSearchQueryHelper.ListItemsDelimeter, rs);
			var res = PicturesSearchQueryHelper.ParseSearchQuery(q);
			Assert.NotNull(res.AllowedRatings);
			Assert.That(rs.SetEquals(res.AllowedRatings));
		}

		[Test]
		public void StartDateParsing()
		{
			const string q =
				PicturesSearchQueryHelper.StartDateKey + PicturesSearchQueryHelper.KeyValueDelimeter + "5-12-2010";
			var res = PicturesSearchQueryHelper.ParseSearchQuery(q);
			Assert.AreEqual(res.StartDate, new DateTime(2010, 12, 5));
		}

		[Test]
		public void EndDateParsing()
		{
			const string q =
				PicturesSearchQueryHelper.EndDateKey + PicturesSearchQueryHelper.KeyValueDelimeter + "5-12-2010";
			var res = PicturesSearchQueryHelper.ParseSearchQuery(q);
			Assert.AreEqual(res.EndDate, new DateTime(2010, 12, 5));
		}

		[Test]
		public void UploaderParsing()
		{
			const string login = "testlogin";
			const string q = PicturesSearchQueryHelper.UploaderKey + PicturesSearchQueryHelper.KeyValueDelimeter + login;
			var res = PicturesSearchQueryHelper.ParseSearchQuery(q);
			Assert.AreEqual(res.Uploader, login);
		}

		[Test]
		public void FavoritedByParsing()
		{
			const string login = "testlogin";
			const string q = PicturesSearchQueryHelper.FavoritedByKey + PicturesSearchQueryHelper.KeyValueDelimeter + login;
			var res = PicturesSearchQueryHelper.ParseSearchQuery(q);
			Assert.AreEqual(res.FavoritedBy, login);
		}
	}
}