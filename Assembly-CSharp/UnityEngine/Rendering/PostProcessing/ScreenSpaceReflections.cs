// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (ScreenSpaceReflectionsRenderer), "Unity/Screen-space reflections", true)]
  [Serializable]
  public sealed class ScreenSpaceReflections : PostProcessEffectSettings
  {
    [Tooltip("Choose a quality preset, or use \"Custom\" to fine tune it. Don't use a preset higher than \"Medium\" if you care about performances on consoles.")]
    public ScreenSpaceReflectionPresetParameter preset;
    [Range(0.0f, 256f)]
    [Tooltip("Maximum iteration count.")]
    public IntParameter maximumIterationCount;
    [Tooltip("Changes the size of the SSR buffer. Downsample it to maximize performances or supersample it to get slow but higher quality results.")]
    public ScreenSpaceReflectionResolutionParameter resolution;
    [Range(1f, 64f)]
    [Tooltip("Ray thickness. Lower values are more expensive but allow the effect to detect smaller details.")]
    public FloatParameter thickness;
    [Tooltip("Maximum distance to traverse after which it will stop drawing reflections.")]
    public FloatParameter maximumMarchDistance;
    [Range(0.0f, 1f)]
    [Tooltip("Fades reflections close to the near planes.")]
    public FloatParameter distanceFade;
    [Range(0.0f, 1f)]
    [Tooltip("Fades reflections close to the screen borders.")]
    public FloatParameter attenuation;

    public ScreenSpaceReflections()
    {
      ScreenSpaceReflectionPresetParameter reflectionPresetParameter = new ScreenSpaceReflectionPresetParameter();
      reflectionPresetParameter.value = ScreenSpaceReflectionPreset.Medium;
      this.preset = reflectionPresetParameter;
      IntParameter intParameter = new IntParameter();
      intParameter.value = 16;
      this.maximumIterationCount = intParameter;
      ScreenSpaceReflectionResolutionParameter resolutionParameter = new ScreenSpaceReflectionResolutionParameter();
      resolutionParameter.value = ScreenSpaceReflectionResolution.Downsampled;
      this.resolution = resolutionParameter;
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 8f;
      this.thickness = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 100f;
      this.maximumMarchDistance = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 0.5f;
      this.distanceFade = floatParameter3;
      FloatParameter floatParameter4 = new FloatParameter();
      floatParameter4.value = 0.25f;
      this.attenuation = floatParameter4;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => (bool) (ParameterOverride<bool>) this.enabled && context.camera.actualRenderingPath == RenderingPath.DeferredShading && (SystemInfo.supportsMotionVectors && SystemInfo.supportsComputeShaders) && SystemInfo.copyTextureSupport > CopyTextureSupport.None;
  }
}
