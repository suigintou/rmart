using System;
using RMArt.Core;

namespace RMArt.ServiceTool
{
	public class ConsoleProgressIndicator : IProgressIndicator
	{
		int _last;

		public void ReportProgress(int total, int current)
		{
			var p = current * 100 / total;
			if (p == _last)
				return;
			_last = p;
			Console.WriteLine(p + "%");
		}

		public void Message(string text)
		{
			Console.WriteLine("[{0}] {1}", DateTime.Now, text);
		}
	}
}