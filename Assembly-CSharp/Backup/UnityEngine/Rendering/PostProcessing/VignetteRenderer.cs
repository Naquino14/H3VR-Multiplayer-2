// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.VignetteRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class VignetteRenderer : PostProcessEffectRenderer<Vignette>
  {
    public override void Render(PostProcessRenderContext context)
    {
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("VIGNETTE");
      uberSheet.properties.SetColor(ShaderIDs.Vignette_Color, this.settings.color.value);
      if ((VignetteMode) (ParameterOverride<VignetteMode>) this.settings.mode == VignetteMode.Classic)
      {
        uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 0.0f);
        uberSheet.properties.SetVector(ShaderIDs.Vignette_Center, (Vector4) this.settings.center.value);
        float z = (float) ((1.0 - (double) this.settings.roundness.value) * 6.0) + this.settings.roundness.value;
        uberSheet.properties.SetVector(ShaderIDs.Vignette_Settings, new Vector4(this.settings.intensity.value * 3f, this.settings.smoothness.value * 5f, z, !this.settings.rounded.value ? 0.0f : 1f));
      }
      else
      {
        uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 1f);
        uberSheet.properties.SetTexture(ShaderIDs.Vignette_Mask, this.settings.mask.value);
        uberSheet.properties.SetFloat(ShaderIDs.Vignette_Opacity, Mathf.Clamp01(this.settings.opacity.value));
      }
    }
  }
}
