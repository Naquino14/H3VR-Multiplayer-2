// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ChromaticAberration
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (ChromaticAberrationRenderer), "Unity/Chromatic Aberration", true)]
  [Serializable]
  public sealed class ChromaticAberration : PostProcessEffectSettings
  {
    [Tooltip("Shift the hue of chromatic aberrations.")]
    public TextureParameter spectralLut;
    [Range(0.0f, 1f)]
    [Tooltip("Amount of tangential distortion.")]
    public FloatParameter intensity;
    [Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms.")]
    public BoolParameter mobileOptimized;

    public ChromaticAberration()
    {
      TextureParameter textureParameter = new TextureParameter();
      textureParameter.value = (Texture) null;
      this.spectralLut = textureParameter;
      FloatParameter floatParameter = new FloatParameter();
      floatParameter.value = 0.0f;
      this.intensity = floatParameter;
      BoolParameter boolParameter = new BoolParameter();
      boolParameter.value = false;
      this.mobileOptimized = boolParameter;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled.value && (double) this.intensity.value > 0.0;
  }
}
