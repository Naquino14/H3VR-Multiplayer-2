using System.Collections.Generic;
using UnityEngine;

public class BoundsOctreeNode<T>
{
	private class OctreeObject
	{
		public T Obj;

		public Bounds Bounds;
	}

	private float looseness;

	private float minSize;

	private float adjLength;

	private Bounds bounds = default(Bounds);

	private readonly List<OctreeObject> objects = new List<OctreeObject>();

	private BoundsOctreeNode<T>[] children;

	private Bounds[] childBounds;

	private const int numObjectsAllowed = 8;

	public Vector3 Center
	{
		get;
		private set;
	}

	public float BaseLength
	{
		get;
		private set;
	}

	public BoundsOctreeNode(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
	{
		SetValues(baseLengthVal, minSizeVal, loosenessVal, centerVal);
	}

	public bool Add(T obj, Bounds objBounds)
	{
		if (!Encapsulates(bounds, objBounds))
		{
			return false;
		}
		SubAdd(obj, objBounds);
		return true;
	}

	public bool Remove(T obj)
	{
		bool flag = false;
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].Obj.Equals(obj))
			{
				flag = objects.Remove(objects[i]);
				break;
			}
		}
		if (!flag && children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				flag = children[j].Remove(obj);
				if (flag)
				{
					break;
				}
			}
		}
		if (flag && children != null && ShouldMerge())
		{
			Merge();
		}
		return flag;
	}

	public bool Remove(T obj, Bounds objBounds)
	{
		if (!Encapsulates(bounds, objBounds))
		{
			return false;
		}
		return SubRemove(obj, objBounds);
	}

	public bool IsColliding(ref Bounds checkBounds)
	{
		if (!bounds.Intersects(checkBounds))
		{
			return false;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].Bounds.Intersects(checkBounds))
			{
				return true;
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				if (children[j].IsColliding(ref checkBounds))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsColliding(ref Ray checkRay, float maxDistance = float.PositiveInfinity)
	{
		if (!bounds.IntersectRay(checkRay, out var distance) || distance > maxDistance)
		{
			return false;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].Bounds.IntersectRay(checkRay, out distance) && distance <= maxDistance)
			{
				return true;
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				if (children[j].IsColliding(ref checkRay, maxDistance))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void GetColliding(ref Bounds checkBounds, List<T> result)
	{
		if (!bounds.Intersects(checkBounds))
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].Bounds.Intersects(checkBounds))
			{
				result.Add(objects[i].Obj);
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				children[j].GetColliding(ref checkBounds, result);
			}
		}
	}

	public void GetColliding(ref Ray checkRay, List<T> result, float maxDistance = float.PositiveInfinity)
	{
		if (!bounds.IntersectRay(checkRay, out var distance) || distance > maxDistance)
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].Bounds.IntersectRay(checkRay, out distance) && distance <= maxDistance)
			{
				result.Add(objects[i].Obj);
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				children[j].GetColliding(ref checkRay, result, maxDistance);
			}
		}
	}

	public void SetChildren(BoundsOctreeNode<T>[] childOctrees)
	{
		if (childOctrees.Length != 8)
		{
			Debug.LogError("Child octree array must be length 8. Was length: " + childOctrees.Length);
		}
		else
		{
			children = childOctrees;
		}
	}

	public Bounds GetBounds()
	{
		return bounds;
	}

	public void DrawAllBounds(float depth = 0f)
	{
		float num = depth / 7f;
		Gizmos.color = new Color(num, 0f, 1f - num);
		Bounds bounds = new Bounds(Center, new Vector3(adjLength, adjLength, adjLength));
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		if (children != null)
		{
			depth += 1f;
			for (int i = 0; i < 8; i++)
			{
				children[i].DrawAllBounds(depth);
			}
		}
		Gizmos.color = Color.white;
	}

	public void DrawAllObjects()
	{
		float num = BaseLength / 20f;
		Gizmos.color = new Color(0f, 1f - num, num, 0.25f);
		foreach (OctreeObject @object in objects)
		{
			Gizmos.DrawCube(@object.Bounds.center, @object.Bounds.size);
		}
		if (children != null)
		{
			for (int i = 0; i < 8; i++)
			{
				children[i].DrawAllObjects();
			}
		}
		Gizmos.color = Color.white;
	}

	public BoundsOctreeNode<T> ShrinkIfPossible(float minLength)
	{
		if (BaseLength < 2f * minLength)
		{
			return this;
		}
		if (objects.Count == 0 && (children == null || children.Length == 0))
		{
			return this;
		}
		int num = -1;
		for (int i = 0; i < objects.Count; i++)
		{
			OctreeObject octreeObject = objects[i];
			int num2 = BestFitChild(octreeObject.Bounds);
			if (i == 0 || num2 == num)
			{
				if (Encapsulates(childBounds[num2], octreeObject.Bounds))
				{
					if (num < 0)
					{
						num = num2;
					}
					continue;
				}
				return this;
			}
			return this;
		}
		if (children != null)
		{
			bool flag = false;
			for (int j = 0; j < children.Length; j++)
			{
				if (children[j].HasAnyObjects())
				{
					if (flag)
					{
						return this;
					}
					if (num >= 0 && num != j)
					{
						return this;
					}
					flag = true;
					num = j;
				}
			}
		}
		if (children == null)
		{
			SetValues(BaseLength / 2f, minSize, looseness, childBounds[num].center);
			return this;
		}
		if (num == -1)
		{
			return this;
		}
		return children[num];
	}

	private void SetValues(float baseLengthVal, float minSizeVal, float loosenessVal, Vector3 centerVal)
	{
		BaseLength = baseLengthVal;
		minSize = minSizeVal;
		looseness = loosenessVal;
		Center = centerVal;
		adjLength = looseness * baseLengthVal;
		bounds = new Bounds(size: new Vector3(adjLength, adjLength, adjLength), center: Center);
		float num = BaseLength / 4f;
		float num2 = BaseLength / 2f * looseness;
		Vector3 size2 = new Vector3(num2, num2, num2);
		childBounds = new Bounds[8];
		ref Bounds reference = ref childBounds[0];
		reference = new Bounds(Center + new Vector3(0f - num, num, 0f - num), size2);
		ref Bounds reference2 = ref childBounds[1];
		reference2 = new Bounds(Center + new Vector3(num, num, 0f - num), size2);
		ref Bounds reference3 = ref childBounds[2];
		reference3 = new Bounds(Center + new Vector3(0f - num, num, num), size2);
		ref Bounds reference4 = ref childBounds[3];
		reference4 = new Bounds(Center + new Vector3(num, num, num), size2);
		ref Bounds reference5 = ref childBounds[4];
		reference5 = new Bounds(Center + new Vector3(0f - num, 0f - num, 0f - num), size2);
		ref Bounds reference6 = ref childBounds[5];
		reference6 = new Bounds(Center + new Vector3(num, 0f - num, 0f - num), size2);
		ref Bounds reference7 = ref childBounds[6];
		reference7 = new Bounds(Center + new Vector3(0f - num, 0f - num, num), size2);
		ref Bounds reference8 = ref childBounds[7];
		reference8 = new Bounds(Center + new Vector3(num, 0f - num, num), size2);
	}

	private void SubAdd(T obj, Bounds objBounds)
	{
		OctreeObject octreeObject;
		if (objects.Count < 8 || BaseLength / 2f < minSize)
		{
			octreeObject = new OctreeObject();
			octreeObject.Obj = obj;
			octreeObject.Bounds = objBounds;
			OctreeObject item = octreeObject;
			objects.Add(item);
			return;
		}
		int num2;
		if (children == null)
		{
			Split();
			if (children == null)
			{
				Debug.Log("Child creation failed for an unknown reason. Early exit.");
				return;
			}
			for (int num = objects.Count - 1; num >= 0; num--)
			{
				OctreeObject octreeObject2 = objects[num];
				num2 = BestFitChild(octreeObject2.Bounds);
				if (Encapsulates(children[num2].bounds, octreeObject2.Bounds))
				{
					children[num2].SubAdd(octreeObject2.Obj, octreeObject2.Bounds);
					objects.Remove(octreeObject2);
				}
			}
		}
		num2 = BestFitChild(objBounds);
		if (Encapsulates(children[num2].bounds, objBounds))
		{
			children[num2].SubAdd(obj, objBounds);
			return;
		}
		octreeObject = new OctreeObject();
		octreeObject.Obj = obj;
		octreeObject.Bounds = objBounds;
		OctreeObject item2 = octreeObject;
		objects.Add(item2);
	}

	private bool SubRemove(T obj, Bounds objBounds)
	{
		bool flag = false;
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i].Obj.Equals(obj))
			{
				flag = objects.Remove(objects[i]);
				break;
			}
		}
		if (!flag && children != null)
		{
			int num = BestFitChild(objBounds);
			flag = children[num].SubRemove(obj, objBounds);
		}
		if (flag && children != null && ShouldMerge())
		{
			Merge();
		}
		return flag;
	}

	private void Split()
	{
		float num = BaseLength / 4f;
		float baseLengthVal = BaseLength / 2f;
		children = new BoundsOctreeNode<T>[8];
		children[0] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(0f - num, num, 0f - num));
		children[1] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(num, num, 0f - num));
		children[2] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(0f - num, num, num));
		children[3] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(num, num, num));
		children[4] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(0f - num, 0f - num, 0f - num));
		children[5] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(num, 0f - num, 0f - num));
		children[6] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(0f - num, 0f - num, num));
		children[7] = new BoundsOctreeNode<T>(baseLengthVal, minSize, looseness, Center + new Vector3(num, 0f - num, num));
	}

	private void Merge()
	{
		for (int i = 0; i < 8; i++)
		{
			BoundsOctreeNode<T> boundsOctreeNode = children[i];
			int count = boundsOctreeNode.objects.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				OctreeObject item = boundsOctreeNode.objects[num];
				objects.Add(item);
			}
		}
		children = null;
	}

	private static bool Encapsulates(Bounds outerBounds, Bounds innerBounds)
	{
		return outerBounds.Contains(innerBounds.min) && outerBounds.Contains(innerBounds.max);
	}

	private int BestFitChild(Bounds objBounds)
	{
		return ((!(objBounds.center.x <= Center.x)) ? 1 : 0) + ((!(objBounds.center.y >= Center.y)) ? 4 : 0) + ((!(objBounds.center.z <= Center.z)) ? 2 : 0);
	}

	private bool ShouldMerge()
	{
		int num = objects.Count;
		if (children != null)
		{
			BoundsOctreeNode<T>[] array = children;
			foreach (BoundsOctreeNode<T> boundsOctreeNode in array)
			{
				if (boundsOctreeNode.children != null)
				{
					return false;
				}
				num += boundsOctreeNode.objects.Count;
			}
		}
		return num <= 8;
	}

	public bool HasAnyObjects()
	{
		if (objects.Count > 0)
		{
			return true;
		}
		if (children != null)
		{
			for (int i = 0; i < 8; i++)
			{
				if (children[i].HasAnyObjects())
				{
					return true;
				}
			}
		}
		return false;
	}
}
