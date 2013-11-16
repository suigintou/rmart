using System;
using System.IO;

namespace RMArt.Core
{
	public class FileInfoWrapper : IFileInfo
	{
		private readonly FileInfo _fileInfo;

		public FileInfoWrapper(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public Stream OpenRead()
		{
			return _fileInfo.OpenRead();
		}

		public DateTime LastModifiedTime
		{
			get { return _fileInfo.LastWriteTimeUtc; }
		}

		public string LocalFilePath
		{
			get { return _fileInfo.FullName; }
		}
	}
}