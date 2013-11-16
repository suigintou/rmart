using System;
using System.IO;

namespace RMArt.Core
{
	public interface IFileInfo
	{
		Stream OpenRead();
		DateTime LastModifiedTime { get; }
		string LocalFilePath { get; }
	}
}