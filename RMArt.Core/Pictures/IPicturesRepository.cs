using System.Linq;

namespace RMArt.Core
{
	public interface IPicturesRepository
	{
		IQueryable<Picture> Find(PicturesQuery query = null);
		int Add(Picture picture);
		Picture Remove(int id);
		Picture Update(int id, PictureUpdate update);
		void IncrementRateAggregates(int id, int count, int total);
		void SetRateAggregates(int id, int count, int total);
		void SetFileHashAndSize(int id, byte[] hash, long fileSize);
		void SetBitmapHash(int id, byte[] hash);
		void RecalcCachedTags();
		IQueryable<Picture> GetSimilar(int id);
		void UpdateSimilar(int id, int maxCount);
	}
}