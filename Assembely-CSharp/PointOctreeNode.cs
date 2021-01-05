using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PointOctreeNode<T> where T : class
{
	private class OctreeObject
	{
		public T Obj;

		public Vector3 Pos;
	}

	private float minSize;

	private Bounds bounds = default(Bounds);

	private readonly List<OctreeObject> objects = new List<OctreeObject>();

	private PointOctreeNode<T>[] children;

	private Bounds[] childBounds;

	private const int NUM_OBJECTS_ALLOWED = 8;

	public Vector3 Center
	{
		get;
		private set;
	}

	public float SideLength
	{
		get;
		private set;
	}

	public PointOctreeNode(float baseLengthVal, float minSizeVal, Vector3 centerVal)
	{
		SetValues(baseLengthVal, minSizeVal, centerVal);
	}

	public bool Add(T obj, Vector3 objPos)
	{
		if (!Encapsulates(bounds, objPos))
		{
			return false;
		}
		SubAdd(obj, objPos);
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

	public bool Remove(T obj, Vector3 objPos)
	{
		if (!Encapsulates(bounds, objPos))
		{
			return false;
		}
		return SubRemove(obj, objPos);
	}

	public void GetNearby(ref Ray ray, ref float maxDistance, List<T> result)
	{
		Bounds bounds = this.bounds;
		bounds.Expand(new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
		if (!bounds.IntersectRay(ray))
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (SqrDistanceToRay(ray, objects[i].Pos) <= maxDistance * maxDistance)
			{
				result.Add(objects[i].Obj);
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				children[j].GetNearby(ref ray, ref maxDistance, result);
			}
		}
	}

	public void GetNearby(ref Vector3 position, ref float maxDistance, List<T> result)
	{
		Bounds bounds = this.bounds;
		bounds.Expand(new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
		if (!bounds.Contains(position))
		{
			return;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			if (Vector3.Distance(position, objects[i].Pos) <= maxDistance)
			{
				result.Add(objects[i].Obj);
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				children[j].GetNearby(ref position, ref maxDistance, result);
			}
		}
	}

	public bool GetClosest(ref Vector3 position, ref float maxDistance, ref float currentMinDistance, ref T currentClosest)
	{
		Bounds bounds = this.bounds;
		bounds.Expand(new Vector3(maxDistance * 2f, maxDistance * 2f, maxDistance * 2f));
		if (!bounds.Contains(position))
		{
			return false;
		}
		for (int i = 0; i < objects.Count; i++)
		{
			float num = Vector3.Distance(position, objects[i].Pos);
			if (num <= maxDistance && num <= currentMinDistance)
			{
				currentClosest = objects[i].Obj;
				currentMinDistance = num;
			}
		}
		if (children != null)
		{
			for (int j = 0; j < 8; j++)
			{
				children[j].GetClosest(ref position, ref maxDistance, ref currentMinDistance, ref currentClosest);
			}
		}
		return currentClosest != null;
	}

	public void GetAll(List<T> result)
	{
		result.AddRange(objects.Select((OctreeObject o) => o.Obj));
		if (children != null)
		{
			for (int i = 0; i < 8; i++)
			{
				children[i].GetAll(result);
			}
		}
	}

	public void SetChildren(PointOctreeNode<T>[] childOctrees)
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

	public void DrawAllBounds(float depth = 0f)
	{
		float num = depth / 7f;
		Gizmos.color = new Color(num, 0f, 1f - num);
		Bounds bounds = new Bounds(Center, new Vector3(SideLength, SideLength, SideLength));
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
		float num = SideLength / 20f;
		Gizmos.color = new Color(0f, 1f - num, num, 0.25f);
		foreach (OctreeObject @object in objects)
		{
			Gizmos.DrawIcon(@object.Pos, "marker.tif", allowScaling: true);
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

	public PointOctreeNode<T> ShrinkIfPossible(float minLength)
	{
		if (SideLength < 2f * minLength)
		{
			return this;
		}
		if (objects.Count == 0 && children.Length == 0)
		{
			return this;
		}
		int num = -1;
		for (int i = 0; i < objects.Count; i++)
		{
			OctreeObject octreeObject = objects[i];
			int num2 = BestFitChild(octreeObject.Pos);
			if (i == 0 || num2 == num)
			{
				if (num < 0)
				{
					num = num2;
				}
				continue;
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
			SetValues(SideLength / 2f, minSize, childBounds[num].center);
			return this;
		}
		return children[num];
	}

	private void SetValues(float baseLengthVal, float minSizeVal, Vector3 centerVal)
	{
		SideLength = baseLengthVal;
		minSize = minSizeVal;
		Center = centerVal;
		bounds = new Bounds(Center, new Vector3(SideLength, SideLength, SideLength));
		float num = SideLength / 4f;
		float num2 = SideLength / 2f;
		Vector3 size = new Vector3(num2, num2, num2);
		childBounds = new Bounds[8];
		ref Bounds reference = ref childBounds[0];
		reference = new Bounds(Center + new Vector3(0f - num, num, 0f - num), size);
		ref Bounds reference2 = ref childBounds[1];
		reference2 = new Bounds(Center + new Vector3(num, num, 0f - num), size);
		ref Bounds reference3 = ref childBounds[2];
		reference3 = new Bounds(Center + new Vector3(0f - num, num, num), size);
		ref Bounds reference4 = ref childBounds[3];
		reference4 = new Bounds(Center + new Vector3(num, num, num), size);
		ref Bounds reference5 = ref childBounds[4];
		reference5 = new Bounds(Center + new Vector3(0f - num, 0f - num, 0f - num), size);
		ref Bounds reference6 = ref childBounds[5];
		reference6 = new Bounds(Center + new Vector3(num, 0f - num, 0f - num), size);
		ref Bounds reference7 = ref childBounds[6];
		reference7 = new Bounds(Center + new Vector3(0f - num, 0f - num, num), size);
		ref Bounds reference8 = ref childBounds[7];
		reference8 = new Bounds(Center + new Vector3(num, 0f - num, num), size);
	}

	private void SubAdd(T obj, Vector3 objPos)
	{
		if (objects.Count < 8 || SideLength / 2f < minSize)
		{
			OctreeObject octreeObject = new OctreeObject();
			octreeObject.Obj = obj;
			octreeObject.Pos = objPos;
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
				num2 = BestFitChild(octreeObject2.Pos);
				children[num2].SubAdd(octreeObject2.Obj, octreeObject2.Pos);
				objects.Remove(octreeObject2);
			}
		}
		num2 = BestFitChild(objPos);
		children[num2].SubAdd(obj, objPos);
	}

	private bool SubRemove(T obj, Vector3 objPos)
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
			int num = BestFitChild(objPos);
			flag = children[num].SubRemove(obj, objPos);
		}
		if (flag && children != null && ShouldMerge())
		{
			Merge();
		}
		return flag;
	}

	private void Split()
	{
		float num = SideLength / 4f;
		float baseLengthVal = SideLength / 2f;
		children = new PointOctreeNode<T>[8];
		children[0] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(0f - num, num, 0f - num));
		children[1] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(num, num, 0f - num));
		children[2] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(0f - num, num, num));
		children[3] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(num, num, num));
		children[4] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(0f - num, 0f - num, 0f - num));
		children[5] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(num, 0f - num, 0f - num));
		children[6] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(0f - num, 0f - num, num));
		children[7] = new PointOctreeNode<T>(baseLengthVal, minSize, Center + new Vector3(num, 0f - num, num));
	}

	private void Merge()
	{
		for (int i = 0; i < 8; i++)
		{
			PointOctreeNode<T> pointOctreeNode = children[i];
			int count = pointOctreeNode.objects.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				OctreeObject item = pointOctreeNode.objects[num];
				objects.Add(item);
			}
		}
		children = null;
	}

	private static bool Encapsulates(Bounds outerBounds, Vector3 point)
	{
		return outerBounds.Contains(point);
	}

	private int BestFitChild(Vector3 objPos)
	{
		return ((!(objPos.x <= Center.x)) ? 1 : 0) + ((!(objPos.y >= Center.y)) ? 4 : 0) + ((!(objPos.z <= Center.z)) ? 2 : 0);
	}

	private bool ShouldMerge()
	{
		int num = objects.Count;
		if (children != null)
		{
			PointOctreeNode<T>[] array = children;
			foreach (PointOctreeNode<T> pointOctreeNode in array)
			{
				if (pointOctreeNode.children != null)
				{
					return false;
				}
				num += pointOctreeNode.objects.Count;
			}
		}
		return num <= 8;
	}

	private bool HasAnyObjects()
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

	public static float SqrDistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).sqrMagnitude;
	}
}
