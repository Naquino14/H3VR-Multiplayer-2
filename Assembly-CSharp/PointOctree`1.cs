// Decompiled with JetBrains decompiler
// Type: PointOctree`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PointOctree<T> where T : class
{
  private PointOctreeNode<T> rootNode;
  private float initialSize;
  private float minSize;

  public PointOctree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize)
  {
    if ((double) minNodeSize > (double) initialWorldSize)
    {
      Debug.LogWarning((object) ("Minimum node size must be at least as big as the initial world size. Was: " + (object) minNodeSize + " Adjusted to: " + (object) initialWorldSize));
      minNodeSize = initialWorldSize;
    }
    this.Count = 0;
    this.initialSize = initialWorldSize;
    this.minSize = minNodeSize;
    this.rootNode = new PointOctreeNode<T>(this.initialSize, this.minSize, initialWorldPos);
  }

  public int Count { get; private set; }

  public void Add(T obj, Vector3 objPos)
  {
    int num = 0;
    while (!this.rootNode.Add(obj, objPos))
    {
      this.Grow(objPos - this.rootNode.Center);
      if (++num > 20)
      {
        Debug.LogError((object) ("Aborted Add operation as it seemed to be going on forever (" + (object) (num - 1) + ") attempts at growing the octree."));
        return;
      }
    }
    ++this.Count;
  }

  public bool Remove(T obj)
  {
    bool flag = this.rootNode.Remove(obj);
    if (flag)
    {
      --this.Count;
      this.Shrink();
    }
    return flag;
  }

  public bool Remove(T obj, Vector3 objPos)
  {
    bool flag = this.rootNode.Remove(obj, objPos);
    if (flag)
    {
      --this.Count;
      this.Shrink();
    }
    return flag;
  }

  public bool GetNearbyNonAlloc(Ray ray, float maxDistance, List<T> nearBy)
  {
    nearBy.Clear();
    this.rootNode.GetNearby(ref ray, ref maxDistance, nearBy);
    return nearBy.Count > 0;
  }

  public T[] GetNearby(Ray ray, float maxDistance)
  {
    List<T> result = new List<T>();
    this.rootNode.GetNearby(ref ray, ref maxDistance, result);
    return result.ToArray();
  }

  public T[] GetNearby(Vector3 position, float maxDistance)
  {
    List<T> result = new List<T>();
    this.rootNode.GetNearby(ref position, ref maxDistance, result);
    return result.ToArray();
  }

  public bool GetNearbyNonAlloc(Vector3 position, float maxDistance, List<T> nearBy)
  {
    nearBy.Clear();
    this.rootNode.GetNearby(ref position, ref maxDistance, nearBy);
    return nearBy.Count > 0;
  }

  public bool GetClosest(Vector3 position, float maxDistance, out T closest)
  {
    float maxValue = float.MaxValue;
    closest = (T) null;
    return this.rootNode.GetClosest(ref position, ref maxDistance, ref maxValue, ref closest);
  }

  public List<T> GetAll()
  {
    List<T> result = new List<T>(this.Count);
    this.rootNode.GetAll(result);
    return result;
  }

  public void DrawAllBounds() => this.rootNode.DrawAllBounds();

  public void DrawAllObjects() => this.rootNode.DrawAllObjects();

  private void Grow(Vector3 direction)
  {
    int xDir = (double) direction.x < 0.0 ? -1 : 1;
    int yDir = (double) direction.y < 0.0 ? -1 : 1;
    int zDir = (double) direction.z < 0.0 ? -1 : 1;
    PointOctreeNode<T> rootNode = this.rootNode;
    float num1 = this.rootNode.SideLength / 2f;
    float baseLengthVal = this.rootNode.SideLength * 2f;
    Vector3 centerVal = this.rootNode.Center + new Vector3((float) xDir * num1, (float) yDir * num1, (float) zDir * num1);
    this.rootNode = new PointOctreeNode<T>(baseLengthVal, this.minSize, centerVal);
    int rootPosIndex = PointOctree<T>.GetRootPosIndex(xDir, yDir, zDir);
    PointOctreeNode<T>[] childOctrees = new PointOctreeNode<T>[8];
    for (int index = 0; index < 8; ++index)
    {
      if (index == rootPosIndex)
      {
        childOctrees[index] = rootNode;
      }
      else
      {
        int num2 = index % 2 != 0 ? 1 : -1;
        int num3 = index <= 3 ? 1 : -1;
        int num4 = index < 2 || index > 3 && index < 6 ? -1 : 1;
        childOctrees[index] = new PointOctreeNode<T>(this.rootNode.SideLength, this.minSize, centerVal + new Vector3((float) num2 * num1, (float) num3 * num1, (float) num4 * num1));
      }
    }
    this.rootNode.SetChildren(childOctrees);
  }

  private void Shrink() => this.rootNode = this.rootNode.ShrinkIfPossible(this.initialSize);

  private static int GetRootPosIndex(int xDir, int yDir, int zDir)
  {
    int num = xDir <= 0 ? 0 : 1;
    if (yDir < 0)
      num += 4;
    if (zDir > 0)
      num += 2;
    return num;
  }
}
