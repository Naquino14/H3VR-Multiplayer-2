using System;
using UnityEngine;

namespace ErosionBrushPlugin
{
	public class Matrix2<T> : ICloneable
	{
		public T[] array;

		public CoordRect rect;

		public int pos;

		public int count;

		public T this[int x, int z]
		{
			get
			{
				return array[(z - rect.offset.z) * rect.size.x + x - rect.offset.x];
			}
			set
			{
				array[(z - rect.offset.z) * rect.size.x + x - rect.offset.x] = value;
			}
		}

		public T this[Coord c]
		{
			get
			{
				return array[(c.z - rect.offset.z) * rect.size.x + c.x - rect.offset.x];
			}
			set
			{
				array[(c.z - rect.offset.z) * rect.size.x + c.x - rect.offset.x] = value;
			}
		}

		public T this[Vector2 pos]
		{
			get
			{
				int num = (int)(pos.x + 0.5f);
				if (pos.x < 0f)
				{
					num--;
				}
				int num2 = (int)(pos.y + 0.5f);
				if (pos.y < 0f)
				{
					num2--;
				}
				return array[(num2 - rect.offset.z) * rect.size.x + num - rect.offset.x];
			}
			set
			{
				int num = (int)(pos.x + 0.5f);
				if (pos.x < 0f)
				{
					num--;
				}
				int num2 = (int)(pos.y + 0.5f);
				if (pos.y < 0f)
				{
					num2--;
				}
				array[(num2 - rect.offset.z) * rect.size.x + num - rect.offset.x] = value;
			}
		}

		public T nextX
		{
			get
			{
				return array[pos + 1];
			}
			set
			{
				array[pos + 1] = value;
			}
		}

		public T prevX
		{
			get
			{
				return array[pos - 1];
			}
			set
			{
				array[pos - 1] = value;
			}
		}

		public T nextZ
		{
			get
			{
				return array[pos + rect.size.x];
			}
			set
			{
				array[pos + rect.size.x] = value;
			}
		}

		public T prevZ
		{
			get
			{
				return array[pos - rect.size.x];
			}
			set
			{
				array[pos - rect.size.x] = value;
			}
		}

		public T nextXnextZ
		{
			get
			{
				return array[pos + rect.size.x + 1];
			}
			set
			{
				array[pos + rect.size.x + 1] = value;
			}
		}

		public T prevXnextZ
		{
			get
			{
				return array[pos + rect.size.x - 1];
			}
			set
			{
				array[pos + rect.size.x - 1] = value;
			}
		}

		public T nextXprevZ
		{
			get
			{
				return array[pos - rect.size.x + 1];
			}
			set
			{
				array[pos - rect.size.x + 1] = value;
			}
		}

		public T prevXprevZ
		{
			get
			{
				return array[pos - rect.size.x - 1];
			}
			set
			{
				array[pos - rect.size.x - 1] = value;
			}
		}

		public Matrix2()
		{
		}

		public Matrix2(int x, int z, T[] array = null)
		{
			rect = new CoordRect(0, 0, x, z);
			count = x * z;
			if (array != null && array.Length < count)
			{
				Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
			}
			if (array != null && array.Length >= count)
			{
				this.array = array;
			}
			else
			{
				this.array = new T[count];
			}
		}

		public Matrix2(CoordRect rect, T[] array = null)
		{
			this.rect = rect;
			count = rect.size.x * rect.size.z;
			if (array != null && array.Length < count)
			{
				Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
			}
			if (array != null && array.Length >= count)
			{
				this.array = array;
			}
			else
			{
				this.array = new T[count];
			}
		}

		public Matrix2(Coord offset, Coord size, T[] array = null)
		{
			rect = new CoordRect(offset, size);
			count = rect.size.x * rect.size.z;
			if (array != null && array.Length < count)
			{
				Debug.Log("Array length: " + array.Length + " is lower then matrix capacity: " + count);
			}
			if (array != null && array.Length >= count)
			{
				this.array = array;
			}
			else
			{
				this.array = new T[count];
			}
		}

