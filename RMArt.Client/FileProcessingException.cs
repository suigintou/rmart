using System;

namespace RMArt.Client
{
	public class FileProcessingException : Exception
	{
		public FileProcessingException(string message) : base(message)
		{
		}

		public FileProcessingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}