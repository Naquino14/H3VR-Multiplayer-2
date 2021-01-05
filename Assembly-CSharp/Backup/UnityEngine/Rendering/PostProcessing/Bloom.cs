// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Bloom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (BloomRenderer), "Unity/Bloom", true)]
  [Serializable]
  public sealed class Bloom : PostProcessEffectSettings
  {
    [Min(0.0f)]
    [Tooltip("Strength of the bloom filter. Values higher than 1 will make bloom contribute more energy to the final render. Keep this under or equal to 1 if you want energy conservation.")]
    public FloatParameter intensity;
    [Min(0.0f)]
    [Tooltip("Filters out pixels under this level of brightness. Value is in gamma-space.")]
    public FloatParameter threshold;
    [Range(0.0f, 1f)]
    [Tooltip("Makes transition between under/over-threshold gradual (0 = hard threshold, 1 = soft threshold).")]
    public FloatParameter softKnee;
    [Range(1f, 10f)]
    [Tooltip("Changes the extent of veiling effects. For maximum quality stick to integer values. Because this value changes the internal iteration count, animating it isn't recommended as it may introduce small hiccups in the perceived radius.")]
    public FloatParameter diffusion;
    [Range(-1f, 1f)]
    [Tooltip("Distorts the bloom to give an anamorphic look. Negative values distort vertically, positive values distort horizontally.")]
    public FloatParameter anamorphicRatio;
    [ColorUsage(false, true, 0.0f, 8f, 0.125f, 3f)]
    [Tooltip("Global tint of the bloom filter.")]
    public ColorParameter color;
    [Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms.")]
    public BoolParameter mobileOptimized;
    [Tooltip("Dirtiness texture to add smudges or dust to the bloom effect.")]
    [DisplayName("Texture")]
    public TextureParameter dirtTexture;
    [Min(0.0f)]
    [Tooltip("Amount of dirtiness.")]
    [DisplayName("Intensity")]
    public FloatParameter dirtIntensity;

    public Bloom()
    {
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 0.0f;
      this.intensity = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 1f;
      this.threshold = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 0.5f;
      this.softKnee = floatParameter3;
      FloatParameter floatParameter4 = new FloatParameter();
      floatParameter4.value = 7f;
      this.diffusion = floatParameter4;
      FloatParameter floatParameter5 = new FloatParameter();
      floatParameter5.value = 0.0f;
      this.anamorphicRatio = floatParameter5;
      ColorParameter colorParameter = new ColorParameter();
      colorParameter.value = Color.white;
      this.color = colorParameter;
      BoolParameter boolParameter = new BoolParameter();
      boolParameter.value = false;
      this.mobileOptimized = boolParameter;
      TextureParameter textureParameter = new TextureParameter();
      textureParameter.value = (Texture) null;
      this.dirtTexture = textureParameter;
      FloatParameter floatParameter6 = new FloatParameter();
      floatParameter6.value = 0.0f;
      this.dirtIntensity = floatParameter6;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled.value && (double) this.intensity.value > 0.0;
  }
}
