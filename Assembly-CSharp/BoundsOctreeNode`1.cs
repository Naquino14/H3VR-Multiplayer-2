// Decompiled with JetBrains decompiler
// Type: BoundsOctreeNode`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BoundsOctreeNode<T>
{
  private float looseness;
  private float minSize;
  private float adjLength;
  private Bounds bounds = new Bounds();
  private readonly List<BoundsOctreeNode<T>.OctreeObject> objects = new List<BoundsOctreeNode<T>.OctreeObject>();
  private BoundsOctreeNode<T>[] children;
  private Bounds[] childBounds;
  private const int numObjectsAllowed = 8;

  public BoundsOctreeNode(
    float baseLengthVal,
    float minSizeVal,
    float loosenessVal,
    Vector3 centerVal)
  {
    this.SetValues(baseLengthVal, minSizeVal, loosenessVal, centerVal);
  }

  public Vector3 Center { get; private set; }

  public float BaseLength { get; private set; }

  public bool Add(T obj, Bounds objBounds)
  {
    if (!BoundsOctreeNode<T>.Encapsulates(this.bounds, objBounds))
      return false;
    this.SubAdd(obj, objBounds);
    return true;
  }

  public bool Remove(T obj)
  {
    bool flag = false;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if (this.objects[index].Obj.Equals((object) obj))
      {
        flag = this.objects.Remove(this.objects[index]);
        break;
      }
    }
    if (!flag && this.children != null)
    {
      for (int index = 0; index < 8; ++index)
      {
        flag = this.children[index].Remove(obj);
        if (flag)
          break;
      }
    }
    if (flag && this.children != null && this.ShouldMerge())
      this.Merge();
    return flag;
  }

  public bool Remove(T obj, Bounds objBounds) => BoundsOctreeNode<T>.Encapsulates(this.bounds, objBounds) && this.SubRemove(obj, objBounds);

  public bool IsColliding(ref Bounds checkBounds)
  {
    if (!this.bounds.Intersects(checkBounds))
      return false;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if (this.objects[index].Bounds.Intersects(checkBounds))
        return true;
    }
    if (this.children != null)
    {
      for (int index = 0; index < 8; ++index)
      {
        if (this.children[index].IsColliding(ref checkBounds))
          return true;
      }
    }
    return false;
  }

  public bool IsColliding(ref Ray checkRay, float maxDistance = float.PositiveInfinity)
  {
    float distance;
    if (!this.bounds.IntersectRay(checkRay, out distance) || (double) distance > (double) maxDistance)
      return false;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if (this.objects[index].Bounds.IntersectRay(checkRay, out distance) && (double) distance <= (double) maxDistance)
        return true;
    }
    if (this.children != null)
    {
      for (int index = 0; index < 8; ++index)
      {
        if (this.children[index].IsColliding(ref checkRay, maxDistance))
          return true;
      }
    }
    return false;
  }

  public void GetColliding(ref Bounds checkBounds, List<T> result)
  {
    if (!this.bounds.Intersects(checkBounds))
      return;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if (this.objects[index].Bounds.Intersects(checkBounds))
        result.Add(this.objects[index].Obj);
    }
    if (this.children == null)
      return;
    for (int index = 0; index < 8; ++index)
      this.children[index].GetColliding(ref checkBounds, result);
  }

  public void GetColliding(ref Ray checkRay, List<T> result, float maxDistance = float.PositiveInfinity)
  {
    float distance;
    if (!this.bounds.IntersectRay(checkRay, out distance) || (double) distance > (double) maxDistance)
      return;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if (this.objects[index].Bounds.IntersectRay(checkRay, out distance) && (double) distance <= (double) maxDistance)
        result.Add(this.objects[index].Obj);
    }
    if (this.children == null)
      return;
    for (int index = 0; index < 8; ++index)
      this.children[index].GetColliding(ref checkRay, result, maxDistance);
  }

  public void SetChildren(BoundsOctreeNode<T>[] childOctrees)
  {
    if (childOctrees.Length != 8)
      Debug.LogError((object) ("Child octree array must be length 8. Was length: " + (object) childOctrees.Length));
    else
      this.children = childOctrees;
  }

  public Bounds GetBounds() => this.bounds;

  public void DrawAllBounds(float depth = 0.0f)
  {
    float r = depth / 7f;
    Gizmos.color = new Color(r, 0.0f, 1f - r);
    Bounds bounds = new Bounds(this.Center, new Vector3(this.adjLength, this.adjLength, this.adjLength));
    Gizmos.DrawWireCube(bounds.center, bounds.size);
    if (this.children != null)
    {
      ++depth;
      for (int index = 0; index < 8; ++index)
        this.children[index].DrawAllBounds(depth);
    }
    Gizmos.color = Color.white;
  }

  public void DrawAllObjects()
  {
    float b = this.BaseLength / 20f;
    Gizmos.color = new Color(0.0f, 1f - b, b, 0.25f);
    foreach (BoundsOctreeNode<T>.OctreeObject octreeObject in this.objects)
      Gizmos.DrawCube(octreeObject.Bounds.center, octreeObject.Bounds.size);
    if (this.children != null)
    {
      for (int index = 0; index < 8; ++index)
        this.children[index].DrawAllObjects();
    }
    Gizmos.color = Color.white;
  }

  public BoundsOctreeNode<T> ShrinkIfPossible(float minLength)
  {
    if ((double) this.BaseLength < 2.0 * (double) minLength || this.objects.Count == 0 && (this.children == null || this.children.Length == 0))
      return this;
    int index1 = -1;
    for (int index2 = 0; index2 < this.objects.Count; ++index2)
    {
      BoundsOctreeNode<T>.OctreeObject octreeObject = this.objects[index2];
      int index3 = this.BestFitChild(octreeObject.Bounds);
      if (index2 != 0 && index3 != index1 || !BoundsOctreeNode<T>.Encapsulates(this.childBounds[index3], octreeObject.Bounds))
        return this;
      if (index1 < 0)
        index1 = index3;
    }
    if (this.children != null)
    {
      bool flag = false;
      for (int index2 = 0; index2 < this.children.Length; ++index2)
      {
        if (this.children[index2].HasAnyObjects())
        {
          if (flag || index1 >= 0 && index1 != index2)
            return this;
          flag = true;
          index1 = index2;
        }
      }
    }
    if (this.children == null)
    {
      this.SetValues(this.BaseLength / 2f, this.minSize, this.looseness, this.childBounds[index1].center);
      return this;
    }
    return index1 == -1 ? this : this.children[index1];
  }

  private void SetValues(
    float baseLengthVal,
    float minSizeVal,
    float loosenessVal,
    Vector3 centerVal)
  {
    this.BaseLength = baseLengthVal;
    this.minSize = minSizeVal;
    this.looseness = loosenessVal;
    this.Center = centerVal;
    this.adjLength = this.looseness * baseLengthVal;
    this.bounds = new Bounds(this.Center, new Vector3(this.adjLength, this.adjLength, this.adjLength));
    float num1 = this.BaseLength / 4f;
    float num2 = this.BaseLength / 2f * this.looseness;
    Vector3 size = new Vector3(num2, num2, num2);
    this.childBounds = new Bounds[8];
    this.childBounds[0] = new Bounds(this.Center + new Vector3(-num1, num1, -num1), size);
    this.childBounds[1] = new Bounds(this.Center + new Vector3(num1, num1, -num1), size);
    this.childBounds[2] = new Bounds(this.Center + new Vector3(-num1, num1, num1), size);
    this.childBounds[3] = new Bounds(this.Center + new Vector3(num1, num1, num1), size);
    this.childBounds[4] = new Bounds(this.Center + new Vector3(-num1, -num1, -num1), size);
    this.childBounds[5] = new Bounds(this.Center + new Vector3(num1, -num1, -num1), size);
    this.childBounds[6] = new Bounds(this.Center + new Vector3(-num1, -num1, num1), size);
    this.childBounds[7] = new Bounds(this.Center + new Vector3(num1, -num1, num1), size);
  }

  private void SubAdd(T obj, Bounds objBounds)
  {
    if (this.objects.Count < 8 || (double) this.BaseLength / 2.0 < (double) this.minSize)
    {
      this.objects.Add(new BoundsOctreeNode<T>.OctreeObject()
      {
        Obj = obj,
        Bounds = objBounds
      });
    }
    else
    {
      if (this.children == null)
      {
        this.Split();
        if (this.children == null)
        {
          Debug.Log((object) "Child creation failed for an unknown reason. Early exit.");
          return;
        }
        for (int index1 = this.objects.Count - 1; index1 >= 0; --index1)
        {
          BoundsOctreeNode<T>.OctreeObject octreeObject = this.objects[index1];
          int index2 = this.BestFitChild(octreeObject.Bounds);
          if (BoundsOctreeNode<T>.Encapsulates(this.children[index2].bounds, octreeObject.Bounds))
          {
            this.children[index2].SubAdd(octreeObject.Obj, octreeObject.Bounds);
            this.objects.Remove(octreeObject);
          }
        }
      }
      int index = this.BestFitChild(objBounds);
      if (BoundsOctreeNode<T>.Encapsulates(this.children[index].bounds, objBounds))
        this.children[index].SubAdd(obj, objBounds);
      else
        this.objects.Add(new BoundsOctreeNode<T>.OctreeObject()
        {
          Obj = obj,
          Bounds = objBounds
        });
    }
  }

  private bool SubRemove(T obj, Bounds objBounds)
  {
    bool flag = false;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if (this.objects[index].Obj.Equals((object) obj))
      {
        flag = this.objects.Remove(this.objects[index]);
        break;
      }
    }
    if (!flag && this.children != null)
      flag = this.children[this.BestFitChild(objBounds)].SubRemove(obj, objBounds);
    if (flag && this.children != null && this.ShouldMerge())
      this.Merge();
    return flag;
  }

  private void Split()
  {
    float num = this.BaseLength / 4f;
    float baseLengthVal = this.BaseLength / 2f;
    this.children = new BoundsOctreeNode<T>[8];
    this.children[0] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, num, -num));
    this.children[1] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, num, -num));
    this.children[2] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, num, num));
    this.children[3] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, num, num));
    this.children[4] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, -num, -num));
    this.children[5] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, -num, -num));
    this.children[6] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(-num, -num, num));
    this.children[7] = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, this.Center + new Vector3(num, -num, num));
  }

  private void Merge()
  {
    for (int index1 = 0; index1 < 8; ++index1)
    {
      BoundsOctreeNode<T> child = this.children[index1];
      for (int index2 = child.objects.Count - 1; index2 >= 0; --index2)
        this.objects.Add(child.objects[index2]);
    }
    this.children = (BoundsOctreeNode<T>[]) null;
  }

  private static bool Encapsulates(Bounds outerBounds, Bounds innerBounds) => outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);

  private int BestFitChild(Bounds objBounds) => ((double) objBounds.center.x > (double) this.Center.x ? 1 : 0) + ((double) objBounds.center.y < (double) this.Center.y ? 4 : 0) + ((double) objBounds.center.z > (double) this.Center.z ? 2 : 0);

  private bool ShouldMerge()
  {
    int count = this.objects.Count;
    if (this.children != null)
    {
      foreach (BoundsOctreeNode<T> child in this.children)
      {
        if (child.children != null)
          return false;
        count += child.objects.Count;
      }
    }
    return count <= 8;
  }

  public bool HasAnyObjects()
  {
    if (this.objects.Count > 0)
      return true;
    if (this.children != null)
    {
      for (int index = 0; index < 8; ++index)
      {
        if (this.children[index].HasAnyObjects())
          return true;
      }
    }
    return false;
  }

  private class OctreeObject
  {
    public T Obj;
    public Bounds Bounds;
  }
}
