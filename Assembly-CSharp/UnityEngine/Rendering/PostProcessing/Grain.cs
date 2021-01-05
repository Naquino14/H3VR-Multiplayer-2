// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Grain
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (GrainRenderer), "Unity/Grain", true)]
  [Serializable]
  public sealed class Grain : PostProcessEffectSettings
  {
    [Tooltip("Enable the use of colored grain.")]
    public BoolParameter colored;
    [Range(0.0f, 1f)]
    [Tooltip("Grain strength. Higher means more visible grain.")]
    public FloatParameter intensity;
    [Range(0.3f, 3f)]
    [Tooltip("Grain particle size.")]
    public FloatParameter size;
    [Range(0.0f, 1f)]
    [DisplayName("Luminance Contribution")]
    [Tooltip("Controls the noisiness response curve based on scene luminance. Lower values mean less noise in dark areas.")]
    public FloatParameter lumContrib;

    public Grain()
    {
      BoolParameter boolParameter = new BoolParameter();
      boolParameter.value = true;
      this.colored = boolParameter;
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 0.0f;
      this.intensity = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 1f;
      this.size = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 0.8f;
      this.lumContrib = floatParameter3;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled.value && (double) this.intensity.value > 0.0;
  }
}
