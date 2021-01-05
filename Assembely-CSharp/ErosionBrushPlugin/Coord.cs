using System;
using System.Collections.Generic;
using UnityEngine;

namespace ErosionBrushPlugin
{
	[Serializable]
	public struct Coord
	{
		public int x;

		public int z;

		public int Minimal => Mathf.Min(x, z);

		public int SqrMagnitude => x * x + z * z;

		public Vector3 vector3 => new Vector3(x, 0f, z);

		public Coord(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public static bool operator >(Coord c1, Coord c2)
		{
			return c1.x > c2.x && c1.z > c2.z;
		}

		public static bool operator <(Coord c1, Coord c2)
		{
			return c1.x < c2.x && c1.z < c2.z;
		}

		public static bool operator ==(Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.z == c2.z;
		}

		public static bool operator !=(Coord c1, Coord c2)
		{
			return c1.x != c2.x && c1.z != c2.z;
		}

		public static Coord operator +(Coord c, int s)
		{
			return new Coord(c.x + s, c.z + s);
		}

		public static Coord operator +(Coord c1, Coord c2)
		{
			return new Coord(c1.x + c2.x, c1.z + c2.z);
		}

		public static Coord operator -(Coord c, int s)
		{
			return new Coord(c.x - s, c.z - s);
		}

		public static Coord operator -(Coord c1, Coord c2)
		{
			return new Coord(c1.x - c2.x, c1.z - c2.z);
		}

		public static Coord operator *(Coord c, int s)
		{
			return new Coord(c.x * s, c.z * s);
		}

		public static Vector2 operator *(Coord c, Vector2 s)
		{
			return new Vector2((float)c.x * s.x, (float)c.z * s.y);
		}

		public static Vector3 operator *(Coord c, Vector3 s)
		{
			return new Vector3((float)c.x * s.x, s.y, (float)c.z * s.z);
		}

		public static Coord operator *(Coord c, float s)
		{
			return new Coord((int)((float)c.x * s), (int)((float)c.z * s));
		}

		public static Coord operator /(Coord c, int s)
		{
			return new Coord(c.x / s, c.z / s);
		}

		public static Coord operator /(Coord c, float s)
		{
			return new Coord((int)((float)c.x / s), (int)((float)c.z / s));
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return x * 10000000 + z;
		}

		public void Round(int val, bool ceil = false)
		{
			x = ((!ceil) ? Mathf.FloorToInt(1f * (float)x / (float)val) : Mathf.CeilToInt(1f * (float)x / (float)val)) * val;
			z = ((!ceil) ? Mathf.FloorToInt(1f * (float)z / (float)val) : Mathf.CeilToInt(1f * (float)z / (float)val)) * val;
		}

		public void Round(Coord c, bool ceil = false)
		{
			x = ((!ceil) ? Mathf.CeilToInt(1f * (float)x / (float)c.x) : Mathf.FloorToInt(1f * (float)x / (float)c.x)) * c.x;
			z = ((!ceil) ? Mathf.CeilToInt(1f * (float)z / (float)c.z) : Mathf.FloorToInt(1f * (float)z / (float)c.z)) * c.z;
		}

		public void ClampPositive()
		{
			x = Mathf.Max(0, x);
			z = Mathf.Max(0, z);
		}

		public void ClampByRect(CoordRect rect)
		{
			if (x < rect.offset.x)
			{
				x = rect.offset.x;
			}
			if (x >= rect.offset.x + rect.size.x)
			{
				x = rect.offset.x + rect.size.x - 1;
			}
			if (z < rect.offset.z)
			{
				z = rect.offset.z;
			}
			if (z >= rect.offset.z + rect.size.z)
			{
				z = rect.offset.z + rect.size.z - 1;
			}
		}

		public static Coord Min(Coord c1, Coord c2)
		{
			return new Coord(Mathf.Min(c1.x, c2.x), Mathf.Min(c1.z, c2.z));
		}

		public static Coord Max(Coord c1, Coord c2)
		{
			return new Coord(Mathf.Max(c1.x, c2.x), Mathf.Max(c1.z, c2.z));
		}

		public static float Distance(Coord c1, Coord c2)
		{
			return Mathf.Sqrt((c1.x - c2.x) * (c1.x - c2.x) + (c1.z - c2.z) * (c1.z - c2.z));
		}

		public override string ToString()
		{
			return base.ToString() + " x:" + x + " z:" + z;
		}

		public IEnumerable<Coord> DistancePerimeter(int dist)
		{
			for (int i = 0; i < dist; i++)
			{
				yield return new Coord(x - i, z - dist);
				yield return new Coord(x - dist, z + i);
				yield return new Coord(x + i, z + dist);
				yield return new Coord(x + dist, z - i);
				yield return new Coord(x + i + 1, z - dist);
				yield return new Coord(x - dist, z - i - 1);
				yield return new Coord(x - i - 1, z + dist);
				yield return new Coord(x + dist, z + i + 1);
			}
		}

		public IEnumerable<Coord> CircularPerimeter(int dist)
		{
			for (int iz3 = 0; iz3 > -dist; iz3--)
			{
				yield return new Coord(x - dist, z + iz3);
			}
			for (int ix2 = -dist; ix2 < dist; ix2++)
			{
				yield return new Coord(x + ix2, z - dist);
			}
			for (int iz2 = -dist; iz2 < dist; iz2++)
			{
				yield return new Coord(x + dist, z + iz2);
			}
			for (int ix = dist; ix > -dist; ix--)
			{
				yield return new Coord(x + ix, z + dist);
			}
			for (int iz = dist; iz > 0; iz--)
			{
				yield return new Coord(x - dist, z + iz);
			}
		}

		public IEnumerable<Coord> CircularArea(int maxDist)
		{
			yield return this;
			for (int i = 0; i < maxDist; i++)
			{
				foreach (Coord item in CircularPerimeter(i))
				{
					yield return item;
				}
			}
		}

		public IEnumerable<Coord> DistanceArea(int maxDist)
		{
			yield return this;
			for (int i = 0; i < maxDist; i++)
			{
				foreach (Coord item in DistancePerimeter(i))
				{
					yield return item;
				}
			}
		}

		public IEnumerable<Coord> DistanceArea(CoordRect rect)
		{
			int maxDist = Mathf.Max(x - rect.offset.x, rect.Max.x - x, z - rect.offset.z, rect.Max.z - z) + 1;
			if (rect.CheckInRange(this))
			{
				yield return this;
			}
			for (int i = 0; i < maxDist; i++)
			{
				foreach (Coord c in DistancePerimeter(i))
				{
					if (rect.CheckInRange(c))
					{
						yield return c;
					}
				}
			}
		}

		public Vector3 ToVector3(float cellSize)
		{
			return new Vector3((float)x * cellSize, 0f, (float)z * cellSize);
		}

		public Vector2 ToVector2(float cellSize)
		{
			return new Vector2((float)x * cellSize, (float)z * cellSize);
		}

		public Rect ToRect(float cellSize)
		{
			return new Rect((float)x * cellSize, (float)z * cellSize, cellSize, cellSize);
		}
	}
}
