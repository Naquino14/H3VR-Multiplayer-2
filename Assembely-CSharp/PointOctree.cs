using System.Collections.Generic;
using UnityEngine;

public class PointOctree<T> where T : class
{
	private PointOctreeNode<T> rootNode;

	private float initialSize;

	private float minSize;

	public int Count
	{
		get;
		private set;
	}

	public PointOctree(float initialWorldSize, Vector3 initialWorldPos, float minNodeSize)
	{
		if (minNodeSize > initialWorldSize)
		{
			Debug.LogWarning("Minimum node size must be at least as big as the initial world size. Was: " + minNodeSize + " Adjusted to: " + initialWorldSize);
			minNodeSize = initialWorldSize;
		}
		Count = 0;
		initialSize = initialWorldSize;
		minSize = minNodeSize;
		rootNode = new PointOctreeNode<T>(initialSize, minSize, initialWorldPos);
	}

	public void Add(T obj, Vector3 objPos)
	{
		int num = 0;
		while (!rootNode.Add(obj, objPos))
		{
			Grow(objPos - rootNode.Center);
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

	public bool Remove(T obj, Vector3 objPos)
	{
		bool flag = rootNode.Remove(obj, objPos);
		if (flag)
		{
			Count--;
			Shrink();
		}
		return flag;
	}

	public bool GetNearbyNonAlloc(Ray ray, float maxDistance, List<T> nearBy)
	{
		nearBy.Clear();
		rootNode.GetNearby(ref ray, ref maxDistance, nearBy);
		if (nearBy.Count > 0)
		{
			return true;
		}
		return false;
	}

	public T[] GetNearby(Ray ray, float maxDistance)
	{
		List<T> list = new List<T>();
		rootNode.GetNearby(ref ray, ref maxDistance, list);
		return list.ToArray();
	}

	public T[] GetNearby(Vector3 position, float maxDistance)
	{
		List<T> list = new List<T>();
		rootNode.GetNearby(ref position, ref maxDistance, list);
		return list.ToArray();
	}

	public bool GetNearbyNonAlloc(Vector3 position, float maxDistance, List<T> nearBy)
	{
		nearBy.Clear();
		rootNode.GetNearby(ref position, ref maxDistance, nearBy);
		if (nearBy.Count > 0)
		{
			return true;
		}
		return false;
	}

	public bool GetClosest(Vector3 position, float maxDistance, out T closest)
	{
		float currentMinDistance = float.MaxValue;
		closest = (T)null;
		return rootNode.GetClosest(ref position, ref maxDistance, ref currentMinDistance, ref closest);
	}

	public List<T> GetAll()
	{
		List<T> result = new List<T>(Count);
		rootNode.GetAll(result);
		return result;
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
		PointOctreeNode<T> pointOctreeNode = rootNode;
		float num4 = rootNode.SideLength / 2f;
		float baseLengthVal = rootNode.SideLength * 2f;
		Vector3 vector = rootNode.Center + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4);
		rootNode = new PointOctreeNode<T>(baseLengthVal, minSize, vector);
		int rootPosIndex = GetRootPosIndex(num, num2, num3);
		PointOctreeNode<T>[] array = new PointOctreeNode<T>[8];
		for (int i = 0; i < 8; i++)
		{
			if (i == rootPosIndex)
			{
				array[i] = pointOctreeNode;
				continue;
			}
			num = ((i % 2 != 0) ? 1 : (-1));
			num2 = ((i <= 3) ? 1 : (-1));
			num3 = ((i >= 2 && (i <= 3 || i >= 6)) ? 1 : (-1));
			array[i] = new PointOctreeNode<T>(rootNode.SideLength, minSize, vector + new Vector3((float)num * num4, (float)num2 * num4, (float)num3 * num4));
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
