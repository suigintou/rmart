using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RMArt.Core
{
	public static class HashHelper
	{
		public static byte[] ComputeMD5Hash(this byte[] data)
		{
			using (var md5 = MD5.Create())
				return md5.ComputeHash(data);
		}

		public static byte[] ComputeMD5Hash(this Stream stream)
		{
			using (var md5 = MD5.Create())
				return md5.ComputeHash(stream);
		}

		public static byte[] ComputeMD5Hash(string filePath)
		{
			using (var fs = File.OpenRead(filePath))
			using (var md5 = MD5.Create())
				return md5.ComputeHash(fs);
		}

		public static byte[] ComputeSHA1Hash(this byte[] data)
		{
			using (var md5 = SHA1.Create())
				return md5.ComputeHash(data);
		}

		public static byte[] ComputeSHA1Hash(this string text)
		{
			return Encoding.Unicode.GetBytes(text).ComputeSHA1Hash();
		}
	}
}