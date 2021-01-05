using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ErosionBrushPlugin
{
	public static class Extensions
	{
		public static bool InRange(this Rect rect, Vector2 pos)
		{
			return (rect.center - pos).sqrMagnitude < rect.width / 2f * (rect.width / 2f);
		}

		public static Vector3 V3(this Vector2 v2)
		{
			return new Vector3(v2.x, 0f, v2.y);
		}

		public static Vector2 V2(this Vector3 v3)
		{
			return new Vector2(v3.x, v3.z);
		}

		public static Vector3 ToV3(this float f)
		{
			return new Vector3(f, f, f);
		}

		public static Quaternion EulerToQuat(this Vector3 v)
		{
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = v;
			return identity;
		}

		public static Quaternion EulerToQuat(this float f)
		{
			Quaternion identity = Quaternion.identity;
			identity.eulerAngles = new Vector3(0f, f, 0f);
			return identity;
		}

		public static Coord ToCoord(this Vector3 pos, float cellSize, bool ceil = false)
		{
			if (!ceil)
			{
				return new Coord(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.z / cellSize));
			}
			return new Coord(Mathf.CeilToInt(pos.x / cellSize), Mathf.CeilToInt(pos.z / cellSize));
		}

		public static Coord ToCoord(this Vector2 pos)
		{
			int num = (int)(pos.x + 0.5f);
			if (pos.x < 0f)
			{
				num--;
			}
			int num2 = (int)(pos.y + 0.5f);
			if (pos.y < 0f)
			{
				num2--;
			}
			return new Coord(num, num2);
		}

		public static CoordRect ToCoordRect(this Vector3 pos, float range, float cellSize)
		{
			Coord coord = (Vector3.one * range * 2f).ToCoord(cellSize, ceil: true) + 1;
			Coord offset = pos.ToCoord(cellSize) - coord / 2;
			return new CoordRect(offset, coord);
		}

		public static List<Type> GetAllChildTypes(this Type type)
		{
			List<Type> list = new List<Type>();
			Assembly assembly = Assembly.GetAssembly(type);
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsSubclassOf(type))
				{
					list.Add(types[i]);
				}
			}
			return list;
		}

		public static Texture2D ColorTexture(int width, int height, Color color)
		{
			Texture2D texture2D = new Texture2D(width, height);
			Color[] pixels = texture2D.GetPixels(0, 0, width, height);
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = color;
			}
			texture2D.SetPixels(0, 0, width, height, pixels);
			texture2D.Apply();
			return texture2D;
		}

		public static bool Equal(Vector3 v1, Vector3 v2)
		{
			return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y) && Mathf.Approximately(v1.z, v2.z);
		}

		public static bool Equal(Ray r1, Ray r2)
		{
			return Equal(r1.origin, r2.origin) && Equal(r1.direction, r2.direction);
		}

		public static void RemoveChildren(this Transform tfm)
		{
			for (int num = tfm.childCount - 1; num >= 0; num--)
			{
				Transform child = tfm.GetChild(num);
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}

		public static void ToggleDisplayWireframe(this Transform tfm, bool show)
		{
		}

		public static int ToInt(this Coord coord)
		{
			int num = ((coord.x >= 0) ? coord.x : (-coord.x));
			int num2 = ((coord.z >= 0) ? coord.z : (-coord.z));
			return (((coord.z < 0) ? 1000000000 : 0) + num * 30000 + num2) * ((coord.x >= 0) ? 1 : (-1));
		}

		public static Coord ToCoord(this int hash)
		{
			int num = ((hash >= 0) ? hash : (-hash));
			int num2 = num / 1000000000 * 1000000000;
			int num3 = (num - num2) / 30000;
			int num4 = num - num2 - num3 * 30000;
			return new Coord((hash >= 0) ? num3 : (-num3), (num2 != 0) ? (-num4) : num4);
		}

		public static void CheckAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value, bool replace = true)
		{
			if (dict.ContainsKey(key))
			{
				if (replace)
				{
					dict[key] = value;
				}
			}
			else
			{
				dict.Add(key, value);
			}
		}

		public static void CheckRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if (dict.ContainsKey(key))
			{
				dict.Remove(key);
			}
		}

		public static TValue CheckGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if (dict.ContainsKey(key))
			{
				return dict[key];
			}
			return default(TValue);
		}

		public static void CheckAdd<T>(this HashSet<T> set, T obj)
		{
			if (!set.Contains(obj))
			{
				set.Add(obj);
			}
		}

		public static void CheckRemove<T>(this HashSet<T> set, T obj)
		{
			if (set.Contains(obj))
			{
				set.Remove(obj);
			}
		}

		public static void SetState<T>(this HashSet<T> set, T obj, bool state)
		{
			if (state && !set.Contains(obj))
			{
				set.Add(obj);
			}
			if (!state && set.Contains(obj))
			{
				set.Remove(obj);
			}
		}

		public static void Normalize(this float[,,] array, int pinnedLayer)
		{
			int length = array.GetLength(0);
			int length2 = array.GetLength(1);
			int length3 = array.GetLength(2);
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					float num = 0f;
					for (int k = 0; k < length3; k++)
					{
						if (k != pinnedLayer)
						{
							num += array[i, j, k];
						}
					}
					float num2 = array[i, j, pinnedLayer];
					if (num2 > 1f)
					{
						num2 = 1f;
						array[i, j, pinnedLayer] = 1f;
					}
					if (num2 < 0f)
					{
						num2 = 0f;
						array[i, j, pinnedLayer] = 0f;
					}
					float num3 = 1f - num2;
					float num4 = ((!(num > 0f)) ? 0f : (num3 / num));
					for (int l = 0; l < length3; l++)
					{
						if (l != pinnedLayer)
						{
							array[i, j, l] *= num4;
						}
					}
				}
			}
		}

		public static int Find(this Array array, object obj)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array.GetValue(i) == obj)
				{
					return i;
				}
			}
			return -1;
		}

		public static int ArrayFind<T>(T[] array, T obj) where T : class
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == obj)
				{
					return i;
				}
			}
			return -1;
		}

		public static void ArrayRemoveAt<T>(ref T[] array, int num)
		{
			array = ArrayRemoveAt(array, num);
		}

		public static T[] ArrayRemoveAt<T>(T[] array, int num)
		{
			T[] array2 = new T[array.Length - 1];
			for (int i = 0; i < array2.Length; i++)
			{
				if (i < num)
				{
					array2[i] = array[i];
				}
				else
				{
					array2[i] = array[i + 1];
				}
			}
			return array2;
		}

		public static void ArrayRemove<T>(ref T[] array, T obj) where T : class
		{
			array = ArrayRemove(array, obj);
		}

		public static T[] ArrayRemove<T>(T[] array, T obj) where T : class
		{
			int num = ArrayFind(array, obj);
			return ArrayRemoveAt(array, num);
		}

		public static void ArrayAdd<T>(ref T[] array, int after, T element = default(T))
		{
			array = ArrayAdd(array, after, element);
		}

		public static T[] ArrayAdd<T>(T[] array, int after, T element = default(T))
		{
			if (array == null || array.Length == 0)
			{
				return new T[1]
				{
					element
				};
			}
			if (after > array.Length - 1)
			{
				after = array.Length - 1;
			}
			T[] array2 = new T[array.Length + 1];
			for (int i = 0; i < array2.Length; i++)
			{
				if (i <= after)
				{
					array2[i] = array[i];
				}
				else if (i == after + 1)
				{
					array2[i] = element;
				}
				else
				{
					array2[i] = array[i - 1];
				}
			}
			return array2;
		}

		public static T[] ArrayAdd<T>(T[] array, T element = default(T))
		{
			return ArrayAdd(array, array.Length - 1, element);
		}

		public static void ArrayAdd<T>(ref T[] array, T element = default(T))
		{
			array = ArrayAdd(array, array.Length - 1, element);
		}

		public static void ArrayResize<T>(ref T[] array, int newSize, T element = default(T))
		{
			array = ArrayResize(array, newSize, element);
		}

		public static T[] ArrayResize<T>(T[] array, int newSize, T element = default(T))
		{
			if (array.Length == newSize)
			{
				return array;
			}
			if (newSize > array.Length)
			{
				array = ArrayAdd(array, element);
				array = ArrayResize(array, newSize);
				return array;
			}
			array = ArrayRemoveAt(array, array.Length - 1);
			array = ArrayResize(array, newSize);
			return array;
		}

		public static void ArraySwitch<T>(T[] array, int num1, int num2)
		{
			if (num1 >= 0 && num1 < array.Length && num2 >= 0 && num2 < array.Length)
			{
				T val = array[num1];
				array[num1] = array[num2];
				array[num2] = val;
			}
		}

		public static void ArraySwitch<T>(T[] array, T obj1, T obj2) where T : class
		{
			int num = ArrayFind(array, obj1);
			int num2 = ArrayFind(array, obj2);
			ArraySwitch(array, num, num2);
		}

		public static void ArrayQSort(float[] array)
		{
			ArrayQSort(array, 0, array.Length - 1);
		}

		public static void ArrayQSort(float[] array, int l, int r)
		{
			float num = array[l + (r - l) / 2];
			int i = l;
			int num2 = r;
			while (i <= num2)
			{
				for (; array[i] < num; i++)
				{
				}
				while (array[num2] > num)
				{
					num2--;
				}
				if (i <= num2)
				{
					float num3 = array[i];
					array[i] = array[num2];
					array[num2] = num3;
					i++;
					num2--;
				}
			}
			if (i < r)
			{
				ArrayQSort(array, i, r);
			}
			if (l < num2)
			{
				ArrayQSort(array, l, num2);
			}
		}

		public static void ArrayQSort<T>(T[] array, float[] reference)
		{
			ArrayQSort(array, reference, 0, reference.Length - 1);
		}

		public static void ArrayQSort<T>(T[] array, float[] reference, int l, int r)
		{
			float num = reference[l + (r - l) / 2];
			int i = l;
			int num2 = r;
			while (i <= num2)
			{
				for (; reference[i] < num; i++)
				{
				}
				while (reference[num2] > num)
				{
					num2--;
				}
				if (i <= num2)
				{
					float num3 = reference[i];
					reference[i] = reference[num2];
					reference[num2] = num3;
					T val = array[i];
					array[i] = array[num2];
					array[num2] = val;
					i++;
					num2--;
				}
			}
			if (i < r)
			{
				ArrayQSort(array, reference, i, r);
			}
			if (l < num2)
			{
				ArrayQSort(array, reference, l, num2);
			}
		}

		public static void ArrayQSort<T>(List<T> list, float[] reference)
		{
			ArrayQSort(list, reference, 0, reference.Length - 1);
		}

		public static void ArrayQSort<T>(List<T> list, float[] reference, int l, int r)
		{
			float num = reference[l + (r - l) / 2];
			int i = l;
			int num2 = r;
			while (i <= num2)
			{
				for (; reference[i] < num; i++)
				{
				}
				while (reference[num2] > num)
				{
					num2--;
				}
				if (i <= num2)
				{
					float num3 = reference[i];
					reference[i] = reference[num2];
					reference[num2] = num3;
					T value = list[i];
					list[i] = list[num2];
					list[num2] = value;
					i++;
					num2--;
				}
			}
			if (i < r)
			{
				ArrayQSort(list, reference, i, r);
			}
			if (l < num2)
			{
				ArrayQSort(list, reference, l, num2);
			}
		}

		public static int[] ArrayOrder(int[] array, int[] order = null, int max = 0, int steps = 1000000, int[] stepsArray = null)
		{
			if (max == 0)
			{
				max = array.Length;
			}
			if (stepsArray == null)
			{
				stepsArray = new int[steps + 1];
			}
			else
			{
				steps = stepsArray.Length - 1;
			}
			int[] array2 = new int[steps + 1];
			for (int i = 0; i < max; i++)
			{
				array2[array[i]]++;
			}
			int num = 0;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] += num;
				num = array2[j];
			}
			for (int num2 = array2.Length - 1; num2 > 0; num2--)
			{
				array2[num2] = array2[num2 - 1];
			}
			array2[0] = 0;
			if (order == null)
			{
				order = new int[max];
			}
			for (int k = 0; k < max; k++)
			{
				int num3 = array[k];
				int num4 = array2[num3];
				order[num4] = k;
				array2[num3]++;
			}
			return order;
		}

		public static T[] ArrayConvert<T, Y>(Y[] src)
		{
			T[] array = new T[src.Length];
			for (int i = 0; i < src.Length; i++)
			{
				array[i] = (T)(object)src[i];
			}
			return array;
		}
	}
}
