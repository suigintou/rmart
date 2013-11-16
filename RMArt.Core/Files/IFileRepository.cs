using System.IO;

namespace RMArt.Core
{
	public interface IFileRepository
	{
		IFileInfo Get(string fileName);
		bool IsExists(string fileName);
		Stream Create(string fileName);
		void Delete(string fileName);
	}
}