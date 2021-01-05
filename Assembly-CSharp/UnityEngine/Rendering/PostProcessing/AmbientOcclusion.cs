// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.AmbientOcclusion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (AmbientOcclusionRenderer), "Unity/Ambient Occlusion", true)]
  [Serializable]
  public sealed class AmbientOcclusion : PostProcessEffectSettings
  {
    [Tooltip("The ambient occlusion method to use. \"Modern\" is higher quality and faster on desktop & console platforms but requires compute shader support.")]
    public AmbientOcclusionModeParameter mode;
    [Range(0.0f, 4f)]
    [Tooltip("Degree of darkness added by ambient occlusion.")]
    public FloatParameter intensity;
    [ColorUsage(false)]
    [Tooltip("Custom color to use for the ambient occlusion.")]
    public ColorParameter color;
    [Tooltip("Only affects ambient lighting. This mode is only available with the Deferred rendering path and HDR rendering. Objects rendered with the Forward rendering path won't get any ambient occlusion.")]
    public BoolParameter ambientOnly;
    [Range(-8f, 0.0f)]
    public FloatParameter noiseFilterTolerance;
    [Range(-8f, -1f)]
    public FloatParameter blurTolerance;
    [Range(-12f, -1f)]
    public FloatParameter upsampleTolerance;
    [Range(1f, 10f)]
    [Tooltip("Modifies thickness of occluders. This increases dark areas but also introduces dark halo around objects.")]
    public FloatParameter thicknessModifier;
    [Tooltip("Radius of sample points, which affects extent of darkened areas.")]
    public FloatParameter radius;
    [Tooltip("Number of sample points, which affects quality and performance. Lowest, Low & Medium passes are downsampled. High and Ultra are not and should only be used on high-end hardware.")]
    public AmbientOcclusionQualityParameter quality;

    public AmbientOcclusion()
    {
      AmbientOcclusionModeParameter occlusionModeParameter = new AmbientOcclusionModeParameter();
      occlusionModeParameter.value = AmbientOcclusionMode.MultiScaleVolumetricObscurance;
      this.mode = occlusionModeParameter;
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 0.0f;
      this.intensity = floatParameter1;
      ColorParameter colorParameter = new ColorParameter();
      colorParameter.value = Color.black;
      this.color = colorParameter;
      BoolParameter boolParameter = new BoolParameter();
      boolParameter.value = true;
      this.ambientOnly = boolParameter;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 0.0f;
      this.noiseFilterTolerance = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = -4.6f;
      this.blurTolerance = floatParameter3;
      FloatParameter floatParameter4 = new FloatParameter();
      floatParameter4.value = -12f;
      this.upsampleTolerance = floatParameter4;
      FloatParameter floatParameter5 = new FloatParameter();
      floatParameter5.value = 1f;
      this.thicknessModifier = floatParameter5;
      FloatParameter floatParameter6 = new FloatParameter();
      floatParameter6.value = 0.25f;
      this.radius = floatParameter6;
      AmbientOcclusionQualityParameter qualityParameter = new AmbientOcclusionQualityParameter();
      qualityParameter.value = AmbientOcclusionQuality.Medium;
      this.quality = qualityParameter;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
      bool flag = this.enabled.value && (double) this.intensity.value > 0.0 && !RuntimeUtilities.scriptableRenderPipelineActive;
      if (this.mode.value == AmbientOcclusionMode.MultiScaleVolumetricObscurance)
        flag = ((flag ? 1 : 0) & (!SystemInfo.supportsComputeShaders ? 0 : (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat) ? 1 : 0))) != 0;
      return flag;
    }
  }
}
