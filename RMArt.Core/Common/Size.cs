using System;

namespace RMArt.Core
{
	public struct Size : IEquatable<Size>
	{
		private readonly int _width;
		private readonly int _height;

		public Size(int width, int height)
		{
			_width = width;
			_height = height;
		}

		public int Height
		{
			get { return _height; }
		}

		public int Width
		{
			get { return _width; }
		}

		public bool Equals(Size other)
		{
			return other._width == _width && other._height == _height;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof(Size))
				return false;
			return Equals((Size)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return _width * _height;
			}
		}

		public static bool operator ==(Size left, Size right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Size left, Size right)
		{
			return !left.Equals(right);
		}
	}
}