using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace RMArt.Core
{
	public static class ImagingHelper
	{
		private static readonly ImageCodecInfo _jpegEncoder =
			ImageCodecInfo.GetImageEncoders().First(c => c.MimeType == ImageFormat.Jpeg.GetMimeType());

		public static Image MakeThumb(Image source, int maxWidth, int maxHeight, Color? backgroundColor = null)
		{
			var thumbSize = ThumbSize(source.Width, source.Height, maxWidth, maxHeight);
			var width = thumbSize.Width;
			var height = thumbSize.Height;

			var thumbnail = new Bitmap(width, height);
			using (var g = Graphics.FromImage(thumbnail))
			{
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.PixelOffsetMode = PixelOffsetMode.HighQuality;
				g.CompositingQuality = CompositingQuality.HighQuality;
				if (backgroundColor.HasValue)
					g.Clear(backgroundColor.Value);
				g.DrawImage(source, 0, 0, width, height);
			}
			return thumbnail;
		}

		public static Size ThumbSize(
			int sourceWidth,
			int sourceHeight,
			int width,
			int height,
			bool preserveAspectRatio = true,
			bool preventEnlarge = true)
		{
			if (preserveAspectRatio)
			{
				var h = height * 100.0 / sourceHeight;
				var w = width * 100.0 / sourceWidth;
				if (h > w)
					height = (int)Math.Round(w * sourceHeight / 100.0);
				else if (h < w)
					width = (int)Math.Round(h * sourceWidth / 100.0);
			}
			if (preventEnlarge)
			{
				if (height > sourceHeight)
					height = sourceHeight;
				if (width > sourceWidth)
					width = sourceWidth;
			}
			return new Size(width, height);
		}

		public static void SaveAsJpeg(this Image image, string path, long quality = 95)
		{
			var encoderParameters = new EncoderParameters();
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
			image.Save(path, _jpegEncoder, encoderParameters);
		}

		public static void SaveAsJpeg(this Image image, Stream stream, long quality = 95)
		{
			var encoderParameters = new EncoderParameters();
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
			image.Save(stream, _jpegEncoder, encoderParameters);
		}

		public static byte[] SaveAsJpeg(this Image image, long quality = 95)
		{
			var encoderParameters = new EncoderParameters();
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
			return image.SaveAsJpeg(_jpegEncoder, encoderParameters);
		}

		public static byte[] SaveAsJpeg(this Image image, ImageCodecInfo encoder, EncoderParameters encoderParams)
		{
			using (var memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, encoder, encoderParams);
				return memoryStream.ToArray();
			}
		}

		public static byte[] ComputeBitmapHash(this Bitmap bitmap)
		{
			var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
			try
			{
				var size = Math.Abs(bmpData.Stride) * bitmap.Height;
				var bytes = new byte[size];
				Marshal.Copy(bmpData.Scan0, bytes, 0, size);
				using (var md5 = MD5.Create())
					return md5.ComputeHash(bytes);
			}
			finally
			{
				bitmap.UnlockBits(bmpData);
			}
		}
	}
}