// Decompiled with JetBrains decompiler
// Type: LowPassFilter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

internal class LowPassFilter
{
  private float y;
  private float a;
  private float s;
  private bool initialized;

  public LowPassFilter(float _alpha, float _initval = 0.0f)
  {
    this.y = this.s = _initval;
    this.setAlpha(_alpha);
    this.initialized = false;
  }

  public void setAlpha(float _alpha)
  {
    if ((double) _alpha <= 0.0 || (double) _alpha > 1.0)
      Debug.LogError((object) "alpha should be in (0.0., 1.0]");
    else
      this.a = _alpha;
  }

  public float Filter(float _value)
  {
    float num;
    if (this.initialized)
    {
      num = (float) ((double) this.a * (double) _value + (1.0 - (double) this.a) * (double) this.s);
    }
    else
    {
      num = _value;
      this.initialized = true;
    }
    this.y = _value;
    this.s = num;
    return num;
  }

  public float filterWithAlpha(float _value, float _alpha)
  {
    this.setAlpha(_alpha);
    return this.Filter(_value);
  }

  public bool hasLastRawValue() => this.initialized;

  public float lastRawValue() => this.y;
}
