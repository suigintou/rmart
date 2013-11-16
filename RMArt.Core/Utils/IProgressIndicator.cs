namespace RMArt.Core
{
	public interface IProgressIndicator
	{
		void ReportProgress(int total, int current);
		void Message(string text);
	}
}