// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Spline
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class Spline
  {
    public const int k_Precision = 128;
    public const float k_Step = 0.0078125f;
    public AnimationCurve curve;
    [SerializeField]
    private bool m_Loop;
    [SerializeField]
    private float m_ZeroValue;
    [SerializeField]
    private float m_Range;
    private AnimationCurve m_InternalLoopingCurve;
    private int frameCount = -1;
    internal float[] cachedData;

    public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
    {
      this.curve = curve;
      this.m_ZeroValue = zeroValue;
      this.m_Loop = loop;
      this.m_Range = bounds.magnitude;
      this.cachedData = new float[128];
    }

    public void Cache(int frame)
    {
      if (frame == this.frameCount)
        return;
      int length = this.curve.length;
      if (this.m_Loop && length > 1)
      {
        if (this.m_InternalLoopingCurve == null)
          this.m_InternalLoopingCurve = new AnimationCurve();
        Keyframe key1 = this.curve[length - 1];
        key1.time -= this.m_Range;
        Keyframe key2 = this.curve[0];
        key2.time += this.m_Range;
        this.m_InternalLoopingCurve.keys = this.curve.keys;
        this.m_InternalLoopingCurve.AddKey(key1);
        this.m_InternalLoopingCurve.AddKey(key2);
      }
      for (int index = 0; index < 128; ++index)
        this.cachedData[index] = this.Evaluate((float) index * (1f / 128f));
      this.frameCount = Time.renderedFrameCount;
    }

    public float Evaluate(float t)
    {
      if (this.curve.length == 0)
        return this.m_ZeroValue;
      return !this.m_Loop || this.curve.length == 1 ? this.curve.Evaluate(t) : this.m_InternalLoopingCurve.Evaluate(t);
    }

    public override int GetHashCode() => 17 * 23 + this.curve.GetHashCode();
  }
}
