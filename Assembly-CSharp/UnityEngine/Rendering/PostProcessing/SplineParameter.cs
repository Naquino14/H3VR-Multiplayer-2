// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.SplineParameter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class SplineParameter : ParameterOverride<Spline>
  {
    public override void Interp(Spline from, Spline to, float t)
    {
      int renderedFrameCount = Time.renderedFrameCount;
      from.Cache(renderedFrameCount);
      to.Cache(renderedFrameCount);
      for (int index = 0; index < 128; ++index)
      {
        float num1 = from.cachedData[index];
        float num2 = to.cachedData[index];
        this.value.cachedData[index] = num1 + (num2 - num1) * t;
      }
    }
  }
}
