// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ChromaticAberrationRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class ChromaticAberrationRenderer : PostProcessEffectRenderer<ChromaticAberration>
  {
    private Texture2D m_InternalSpectralLut;

    public override void Render(PostProcessRenderContext context)
    {
      Texture internalSpectralLut = this.settings.spectralLut.value;
      if ((Object) internalSpectralLut == (Object) null)
      {
        if ((Object) this.m_InternalSpectralLut == (Object) null)
        {
          Texture2D texture2D = new Texture2D(3, 1, TextureFormat.RGB24, false);
          texture2D.name = "Chromatic Aberration Spectrum Lookup";
          texture2D.filterMode = FilterMode.Bilinear;
          texture2D.wrapMode = TextureWrapMode.Clamp;
          texture2D.anisoLevel = 0;
          texture2D.hideFlags = HideFlags.DontSave;
          this.m_InternalSpectralLut = texture2D;
          this.m_InternalSpectralLut.SetPixels(new Color[3]
          {
            new Color(1f, 0.0f, 0.0f),
            new Color(0.0f, 1f, 0.0f),
            new Color(0.0f, 0.0f, 1f)
          });
          this.m_InternalSpectralLut.Apply();
        }
        internalSpectralLut = (Texture) this.m_InternalSpectralLut;
      }
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword(!(bool) (ParameterOverride<bool>) this.settings.mobileOptimized ? "CHROMATIC_ABERRATION" : "CHROMATIC_ABERRATION_LOW");
      uberSheet.properties.SetFloat(ShaderIDs.ChromaticAberration_Amount, (float) (ParameterOverride<float>) this.settings.intensity * 0.05f);
      uberSheet.properties.SetTexture(ShaderIDs.ChromaticAberration_SpectralLut, internalSpectralLut);
    }

    public override void Release()
    {
      RuntimeUtilities.Destroy((Object) this.m_InternalSpectralLut);
      this.m_InternalSpectralLut = (Texture2D) null;
    }
  }
}
