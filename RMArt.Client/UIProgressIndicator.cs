using System;
using System.Windows.Controls;
using System.Windows.Shell;
using RMArt.Core;

namespace RMArt.Client
{
	public class UIProgressIndicator : IProgressIndicator
	{
		private readonly TextBox _logTextBox;
		private readonly ProgressBar _progressBar;
		private readonly TaskbarItemInfo _taskbarItemInfo;

		public UIProgressIndicator(TextBox logTextBox, ProgressBar progressBar, TaskbarItemInfo taskbarItemInfo)
		{
			_logTextBox = logTextBox;
			_progressBar = progressBar;
			_taskbarItemInfo = taskbarItemInfo;
		}

		public void ReportProgress(int total, int current)
		{
			_progressBar.Dispatcher.InvokeAsync(
				() =>
				{
					_progressBar.Maximum = total;
					_progressBar.Value = current;
				});
			_taskbarItemInfo.Dispatcher.InvokeAsync(
				() =>
				{
					_taskbarItemInfo.ProgressValue = (double)current / total;
				});
		}

		public void Message(string text)
		{
			var logMessage = string.Concat("<", DateTime.Now.ToLongTimeString(), "> ", text, Environment.NewLine);
			_logTextBox.Dispatcher.InvokeAsync(
				() =>
				{
					_logTextBox.AppendText(logMessage);
					_logTextBox.ScrollToEnd();
				});
		}
	}
}