using System;
using System.Data;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class PicturesRepository : IPicturesRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public PicturesRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public IQueryable<Picture> Find(PicturesQuery query = null)
		{
			return
				_dataContextProvider
					.CreateDataContext()
					.Pictures()
					.Query(query);
		}

		public int Add(Picture picture)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return
					Convert.ToInt32(
						context
							.Pictures()
							.Value(_ => _.FileHash, picture.FileHash)
							.Value(_ => _.BitmapHash, picture.BitmapHash)
							.Value(_ => _.Format, picture.Format)
							.Value(_ => _.Width, picture.Width)
							.Value(_ => _.Height, picture.Height)
							.Value(_ => _.FileSize, picture.FileSize)
							.Value(_ => _.CreatorID, picture.CreatorID)
							.Value(_ => _.CreatorIP, picture.CreatorIP)
							.Value(_ => _.CreationDate, picture.CreationDate)
							.Value(_ => _.Status, picture.Status)
							.Value(_ => _.RequiresTagging, picture.RequiresTagging)
							.Value(_ => _.Rating, picture.Rating)
							.Value(_ => _.Score, picture.Score)
							.Value(_ => _.RatesCount, picture.RatesCount)
							.InsertWithIdentity());
		}

		public Picture Remove(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(context))
			{
				transaction.BeginTransaction(IsolationLevel.Serializable);

				var picture = context.Pictures().SingleOrDefault(_ => _.ID == id);

				context.TagsAssignments().Delete(_ => _.PictureID == id);
				context.SimilarPictures().Delete(_ => _.PictureID == id);
				context.SimilarPictures().Delete(_ => _.SimilarPictureID == id);
				context.Pictures().Delete(p => id == p.ID);

				transaction.CommitTransaction();

				return picture;
			}
		}

		public Picture Update(int id, PictureUpdate update)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(context))
			{
				transaction.BeginTransaction(IsolationLevel.Serializable);

				var oldPicture = context.Pictures().SingleOrDefault(_ => _.ID == id);

				if (oldPicture != null)
				{
					var source = context.Pictures().Where(_ => _.ID == id);
					if (update.Status != null)
						source.Set(_ => _.Status, update.Status).Update();
					if (update.RequiresTagging != null)
						source.Set(_ => _.RequiresTagging, update.RequiresTagging).Update();
					if (update.Rating != null)
						source.Set(_ => _.Rating, update.Rating).Update();
					if (update.Source != null)
						source.Set(_ => _.Source, update.Source).Update();

					var isTagsModified = false;
					if (update.AddTags != null)
						foreach (var tid in update.AddTags.Except(oldPicture.Tags))
						{
							context
								.TagsAssignments()
								.Value(_ => _.PictureID, id)
								.Value(_ => _.TagID, tid)
								.Insert();
							isTagsModified = true;
						}
					if (update.RemoveTags != null)
						isTagsModified |=
							context
								.TagsAssignments()
								.Delete(_ => _.PictureID == id && update.RemoveTags.Contains(_.TagID)) > 0;
					if (isTagsModified)
						RecalcCachedTags(context, id);
				}

				transaction.CommitTransaction();

				return oldPicture;
			}
		}

		public void IncrementRateAggregates(int id, int count, int total)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Pictures()
					.Where(p => p.ID == id)
					.Set(_ => _.RatesCount, _ => _.RatesCount + count)
					.Set(_ => _.Score, _ => _.Score + total)
					.Update();
		}

		public void SetRateAggregates(int id, int count, int total)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Pictures()
					.Where(p => p.ID == id)
					.Set(_ => _.RatesCount, count)
					.Set(_ => _.Score, total)
					.Update();
		}

		public void SetFileHashAndSize(int id, byte[] hash, long fileSize)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Pictures()
					.Where(p => p.ID == id)
					.Set(_ => _.FileHash, hash)
					.Set(_ => _.FileSize, fileSize)
					.Update();
		}

		public void SetBitmapHash(int id, byte[] hash)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Pictures()
					.Where(p => p.ID == id)
					.Set(_ => _.BitmapHash, hash)
					.Update();
		}

		public void RecalcCachedTags()
		{
			using (var context = _dataContextProvider.CreateDataContext())
			{
				var pictureIDs = context.Pictures().Select(_ => _.ID).ToArray();
				foreach (var pid in pictureIDs)
					using (var transaction = new DataContextTransaction(context))
					{
						transaction.BeginTransaction(IsolationLevel.Serializable);
						RecalcCachedTags(context, pid);
						transaction.CommitTransaction();
					}
			}
		}

		public IQueryable<Picture> GetSimilar(int id)
		{
			using (var dc = _dataContextProvider.CreateDataContext())
				return (from s in dc.SimilarPictures()
						where s.PictureID == id
						join p in dc.Pictures() on s.SimilarPictureID equals p.ID
						select p);
		}

		public void UpdateSimilar(int id, int maxCount)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(context))
			{
				transaction.BeginTransaction();
				context.SimilarPictures().Delete(_ => _.PictureID == id);
				var p = context.Pictures().Single(_ => _.ID == id);
				(from _ in context.Pictures()
				 where _.ID != p.ID
				 where _.Status == ModerationStatus.Accepted
				 where _.Rating <= p.Rating
				 let r = _.TagRelations.Count(t => p.Tags.Contains(t.TagID))
				 where r > 0
				 orderby (_.Rating - p.Rating) + r descending, Sql.NewGuid()
				 select _)
					.Take(maxCount)
					.Insert(context.SimilarPictures(), _ => new SimilarPictureRelation { PictureID = p.ID, SimilarPictureID = _.ID });
				transaction.CommitTransaction();
			}
		}

		private void RecalcCachedTags(IDataContext context, int id)
		{
			var tagIDs = context.TagsAssignments().Where(_ => _.PictureID == id).Select(_ => _.TagID);
			var s = string.Join(" ", tagIDs);
			context.Pictures().Where(p => p.ID == id).Set(_ => _.CachedTagIDs, s).Update();
		}
	}
}