using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace RMArt.Core
{
	public class PicturesService : IPicturesService, IDisposable
	{
		private readonly IPicturesRepository _picturesRepository;
		private readonly IFileRepository _picturesFileRepository;
		private readonly IFileRepository _thumbsFileRepository;
		private readonly IRatesRepository _ratesRepository;
		private readonly IFavoritesRepository _favoritesRepository;
		private readonly ITagsService _tagsService;
		private readonly IHistoryService _historyService;
		private readonly IUsersService _usersService;
		private readonly long? _maxFileSize;
		private readonly IDictionary<int, Size> _thumbnailSizePresets;

		public PicturesService(
			IPicturesRepository picturesRepository,
			IFileRepository picturesFileRepository,
			IFileRepository thumbsFileRepository,
			IRatesRepository ratesRepository,
			IFavoritesRepository favoritesRepository,
			ITagsService tagsService,
			IHistoryService historyService,
			IUsersService usersService,
			IDictionary<int, Size> thumbnailSizePresets,
			long? maxFileSize = null)
		{
			_picturesRepository = picturesRepository;
			_picturesFileRepository = picturesFileRepository;
			_thumbsFileRepository = thumbsFileRepository;
			_tagsService = tagsService;
			_historyService = historyService;
			_usersService = usersService;
			_maxFileSize = maxFileSize;
			_thumbnailSizePresets = thumbnailSizePresets;
			_ratesRepository = ratesRepository;
			_favoritesRepository = favoritesRepository;
			_usersService = usersService;
			_usersService.RoleChanged += OnUserRoleChanged;
		}

		public void Dispose()
		{
			_usersService.RoleChanged -= OnUserRoleChanged;
		}

		public IQueryable<Picture> Find(PicturesQuery query = null)
		{
			if (query == PicturesQuery.None)
				return Enumerable.Empty<Picture>().AsQueryable();

			return _picturesRepository.Find(query);
		}

		public IQueryable<Picture> GetSimilar(int id)
		{
			return _picturesRepository.GetSimilar(id);
		}

		public IFileInfo GetPictureFile(int id)
		{
			return _picturesFileRepository.Get(GetPictureFileName(id));
		}

		public IFileInfo GetThumbFile(int id, int size)
		{
			return _thumbsFileRepository.Get(GetThumbFileName(id, size));
		}

		public PictureAddingResult Add(Stream inputStream, ModerationStatus status, Identity identity, bool replaceFile, out int id)
		{
			if (inputStream == null)
				throw new ArgumentNullException("inputStream");
			if (identity == null)
				throw new ArgumentNullException("identity");

			var data = inputStream.ReadAll();

			var size = data.Length;
			if (_maxFileSize != null && size > _maxFileSize.Value)
			{
				id = -1;
				return PictureAddingResult.FileTooBig;
			}

			var fileHash = data.ComputeMD5Hash();
			ImageFormat format;
			int width;
			int height;
			byte[] bitmapHash;
			try
			{
				using (var sourceStream = new MemoryStream(data, false))
				using (var image = new Bitmap(sourceStream, true))
				{
					format = image.GetFormat();
					width = image.Width;
					height = image.Height;
					bitmapHash = image.ComputeBitmapHash();
				}
			}
			catch (ArgumentException)
			{
				id = -1;
				return PictureAddingResult.InvalidData;
			}
			catch (NotSupportedException)
			{
				id = -1;
				return PictureAddingResult.NotSupportedFormat;
			}

			var existing = Find(new PicturesQuery { BitmapHash = bitmapHash }).SingleOrDefault();
			if (existing != null)
			{
				id = existing.ID;
				if (replaceFile)
				{
					var pictureFileName = GetPictureFileName(id);
					_picturesFileRepository.Delete(pictureFileName);
					using (var pictureFileStream = _picturesFileRepository.Create(pictureFileName))
						pictureFileStream.Write(data, 0, data.Length);
					_picturesRepository.SetFileHashAndSize(id, fileHash, size);
				}
				return PictureAddingResult.AlreadyExists;
			}

			id =
				_picturesRepository.Add(
					new Picture
					{
						FileHash = fileHash,
						BitmapHash = bitmapHash,
						Format = format,
						Width = width,
						Height = height,
						FileSize = size,
						Status = status,
						CreatorID = identity.UserID,
						CreatorIP = identity.IPAddress.GetAddressBytes(),
						CreationDate = DateTime.UtcNow
					});

			try
			{
				using (var pictureFileStream = _picturesFileRepository.Create(GetPictureFileName(id)))
					pictureFileStream.Write(data, 0, data.Length);
				using (var memoryStream = new MemoryStream(data, false))
					MakeThumbs(id, memoryStream, true);
			}
			catch
			{
				Delete(id);
				throw;
			}

			return PictureAddingResult.Added;
		}

		public Picture CheckExists(Stream inputStream)
		{
			if (inputStream == null)
				throw new ArgumentNullException("inputStream");

			byte[] bitmapHash;
			using (var image = new Bitmap(inputStream, true))
				bitmapHash = image.ComputeBitmapHash();

			return Find(new PicturesQuery { BitmapHash = bitmapHash }).SingleOrDefault();
		}

		public void Delete(int id)
		{
			_ratesRepository.Delete(new RatesQuery { PictureID = id });
			_favoritesRepository.Delete(new FavoritesQuery { PictureID = id });

			var removedPicture = _picturesRepository.Remove(id);

			if (removedPicture.Status == ModerationStatus.Accepted)
				foreach (var tid in removedPicture.Tags)
					_tagsService.IncrementUsageCount(tid, -1);

			DeleteFiles(id);

			_historyService.DeleteFor(new ObjectReference(ObjectType.Picture, id));
		}

		public void Update(int id, PictureUpdate update, Identity identity, string comment)
		{
			if (update == null)
				throw new ArgumentNullException("update");
			if (update.Source != null && !PicturesHelper.IsValidSource(update.Source))
				throw new ArgumentException("Invalid source format.", "update");
			if (identity == null)
				throw new ArgumentNullException("identity");

			var old = _picturesRepository.Update(id, update);

			if (update.Status != null && update.Status.Value != old.Status)
			{
				if (update.Status == ModerationStatus.Accepted)
					foreach (var tid in old.Tags)
						_tagsService.IncrementUsageCount(tid, 1);
				else if (old.Status == ModerationStatus.Accepted)
					foreach (var tid in old.Tags)
						_tagsService.IncrementUsageCount(tid, -1);

				_historyService.LogValueChange(
					new ObjectReference(ObjectType.Picture, id),
					HistoryField.Status,
					(int)old.Status,
					(int)update.Status,
					identity,
					comment);
			}

			var isAccepted = (update.Status ?? old.Status) == ModerationStatus.Accepted;

			if (update.AddTags != null)
				foreach (var tagID in update.AddTags.Except(old.Tags).Distinct())
				{
					if (isAccepted)
						_tagsService.IncrementUsageCount(tagID, 1);

					_historyService.LogCollectionAdd(
						new ObjectReference(ObjectType.Picture, id),
						HistoryField.Tags,
						tagID,
						identity,
						comment);
				}

			if (update.RemoveTags != null)
				foreach (var tagID in update.RemoveTags.Intersect(old.Tags).Distinct())
				{
					if (isAccepted)
						_tagsService.IncrementUsageCount(tagID, -1);

					_historyService.LogCollectionRemove(
						new ObjectReference(ObjectType.Picture, id),
						HistoryField.Tags,
						tagID,
						identity,
						comment);
				}

			if (update.RequiresTagging != null && update.RequiresTagging.Value != old.RequiresTagging)
				_historyService.LogValueChange(
					new ObjectReference(ObjectType.Picture, id),
					HistoryField.RequiresTagging,
					!update.RequiresTagging,
					update.RequiresTagging,
					identity,
					comment);

			if (update.Rating != null && update.Rating.Value != old.Rating)
				_historyService.LogValueChange(
					new ObjectReference(ObjectType.Picture, id),
					HistoryField.Rating,
					(int)old.Rating,
					(int)update.Rating,
					identity, comment);

			if (update.Source != null && !PicturesHelper.IsSourcesEquals(update.Source, old.Source))
				_historyService.LogValueChange(
					new ObjectReference(ObjectType.Picture, id),
					HistoryField.Source,
					old.Source,
					update.Source,
					identity,
					comment);

			_picturesRepository.UpdateSimilar(id, 5);
		}

		public void Rate(int id, int score, Identity identity)
		{
			if (score < 0 || score > 3)
				throw new ArgumentOutOfRangeException("score");

			// ReSharper disable PossibleInvalidOperationException
			var userID = identity.UserID.Value;
			// ReSharper restore PossibleInvalidOperationException

			var existingPredicate = new RatesQuery { PictureID = id, UserID = userID };
			var existing = _ratesRepository.List(existingPredicate).SingleOrDefault();
			if (existing != null)
				if (existing.Score == score)
					return;
				else
					_ratesRepository.Delete(existingPredicate);

			if (score != 0)
				_ratesRepository.Add(
					new Rate
					{
						PictureID = id,
						UserID = userID,
						Score = score,
						Date = DateTime.UtcNow,
						IPAddress = identity.IPAddress.GetAddressBytes(),
						IsActive = _usersService.GetRole(userID) >= UserRole.User
					});

			UpdatePictureRateAggregates(id);
		}

		public void SetFavorited(int id, bool favorited, Identity identity)
		{
			if (identity.UserID == null)
				throw new InvalidOperationException();

			if (_favoritesRepository.IsExists(new FavoritesQuery(identity.UserID, id)) == favorited)
				return;

			if (favorited)
				_favoritesRepository.Add(new FavoritesItem { PictureID = id, UserID = identity.UserID.Value, Date = DateTime.UtcNow });
			else
				_favoritesRepository.Delete(new FavoritesQuery(identity.UserID, id));
		}

		public void AssignTagChilds(int tagID, Identity identity)
		{
			var pictureIDsForTagging = new HashSet<int>();
			foreach (var childrenTagID in _tagsService.GetChildsOf(tagID, true))
			{
				var picturesWithTag = Find(new PicturesQuery { ReqiredTagIDs = new SortedSet<int> { childrenTagID } }).Select(_ => _.ID);
				foreach (var pid in picturesWithTag)
					pictureIDsForTagging.Add(pid);
			}
			foreach (var pid in pictureIDsForTagging)
				Update(pid, new PictureUpdate { AddTags = new[] { tagID } }, identity, null);
		}

		public void RecalcCachedTags()
		{
			_picturesRepository.RecalcCachedTags();
		}

		public void UpdateRateAggregates()
		{
			foreach (var id in Find().Select(_ => _.ID).ToArray())
				UpdatePictureRateAggregates(id);
		}

		public void RebuildThumbs(bool overwriteExisting, IProgressIndicator progressIndicator)
		{
			var ids = Find(new PicturesQuery { SortBy = PicturesSortOrder.Newest }).Select(_ => _.ID).ToArray();
			for (var i = 0; i < ids.Length; i++)
			{
				if (progressIndicator != null && i % 100 == 0)
					progressIndicator.ReportProgress(ids.Length, i);

				var id = ids[i];
				try
				{
					var pictureFile = GetPictureFile(id);
					if (pictureFile == null)
						continue;
					using (var fs = pictureFile.OpenRead())
						MakeThumbs(id, fs, overwriteExisting);
				}
				catch (Exception exception)
				{
					if (progressIndicator != null)
						progressIndicator.Message(string.Format("Error making thumbnail for #{0}:\n{1}", id, exception));
				}
			}
		}

		public void RecalcFileHashes(IProgressIndicator progressIndicator)
		{
			var ids = _picturesRepository.Find().Select(_ => _.ID).ToArray();
			for (var i = 0; i < ids.Length; i++)
			{
				if (progressIndicator != null && i % 100 == 0)
					progressIndicator.ReportProgress(ids.Length, i);

				var currentID = ids[i];

				var pictureFile = GetPictureFile(currentID);
				if (pictureFile == null)
				{
					if (progressIndicator != null)
						progressIndicator.Message(string.Format("Picture #{0} file is missing.", currentID));
					continue;
				}

				byte[] hash;
				long fileSize;
				using (var fs = pictureFile.OpenRead())
				{
					hash = fs.ComputeMD5Hash();
					fileSize = fs.Length;
				}

				var conflicted = Find(new PicturesQuery { FileHash = hash }).SingleOrDefault();
				if (conflicted == null)
					_picturesRepository.SetFileHashAndSize(currentID, hash, fileSize);
				else if (conflicted.ID != currentID)
				{
					var destantionID = Merge(currentID, conflicted.ID);

					if (destantionID == currentID)
						_picturesRepository.SetFileHashAndSize(currentID, hash, fileSize);

					if (progressIndicator != null)
						progressIndicator.Message(string.Format("Picture #{0} merged with #{1} into #{2}.", currentID, conflicted.ID, destantionID));
				}
			}
		}

		public void RecalcBitmapHashes(IProgressIndicator progressIndicator)
		{
			var ids = _picturesRepository.Find().Select(_ => _.ID).ToArray();
			var completed = 0;
			Parallel.ForEach(
				ids,
				currentID =>
				{
					var pictureFile = GetPictureFile(currentID);
					if (pictureFile != null)
					{
						byte[] bitmapHash;
						using (var fs = pictureFile.OpenRead())
						using (var image = new Bitmap(fs, true))
							bitmapHash = image.ComputeBitmapHash();

						var conflicted = Find(new PicturesQuery { BitmapHash = bitmapHash }).SingleOrDefault();
						if (conflicted == null)
							_picturesRepository.SetBitmapHash(currentID, bitmapHash);
						else if (conflicted.ID != currentID)
						{
							var destantionID = Merge(currentID, conflicted.ID);
							if (destantionID == currentID)
								_picturesRepository.SetBitmapHash(currentID, bitmapHash);
							if (progressIndicator != null)
								progressIndicator.Message(string.Format("Picture #{0} merged with #{1} into #{2}.", currentID, conflicted.ID, destantionID));
						}
					}
					else if (progressIndicator != null)
						progressIndicator.Message(string.Format("Picture #{0} file is missing.", currentID));

					Interlocked.Increment(ref completed);
					if (progressIndicator != null && completed % 100 == 0)
						progressIndicator.ReportProgress(ids.Length, completed);
				});
		}

		public void RecalcTagCount(IProgressIndicator progressIndicator)
		{
			var tagIDs = _tagsService.Find().Select(t => t.ID).ToArray();
			foreach (var tagID in tagIDs)
			{
				var count = Find(
					new PicturesQuery
					{
						ReqiredTagIDs = new SortedSet<int> { tagID },
						AllowedStatuses = new SortedSet<ModerationStatus> { ModerationStatus.Accepted }
					}).Count();
				_tagsService.SetUsageCount(tagID, count);
			}
		}

		public void UpdateSimilarPictures(IProgressIndicator progressIndicator)
		{
			var ids = Find(new PicturesQuery { SortBy = PicturesSortOrder.Newest }).Select(_ => _.ID).ToArray();
			for (var i = 0; i < ids.Length; i++)
			{
				if (progressIndicator != null && i % 100 == 0)
					progressIndicator.ReportProgress(ids.Length, i);

				_picturesRepository.UpdateSimilar(ids[i], 5);
			}
		}

		public IDictionary<int, Size> ThumbnailSizePresets
		{
			get { return _thumbnailSizePresets; }
		}

		private void DeleteFiles(int id)
		{
			_picturesFileRepository.Delete(GetPictureFileName(id));
			foreach (var size in _thumbnailSizePresets.Keys)
				_thumbsFileRepository.Delete(GetThumbFileName(id, size));
		}

		private void MakeThumbs(int id, Stream sourceStream, bool overwriteExisting)
		{
			using (var source = Image.FromStream(sourceStream))
				foreach (var preset in _thumbnailSizePresets)
				{
					var thumbFileName = GetThumbFileName(id, preset.Key);

					if (_thumbsFileRepository.IsExists(thumbFileName))
						if (overwriteExisting)
							_thumbsFileRepository.Delete(thumbFileName);
						else
							continue;

					using (var thumb = ImagingHelper.MakeThumb(source, preset.Value.Width, preset.Value.Height, Color.White))
					using (var thumbFileStream = _thumbsFileRepository.Create(thumbFileName))
						thumb.SaveAsJpeg(thumbFileStream);
				}
		}

		private void UpdatePictureRateAggregates(int id)
		{
			var pictureRates = _ratesRepository.List(new RatesQuery { PictureID = id, IsActive = true });
			var count = pictureRates.Count();
			var totalScore = pictureRates.Sum(r => r.Score);
			_picturesRepository.SetRateAggregates(id, count, totalScore);
		}

		private int Merge(int a, int b)
		{
			var source = _picturesRepository.Find(new PicturesQuery { ID = Math.Max(a, b) }).Single();
			var destantion = _picturesRepository.Find(new PicturesQuery { ID = Math.Min(a, b) }).Single();

			var update = new PictureUpdate();
			if (source.Status == ModerationStatus.Accepted && destantion.Status != ModerationStatus.Accepted)
				update.Status = source.Status;
			if (source.Rating != Rating.Unrated && destantion.Rating == Rating.Unrated)
				update.Rating = source.Rating;
			if (!string.IsNullOrEmpty(source.Source) && string.IsNullOrEmpty(destantion.Source))
				update.Source = source.Source;
			update.AddTags = source.Tags.Except(destantion.Tags).ToArray();
			Update(destantion.ID, update, Identity.Empty, null);

			foreach (var rate in _ratesRepository.List(new RatesQuery { PictureID = source.ID }))
				if (!_ratesRepository.IsExists(new RatesQuery { PictureID = destantion.ID, UserID = rate.UserID }))
					_ratesRepository.Add(new Rate { PictureID = destantion.ID, UserID = rate.UserID, Score = rate.Score, Date = rate.Date, IPAddress = rate.IPAddress, IsActive = rate.IsActive });
			foreach (var f in _favoritesRepository.Find(new FavoritesQuery { PictureID = source.ID }))
				if (!_favoritesRepository.IsExists(new FavoritesQuery(f.UserID, destantion.ID)))
					_favoritesRepository.Add(new FavoritesItem { PictureID = destantion.ID, UserID = f.UserID, Date = f.Date });

			Delete(source.ID);

			return destantion.ID;
		}

		private void OnUserRoleChanged(object sender, RoleChangedEventArgs args)
		{
			if (args.NewRole == UserRole.Guest || args.OldRole == UserRole.Guest)
			{
				var isActive = args.NewRole != UserRole.Guest;
				_ratesRepository.UpdateIsActive(new RatesQuery { UserID = args.UserID }, isActive);
				foreach (var id in Find(new PicturesQuery { RatedBy = args.UserID }).Select(_ => _.ID))
					UpdatePictureRateAggregates(id);
			}
		}

		private static string GetPictureFileName(int id)
		{
			return id.ToString();
		}

		private static string GetThumbFileName(int id, int size)
		{
			return string.Concat(id, "_Thumb_", size);
		}
	}
}