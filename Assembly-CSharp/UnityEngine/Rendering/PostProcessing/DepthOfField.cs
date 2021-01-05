// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.DepthOfField
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (DepthOfFieldRenderer), "Unity/Depth of Field", false)]
  [Serializable]
  public sealed class DepthOfField : PostProcessEffectSettings
  {
    [Min(0.1f)]
    [Tooltip("Distance to the point of focus.")]
    public FloatParameter focusDistance;
    [Range(0.05f, 32f)]
    [Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
    public FloatParameter aperture;
    [Range(1f, 300f)]
    [Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
    public FloatParameter focalLength;
    [DisplayName("Max Blur Size")]
    [Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects performances (the larger the kernel is, the longer the GPU time is required).")]
    public KernelSizeParameter kernelSize;

    public DepthOfField()
    {
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 10f;
      this.focusDistance = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 5.6f;
      this.aperture = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 50f;
      this.focalLength = floatParameter3;
      KernelSizeParameter kernelSizeParameter = new KernelSizeParameter();
      kernelSizeParameter.value = KernelSize.Medium;
      this.kernelSize = kernelSizeParameter;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }
  }
}
