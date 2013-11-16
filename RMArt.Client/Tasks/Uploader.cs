using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using RMArt.Client.Core;
using RMArt.Core;

namespace RMArt.Client
{
	public static class Uploader
	{
		public static void Upload(
			SiteConfig siteConfig,
			AuthenticationCookie authenticationCookie,
			UIProgressIndicator progressIndicator,
			ref bool isPaused,
			ref bool isCancelationRequested,
			string uploadDirectory,
			bool includeSubdirectories,
			bool doNotUploadNew,
			bool setPixivSources,
			bool replaceExistingFiles,
			bool deleteFilesAfterUpload,
			bool doNotEditExisting,
			string tags,
			string rating)
		{
			progressIndicator.Message("Upload started.");
			progressIndicator.Message("Site: " + siteConfig.UploadUrl);
			progressIndicator.Message("Tags: " + tags);
			progressIndicator.Message("Rating: " + rating);

			progressIndicator.Message("Building list of files to upload...");
			var files =
				new Queue<string>(
					Directory.EnumerateFiles(
						uploadDirectory,
						"*",
						includeSubdirectories
							? SearchOption.AllDirectories
							: SearchOption.TopDirectoryOnly));

			var filesCount = files.Count;
			progressIndicator.Message(filesCount + " files found.");
			
			progressIndicator.Message("Uploading...");
			while (files.Count > 0)
			{
				while (isPaused && !isCancelationRequested)
					Thread.Sleep(1000);
				if (isCancelationRequested)
					return;

				var filePath = files.Dequeue();

				try
				{
					byte[] fileData;
					try
					{
						fileData = File.ReadAllBytes(filePath);
					}
					catch (IOException ioException)
					{
						throw new FileProcessingException("Reading failed. " + ioException.Message, ioException);
					}

					string source = null;
					if (setPixivSources)
					{
						var match = Regex.Match(Path.GetFileNameWithoutExtension(filePath), @"^(?<id>\d+)(_big_p\d+)?$");
						if (match.Success)
							source = "http://www.pixiv.net/member_illust.php?mode=medium&illust_id=" + int.Parse(match.Groups["id"].Value);
					}

					var isExistsOnServer = false;

					byte[] hash;
					using (var sourceStream = new MemoryStream(fileData, false))
					using (var bitmap = new Bitmap(sourceStream, true))
						hash = bitmap.ComputeBitmapHash();

					try
					{
						var id = PicturesClient.CheckByHash(siteConfig, hash, authenticationCookie);
						if (id != null)
						{
							isExistsOnServer = true;
							progressIndicator.Message(string.Format("File \"{0}\" already uploaded with ID: {1}.", filePath, id));
							if (replaceExistingFiles)
							{
								if (doNotEditExisting)
								{
									var res = PicturesClient.Upload(siteConfig, authenticationCookie, fileData, replaceFile: true);
									progressIndicator.Message(string.Format("Picture #{0} file replaced by \"{1}\" with result: '{2}'.", id, filePath, res));
								}
								else
								{
									var res = PicturesClient.Upload(siteConfig, authenticationCookie, fileData, tags, rating, source, true);
									progressIndicator.Message(string.Format("Picture #{0} file replaced by \"{1}\" with result: '{2}'. Rating: '{3}', source: '{4}'.", id, filePath, res, rating, source));
								}
							}
							else if (!doNotEditExisting)
							{
								PicturesClient.Edit(siteConfig, authenticationCookie, new[] { id.Value }, rating != "Unrated" ? rating : null, source);
								progressIndicator.Message(string.Format("Picture #{0} edited. Rating: '{1}', source: '{2}'.", id, rating, source));
							}
						}
						else if (!doNotUploadNew)
						{
							var res = PicturesClient.Upload(siteConfig, authenticationCookie, fileData, tags, rating, source);
							isExistsOnServer = true;
							progressIndicator.Message(string.Format("File \"{0}\" upload result: {1}", filePath, res));
						}
					}
					catch (WebException webException)
					{
						throw new FileProcessingException("Uploading failed. " + webException.Message, webException);
					}

					if (deleteFilesAfterUpload && isExistsOnServer)
					{
						try
						{
							File.Delete(filePath);
							progressIndicator.Message(string.Format("File \"{0}\" deleted.", filePath));
						}
						catch (IOException ioException)
						{
							throw new FileProcessingException("Deleting failed. " + ioException.Message, ioException);
						}
					}
				}
				catch (FileProcessingException ex)
				{
					progressIndicator.Message(string.Format("Error processing file \"{0}\": {1}", filePath, ex.Message));
					files.Enqueue(filePath);
				}

				progressIndicator.ReportProgress(filesCount, filesCount - files.Count);
			}
		}
	}
}