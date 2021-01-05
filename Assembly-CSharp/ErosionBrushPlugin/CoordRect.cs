// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.CoordRect
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace ErosionBrushPlugin
{
  [Serializable]
  public struct CoordRect
  {
    public Coord offset;
    public Coord size;

    public CoordRect(Coord offset, Coord size)
    {
      this.offset = offset;
      this.size = size;
    }

    public CoordRect(int offsetX, int offsetZ, int sizeX, int sizeZ)
    {
      this.offset = new Coord(offsetX, offsetZ);
      this.size = new Coord(sizeX, sizeZ);
    }

    public CoordRect(float offsetX, float offsetZ, float sizeX, float sizeZ)
    {
      this.offset = new Coord((int) offsetX, (int) offsetZ);
      this.size = new Coord((int) sizeX, (int) sizeZ);
    }

    public CoordRect(Rect r)
    {
      this.offset = new Coord((int) r.x, (int) r.y);
      this.size = new Coord((int) r.width, (int) r.height);
    }

    public bool isZero => this.size.x == 0 || this.size.z == 0;

    public int GetPos(int x, int z) => (z - this.offset.z) * this.size.x + x - this.offset.x;

    public Coord Max
    {
      get => this.offset + this.size;
      set => this.offset = value - this.size;
    }

    public Coord Min
    {
      get => this.offset;
      set => this.offset = value;
    }

    public Coord Center => this.offset + this.size / 2;

    public static bool operator >(CoordRect c1, CoordRect c2) => c1.size > c2.size;

    public static bool operator <(CoordRect c1, CoordRect c2) => c1.size < c2.size;

    public static bool operator ==(CoordRect c1, CoordRect c2) => c1.offset == c2.offset && c1.size == c2.size;

    public static bool operator !=(CoordRect c1, CoordRect c2) => c1.offset != c2.offset || c1.size != c2.size;

    public static CoordRect operator *(CoordRect c, int s) => new CoordRect(c.offset * s, c.size * s);

    public static CoordRect operator *(CoordRect c, float s) => new CoordRect(c.offset * s, c.size * s);

    public static CoordRect operator /(CoordRect c, int s) => new CoordRect(c.offset / s, c.size / s);

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => this.offset.x * 100000000 + this.offset.z * 1000000 + this.size.x * 1000 + this.size.z;

    public void Round(int val, bool inscribed = false)
    {
      this.offset.Round(val, inscribed);
      this.size.Round(val, !inscribed);
    }

    public void Round(CoordRect r, bool inscribed = false)
    {
      this.offset.Round(r.offset, inscribed);
      this.size.Round(r.size, !inscribed);
    }

    public void Clamp(Coord min, Coord max)
    {
      Coord max1 = this.Max;
      this.offset = Coord.Max(min, this.offset);
      this.size = Coord.Min(max - this.offset, max1 - this.offset);
      this.size.ClampPositive();
    }

    public static CoordRect Intersect(CoordRect c1, CoordRect c2)
    {
      c1.Clamp(c2.Min, c2.Max);
      return c1;
    }

    public Coord CoordByNum(int num)
    {
      int num1 = num / this.size.x;
      return new Coord(num - num1 * this.size.x + this.offset.x, num1 + this.offset.z);
    }

    public bool CheckInRange(int x, int z) => x - this.offset.x >= 0 && x - this.offset.x < this.size.x && z - this.offset.z >= 0 && z - this.offset.z < this.size.z;

    public bool CheckInRange(Coord coord) => coord.x >= this.offset.x && coord.x < this.offset.x + this.size.x && coord.z >= this.offset.z && coord.z < this.offset.z + this.size.z;

    public bool CheckInRangeAndBounds(int x, int z) => x > this.offset.x && x < this.offset.x + this.size.x - 1 && z > this.offset.z && z < this.offset.z + this.size.z - 1;

    public bool CheckInRangeAndBounds(Coord coord) => coord.x > this.offset.x && coord.x < this.offset.x + this.size.x - 1 && coord.z > this.offset.z && coord.z < this.offset.z + this.size.z - 1;

    public bool Divisible(float factor) => (double) this.offset.x % (double) factor == 0.0 && (double) this.offset.z % (double) factor == 0.0 && (double) this.size.x % (double) factor == 0.0 && (double) this.size.z % (double) factor == 0.0;

    public override string ToString() => base.ToString() + ": offsetX:" + (object) this.offset.x + " offsetZ:" + (object) this.offset.z + " sizeX:" + (object) this.size.x + " sizeZ:" + (object) this.size.z;

    public Vector2 ToWorldspace(Coord coord, Rect worldRect) => new Vector2(1f * (float) (coord.x - this.offset.x) / (float) this.size.x * worldRect.width + worldRect.x, 1f * (float) (coord.z - this.offset.z) / (float) this.size.z * worldRect.height + worldRect.y);

    public Coord ToLocalspace(Vector2 pos, Rect worldRect) => new Coord((int) (((double) pos.x - (double) worldRect.x) / (double) worldRect.width * (double) this.size.x + (double) this.offset.x), (int) (((double) pos.y - (double) worldRect.y) / (double) worldRect.height * (double) this.size.z + (double) this.offset.z));
  }
}
