// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Matrix2`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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

    public Matrix2()
    {
    }

    public Matrix2(int x, int z, T[] array = null)
    {
      this.rect = new CoordRect(0, 0, x, z);
      this.count = x * z;
      if (array != null && array.Length < this.count)
        Debug.Log((object) ("Array length: " + (object) array.Length + " is lower then matrix capacity: " + (object) this.count));
      if (array != null && array.Length >= this.count)
        this.array = array;
      else
        this.array = new T[this.count];
    }

    public Matrix2(CoordRect rect, T[] array = null)
    {
      this.rect = rect;
      this.count = rect.size.x * rect.size.z;
      if (array != null && array.Length < this.count)
        Debug.Log((object) ("Array length: " + (object) array.Length + " is lower then matrix capacity: " + (object) this.count));
      if (array != null && array.Length >= this.count)
        this.array = array;
      else
        this.array = new T[this.count];
    }

    public Matrix2(Coord offset, Coord size, T[] array = null)
    {
      this.rect = new CoordRect(offset, size);
      this.count = this.rect.size.x * this.rect.size.z;
      if (array != null && array.Length < this.count)
        Debug.Log((object) ("Array length: " + (object) array.Length + " is lower then matrix capacity: " + (object) this.count));
      if (array != null && array.Length >= this.count)
        this.array = array;
      else
        this.array = new T[this.count];
    }

    public T this[int x, int z]
    {
      get => this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x];
      set => this.array[(z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x] = value;
    }

    public T this[Coord c]
    {
      get => this.array[(c.z - this.rect.offset.z) * this.rect.size.x + c.x - this.rect.offset.x];
      set => this.array[(c.z - this.rect.offset.z) * this.rect.size.x + c.x - this.rect.offset.x] = value;
    }

    public T this[Vector2 pos]
    {
      get
      {
        int num1 = (int) ((double) pos.x + 0.5);
        if ((double) pos.x < 0.0)
          --num1;
        int num2 = (int) ((double) pos.y + 0.5);
        if ((double) pos.y < 0.0)
          --num2;
        return this.array[(num2 - this.rect.offset.z) * this.rect.size.x + num1 - this.rect.offset.x];
      }
      set
      {
        int num1 = (int) ((double) pos.x + 0.5);
        if ((double) pos.x < 0.0)
          --num1;
        int num2 = (int) ((double) pos.y + 0.5);
        if ((double) pos.y < 0.0)
          --num2;
        this.array[(num2 - this.rect.offset.z) * this.rect.size.x + num1 - this.rect.offset.x] = value;
      }
    }

    public void Clear()
    {
      for (int index = 0; index < this.array.Length; ++index)
        this.array[index] = default (T);
    }

    public void ChangeRect(CoordRect newRect)
    {
      this.rect = newRect;
      this.count = newRect.size.x * newRect.size.z;
      if (this.array.Length >= this.count)
        return;
      this.array = new T[this.count];
    }

    public virtual object Clone() => (object) this.Clone((Matrix2<T>) null);

    public Matrix2<T> Clone(Matrix2<T> result)
    {
      if (result == null)
        result = new Matrix2<T>(this.rect);
      result.rect = this.rect;
      result.pos = this.pos;
      result.count = this.count;
      if (result.array.Length != this.array.Length)
        result.array = new T[this.array.Length];
      for (int index = 0; index < this.array.Length; ++index)
        result.array[index] = this.array[index];
      return result;
    }

    public void Fill(T v)
    {
      for (int index = 0; index < this.count; ++index)
        this.array[index] = v;
    }

    public void Fill(Matrix2<T> m, bool removeBorders = false)
    {
      CoordRect centerRect = CoordRect.Intersect(this.rect, m.rect);
      Coord min = centerRect.Min;
      Coord max = centerRect.Max;
      for (int x = min.x; x < max.x; ++x)
      {
        for (int z = min.z; z < max.z; ++z)
          this[x, z] = m[x, z];
      }
      if (!removeBorders)
        return;
      this.RemoveBorders(centerRect);
    }

    public void SetPos(int x, int z) => this.pos = (z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x;

    public void SetPos(int x, int z, int s) => this.pos = (z - this.rect.offset.z) * this.rect.size.x + x - this.rect.offset.x + s * this.rect.size.x * this.rect.size.z;

    public void MoveX() => ++this.pos;

    public void MoveZ() => this.pos += this.rect.size.x;

    public void MovePrevX() => --this.pos;

    public void MovePrevZ() => this.pos -= this.rect.size.x;

    public T nextX
    {
      get => this.array[this.pos + 1];
      set => this.array[this.pos + 1] = value;
    }

    public T prevX
    {
      get => this.array[this.pos - 1];
      set => this.array[this.pos - 1] = value;
    }

    public T nextZ
    {
      get => this.array[this.pos + this.rect.size.x];
      set => this.array[this.pos + this.rect.size.x] = value;
    }

    public T prevZ
    {
      get => this.array[this.pos - this.rect.size.x];
      set => this.array[this.pos - this.rect.size.x] = value;
    }

    public T nextXnextZ
    {
      get => this.array[this.pos + this.rect.size.x + 1];
      set => this.array[this.pos + this.rect.size.x + 1] = value;
    }

    public T prevXnextZ
    {
      get => this.array[this.pos + this.rect.size.x - 1];
      set => this.array[this.pos + this.rect.size.x - 1] = value;
    }

    public T nextXprevZ
    {
      get => this.array[this.pos - this.rect.size.x + 1];
      set => this.array[this.pos - this.rect.size.x + 1] = value;
    }

    public T prevXprevZ
    {
      get => this.array[this.pos - this.rect.size.x - 1];
      set => this.array[this.pos - this.rect.size.x - 1] = value;
    }

    public void RemoveBorders()
    {
      Coord min = this.rect.Min;
      Coord coord = this.rect.Max - 1;
      for (int x = min.x; x <= coord.x; ++x)
      {
        this.SetPos(x, min.z);
        this.array[this.pos] = this.nextZ;
      }
      for (int x = min.x; x <= coord.x; ++x)
      {
        this.SetPos(x, coord.z);
        this.array[this.pos] = this.prevZ;
      }
      for (int z = min.z; z <= coord.z; ++z)
      {
        this.SetPos(min.x, z);
        this.array[this.pos] = this.nextX;
      }
      for (int z = min.z; z <= coord.z; ++z)
      {
        this.SetPos(coord.x, z);
        this.array[this.pos] = this.prevX;
      }
    }

    public void RemoveBorders(int borderMinX, int borderMinZ, int borderMaxX, int borderMaxZ)
    {
      Coord min = this.rect.Min;
      Coord max = this.rect.Max;
      if (borderMinZ != 0)
      {
        for (int x = min.x; x < max.x; ++x)
        {
          T obj = this[x, min.z + borderMinZ];
          for (int z = min.z; z < min.z + borderMinZ; ++z)
            this[x, z] = obj;
        }
      }
      if (borderMaxZ != 0)
      {
        for (int x = min.x; x < max.x; ++x)
        {
          T obj = this[x, max.z - borderMaxZ];
          for (int z = max.z - borderMaxZ; z < max.z; ++z)
            this[x, z] = obj;
        }
      }
      if (borderMinX != 0)
      {
        for (int z = min.z; z < max.z; ++z)
        {
          T obj = this[min.x + borderMinX, z];
          for (int x = min.x; x < min.x + borderMinX; ++x)
            this[x, z] = obj;
        }
      }
      if (borderMaxX == 0)
        return;
      for (int z = min.z; z < max.z; ++z)
      {
        T obj = this[max.x - borderMaxX, z];
        for (int x = max.x - borderMaxX; x < max.x; ++x)
          this[x, z] = obj;
      }
    }

    public void RemoveBorders(CoordRect centerRect) => this.RemoveBorders(Mathf.Max(0, centerRect.offset.x - this.rect.offset.x), Mathf.Max(0, centerRect.offset.z - this.rect.offset.z), Mathf.Max(0, this.rect.Max.x - centerRect.Max.x + 1), Mathf.Max(0, this.rect.Max.z - centerRect.Max.z + 1));
  }
}
