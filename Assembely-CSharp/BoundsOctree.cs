using System.Collections.Generic;
using UnityEngine;

public class BoundsOctree<T>
{
	private BoundsOctreeNode<T> rootNode;

	private readonly float looseness;

	private readonly float initialSize;

	private readonly float minSize;

	public int Count
	{
		get;
		private set;
	}

	public BoundsOctree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize, float loosenessVal)
	{
		if (minNodeSize > initialWorldSize)
		{
			Debug.LogWarning("Minimum node size must be at least as big as the initial world size. Was: " + minNodeSize + " Adjusted to: " + initialWorldSize);
			minNodeSize = initialWorldSize;
		}
		Count = 0;
		initialSize = initialWorldSize;
		minSize = minNodeSize;
		looseness = Mathf.Clamp(loosenessVal, 1f, 2f);
		rootNode = new BoundsOctreeNode<T>(initialSize, minSize, loosenessVal, initialWorldPos);
	}

	public void Add(T obj, Bounds objBounds)
	{
		int num = 0;
		while (!rootNode.Add(obj, objBounds))
		{
			Grow(objBounds.center - rootNode.Center);
			if (++num > 20)
			{
				Debug.LogError("Aborted Add operation as it seemed to be going on forever (" + (num - 1) + ") attempts at growing the octree.");
				return;
			}
		}
		Count++;
	}

	public bool Remove(T obj)
	{
		bool flag = rootNode.Remove(obj);
		if (flag)
		{
			Count--;
			Shrink();
		}
		return flag;
	}

	public bool Remove(T obj, Bounds objBounds)
	{
		bool flag = rootNode.Remove(obj, objBounds);
		if (flag)
		{
			Count--;
			Shrink();
		}
		return flag;
	}

	public bool IsColliding(Bounds checkBounds)
	{
		return rootNode.IsColliding(ref checkBounds);
	}

	public bool IsColliding(Ray checkRay, float maxDistance)
	{
		return rootNode.IsColliding(ref checkRay, maxDistance);
	}

	public void GetColliding(List<T> collidingWith, Bounds checkBounds)
	{
		rootNode.GetColliding(ref checkBounds, collidingWith);
	}

	public void GetColliding(List<T> collidingWith, Ray checkRay, float maxDistance = float.PositiveInfinity)
	{
		rootNode.GetColliding(ref checkRay, collidingWith, maxDistance);
	}

	public Bounds GetMaxBounds()
	{
		return rootNode.GetBounds();
	}

	public void DrawAllBounds()
	{
		rootNode.DrawAllBounds();
	}

	public void DrawAllObjects()
	{
		rootNode.DrawAllObjects();
	}

	private void Grow(Vector3 direction)
	{
		int num = ((direction.x >= 0f) ? 1 : (-1));
		int num2 = ((direction.y >= 0f) ? 1 : (-1));
		int num3 = ((direction.z >= 0f) ? 1 : (-1));
		BoundsOctreeNode<T> boundsOctreeNode = rootNode;
		float num4 = rootNode.BaseLength / 2f;
		float baseLengthVal = rootNode.BaseLength * 2f;
		Vector3 vector = rootNode.Center + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4);
		rootNode = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, vector);
		if (!boundsOctreeNode.HasAnyObjects())
		{
			return;
		}
		int rootPosIndex = GetRootPosIndex(num, num2, num3);
		BoundsOctreeNode<T>[] array = new BoundsOctreeNode<T>[8];
		for (int i = 0; i < 8; i++)
		{
			if (i == rootPosIndex)
			{
				array[i] = boundsOctreeNode;
				continue;
			}
			num = ((i % 2 != 0) ? 1 : (-1));
			num2 = ((i <= 3) ? 1 : (-1));
			num3 = ((i >= 2 && (i <= 3 || i >= 6)) ? 1 : (-1));
			array[i] = new BoundsOctreeNode<T>(rootNode.BaseLength, minSize, looseness, vector + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4));
		}
		rootNode.SetChildren(array);
	}

	private void Shrink()
	{
		rootNode = rootNode.ShrinkIfPossible(initialSize);
	}

	private static int GetRootPosIndex(int xDir, int yDir, int zDir)
	{
		int num = ((xDir > 0) ? 1 : 0);
		if (yDir < 0)
		{
			num += 4;
		}
		if (zDir > 0)
		{
			num += 2;
		}
		return num;
	}
}
