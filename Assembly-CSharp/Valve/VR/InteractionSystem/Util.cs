// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Util
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Valve.VR.InteractionSystem
{
  public static class Util
  {
    public const float FeetToMeters = 0.3048f;
    public const float FeetToCentimeters = 30.48f;
    public const float InchesToMeters = 0.0254f;
    public const float InchesToCentimeters = 2.54f;
    public const float MetersToFeet = 3.28084f;
    public const float MetersToInches = 39.3701f;
    public const float CentimetersToFeet = 0.0328084f;
    public const float CentimetersToInches = 0.393701f;
    public const float KilometersToMiles = 0.621371f;
    public const float MilesToKilometers = 1.60934f;

    public static float RemapNumber(float num, float low1, float high1, float low2, float high2) => low2 + (float) (((double) num - (double) low1) * ((double) high2 - (double) low2) / ((double) high1 - (double) low1));

    public static float RemapNumberClamped(
      float num,
      float low1,
      float high1,
      float low2,
      float high2)
    {
      return Mathf.Clamp(Util.RemapNumber(num, low1, high1, low2, high2), Mathf.Min(low2, high2), Mathf.Max(low2, high2));
    }

    public static float Approach(float target, float value, float speed)
    {
      float num = target - value;
      if ((double) num > (double) speed)
        value += speed;
      else if ((double) num < -(double) speed)
        value -= speed;
      else
        value = target;
      return value;
    }

    public static Vector3 BezierInterpolate3(Vector3 p0, Vector3 c0, Vector3 p1, float t) => Vector3.Lerp(Vector3.Lerp(p0, c0, t), Vector3.Lerp(c0, p1, t), t);

    public static Vector3 BezierInterpolate4(
      Vector3 p0,
      Vector3 c0,
      Vector3 c1,
      Vector3 p1,
      float t)
    {
      Vector3 a = Vector3.Lerp(p0, c0, t);
      Vector3 vector3 = Vector3.Lerp(c0, c1, t);
      Vector3 b = Vector3.Lerp(c1, p1, t);
      return Vector3.Lerp(Vector3.Lerp(a, vector3, t), Vector3.Lerp(vector3, b, t), t);
    }

    public static Vector3 Vector3FromString(string szString)
    {
      string[] strArray = szString.Substring(1, szString.Length - 1).Split(',');
      return new Vector3(float.Parse(strArray[0]), float.Parse(strArray[1]), float.Parse(strArray[2]));
    }

    public static Vector2 Vector2FromString(string szString)
    {
      string[] strArray = szString.Substring(1, szString.Length - 1).Split(',');
      return (Vector2) (Vector3) new Vector2(float.Parse(strArray[0]), float.Parse(strArray[1]));
    }

    public static float Normalize(float value, float min, float max) => (float) (((double) value - (double) min) / ((double) max - (double) min));

    public static Vector3 Vector2AsVector3(Vector2 v) => new Vector3(v.x, 0.0f, v.y);

    public static Vector2 Vector3AsVector2(Vector3 v) => new Vector2(v.x, v.z);

    public static float AngleOf(Vector2 v)
    {
      float magnitude = v.magnitude;
      return (double) v.y >= 0.0 ? Mathf.Acos(v.x / magnitude) : Mathf.Acos(-v.x / magnitude) + 3.141593f;
    }

    public static float YawOf(Vector3 v)
    {
      float magnitude = v.magnitude;
      return (double) v.z >= 0.0 ? Mathf.Acos(v.x / magnitude) : Mathf.Acos(-v.x / magnitude) + 3.141593f;
    }

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
      T obj = lhs;
      lhs = rhs;
      rhs = obj;
    }

    public static void Shuffle<T>(T[] array)
    {
      for (int max = array.Length - 1; max > 0; --max)
      {
        int index = UnityEngine.Random.Range(0, max);
        Util.Swap<T>(ref array[max], ref array[index]);
      }
    }

    public static void Shuffle<T>(List<T> list)
    {
      for (int index1 = list.Count - 1; index1 > 0; --index1)
      {
        int index2 = UnityEngine.Random.Range(0, index1);
        T obj = list[index1];
        list[index1] = list[index2];
        list[index2] = obj;
      }
    }

    public static int RandomWithLookback(int min, int max, List<int> history, int historyCount)
    {
      int num = UnityEngine.Random.Range(min, max - history.Count);
      for (int index = 0; index < history.Count; ++index)
      {
        if (num >= history[index])
          ++num;
      }
      history.Add(num);
      if (history.Count > historyCount)
        history.RemoveRange(0, history.Count - historyCount);
      return num;
    }

    public static Transform FindChild(Transform parent, string name)
    {
      if (parent.name == name)
        return parent;
      foreach (Transform parent1 in parent)
      {
        Transform child = Util.FindChild(parent1, name);
        if ((UnityEngine.Object) child != (UnityEngine.Object) null)
          return child;
      }
      return (Transform) null;
    }

    public static bool IsNullOrEmpty<T>(T[] array) => array == null || array.Length == 0;

    public static bool IsValidIndex<T>(T[] array, int i) => array != null && i >= 0 && i < array.Length;

    public static bool IsValidIndex<T>(List<T> list, int i) => list != null && list.Count != 0 && i >= 0 && i < list.Count;

    public static int FindOrAdd<T>(List<T> list, T item)
    {
      int num = list.IndexOf(item);
      if (num == -1)
      {
        list.Add(item);
        num = list.Count - 1;
      }
      return num;
    }

    public static List<T> FindAndRemove<T>(List<T> list, Predicate<T> match)
    {
      List<T> all = list.FindAll(match);
      list.RemoveAll(match);
      return all;
    }

    public static T FindOrAddComponent<T>(GameObject gameObject) where T : Component
    {
      T component = gameObject.GetComponent<T>();
      return (bool) (UnityEngine.Object) component ? component : gameObject.AddComponent<T>();
    }

    public static void FastRemove<T>(List<T> list, int index)
    {
      list[index] = list[list.Count - 1];
      list.RemoveAt(list.Count - 1);
    }

    public static void ReplaceGameObject<T, U>(T replace, U replaceWith)
      where T : MonoBehaviour
      where U : MonoBehaviour
    {
      replace.gameObject.SetActive(false);
      replaceWith.gameObject.SetActive(true);
    }

    public static void SwitchLayerRecursively(Transform transform, int fromLayer, int toLayer)
    {
      if (transform.gameObject.layer == fromLayer)
        transform.gameObject.layer = toLayer;
      int childCount = transform.childCount;
      for (int index = 0; index < childCount; ++index)
        Util.SwitchLayerRecursively(transform.GetChild(index), fromLayer, toLayer);
    }

    public static void DrawCross(Vector3 origin, Color crossColor, float size)
    {
      UnityEngine.Debug.DrawLine(origin + Vector3.right * size, origin - Vector3.right * size, crossColor);
      UnityEngine.Debug.DrawLine(origin + Vector3.up * size, origin - Vector3.up * size, crossColor);
      UnityEngine.Debug.DrawLine(origin + Vector3.forward * size, origin - Vector3.forward * size, crossColor);
    }

    public static void ResetTransform(Transform t, bool resetScale = true)
    {
      t.localPosition = Vector3.zero;
      t.localRotation = Quaternion.identity;
      if (!resetScale)
        return;
      t.localScale = new Vector3(1f, 1f, 1f);
    }

    public static Vector3 ClosestPointOnLine(Vector3 vA, Vector3 vB, Vector3 vPoint)
    {
      Vector3 rhs = vPoint - vA;
      Vector3 normalized = (vB - vA).normalized;
      float num1 = Vector3.Distance(vA, vB);
      float num2 = Vector3.Dot(normalized, rhs);
      if ((double) num2 <= 0.0)
        return vA;
      if ((double) num2 >= (double) num1)
        return vB;
      Vector3 vector3 = normalized * num2;
      return vA + vector3;
    }

    public static void AfterTimer(
      GameObject go,
      float _time,
      System.Action callback,
      bool trigger_if_destroyed_early = false)
    {
      go.AddComponent<AfterTimer_Component>().Init(_time, callback, trigger_if_destroyed_early);
    }

    public static void SendPhysicsMessage(
      Collider collider,
      string message,
      SendMessageOptions sendMessageOptions)
    {
      Rigidbody attachedRigidbody = collider.attachedRigidbody;
      if ((bool) (UnityEngine.Object) attachedRigidbody && (UnityEngine.Object) attachedRigidbody.gameObject != (UnityEngine.Object) collider.gameObject)
        attachedRigidbody.SendMessage(message, sendMessageOptions);
      collider.SendMessage(message, sendMessageOptions);
    }

    public static void SendPhysicsMessage(
      Collider collider,
      string message,
      object arg,
      SendMessageOptions sendMessageOptions)
    {
      Rigidbody attachedRigidbody = collider.attachedRigidbody;
      if ((bool) (UnityEngine.Object) attachedRigidbody && (UnityEngine.Object) attachedRigidbody.gameObject != (UnityEngine.Object) collider.gameObject)
        attachedRigidbody.SendMessage(message, arg, sendMessageOptions);
      collider.SendMessage(message, arg, sendMessageOptions);
    }

    public static void IgnoreCollisions(GameObject goA, GameObject goB)
    {
      Collider[] componentsInChildren1 = goA.GetComponentsInChildren<Collider>();
      Collider[] componentsInChildren2 = goB.GetComponentsInChildren<Collider>();
      if (componentsInChildren1.Length == 0 || componentsInChildren2.Length == 0)
        return;
      foreach (Collider collider1 in componentsInChildren1)
      {
        foreach (Collider collider2 in componentsInChildren2)
        {
          if (collider1.enabled && collider2.enabled)
            Physics.IgnoreCollision(collider1, collider2, true);
        }
      }
    }

    [DebuggerHidden]
    public static IEnumerator WrapCoroutine(
      IEnumerator coroutine,
      System.Action onCoroutineFinished)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new Util.\u003CWrapCoroutine\u003Ec__Iterator0()
      {
        coroutine = coroutine,
        onCoroutineFinished = onCoroutineFinished
      };
    }

    public static Color ColorWithAlpha(this Color color, float alpha)
    {
      color.a = alpha;
      return color;
    }

    public static void Quit() => Process.GetCurrentProcess().Kill();

    public static Decimal FloatToDecimal(float value, int decimalPlaces = 2) => Math.Round((Decimal) value, decimalPlaces);

    public static T Median<T>(this IEnumerable<T> source)
    {
      int num = source != null ? source.Count<T>() : throw new ArgumentException("Argument cannot be null.", nameof (source));
      if (num == 0)
        throw new InvalidOperationException("Enumerable must contain at least one element.");
      return source.OrderBy<T, T>((Func<T, T>) (x => x)).ElementAt<T>(num / 2);
    }

    public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
    {
      if (source == null)
        throw new ArgumentException("Argument cannot be null.", nameof (source));
      foreach (T obj in source)
        action(obj);
    }

    public static string FixupNewlines(string text)
    {
      bool flag = true;
      while (flag)
      {
        int num = text.IndexOf("\\n");
        if (num == -1)
        {
          flag = false;
        }
        else
        {
          text = text.Remove(num - 1, 3);
          text = text.Insert(num - 1, "\n");
        }
      }
      return text;
    }

    public static float PathLength(NavMeshPath path)
    {
      if (path.corners.Length < 2)
        return 0.0f;
      Vector3 a = path.corners[0];
      float num = 0.0f;
      for (int index = 1; index < path.corners.Length; ++index)
      {
        Vector3 corner = path.corners[index];
        num += Vector3.Distance(a, corner);
        a = corner;
      }
      return num;
    }

    public static bool HasCommandLineArgument(string argumentName)
    {
      foreach (string commandLineArg in Environment.GetCommandLineArgs())
      {
        if (commandLineArg.Equals(argumentName))
          return true;
      }
      return false;
    }

    public static int GetCommandLineArgValue(string argumentName, int nDefaultValue)
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 0; index < commandLineArgs.Length; ++index)
      {
        if (commandLineArgs[index].Equals(argumentName))
          return index == commandLineArgs.Length - 1 ? nDefaultValue : int.Parse(commandLineArgs[index + 1]);
      }
      return nDefaultValue;
    }

    public static float GetCommandLineArgValue(string argumentName, float flDefaultValue)
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      for (int index = 0; index < commandLineArgs.Length; ++index)
      {
        if (commandLineArgs[index].Equals(argumentName))
          return index == commandLineArgs.Length - 1 ? flDefaultValue : (float) double.Parse(commandLineArgs[index + 1]);
      }
      return flDefaultValue;
    }

    public static void SetActive(GameObject gameObject, bool active)
    {
      if (!((UnityEngine.Object) gameObject != (UnityEngine.Object) null))
        return;
      gameObject.SetActive(active);
    }

    public static string CombinePaths(params string[] paths)
    {
      if (paths.Length == 0)
        return string.Empty;
      string path1 = paths[0];
      for (int index = 1; index < paths.Length; ++index)
        path1 = Path.Combine(path1, paths[index]);
      return path1;
    }
  }
}
