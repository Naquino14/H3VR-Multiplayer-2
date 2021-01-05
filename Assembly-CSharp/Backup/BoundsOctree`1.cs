// Decompiled with JetBrains decompiler
// Type: BoundsOctree`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BoundsOctree<T>
{
  private BoundsOctreeNode<T> rootNode;
  private readonly float looseness;
  private readonly float initialSize;
  private readonly float minSize;

  public BoundsOctree(
    float initialWorldSize,
    Vector3 initialWorldPos,
    float minNodeSize,
    float loosenessVal)
  {
    if ((double) minNodeSize > (double) initialWorldSize)
    {
      Debug.LogWarning((object) ("Minimum node size must be at least as big as the initial world size. Was: " + (object) minNodeSize + " Adjusted to: " + (object) initialWorldSize));
      minNodeSize = initialWorldSize;
    }
    this.Count = 0;
    this.initialSize = initialWorldSize;
    this.minSize = minNodeSize;
    this.looseness = Mathf.Clamp(loosenessVal, 1f, 2f);
    this.rootNode = new BoundsOctreeNode<T>(this.initialSize, this.minSize, loosenessVal, initialWorldPos);
  }

  public int Count { get; private set; }

  public void Add(T obj, Bounds objBounds)
  {
    int num = 0;
    while (!this.rootNode.Add(obj, objBounds))
    {
      this.Grow(objBounds.center - this.rootNode.Center);
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

  public bool Remove(T obj, Bounds objBounds)
  {
    bool flag = this.rootNode.Remove(obj, objBounds);
    if (flag)
    {
      --this.Count;
      this.Shrink();
    }
    return flag;
  }

  public bool IsColliding(Bounds checkBounds) => this.rootNode.IsColliding(ref checkBounds);

  public bool IsColliding(Ray checkRay, float maxDistance) => this.rootNode.IsColliding(ref checkRay, maxDistance);

  public void GetColliding(List<T> collidingWith, Bounds checkBounds) => this.rootNode.GetColliding(ref checkBounds, collidingWith);

  public void GetColliding(List<T> collidingWith, Ray checkRay, float maxDistance = float.PositiveInfinity) => this.rootNode.GetColliding(ref checkRay, collidingWith, maxDistance);

  public Bounds GetMaxBounds() => this.rootNode.GetBounds();

  public void DrawAllBounds() => this.rootNode.DrawAllBounds();

  public void DrawAllObjects() => this.rootNode.DrawAllObjects();

  private void Grow(Vector3 direction)
  {
    int xDir = (double) direction.x < 0.0 ? -1 : 1;
    int yDir = (double) direction.y < 0.0 ? -1 : 1;
    int zDir = (double) direction.z < 0.0 ? -1 : 1;
    BoundsOctreeNode<T> rootNode = this.rootNode;
    float num1 = this.rootNode.BaseLength / 2f;
    float baseLengthVal = this.rootNode.BaseLength * 2f;
    Vector3 centerVal = this.rootNode.Center + new Vector3((float) xDir * num1, (float) yDir * num1, (float) zDir * num1);
    this.rootNode = new BoundsOctreeNode<T>(baseLengthVal, this.minSize, this.looseness, centerVal);
    if (!rootNode.HasAnyObjects())
      return;
    int rootPosIndex = BoundsOctree<T>.GetRootPosIndex(xDir, yDir, zDir);
    BoundsOctreeNode<T>[] childOctrees = new BoundsOctreeNode<T>[8];
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
        childOctrees[index] = new BoundsOctreeNode<T>(this.rootNode.BaseLength, this.minSize, this.looseness, centerVal + new Vector3((float) num2 * num1, (float) num3 * num1, (float) num4 * num1));
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
