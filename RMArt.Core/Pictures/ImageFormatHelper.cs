using System;
using System.Drawing;

namespace RMArt.Core
{
	public static class ImageFormatHelper
	{
		public static string GetFileExtension(this ImageFormat imageFormat)
		{
			switch (imageFormat)
			{
				case ImageFormat.Jpeg:
					return "jpg";
				case ImageFormat.Gif:
					return "gif";
				case ImageFormat.Png:
					return "png";
				default:
					throw new NotSupportedException("Format is not supported.");
			}
		}

		public static string GetMimeType(this ImageFormat imageFormat)
		{
			switch (imageFormat)
			{
				case ImageFormat.Jpeg:
					return "image/jpeg";
				case ImageFormat.Gif:
					return "image/gif";
				case ImageFormat.Png:
					return "image/png";
				default:
					throw new NotSupportedException("Format is not supported.");
			}
		}

		public static string GetName(this ImageFormat imageFormat)
		{
			return imageFormat.ToString().ToUpper();
		}

		public static ImageFormat GetFormat(this Image image)
		{
			if (image == null)
				throw new ArgumentNullException("image");

			if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
				return ImageFormat.Jpeg;
			if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
				return ImageFormat.Png;
			if (image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
				return ImageFormat.Gif;

			throw new NotSupportedException("Format is not supported.");
		} 
	}
}