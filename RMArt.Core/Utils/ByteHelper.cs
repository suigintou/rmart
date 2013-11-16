using System;
using System.IO;
using System.Text;

namespace RMArt.Core
{
	public static class ByteHelper
	{
		public static string ToHexString(this byte[] data)
		{
			var hex = new StringBuilder(data.Length * 2);
			foreach (var b in data)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		public static byte[] FromHexString(string hex)
		{
			var bytes = new byte[hex.Length / 2];
			for (var i = 0; i < hex.Length; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		public static MemoryStream CopyToMemoryStream(this Stream stream)
		{
			var memoryStream = new MemoryStream(stream.CanSeek ? (int)stream.Length : 0);
			stream.CopyTo(memoryStream);
			return memoryStream;
		}

		public static byte[] ReadAll(this Stream stream)
		{
			using (var memoryStream = stream.CopyToMemoryStream())
				return memoryStream.ToArray();
		}
	}
}