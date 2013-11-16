using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using RMArt.Client.Core;
using RMArt.Core;

namespace RMArt.Client
{
	public static class Downloader
	{
		public static void Download(
			SiteConfig siteConfig,
			string query,
			string destantion,
			string customSource,
			AuthenticationCookie authenticationCookie,
			IProgressIndicator progressIndicator,
			ref bool isPaused,
			ref bool isCancelationRequested)
		{
			progressIndicator.Message("Download started.");
			progressIndicator.Message("Site: " + siteConfig.SearchUrl);
			progressIndicator.Message("Receiving pictures list...");
			int[] list;
			try
			{
				list = PicturesClient.Search(siteConfig, authenticationCookie, query);
			}
			catch (PicturesSearchException ex)
			{
				progressIndicator.Message(ex.Message);
				return;
			}
			if (list.Length == 0)
			{
				progressIndicator.Message("No results for your query.");
				return;
			}

			var urlQueue = new Queue<int>(list);
			var picturesCount = urlQueue.Count;
			progressIndicator.Message(picturesCount + " picture(s) found.");

			progressIndicator.Message("Downloading...");
			while (urlQueue.Count > 0)
			{
				while (isPaused && !isCancelationRequested)
					Thread.Sleep(1000);
				if (isCancelationRequested)
					return;

				var pictureID = urlQueue.Dequeue();
				var fileName = pictureID.ToString();
				var pictureUrl = customSource != null ? Path.Combine(customSource, fileName) : PicturesClient.GetPictureUrl(siteConfig, pictureID);
				var filePath = Path.Combine(destantion, fileName);

				if (!File.Exists(filePath))
				{
					try
					{
						using (var webClient = new WebClient())
						{
							webClient.Headers.Add(HttpRequestHeader.Cookie, authenticationCookie.Cookie.ToString());
							webClient.DownloadFile(pictureUrl, filePath);
						}
					}
					catch (WebException webException)
					{
						progressIndicator.Message(string.Format("Error downloading file \"{0}\": {1}", fileName, webException.Message));
						urlQueue.Enqueue(pictureID);
					}
					catch (IOException ioException)
					{
						progressIndicator.Message(string.Format("Error saving file \"{0}\": {1}", fileName, ioException.Message));
						urlQueue.Enqueue(pictureID);
					}
				}

				progressIndicator.ReportProgress(picturesCount, picturesCount - urlQueue.Count);
			}
		}
	}
}