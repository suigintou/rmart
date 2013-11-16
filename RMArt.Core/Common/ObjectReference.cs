using System;

namespace RMArt.Core
{
	public struct ObjectReference : IEquatable<ObjectReference>
	{
		private readonly ObjectType _type;
		private readonly int _id;

		public ObjectReference(ObjectType type, int id)
		{
			_type = type;
			_id = id;
		}
		
		public ObjectType Type
		{
			get { return _type; }
		}

		public int ID
		{
			get { return _id; }
		}

		public bool Equals(ObjectReference other)
		{
			return Equals(other._type, _type) && other._id == _id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (obj.GetType() != typeof (ObjectReference))
				return false;
			return Equals((ObjectReference) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_type.GetHashCode()*397) ^ _id;
			}
		}

		public static bool operator ==(ObjectReference left, ObjectReference right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ObjectReference left, ObjectReference right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			return string.Format("Type: {0}, ID: {1}", _type, _id);
		}
	}
}