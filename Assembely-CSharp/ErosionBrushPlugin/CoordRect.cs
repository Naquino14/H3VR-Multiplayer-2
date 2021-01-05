using System;
using UnityEngine;

namespace ErosionBrushPlugin
{
	[Serializable]
	public struct CoordRect
	{
		public Coord offset;

		public Coord size;

		public bool isZero => size.x == 0 || size.z == 0;

		public Coord Max
		{
			get
			{
				return offset + size;
			}
			set
			{
				offset = value - size;
			}
		}

		public Coord Min
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		public Coord Center => offset + size / 2;

		public CoordRect(Coord offset, Coord size)
		{
			this.offset = offset;
			this.size = size;
		}

		public CoordRect(int offsetX, int offsetZ, int sizeX, int sizeZ)
		{
			offset = new Coord(offsetX, offsetZ);
			size = new Coord(sizeX, sizeZ);
		}

		public CoordRect(float offsetX, float offsetZ, float sizeX, float sizeZ)
		{
			offset = new Coord((int)offsetX, (int)offsetZ);
			size = new Coord((int)sizeX, (int)sizeZ);
		}

		public CoordRect(Rect r)
		{
			offset = new Coord((int)r.x, (int)r.y);
			size = new Coord((int)r.width, (int)r.height);
		}

		public int GetPos(int x, int z)
		{
			return (z - offset.z) * size.x + x - offset.x;
		}

		public static bool operator >(CoordRect c1, CoordRect c2)
		{
			return c1.size > c2.size;
		}

		public static bool operator <(CoordRect c1, CoordRect c2)
		{
			return c1.size < c2.size;
		}

		public static bool operator ==(CoordRect c1, CoordRect c2)
		{
			return c1.offset == c2.offset && c1.size == c2.size;
		}

		public static bool operator !=(CoordRect c1, CoordRect c2)
		{
			return c1.offset != c2.offset || c1.size != c2.size;
		}

		public static CoordRect operator *(CoordRect c, int s)
		{
			return new CoordRect(c.offset * s, c.size * s);
		}

		public static CoordRect operator *(CoordRect c, float s)
		{
			return new CoordRect(c.offset * s, c.size * s);
		}

		public static CoordRect operator /(CoordRect c, int s)
		{
			return new CoordRect(c.offset / s, c.size / s);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return offset.x * 100000000 + offset.z * 1000000 + size.x * 1000 + size.z;
		}

		public void Round(int val, bool inscribed = false)
		{
			offset.Round(val, inscribed);
			size.Round(val, !inscribed);
		}

		public void Round(CoordRect r, bool inscribed = false)
		{
			offset.Round(r.offset, inscribed);
			size.Round(r.size, !inscribed);
		}

		public void Clamp(Coord min, Coord max)
		{
			Coord max2 = Max;
			offset = Coord.Max(min, offset);
			size = Coord.Min(max - offset, max2 - offset);
			size.ClampPositive();
		}

		public static CoordRect Intersect(CoordRect c1, CoordRect c2)
		{
			c1.Clamp(c2.Min, c2.Max);
			return c1;
		}

		public Coord CoordByNum(int num)
		{
			int num2 = num / size.x;
			int num3 = num - num2 * size.x;
			return new Coord(num3 + offset.x, num2 + offset.z);
		}

		public bool CheckInRange(int x, int z)
		{
			return x - offset.x >= 0 && x - offset.x < size.x && z - offset.z >= 0 && z - offset.z < size.z;
		}

		public bool CheckInRange(Coord coord)
		{
			return coord.x >= offset.x && coord.x < offset.x + size.x && coord.z >= offset.z && coord.z < offset.z + size.z;
		}

		public bool CheckInRangeAndBounds(int x, int z)
		{
			return x > offset.x && x < offset.x + size.x - 1 && z > offset.z && z < offset.z + size.z - 1;
		}

		public bool CheckInRangeAndBounds(Coord coord)
		{
			return coord.x > offset.x && coord.x < offset.x + size.x - 1 && coord.z > offset.z && coord.z < offset.z + size.z - 1;
		}

		public bool Divisible(float factor)
		{
			return (float)offset.x % factor == 0f && (float)offset.z % factor == 0f && (float)size.x % factor == 0f && (float)size.z % factor == 0f;
		}

		public override string ToString()
		{
			return base.ToString() + ": offsetX:" + offset.x + " offsetZ:" + offset.z + " sizeX:" + size.x + " sizeZ:" + size.z;
		}

		public Vector2 ToWorldspace(Coord coord, Rect worldRect)
		{
			return new Vector2(1f * (float)(coord.x - offset.x) / (float)size.x * worldRect.width + worldRect.x, 1f * (float)(coord.z - offset.z) / (float)size.z * worldRect.height + worldRect.y);
		}

		public Coord ToLocalspace(Vector2 pos, Rect worldRect)
		{
			return new Coord((int)((pos.x - worldRect.x) / worldRect.width * (float)size.x + (float)offset.x), (int)((pos.y - worldRect.y) / worldRect.height * (float)size.z + (float)offset.z));
		}
	}
}
