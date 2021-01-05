// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.MotionBlur
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (MotionBlurRenderer), "Unity/Motion Blur", false)]
  [Serializable]
  public sealed class MotionBlur : PostProcessEffectSettings
  {
    [Range(0.0f, 360f)]
    [Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
    public FloatParameter shutterAngle;
    [Range(4f, 32f)]
    [Tooltip("The amount of sample points, which affects quality and performances.")]
    public IntParameter sampleCount;

    public MotionBlur()
    {
      FloatParameter floatParameter = new FloatParameter();
      floatParameter.value = 270f;
      this.shutterAngle = floatParameter;
      IntParameter intParameter = new IntParameter();
      intParameter.value = 10;
      this.sampleCount = intParameter;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled.value && (double) this.shutterAngle.value > 0.0 && (SystemInfo.supportsMotionVectors && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)) && !RuntimeUtilities.isVREnabled;
  }
}
