using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RMArt.Core
{
	public interface IPicturesService
	{
		IQueryable<Picture> Find(PicturesQuery query = null);
		IQueryable<Picture> GetSimilar(int id);
		IFileInfo GetPictureFile(int id);
		IFileInfo GetThumbFile(int id, int size);

		PictureAddingResult Add(Stream inputStream, ModerationStatus status, Identity identity, bool replaceFile, out int id);
		Picture CheckExists(Stream inputStream);
		void Delete(int id);
		void Update(int id, PictureUpdate update, Identity identity, string comment = null);
		void Rate(int id, int score, Identity identity);
		void SetFavorited(int id, bool favorited, Identity identity);
		void AssignTagChilds(int tagID, Identity identity);

		void RecalcCachedTags();
		void UpdateRateAggregates();
		void RebuildThumbs(bool overwriteExisting, IProgressIndicator progressIndicator);
		void RecalcFileHashes(IProgressIndicator progressIndicator);
		void RecalcBitmapHashes(IProgressIndicator progressIndicator);
		void RecalcTagCount(IProgressIndicator progressIndicator);
		void UpdateSimilarPictures(IProgressIndicator progressIndicator);

		IDictionary<int, Size> ThumbnailSizePresets { get; }
	}
}