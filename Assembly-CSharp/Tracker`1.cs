// Decompiled with JetBrains decompiler
// Type: Tracker`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

public class Tracker<T> where T : ITracked
{
  public static T[] All = new T[32];
  public static int Count;

  public static void Register(T item)
  {
    if (Tracker<T>.Count >= Tracker<T>.All.Length)
      Array.Resize<T>(ref Tracker<T>.All, Tracker<T>.All.Length * 2);
    Tracker<T>.All[Tracker<T>.Count] = item;
    item.Index = Tracker<T>.Count;
    ++Tracker<T>.Count;
  }

  public static void Deregister(T item)
  {
    if (item.Index == -1)
      return;
    --Tracker<T>.Count;
    if (Tracker<T>.Count < 0)
      return;
    Tracker<T>.All[Tracker<T>.Count].Index = item.Index;
    Tracker<T>.All[item.Index] = Tracker<T>.All[Tracker<T>.Count];
  }
}