		public void Clear()
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = default(T);
			}
		}

		public void ChangeRect(CoordRect newRect)
		{
			rect = newRect;
			count = newRect.size.x * newRect.size.z;
			if (array.Length < count)
			{
				array = new T[count];
			}
		}

		public virtual object Clone()
		{
			return Clone(null);
		}

		public Matrix2<T> Clone(Matrix2<T> result)
		{
			if (result == null)
			{
				result = new Matrix2<T>(rect);
			}
			result.rect = rect;
			result.pos = pos;
			result.count = count;
			if (result.array.Length != array.Length)
			{
				result.array = new T[array.Length];
			}
			for (int i = 0; i < array.Length; i++)
			{
				result.array[i] = array[i];
			}
			return result;
		}

		public void Fill(T v)
		{
			for (int i = 0; i < count; i++)
			{
				array[i] = v;
			}
		}

		public void Fill(Matrix2<T> m, bool removeBorders = false)
		{
			CoordRect centerRect = CoordRect.Intersect(rect, m.rect);
			Coord min = centerRect.Min;
			Coord max = centerRect.Max;
			for (int i = min.x; i < max.x; i++)
			{
				for (int j = min.z; j < max.z; j++)
				{
					this[i, j] = m[i, j];
				}
			}
			if (removeBorders)
			{
				RemoveBorders(centerRect);
			}
		}

		public void SetPos(int x, int z)
		{
			pos = (z - rect.offset.z) * rect.size.x + x - rect.offset.x;
		}

		public void SetPos(int x, int z, int s)
		{
			pos = (z - rect.offset.z) * rect.size.x + x - rect.offset.x + s * rect.size.x * rect.size.z;
		}

		public void MoveX()
		{
			pos++;
		}

		public void MoveZ()
		{
			pos += rect.size.x;
		}

		public void MovePrevX()
		{
			pos--;
		}

		public void MovePrevZ()
		{
			pos -= rect.size.x;
		}

		public void RemoveBorders()
		{
			Coord min = rect.Min;
			Coord coord = rect.Max - 1;
			for (int i = min.x; i <= coord.x; i++)
			{
				SetPos(i, min.z);
				array[pos] = nextZ;
			}
			for (int j = min.x; j <= coord.x; j++)
			{
				SetPos(j, coord.z);
				array[pos] = prevZ;
			}
			for (int k = min.z; k <= coord.z; k++)
			{
				SetPos(min.x, k);
				array[pos] = nextX;
			}
			for (int l = min.z; l <= coord.z; l++)
			{
				SetPos(coord.x, l);
				array[pos] = prevX;
			}
		}

		public void RemoveBorders(int borderMinX, int borderMinZ, int borderMaxX, int borderMaxZ)
		{
			Coord min = rect.Min;
			Coord max = rect.Max;
			if (borderMinZ != 0)
			{
				for (int i = min.x; i < max.x; i++)
				{
					T value = this[i, min.z + borderMinZ];
					for (int j = min.z; j < min.z + borderMinZ; j++)
					{
						this[i, j] = value;
					}
				}
			}
			if (borderMaxZ != 0)
			{
				for (int k = min.x; k < max.x; k++)
				{
					T value2 = this[k, max.z - borderMaxZ];
					for (int l = max.z - borderMaxZ; l < max.z; l++)
					{
						this[k, l] = value2;
					}
				}
			}
			if (borderMinX != 0)
			{
				for (int m = min.z; m < max.z; m++)
				{
					T value3 = this[min.x + borderMinX, m];
					for (int n = min.x; n < min.x + borderMinX; n++)
					{
						this[n, m] = value3;
					}
				}
			}
			if (borderMaxX == 0)
			{
				return;
			}
			for (int num = min.z; num < max.z; num++)
			{
				T value4 = this[max.x - borderMaxX, num];
				for (int num2 = max.x - borderMaxX; num2 < max.x; num2++)
				{
					this[num2, num] = value4;
				}
			}
		}

		public void RemoveBorders(CoordRect centerRect)
		{
			RemoveBorders(Mathf.Max(0, centerRect.offset.x - rect.offset.x), Mathf.Max(0, centerRect.offset.z - rect.offset.z), Mathf.Max(0, rect.Max.x - centerRect.Max.x + 1), Mathf.Max(0, rect.Max.z - centerRect.Max.z + 1));
		}
	}
}
