using System;

namespace Karen90MmoFramework
{
	public struct MmoGuid
	{
		private readonly Int64 raw;

		/// <summary>
		/// Creates a <see cref="MmoGuid"/> using an exisiting Guid
		/// </summary>
		public MmoGuid(Int64 guid)
		{
			this.raw = guid;
		}

		/// <summary>
		/// Creates a new <see cref="MmoGuid"/> using a type and id.
		/// </summary>
		public MmoGuid(Byte type, Int32 id)
		{
			this.raw = (((Int64) type) << 56) | (uint) id;
		}

		/// <summary>
		/// Creates a new <see cref="MmoGuid"/> using a type, id, and a family id
		/// </summary>
		public MmoGuid(Byte type, Int32 id, Int16 subId)
		{
			this.raw = (((Int64) type) << 56) | (((Int64) (uint) subId) << 40) | (uint) id;
		}

		/// <summary>
		/// Gets the id.
		/// </summary>
		public Int32 Id
		{
			get
			{
				return (Int32) (raw & 0xFFFFFFFF);
			}
		}

		/// <summary>
		/// Gets the family id.
		/// </summary>
		public Int16 SubId
		{
			get
			{
				return (Int16) ((raw & 0xFFFF0000000000) >> 40);
			}
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public Byte Type
		{
			get
			{
				return (Byte) (raw >> 56);
			}
		}

		#region Operator Overloading

		public static implicit operator long(MmoGuid guid)
		{
			return guid.raw;
		}

		public static implicit operator MmoGuid(long guid)
		{
			return new MmoGuid(guid);
		}

		public static bool operator ==(MmoGuid lhs, MmoGuid rhs)
		{
			return lhs.raw == rhs.raw;
		}

		public static bool operator !=(MmoGuid lhs, MmoGuid rhs)
		{
			return lhs.raw != rhs.raw;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Compares two object for equality
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof (MmoGuid))
				return false;

			return this.raw == ((MmoGuid) obj).raw;
		}

		/// <summary>
		/// Gets the hash code
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.raw.GetHashCode();
		}

		/// <summary>
		/// Converts to String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "{ " + this.Type + ", " + this.Id + " }";
		}

		#endregion
	};
}
