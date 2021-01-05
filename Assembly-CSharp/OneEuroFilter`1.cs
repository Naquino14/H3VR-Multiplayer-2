// Decompiled with JetBrains decompiler
// Type: OneEuroFilter`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

public class OneEuroFilter<T> where T : struct
{
  private System.Type type;
  private OneEuroFilter[] oneEuroFilters;

  public OneEuroFilter(float _freq, float _mincutoff = 1f, float _beta = 0.0f, float _dcutoff = 1f)
  {
    this.type = typeof (T);
    this.currValue = new T();
    this.prevValue = new T();
    this.freq = _freq;
    this.mincutoff = _mincutoff;
    this.beta = _beta;
    this.dcutoff = _dcutoff;
    if (this.type == typeof (Vector2))
      this.oneEuroFilters = new OneEuroFilter[2];
    else if (this.type == typeof (Vector3))
      this.oneEuroFilters = new OneEuroFilter[3];
    else if (this.type == typeof (Vector4) || this.type == typeof (Quaternion))
    {
      this.oneEuroFilters = new OneEuroFilter[4];
    }
    else
    {
      Debug.LogError((object) (this.type.ToString() + " is not a supported type"));
      return;
    }
    for (int index = 0; index < this.oneEuroFilters.Length; ++index)
      this.oneEuroFilters[index] = new OneEuroFilter(this.freq, this.mincutoff, this.beta, this.dcutoff);
  }

  public float freq { get; protected set; }

  public float mincutoff { get; protected set; }

  public float beta { get; protected set; }

  public float dcutoff { get; protected set; }

  public T currValue { get; protected set; }

  public T prevValue { get; protected set; }

  public void UpdateParams(float _freq, float _mincutoff = 1f, float _beta = 0.0f, float _dcutoff = 1f)
  {
    this.freq = _freq;
    this.mincutoff = _mincutoff;
    this.beta = _beta;
    this.dcutoff = _dcutoff;
    for (int index = 0; index < this.oneEuroFilters.Length; ++index)
      this.oneEuroFilters[index].UpdateParams(this.freq, this.mincutoff, this.beta, this.dcutoff);
  }

  public T Filter<U>(U _value, float timestamp = -1f) where U : struct
  {
    this.prevValue = this.currValue;
    if (typeof (U) != this.type)
    {
      Debug.LogError((object) ("WARNING! " + (object) typeof (U) + " when " + (object) this.type + " is expected!\nReturning previous filtered value"));
      this.currValue = this.prevValue;
      return (T) Convert.ChangeType((object) this.currValue, typeof (T));
    }
    if (this.type == typeof (Vector2))
    {
      Vector2 zero = Vector2.zero;
      Vector2 vector2 = (Vector2) Convert.ChangeType((object) _value, typeof (Vector2));
      for (int index = 0; index < this.oneEuroFilters.Length; ++index)
        zero[index] = this.oneEuroFilters[index].Filter(vector2[index], timestamp);
      this.currValue = (T) Convert.ChangeType((object) zero, typeof (T));
    }
    else if (this.type == typeof (Vector3))
    {
      Vector3 zero = Vector3.zero;
      Vector3 vector3 = (Vector3) Convert.ChangeType((object) _value, typeof (Vector3));
      for (int index = 0; index < this.oneEuroFilters.Length; ++index)
        zero[index] = this.oneEuroFilters[index].Filter(vector3[index], timestamp);
      this.currValue = (T) Convert.ChangeType((object) zero, typeof (T));
    }
    else if (this.type == typeof (Vector4))
    {
      Vector4 zero = Vector4.zero;
      Vector4 vector4 = (Vector4) Convert.ChangeType((object) _value, typeof (Vector4));
      for (int index = 0; index < this.oneEuroFilters.Length; ++index)
        zero[index] = this.oneEuroFilters[index].Filter(vector4[index], timestamp);
      this.currValue = (T) Convert.ChangeType((object) zero, typeof (T));
    }
    else
    {
      Quaternion identity = Quaternion.identity;
      Quaternion quaternion = (Quaternion) Convert.ChangeType((object) _value, typeof (Quaternion));
      for (int index = 0; index < this.oneEuroFilters.Length; ++index)
        identity[index] = this.oneEuroFilters[index].Filter(quaternion[index], timestamp);
      this.currValue = (T) Convert.ChangeType((object) identity, typeof (T));
    }
    return (T) Convert.ChangeType((object) this.currValue, typeof (T));
  }
}
