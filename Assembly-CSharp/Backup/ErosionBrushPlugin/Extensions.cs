// Decompiled with JetBrains decompiler
// Type: ErosionBrushPlugin.Extensions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ErosionBrushPlugin
{
  public static class Extensions
  {
    public static bool InRange(this Rect rect, Vector2 pos) => (double) (rect.center - pos).sqrMagnitude < (double) rect.width / 2.0 * ((double) rect.width / 2.0);

    public static Vector3 V3(this Vector2 v2) => new Vector3(v2.x, 0.0f, v2.y);

    public static Vector2 V2(this Vector3 v3) => new Vector2(v3.x, v3.z);

    public static Vector3 ToV3(this float f) => new Vector3(f, f, f);

    public static Quaternion EulerToQuat(this Vector3 v)
    {
      Quaternion identity = Quaternion.identity;
      identity.eulerAngles = v;
      return identity;
    }

    public static Quaternion EulerToQuat(this float f)
    {
      Quaternion identity = Quaternion.identity;
      identity.eulerAngles = new Vector3(0.0f, f, 0.0f);
      return identity;
    }

    public static Coord ToCoord(this Vector3 pos, float cellSize, bool ceil = false) => !ceil ? new Coord(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.z / cellSize)) : new Coord(Mathf.CeilToInt(pos.x / cellSize), Mathf.CeilToInt(pos.z / cellSize));

    public static Coord ToCoord(this Vector2 pos)
    {
      int x = (int) ((double) pos.x + 0.5);
      if ((double) pos.x < 0.0)
        --x;
      int z = (int) ((double) pos.y + 0.5);
      if ((double) pos.y < 0.0)
        --z;
      return new Coord(x, z);
    }

    public static CoordRect ToCoordRect(this Vector3 pos, float range, float cellSize)
    {
      Coord size = (Vector3.one * range * 2f).ToCoord(cellSize, true) + 1;
      return new CoordRect(pos.ToCoord(cellSize) - size / 2, size);
    }

    public static List<System.Type> GetAllChildTypes(this System.Type type)
    {
      List<System.Type> typeList = new List<System.Type>();
      System.Type[] types = Assembly.GetAssembly(type).GetTypes();
      for (int index = 0; index < types.Length; ++index)
      {
        if (types[index].IsSubclassOf(type))
          typeList.Add(types[index]);
      }
      return typeList;
    }

    public static Texture2D ColorTexture(int width, int height, Color color)
    {
      Texture2D texture2D = new Texture2D(width, height);
      Color[] pixels = texture2D.GetPixels(0, 0, width, height);
      for (int index = 0; index < pixels.Length; ++index)
        pixels[index] = color;
      texture2D.SetPixels(0, 0, width, height, pixels);
      texture2D.Apply();
      return texture2D;
    }

    public static bool Equal(Vector3 v1, Vector3 v2) => Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y) && Mathf.Approximately(v1.z, v2.z);

    public static bool Equal(Ray r1, Ray r2) => Extensions.Equal(r1.origin, r2.origin) && Extensions.Equal(r1.direction, r2.direction);

    public static void RemoveChildren(this Transform tfm)
    {
      for (int index = tfm.childCount - 1; index >= 0; --index)
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) tfm.GetChild(index).gameObject);
    }

    public static void ToggleDisplayWireframe(this Transform tfm, bool show)
    {
    }

    public static int ToInt(this Coord coord)
    {
      int num1 = coord.x >= 0 ? coord.x : -coord.x;
      int num2 = coord.z >= 0 ? coord.z : -coord.z;
      return ((coord.z >= 0 ? 0 : 1000000000) + num1 * 30000 + num2) * (coord.x >= 0 ? 1 : -1);
    }

    public static Coord ToCoord(this int hash)
    {
      int num1 = hash >= 0 ? hash : -hash;
      int num2 = num1 / 1000000000 * 1000000000;
      int num3 = (num1 - num2) / 30000;
      int num4 = num1 - num2 - num3 * 30000;
      return new Coord(hash >= 0 ? num3 : -num3, num2 != 0 ? -num4 : num4);
    }

    public static void CheckAdd<TKey, TValue>(
      this Dictionary<TKey, TValue> dict,
      TKey key,
      TValue value,
      bool replace = true)
    {
      if (dict.ContainsKey(key))
      {
        if (!replace)
          return;
        dict[key] = value;
      }
      else
        dict.Add(key, value);
    }

    public static void CheckRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
    {
      if (!dict.ContainsKey(key))
        return;
      dict.Remove(key);
    }

    public static TValue CheckGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) => dict.ContainsKey(key) ? dict[key] : default (TValue);

    public static void CheckAdd<T>(this HashSet<T> set, T obj)
    {
      if (set.Contains(obj))
        return;
      set.Add(obj);
    }

    public static void CheckRemove<T>(this HashSet<T> set, T obj)
    {
      if (!set.Contains(obj))
        return;
      set.Remove(obj);
    }

    public static void SetState<T>(this HashSet<T> set, T obj, bool state)
    {
      if (state && !set.Contains(obj))
        set.Add(obj);
      if (state || !set.Contains(obj))
        return;
      set.Remove(obj);
    }

    public static void Normalize(this float[,,] array, int pinnedLayer)
    {
      int length1 = array.GetLength(0);
      int length2 = array.GetLength(1);
      int length3 = array.GetLength(2);
      for (int index1 = 0; index1 < length1; ++index1)
      {
        for (int index2 = 0; index2 < length2; ++index2)
        {
          float num1 = 0.0f;
          for (int index3 = 0; index3 < length3; ++index3)
          {
            if (index3 != pinnedLayer)
              num1 += array[index1, index2, index3];
          }
          float num2 = array[index1, index2, pinnedLayer];
          if ((double) num2 > 1.0)
          {
            num2 = 1f;
            array[index1, index2, pinnedLayer] = 1f;
          }
          if ((double) num2 < 0.0)
          {
            num2 = 0.0f;
            array[index1, index2, pinnedLayer] = 0.0f;
          }
          float num3 = 1f - num2;
          float num4 = (double) num1 <= 0.0 ? 0.0f : num3 / num1;
          for (int index3 = 0; index3 < length3; ++index3)
          {
            if (index3 != pinnedLayer)
              array[index1, index2, index3] *= num4;
          }
        }
      }
    }

    public static int Find(this Array array, object obj)
    {
      for (int index = 0; index < array.Length; ++index)
      {
        if (array.GetValue(index) == obj)
          return index;
      }
      return -1;
    }

    public static int ArrayFind<T>(T[] array, T obj) where T : class
    {
      for (int index = 0; index < array.Length; ++index)
      {
        if ((object) array[index] == (object) obj)
          return index;
      }
      return -1;
    }

    public static void ArrayRemoveAt<T>(ref T[] array, int num) => array = Extensions.ArrayRemoveAt<T>(array, num);

    public static T[] ArrayRemoveAt<T>(T[] array, int num)
    {
      T[] objArray = new T[array.Length - 1];
      for (int index = 0; index < objArray.Length; ++index)
        objArray[index] = index >= num ? array[index + 1] : array[index];
      return objArray;
    }

    public static void ArrayRemove<T>(ref T[] array, T obj) where T : class => array = Extensions.ArrayRemove<T>(array, obj);

    public static T[] ArrayRemove<T>(T[] array, T obj) where T : class
    {
      int num = Extensions.ArrayFind<T>(array, obj);
      return Extensions.ArrayRemoveAt<T>(array, num);
    }

    public static void ArrayAdd<T>(ref T[] array, int after, T element = null) => array = Extensions.ArrayAdd<T>(array, after, element);

    public static T[] ArrayAdd<T>(T[] array, int after, T element = null)
    {
      if (array == null || array.Length == 0)
        return new T[1]{ element };
      if (after > array.Length - 1)
        after = array.Length - 1;
      T[] objArray = new T[array.Length + 1];
      for (int index = 0; index < objArray.Length; ++index)
        objArray[index] = index > after ? (index != after + 1 ? array[index - 1] : element) : array[index];
      return objArray;
    }

    public static T[] ArrayAdd<T>(T[] array, T element = null) => Extensions.ArrayAdd<T>(array, array.Length - 1, element);

    public static void ArrayAdd<T>(ref T[] array, T element = null) => array = Extensions.ArrayAdd<T>(array, array.Length - 1, element);

    public static void ArrayResize<T>(ref T[] array, int newSize, T element = null) => array = Extensions.ArrayResize<T>(array, newSize, element);

    public static T[] ArrayResize<T>(T[] array, int newSize, T element = null)
    {
      if (array.Length == newSize)
        return array;
      if (newSize > array.Length)
      {
        array = Extensions.ArrayAdd<T>(array, element);
        array = Extensions.ArrayResize<T>(array, newSize);
        return array;
      }
      array = Extensions.ArrayRemoveAt<T>(array, array.Length - 1);
      array = Extensions.ArrayResize<T>(array, newSize);
      return array;
    }

    public static void ArraySwitch<T>(T[] array, int num1, int num2)
    {
      if (num1 < 0 || num1 >= array.Length || (num2 < 0 || num2 >= array.Length))
        return;
      T obj = array[num1];
      array[num1] = array[num2];
      array[num2] = obj;
    }

    public static void ArraySwitch<T>(T[] array, T obj1, T obj2) where T : class
    {
      int num1 = Extensions.ArrayFind<T>(array, obj1);
      int num2 = Extensions.ArrayFind<T>(array, obj2);
      Extensions.ArraySwitch<T>(array, num1, num2);
    }

    public static void ArrayQSort(float[] array) => Extensions.ArrayQSort(array, 0, array.Length - 1);

    public static void ArrayQSort(float[] array, int l, int r)
    {
      float num1 = array[l + (r - l) / 2];
      int l1 = l;
      int r1 = r;
      while (l1 <= r1)
      {
        while ((double) array[l1] < (double) num1)
          ++l1;
        while ((double) array[r1] > (double) num1)
          --r1;
        if (l1 <= r1)
        {
          float num2 = array[l1];
          array[l1] = array[r1];
          array[r1] = num2;
          ++l1;
          --r1;
        }
      }
      if (l1 < r)
        Extensions.ArrayQSort(array, l1, r);
      if (l >= r1)
        return;
      Extensions.ArrayQSort(array, l, r1);
    }

    public static void ArrayQSort<T>(T[] array, float[] reference) => Extensions.ArrayQSort<T>(array, reference, 0, reference.Length - 1);

    public static void ArrayQSort<T>(T[] array, float[] reference, int l, int r)
    {
      float num1 = reference[l + (r - l) / 2];
      int l1 = l;
      int r1 = r;
      while (l1 <= r1)
      {
        while ((double) reference[l1] < (double) num1)
          ++l1;
        while ((double) reference[r1] > (double) num1)
          --r1;
        if (l1 <= r1)
        {
          float num2 = reference[l1];
          reference[l1] = reference[r1];
          reference[r1] = num2;
          T obj = array[l1];
          array[l1] = array[r1];
          array[r1] = obj;
          ++l1;
          --r1;
        }
      }
      if (l1 < r)
        Extensions.ArrayQSort<T>(array, reference, l1, r);
      if (l >= r1)
        return;
      Extensions.ArrayQSort<T>(array, reference, l, r1);
    }

    public static void ArrayQSort<T>(List<T> list, float[] reference) => Extensions.ArrayQSort<T>(list, reference, 0, reference.Length - 1);

    public static void ArrayQSort<T>(List<T> list, float[] reference, int l, int r)
    {
      float num1 = reference[l + (r - l) / 2];
      int index1 = l;
      int index2 = r;
      while (index1 <= index2)
      {
        while ((double) reference[index1] < (double) num1)
          ++index1;
        while ((double) reference[index2] > (double) num1)
          --index2;
        if (index1 <= index2)
        {
          float num2 = reference[index1];
          reference[index1] = reference[index2];
          reference[index2] = num2;
          T obj = list[index1];
          list[index1] = list[index2];
          list[index2] = obj;
          ++index1;
          --index2;
        }
      }
      if (index1 < r)
        Extensions.ArrayQSort<T>(list, reference, index1, r);
      if (l >= index2)
        return;
      Extensions.ArrayQSort<T>(list, reference, l, index2);
    }

    public static int[] ArrayOrder(
      int[] array,
      int[] order = null,
      int max = 0,
      int steps = 1000000,
      int[] stepsArray = null)
    {
      if (max == 0)
        max = array.Length;
      if (stepsArray == null)
        stepsArray = new int[steps + 1];
      else
        steps = stepsArray.Length - 1;
      int[] numArray = new int[steps + 1];
      for (int index = 0; index < max; ++index)
        ++numArray[array[index]];
      int num = 0;
      for (int index = 0; index < numArray.Length; ++index)
      {
        numArray[index] += num;
        num = numArray[index];
      }
      for (int index = numArray.Length - 1; index > 0; --index)
        numArray[index] = numArray[index - 1];
      numArray[0] = 0;
      if (order == null)
        order = new int[max];
      for (int index1 = 0; index1 < max; ++index1)
      {
        int index2 = array[index1];
        int index3 = numArray[index2];
        order[index3] = index1;
        ++numArray[index2];
      }
      return order;
    }

    public static T[] ArrayConvert<T, Y>(Y[] src)
    {
      T[] objArray = new T[src.Length];
      for (int index = 0; index < src.Length; ++index)
        objArray[index] = (T) (object) src[index];
      return objArray;
    }
  }
}
