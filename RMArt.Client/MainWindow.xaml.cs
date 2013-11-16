using System.Windows.Shell;
using RMArt.Client.Core;
using RMArt.Client.Properties;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RMArt.Client
{
	public partial class MainWindow
	{
		private bool _isWorking;
		private bool _isPaused;
		private bool _isCancelationRequested;
		private readonly UIProgressIndicator _progressIndicator;

		public MainWindow()
		{
			InitializeComponent();
			foreach (var configName in SiteConfigs.Configs.Keys)
				_siteComboBox.Items.Add(configName);
			_progressIndicator = new UIProgressIndicator(_logTextBox, _progressBar, TaskbarItemInfo);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_isWorking)
				if (MessageBox.Show("Program is working. Cancel?", Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
					_isCancelationRequested = true;
				else
					e.Cancel = true;

			Settings.Default.Save();

			base.OnClosing(e);
		}

		private void DownloadButtonClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(Settings.Default.DownloadDirectory))
			{
				MessageBox.Show(this, "Invalid destantion directory path.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			Run(Download);
		}

		private void UploadButtonClick(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(Settings.Default.UploadDirectory))
			{
				MessageBox.Show(this, "Invalid source directory path.", Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			Run(Upload);
		}

		private void PauseButtonClick(object sender, RoutedEventArgs e)
		{
			_isPaused = !_isPaused;
			if (_isPaused)
			{
				_pauseButton.Content = "Resume";
				TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
			}
			else
			{
				_pauseButton.Content = "Pause";
				TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
			}
		}

		private void CancellButtonClick(object sender, RoutedEventArgs e)
		{
			_isCancelationRequested = true;
			_pauseButton.IsEnabled = false;
			_cancellButton.IsEnabled = false;
		}

		private void Download()
		{
			var siteConfig = LoadSiteConfig();
			if (siteConfig == null)
				return;

			var authentification = Authenticate(siteConfig);
			if (authentification == null)
				return;

			Downloader.Download(
				siteConfig,
				Settings.Default.DownloadQuery,
				Settings.Default.DownloadDirectory,
				Settings.Default.UseCustomDownloadSource ? Settings.Default.CustomDownloadSource : null,
				authentification,
				_progressIndicator,
				ref _isPaused,
				ref _isCancelationRequested);
		}

		private void Upload()
		{
			var siteConfig = LoadSiteConfig();
			if (siteConfig == null)
				return;

			var authentification = Authenticate(siteConfig);
			if (authentification == null)
				return;

			Uploader.Upload(
				siteConfig,
				authentification,
				_progressIndicator,
				ref _isPaused,
				ref _isCancelationRequested,
				Settings.Default.UploadDirectory,
				Settings.Default.IncludeUploadSubdirectories,
				Settings.Default.DoNotUploadNewPictures,
				Settings.Default.SetPixivSources,
				Settings.Default.ReplaceExistingFiles,
				Settings.Default.DeleteFilesAfterUpload,
				Settings.Default.DoNotEditExisting,
				Settings.Default.UploadTags,
				PicturesClient.Ratings[Settings.Default.UploadRatingSelectedIndex]);
		}

		private static SiteConfig LoadSiteConfig()
		{
			if (!SiteConfigs.Configs.ContainsKey(Settings.Default.Site))
				return null;

			return SiteConfigs.Configs[Settings.Default.Site];
		}

		private AuthenticationCookie Authenticate(SiteConfig siteConfig)
		{
			return Authenticator.Authenticate(siteConfig, Settings.Default.Login, Settings.Default.Password, _progressIndicator);
		}

		private void Run(Action worker)
		{
			_isWorking = true;
			_settingsTabControl.IsEnabled = false;
			_pauseButton.IsEnabled = true;
			_cancellButton.IsEnabled = true;
			_isCancelationRequested = false;
			_isPaused = false;
			_logTextBox.Clear();
			_progressBar.Value = 0;
			TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
			Task
				.Run(worker)
				.ContinueWith(
					t =>
					{
						_settingsTabControl.IsEnabled = true;
						_pauseButton.IsEnabled = false;
						_pauseButton.Content = "Pause";
						_cancellButton.IsEnabled = false;
						_isWorking = false;
						TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
					},
					TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}