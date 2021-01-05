// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Coord
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ErosionBrushPlugin
{
  [Serializable]
  public struct Coord
  {
    public int x;
    public int z;

    public Coord(int x, int z)
    {
      this.x = x;
      this.z = z;
    }

    public static bool operator >(Coord c1, Coord c2) => c1.x > c2.x && c1.z > c2.z;

    public static bool operator <(Coord c1, Coord c2) => c1.x < c2.x && c1.z < c2.z;

    public static bool operator ==(Coord c1, Coord c2) => c1.x == c2.x && c1.z == c2.z;

    public static bool operator !=(Coord c1, Coord c2) => c1.x != c2.x && c1.z != c2.z;

    public static Coord operator +(Coord c, int s) => new Coord(c.x + s, c.z + s);

    public static Coord operator +(Coord c1, Coord c2) => new Coord(c1.x + c2.x, c1.z + c2.z);

    public static Coord operator -(Coord c, int s) => new Coord(c.x - s, c.z - s);

    public static Coord operator -(Coord c1, Coord c2) => new Coord(c1.x - c2.x, c1.z - c2.z);

    public static Coord operator *(Coord c, int s) => new Coord(c.x * s, c.z * s);

    public static Vector2 operator *(Coord c, Vector2 s) => new Vector2((float) c.x * s.x, (float) c.z * s.y);

    public static Vector3 operator *(Coord c, Vector3 s) => new Vector3((float) c.x * s.x, s.y, (float) c.z * s.z);

    public static Coord operator *(Coord c, float s) => new Coord((int) ((double) c.x * (double) s), (int) ((double) c.z * (double) s));

    public static Coord operator /(Coord c, int s) => new Coord(c.x / s, c.z / s);

    public static Coord operator /(Coord c, float s) => new Coord((int) ((double) c.x / (double) s), (int) ((double) c.z / (double) s));

    public override bool Equals(object obj) => base.Equals(obj);

    public override int GetHashCode() => this.x * 10000000 + this.z;

    public int Minimal => Mathf.Min(this.x, this.z);

    public int SqrMagnitude => this.x * this.x + this.z * this.z;

    public Vector3 vector3 => new Vector3((float) this.x, 0.0f, (float) this.z);

    public void Round(int val, bool ceil = false)
    {
      this.x = (!ceil ? Mathf.FloorToInt(1f * (float) this.x / (float) val) : Mathf.CeilToInt(1f * (float) this.x / (float) val)) * val;
      this.z = (!ceil ? Mathf.FloorToInt(1f * (float) this.z / (float) val) : Mathf.CeilToInt(1f * (float) this.z / (float) val)) * val;
    }

    public void Round(Coord c, bool ceil = false)
    {
      this.x = (!ceil ? Mathf.CeilToInt(1f * (float) this.x / (float) c.x) : Mathf.FloorToInt(1f * (float) this.x / (float) c.x)) * c.x;
      this.z = (!ceil ? Mathf.CeilToInt(1f * (float) this.z / (float) c.z) : Mathf.FloorToInt(1f * (float) this.z / (float) c.z)) * c.z;
    }

    public void ClampPositive()
    {
      this.x = Mathf.Max(0, this.x);
      this.z = Mathf.Max(0, this.z);
    }

    public void ClampByRect(CoordRect rect)
    {
      if (this.x < rect.offset.x)
        this.x = rect.offset.x;
      if (this.x >= rect.offset.x + rect.size.x)
        this.x = rect.offset.x + rect.size.x - 1;
      if (this.z < rect.offset.z)
        this.z = rect.offset.z;
      if (this.z < rect.offset.z + rect.size.z)
        return;
      this.z = rect.offset.z + rect.size.z - 1;
    }

    public static Coord Min(Coord c1, Coord c2) => new Coord(Mathf.Min(c1.x, c2.x), Mathf.Min(c1.z, c2.z));

    public static Coord Max(Coord c1, Coord c2) => new Coord(Mathf.Max(c1.x, c2.x), Mathf.Max(c1.z, c2.z));

    public static float Distance(Coord c1, Coord c2) => Mathf.Sqrt((float) ((c1.x - c2.x) * (c1.x - c2.x) + (c1.z - c2.z) * (c1.z - c2.z)));

    public override string ToString() => base.ToString() + " x:" + (object) this.x + " z:" + (object) this.z;

    [DebuggerHidden]
    public IEnumerable<Coord> DistancePerimeter(int dist)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Coord.\u003CDistancePerimeter\u003Ec__Iterator0 perimeterCIterator0 = new Coord.\u003CDistancePerimeter\u003Ec__Iterator0()
      {
        dist = dist,
        \u0024this = this
      };
      // ISSUE: reference to a compiler-generated field
      perimeterCIterator0.\u0024PC = -2;
      return (IEnumerable<Coord>) perimeterCIterator0;
    }

    [DebuggerHidden]
    public IEnumerable<Coord> CircularPerimeter(int dist)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Coord.\u003CCircularPerimeter\u003Ec__Iterator1 perimeterCIterator1 = new Coord.\u003CCircularPerimeter\u003Ec__Iterator1()
      {
        dist = dist,
        \u0024this = this
      };
      // ISSUE: reference to a compiler-generated field
      perimeterCIterator1.\u0024PC = -2;
      return (IEnumerable<Coord>) perimeterCIterator1;
    }

    [DebuggerHidden]
    public IEnumerable<Coord> CircularArea(int maxDist)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Coord.\u003CCircularArea\u003Ec__Iterator2 circularAreaCIterator2 = new Coord.\u003CCircularArea\u003Ec__Iterator2()
      {
        maxDist = maxDist,
        \u0024this = this
      };
      // ISSUE: reference to a compiler-generated field
      circularAreaCIterator2.\u0024PC = -2;
      return (IEnumerable<Coord>) circularAreaCIterator2;
    }

    [DebuggerHidden]
    public IEnumerable<Coord> DistanceArea(int maxDist)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Coord.\u003CDistanceArea\u003Ec__Iterator3 distanceAreaCIterator3 = new Coord.\u003CDistanceArea\u003Ec__Iterator3()
      {
        maxDist = maxDist,
        \u0024this = this
      };
      // ISSUE: reference to a compiler-generated field
      distanceAreaCIterator3.\u0024PC = -2;
      return (IEnumerable<Coord>) distanceAreaCIterator3;
    }

    [DebuggerHidden]
    public IEnumerable<Coord> DistanceArea(CoordRect rect)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      Coord.\u003CDistanceArea\u003Ec__Iterator4 distanceAreaCIterator4 = new Coord.\u003CDistanceArea\u003Ec__Iterator4()
      {
        rect = rect,
        \u0024this = this
      };
      // ISSUE: reference to a compiler-generated field
      distanceAreaCIterator4.\u0024PC = -2;
      return (IEnumerable<Coord>) distanceAreaCIterator4;
    }

    public Vector3 ToVector3(float cellSize) => new Vector3((float) this.x * cellSize, 0.0f, (float) this.z * cellSize);

    public Vector2 ToVector2(float cellSize) => new Vector2((float) this.x * cellSize, (float) this.z * cellSize);

    public Rect ToRect(float cellSize) => new Rect((float) this.x * cellSize, (float) this.z * cellSize, cellSize, cellSize);
  }
}
