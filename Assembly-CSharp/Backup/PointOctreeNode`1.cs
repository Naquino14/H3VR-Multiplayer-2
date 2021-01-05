// Decompiled with JetBrains decompiler
// Type: PointOctreeNode`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointOctreeNode<T> where T : class
{
  private float minSize;
  private Bounds bounds = new Bounds();
  private readonly List<PointOctreeNode<T>.OctreeObject> objects = new List<PointOctreeNode<T>.OctreeObject>();
  private PointOctreeNode<T>[] children;
  private Bounds[] childBounds;
  private const int NUM_OBJECTS_ALLOWED = 8;

  public PointOctreeNode(float baseLengthVal, float minSizeVal, Vector3 centerVal) => this.SetValues(baseLengthVal, minSizeVal, centerVal);

  public Vector3 Center { get; private set; }

  public float SideLength { get; private set; }

  public bool Add(T obj, Vector3 objPos)
  {
    if (!PointOctreeNode<T>.Encapsulates(this.bounds, objPos))
      return false;
    this.SubAdd(obj, objPos);
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

  public bool Remove(T obj, Vector3 objPos) => PointOctreeNode<T>.Encapsulates(this.bounds, objPos) && this.SubRemove(obj, objPos);

  public void GetNearby(ref Ray ray, ref float maxDistance, List<T> result)
  {
    Bounds bounds = this.bounds;
    bounds.Expand(new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
    if (!bounds.IntersectRay(ray))
      return;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if ((double) PointOctreeNode<T>.SqrDistanceToRay(ray, this.objects[index].Pos) <= (double) maxDistance * (double) maxDistance)
        result.Add(this.objects[index].Obj);
    }
    if (this.children == null)
      return;
    for (int index = 0; index < 8; ++index)
      this.children[index].GetNearby(ref ray, ref maxDistance, result);
  }

  public void GetNearby(ref Vector3 position, ref float maxDistance, List<T> result)
  {
    Bounds bounds = this.bounds;
    bounds.Expand(new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
    if (!bounds.Contains(position))
      return;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      if ((double) Vector3.Distance(position, this.objects[index].Pos) <= (double) maxDistance)
        result.Add(this.objects[index].Obj);
    }
    if (this.children == null)
      return;
    for (int index = 0; index < 8; ++index)
      this.children[index].GetNearby(ref position, ref maxDistance, result);
  }

  public bool GetClosest(
    ref Vector3 position,
    ref float maxDistance,
    ref float currentMinDistance,
    ref T currentClosest)
  {
    Bounds bounds = this.bounds;
    bounds.Expand(new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
    if (!bounds.Contains(position))
      return false;
    for (int index = 0; index < this.objects.Count; ++index)
    {
      float num = Vector3.Distance(position, this.objects[index].Pos);
      if ((double) num <= (double) maxDistance && (double) num <= (double) currentMinDistance)
      {
        currentClosest = this.objects[index].Obj;
        currentMinDistance = num;
      }
    }
    if (this.children != null)
    {
      for (int index = 0; index < 8; ++index)
        this.children[index].GetClosest(ref position, ref maxDistance, ref currentMinDistance, ref currentClosest);
    }
    return (object) currentClosest != null;
  }

  public void GetAll(List<T> result)
  {
    result.AddRange(this.objects.Select<PointOctreeNode<T>.OctreeObject, T>((Func<PointOctreeNode<T>.OctreeObject, T>) (o => o.Obj)));
    if (this.children == null)
      return;
    for (int index = 0; index < 8; ++index)
      this.children[index].GetAll(result);
  }

  public void SetChildren(PointOctreeNode<T>[] childOctrees)
  {
    if (childOctrees.Length != 8)
      Debug.LogError((object) ("Child octree array must be length 8. Was length: " + (object) childOctrees.Length));
    else
      this.children = childOctrees;
  }

  public void DrawAllBounds(float depth = 0.0f)
  {
    float r = depth / 7f;
    Gizmos.color = new Color(r, 0.0f, 1f - r);
    Bounds bounds = new Bounds(this.Center, new Vector3(this.SideLength, this.SideLength, this.SideLength));
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
    float b = this.SideLength / 20f;
    Gizmos.color = new Color(0.0f, 1f - b, b, 0.25f);
    foreach (PointOctreeNode<T>.OctreeObject octreeObject in this.objects)
      Gizmos.DrawIcon(octreeObject.Pos, "marker.tif", true);
    if (this.children != null)
    {
      for (int index = 0; index < 8; ++index)
        this.children[index].DrawAllObjects();
    }
    Gizmos.color = Color.white;
  }

  public PointOctreeNode<T> ShrinkIfPossible(float minLength)
  {
    if ((double) this.SideLength < 2.0 * (double) minLength || this.objects.Count == 0 && this.children.Length == 0)
      return this;
    int index1 = -1;
    for (int index2 = 0; index2 < this.objects.Count; ++index2)
    {
      int num = this.BestFitChild(this.objects[index2].Pos);
      if (index2 != 0 && num != index1)
        return this;
      if (index1 < 0)
        index1 = num;
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
    if (this.children != null)
      return this.children[index1];
    this.SetValues(this.SideLength / 2f, this.minSize, this.childBounds[index1].center);
    return this;
  }

  private void SetValues(float baseLengthVal, float minSizeVal, Vector3 centerVal)
  {
    this.SideLength = baseLengthVal;
    this.minSize = minSizeVal;
    this.Center = centerVal;
    this.bounds = new Bounds(this.Center, new Vector3(this.SideLength, this.SideLength, this.SideLength));
    float num1 = this.SideLength / 4f;
    float num2 = this.SideLength / 2f;
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

  private void SubAdd(T obj, Vector3 objPos)
  {
    if (this.objects.Count < 8 || (double) this.SideLength / 2.0 < (double) this.minSize)
    {
      this.objects.Add(new PointOctreeNode<T>.OctreeObject()
      {
        Obj = obj,
        Pos = objPos
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
        for (int index = this.objects.Count - 1; index >= 0; --index)
        {
          PointOctreeNode<T>.OctreeObject octreeObject = this.objects[index];
          this.children[this.BestFitChild(octreeObject.Pos)].SubAdd(octreeObject.Obj, octreeObject.Pos);
          this.objects.Remove(octreeObject);
        }
      }
      this.children[this.BestFitChild(objPos)].SubAdd(obj, objPos);
    }
  }

  private bool SubRemove(T obj, Vector3 objPos)
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
      flag = this.children[this.BestFitChild(objPos)].SubRemove(obj, objPos);
    if (flag && this.children != null && this.ShouldMerge())
      this.Merge();
    return flag;
  }

  private void Split()
  {
    float num = this.SideLength / 4f;
    float baseLengthVal = this.SideLength / 2f;
    this.children = new PointOctreeNode<T>[8];
    this.children[0] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, num, -num));
    this.children[1] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, num, -num));
    this.children[2] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, num, num));
    this.children[3] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, num, num));
    this.children[4] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, -num, -num));
    this.children[5] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, -num, -num));
    this.children[6] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(-num, -num, num));
    this.children[7] = new PointOctreeNode<T>(baseLengthVal, this.minSize, this.Center + new Vector3(num, -num, num));
  }

  private void Merge()
  {
    for (int index1 = 0; index1 < 8; ++index1)
    {
      PointOctreeNode<T> child = this.children[index1];
      for (int index2 = child.objects.Count - 1; index2 >= 0; --index2)
        this.objects.Add(child.objects[index2]);
    }
    this.children = (PointOctreeNode<T>[]) null;
  }

  private static bool Encapsulates(Bounds outerBounds, Vector3 point) => outerBounds.Contains(point);

  private int BestFitChild(Vector3 objPos) => ((double) objPos.x > (double) this.Center.x ? 1 : 0) + ((double) objPos.y < (double) this.Center.y ? 4 : 0) + ((double) objPos.z > (double) this.Center.z ? 2 : 0);

  private bool ShouldMerge()
  {
    int count = this.objects.Count;
    if (this.children != null)
    {
      foreach (PointOctreeNode<T> child in this.children)
      {
        if (child.children != null)
          return false;
        count += child.objects.Count;
      }
    }
    return count <= 8;
  }

  private bool HasAnyObjects()
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

  public static float SqrDistanceToRay(Ray ray, Vector3 point) => Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;

  private class OctreeObject
  {
    public T Obj;
    public Vector3 Pos;
  }
}
