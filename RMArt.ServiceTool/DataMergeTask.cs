//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using RMArt.DataModel;

//namespace RMArt.ServiceTool
//{
//    internal class DataMergeTask
//    {
//        private readonly IUsersRepository _sourceUsersRepository;
//        private readonly IUsersRepository _destntionUsersRepository;
//        private readonly ITagsRepository _sourceTagsRepository;
//        private readonly ITagsRepository _destntionTagsRepository;
//        private readonly IPicturesRepository _sourcePicturesRepository;
//        private readonly IPicturesDataRepository _sourcePicturesDataRepository;
//        private readonly IPicturesRepository _destntionPicturesRepository;
//        private readonly IPicturesDataRepository _destantionPicturesDataRepository;

//        public DataMergeTask(
//            IUsersRepository sourceUsersRepository,
//            IUsersRepository destntionUsersRepository,
//            ITagsRepository sourceTagsRepository,
//            ITagsRepository destntionTagsRepository,
//            IPicturesRepository sourcePicturesRepository,
//            IPicturesDataRepository sourcePicturesDataRepository,
//            IPicturesRepository destntionPicturesRepository,
//            IPicturesDataRepository destantionPicturesDataRepository)
//        {
//            _sourceUsersRepository = sourceUsersRepository;
//            _destntionUsersRepository = destntionUsersRepository;
//            _sourceTagsRepository = sourceTagsRepository;
//            _destntionTagsRepository = destntionTagsRepository;
//            _sourcePicturesRepository = sourcePicturesRepository;
//            _sourcePicturesDataRepository = sourcePicturesDataRepository;
//            _destntionPicturesRepository = destntionPicturesRepository;
//            _destantionPicturesDataRepository = destantionPicturesDataRepository;
//        }

//        public void Run()
//        {
//            Console.WriteLine("Processing users...");
//            var movedUsersMap = MergeUsers();
//            Console.WriteLine("Done. {0} users processed.", movedUsersMap.Count);

//            Console.WriteLine("Processing tags...");
//            var movedTagsMap = MergeTags();
//            Console.WriteLine("Done. {0} tags processed.", movedTagsMap.Count);

//            Console.WriteLine("Processing pictures...");
//            var movedPicturesMap = MergePictures(movedTagsMap, movedUsersMap);
//            Console.WriteLine("Done. {0} pictures processed.", movedPicturesMap.Count);

//            Console.WriteLine("Processing history...");
//            MergeHistory(movedPicturesMap, movedTagsMap, movedUsersMap);
//            Console.WriteLine("Done.");

//            Console.WriteLine("Finished.");
//        }

//        private IDictionary<int, int> MergeUsers()
//        {
//            var movedUsersMap = new Dictionary<int, int>();
//            foreach (var user in _sourceUsersRepository.List())
//            {
//                var withSameLogin = _destntionUsersRepository.GetByLogin(user.Login);
//                if (withSameLogin != null)
//                {
//                    movedUsersMap.Add(user.ID, withSameLogin.ID);
//                    Console.WriteLine(
//                        "User {0} (ID {1}) merged with {2} (ID {3}).",
//                        user.Login,
//                        user.ID,
//                        withSameLogin.Login,
//                        withSameLogin.ID);
//                }
//                else
//                {
//                    var id =
//                        _destntionUsersRepository.Create(
//                            user.Login,
//                            user.PasswordHash,
//                            user.Name,
//                            user.Email,
//                            user.Role,
//                            user.RegistrationDate,
//                            user.IsPrivateProfile);
//                    movedUsersMap.Add(user.ID, id);
//                    Console.WriteLine(
//                        "User {0} ({1}) created with ID {2}.",
//                        user.Login,
//                        user.ID,
//                        id);
//                }
//            }
//            return movedUsersMap;
//        }

//        private IDictionary<int, int> MergeTags()
//        {
//            var movedTagsMap = new Dictionary<int, int>();
//            foreach (var tid in _sourceTagsRepository.List())
//            {
//                var names = _sourceTagsRepository.GetByID(tid).Names;
//                var existing =
//                    names
//                        .Select(n => _destntionTagsRepository.GetIDByName(n.Name))
//                        .FirstOrDefault(id => id.HasValue);
//                var destTagID = existing ?? _destntionTagsRepository.Create();
//                movedTagsMap.Add(tid, destTagID);
//                foreach (var n in names.Where(n => !_destntionTagsRepository.IsNameExists(n.Name)))
//                    _destntionTagsRepository.CreateName(destTagID, n.Name, n.CultureID, n.Priority);
//            }
//            return movedTagsMap;
//        }

//        private IDictionary<int, int> MergePictures(
//            IDictionary<int, int> tagsMap,
//            IDictionary<int, int> usersMap)
//        {
//            var movedPicturesMap = new Dictionary<int, int>();
//            foreach (var p in _sourcePicturesRepository.MinimalList().Select(_sourcePicturesRepository.GetByID))
//            {
//                var existingID = _destntionPicturesRepository.GetIDByHash(p.Hash);
//                var destID =
//                    existingID != null
//                        ? existingID.Value
//                        : _destntionPicturesRepository.Create(
//                            p.Hash,
//                            p.Format,
//                            p.Width,
//                            p.Height,
//                            p.FileSize,
//                            p.Rating,
//                            p.RequiresTagging,
//                            p.CreatorID != null ? usersMap[p.CreatorID.Value] : default(int?),
//                            p.CreatorIP,
//                            p.CreationDate,
//                            p.Status);

//                movedPicturesMap.Add(p.ID, destID);

//                foreach (var tid in _sourcePicturesRepository.GetTags(p.ID))
//                    _destntionPicturesRepository.AssignTag(destID, tagsMap[tid]);
//                foreach (var rate in _sourcePicturesRepository.GetRates(pictureID: p.ID))
//                    _destntionPicturesRepository.Rate(destID, usersMap[rate.UserID], rate.Score, rate.Date);
//                foreach (var f in _sourcePicturesRepository.GetFavorites(p.ID))
//                    _destntionPicturesRepository.Favorite(destID, usersMap[f.UserID], f.Date);

//                var sourcePicturePath = _sourcePicturesDataRepository.GetPicturePath(p.ID);
//                if (File.Exists(sourcePicturePath))
//                    File.Copy(sourcePicturePath, _destantionPicturesDataRepository.GetPicturePath(destID));
//                for (var i = 0; i < ThumbnailSizeHelper.PresetsCount; i++)
//                {
//                    var sourceThumbPath = _sourcePicturesDataRepository.GetThumbPath(p.ID, i);
//                    if (File.Exists(sourceThumbPath))
//                        File.Copy(sourceThumbPath, _destantionPicturesDataRepository.GetThumbPath(destID, i));
//                }
//            }
//            return movedPicturesMap;
//        }

//        private void MergeHistory(
//            IDictionary<int, int> picturesMap,
//            IDictionary<int, int> tagsMap,
//            IDictionary<int, int> usersMap)
//        {
//            //foreach (var e in _sourceHistoryRepository.List())
//            //{
//            //    _destantionHistoryRepository.Add(
//            //        picturesMap[e.TargetID],
//            //        tagsMap[(int)e.ActionData["TagID"]],
//            //        (TagHistoryAction)e.ActionData["Action"],
//            //        new IdentityInfo(
//            //            e.Date,
//            //            e.UserID != null ? usersMap[e.UserID.Value] : default(int?),
//            //            e.UserIP));
//            //}
//        }
//    }
//}