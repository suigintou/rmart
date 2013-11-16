using RMArt.Core;
using Thorn;

namespace RMArt.ServiceTool
{
	[ThornExport]
	public class Pictures
	{
		private readonly IPicturesService _picturesService;
		private readonly ITagsService _tagsService;

		public Pictures(IPicturesService picturesService, ITagsService tagsService)
		{
			_picturesService = picturesService;
			_tagsService = tagsService;
		}

		public void RecalcScores()
		{
			_picturesService.UpdateRateAggregates();
		}

		public void RecalcFileHashes()
		{
			_picturesService.RecalcFileHashes(new ConsoleProgressIndicator());
		}

		public void RecalcBitmapHashes()
		{
			_picturesService.RecalcBitmapHashes(new ConsoleProgressIndicator());
		}

		public void RecalcTagCount()
		{
			_picturesService.RecalcTagCount(new ConsoleProgressIndicator());
		}

		public void UpdateCachedTags()
		{
			_picturesService.RecalcCachedTags();
		}

		public void RebuildThumbs(RebuidThumbsOptions options)
		{
			_picturesService.RebuildThumbs(options.OverwriteExisting, new ConsoleProgressIndicator());
		}

		public void DeleteDeclined()
		{
			_picturesService.DeleteDeclined();
		}

		public void UpdateSimilar()
		{
			_picturesService.UpdateSimilarPictures(new ConsoleProgressIndicator());
		}

		public void FindPixivSources()
		{
			PixivSourcesFinder.FindPixivSources(_picturesService, _tagsService, new ConsoleProgressIndicator());
		}

		public void FindPixivAuthors()
		{
			PixivSourcesFinder.FindPixivAuthors(_picturesService, _tagsService, new ConsoleProgressIndicator());
		}
	}
}