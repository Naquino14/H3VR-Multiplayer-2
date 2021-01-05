// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.AutoExposure
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (AutoExposureRenderer), "Unity/Auto Exposure", true)]
  [Serializable]
  public sealed class AutoExposure : PostProcessEffectSettings
  {
    [MinMax(1f, 99f)]
    [DisplayName("Filtering (%)")]
    [Tooltip("Filters the bright & dark part of the histogram when computing the average luminance to avoid very dark pixels & very bright pixels from contributing to the auto exposure. Unit is in percent.")]
    public Vector2Parameter filtering;
    [Range(-9f, 9f)]
    [DisplayName("Minimum (EV)")]
    [Tooltip("Minimum average luminance to consider for auto exposure (in EV).")]
    public FloatParameter minLuminance;
    [Range(-9f, 9f)]
    [DisplayName("Maximum (EV)")]
    [Tooltip("Maximum average luminance to consider for auto exposure (in EV).")]
    public FloatParameter maxLuminance;
    [Min(0.0f)]
    [Tooltip("Exposure bias. Use this to offset the global exposure of the scene.")]
    public FloatParameter keyValue;
    [DisplayName("Type")]
    [Tooltip("Use \"Progressive\" if you want auto exposure to be animated. Use \"Fixed\" otherwise.")]
    public EyeAdaptationParameter eyeAdaptation;
    [Min(0.0f)]
    [Tooltip("Adaptation speed from a dark to a light environment.")]
    public FloatParameter speedUp;
    [Min(0.0f)]
    [Tooltip("Adaptation speed from a light to a dark environment.")]
    public FloatParameter speedDown;

    public AutoExposure()
    {
      Vector2Parameter vector2Parameter = new Vector2Parameter();
      vector2Parameter.value = new Vector2(50f, 95f);
      this.filtering = vector2Parameter;
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 0.0f;
      this.minLuminance = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 0.0f;
      this.maxLuminance = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 1f;
      this.keyValue = floatParameter3;
      EyeAdaptationParameter adaptationParameter = new EyeAdaptationParameter();
      adaptationParameter.value = EyeAdaptation.Progressive;
      this.eyeAdaptation = adaptationParameter;
      FloatParameter floatParameter4 = new FloatParameter();
      floatParameter4.value = 2f;
      this.speedUp = floatParameter4;
      FloatParameter floatParameter5 = new FloatParameter();
      floatParameter5.value = 1f;
      this.speedDown = floatParameter5;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled.value && SystemInfo.supportsComputeShaders && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat);
  }
}
