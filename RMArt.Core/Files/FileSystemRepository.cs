using System.IO;

namespace RMArt.Core
{
	public class FileSystemRepository : IFileRepository
	{
		private readonly string _directory;

		public FileSystemRepository(string directory)
		{
			_directory = directory;
		}

		public IFileInfo Get(string fileName)
		{
			var fileInfo = new FileInfo(Path.Combine(_directory, fileName));
			return fileInfo.Exists ? new FileInfoWrapper(fileInfo) : null;
		}

		public bool IsExists(string fileName)
		{
			return File.Exists(Path.Combine(_directory, fileName));
		}

		public Stream Create(string fileName)
		{
			return new FileStream(Path.Combine(_directory, fileName), FileMode.CreateNew, FileAccess.Write);
		}

		public void Delete(string fileName)
		{
			File.Delete(Path.Combine(_directory, fileName));
		}
	}
}