using NUnit.Framework;

namespace RMArt.ServiceTool.UnitTests
{
	[TestFixture]
	public class PixivSourcesFinderTests
	{
		[Test]
		public void GetPixivUrlFromSaucenao()
		{
			var pixivUrls = PixivSourcesFinder.GetPixivUrlFromSaucenao("http://rmart.org/71563/Src/1cc51d0a43181a6461ab2301978ea8b8.jpg");
			Assert.Contains("http://www.pixiv.net/member_illust.php?mode=medium&illust_id=34656447", pixivUrls);
		}

		[Test]
		public void AuthPixiv()
		{
			var authCookie = PixivSourcesFinder.AuthPixiv("ginfag", "desu123");
			Assert.NotNull(authCookie);
			Assert.Greater(authCookie.Count, 0);
		}

		[Test]
		public void GetPixivPictureUrl()
		{
			var authCookie = PixivSourcesFinder.AuthPixiv("ginfag", "desu123");
			var pictureUrl = PixivSourcesFinder.GetPixivPictureUrl("http://www.pixiv.net/member_illust.php?mode=medium&illust_id=34656447", authCookie);
			Assert.AreEqual(pictureUrl, "http://i1.pixiv.net/img01/img/paperpaper/34656447.jpg");
		}

		[Test]
		public void GetDeletedPixivPictureUrl()
		{
			var authCookie = PixivSourcesFinder.AuthPixiv("ginfag", "desu123");
			var pictureUrl = PixivSourcesFinder.GetPixivPictureUrl("http://www.pixiv.net/member_illust.php?mode=medium&illust_id=25945325", authCookie);
			Assert.AreEqual(pictureUrl, null);
		}

		[Test]
		public void GetMangaPixivPictureUrl()
		{
			var authCookie = PixivSourcesFinder.AuthPixiv("ginfag", "desu123");
			var pictureUrl = PixivSourcesFinder.GetPixivPictureUrl("http://www.pixiv.net/member_illust.php?mode=medium&illust_id=34492623", authCookie);
			Assert.AreEqual(pictureUrl, null);
		}

		[Test]
		public void GetPixivAuthor()
		{
			var authCookie = PixivSourcesFinder.AuthPixiv("ginfag", "desu123");
			var author = PixivSourcesFinder.GetPixivAuthor("http://i1.pixiv.net/img01/img/paperpaper/34656447.jpg");
			Assert.AreEqual(author, "paperpaper");
		}
	}
}