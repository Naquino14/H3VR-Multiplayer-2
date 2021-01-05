// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.Vignette
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [PostProcess(typeof (VignetteRenderer), "Unity/Vignette", true)]
  [Serializable]
  public sealed class Vignette : PostProcessEffectSettings
  {
    [Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
    public VignetteModeParameter mode;
    [Tooltip("Vignette color. Use the alpha channel for transparency.")]
    public ColorParameter color;
    [Tooltip("Sets the vignette center point (screen center is [0.5,0.5]).")]
    public Vector2Parameter center;
    [Range(0.0f, 1f)]
    [Tooltip("Amount of vignetting on screen.")]
    public FloatParameter intensity;
    [Range(0.01f, 1f)]
    [Tooltip("Smoothness of the vignette borders.")]
    public FloatParameter smoothness;
    [Range(0.0f, 1f)]
    [Tooltip("Lower values will make a square-ish vignette.")]
    public FloatParameter roundness;
    [Tooltip("Should the vignette be perfectly round or be dependent on the current aspect ratio?")]
    public BoolParameter rounded;
    [Tooltip("A black and white mask to use as a vignette.")]
    public TextureParameter mask;
    [Range(0.0f, 1f)]
    [Tooltip("Mask opacity.")]
    public FloatParameter opacity;

    public Vignette()
    {
      VignetteModeParameter vignetteModeParameter = new VignetteModeParameter();
      vignetteModeParameter.value = VignetteMode.Classic;
      this.mode = vignetteModeParameter;
      ColorParameter colorParameter = new ColorParameter();
      colorParameter.value = new Color(0.0f, 0.0f, 0.0f, 1f);
      this.color = colorParameter;
      Vector2Parameter vector2Parameter = new Vector2Parameter();
      vector2Parameter.value = new Vector2(0.5f, 0.5f);
      this.center = vector2Parameter;
      FloatParameter floatParameter1 = new FloatParameter();
      floatParameter1.value = 0.0f;
      this.intensity = floatParameter1;
      FloatParameter floatParameter2 = new FloatParameter();
      floatParameter2.value = 0.2f;
      this.smoothness = floatParameter2;
      FloatParameter floatParameter3 = new FloatParameter();
      floatParameter3.value = 1f;
      this.roundness = floatParameter3;
      BoolParameter boolParameter = new BoolParameter();
      boolParameter.value = false;
      this.rounded = boolParameter;
      TextureParameter textureParameter = new TextureParameter();
      textureParameter.value = (Texture) null;
      this.mask = textureParameter;
      FloatParameter floatParameter4 = new FloatParameter();
      floatParameter4.value = 1f;
      this.opacity = floatParameter4;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
      if (!this.enabled.value)
        return false;
      if (this.mode.value == VignetteMode.Classic && (double) this.intensity.value > 0.0)
        return true;
      return this.mode.value == VignetteMode.Masked && (double) this.opacity.value > 0.0 && (UnityEngine.Object) this.mask.value != (UnityEngine.Object) null;
    }
  }
}
