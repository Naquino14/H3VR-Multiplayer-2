// Decompiled with JetBrains decompiler
// Type: OneEuroFilter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

internal class OneEuroFilter
{
  private float freq;
  private float mincutoff;
  private float beta;
  private float dcutoff;
  private LowPassFilter x;
  private LowPassFilter dx;
  private float lasttime;

  public OneEuroFilter(float _freq, float _mincutoff = 1f, float _beta = 0.0f, float _dcutoff = 1f)
  {
    this.setFrequency(_freq);
    this.setMinCutoff(_mincutoff);
    this.setBeta(_beta);
    this.setDerivateCutoff(_dcutoff);
    this.x = new LowPassFilter(this.alpha(this.mincutoff));
    this.dx = new LowPassFilter(this.alpha(this.dcutoff));
    this.lasttime = -1f;
    this.currValue = 0.0f;
    this.prevValue = this.currValue;
  }

  public float currValue { get; protected set; }

  public float prevValue { get; protected set; }

  private float alpha(float _cutoff)
  {
    float num = 1f / this.freq;
    return (float) (1.0 / (1.0 + 1.0 / (6.28318548202515 * (double) _cutoff) / (double) num));
  }

  private void setFrequency(float _f)
  {
    if ((double) _f <= 0.0)
      Debug.LogError((object) "freq should be > 0");
    else
      this.freq = _f;
  }

  private void setMinCutoff(float _mc)
  {
    if ((double) _mc <= 0.0)
      Debug.LogError((object) "mincutoff should be > 0");
    else
      this.mincutoff = _mc;
  }

  private void setBeta(float _b) => this.beta = _b;

  private void setDerivateCutoff(float _dc)
  {
    if ((double) _dc <= 0.0)
      Debug.LogError((object) "dcutoff should be > 0");
    else
      this.dcutoff = _dc;
  }

  public void UpdateParams(float _freq, float _mincutoff, float _beta, float _dcutoff)
  {
    this.setFrequency(_freq);
    this.setMinCutoff(_mincutoff);
    this.setBeta(_beta);
    this.setDerivateCutoff(_dcutoff);
    this.x.setAlpha(this.alpha(this.mincutoff));
    this.dx.setAlpha(this.alpha(this.dcutoff));
  }

  public float Filter(float value, float timestamp = -1f)
  {
    this.prevValue = this.currValue;
    if ((double) this.lasttime != -1.0 && (double) timestamp != -1.0)
      this.freq = (float) (1.0 / ((double) timestamp - (double) this.lasttime));
    this.lasttime = timestamp;
    float _cutoff = this.mincutoff + this.beta * Mathf.Abs(this.dx.filterWithAlpha(!this.x.hasLastRawValue() ? 0.0f : (value - this.x.lastRawValue()) * this.freq, this.alpha(this.dcutoff)));
    this.currValue = this.x.filterWithAlpha(value, this.alpha(_cutoff));
    return this.currValue;
  }
}
