// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.GrainRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class GrainRenderer : PostProcessEffectRenderer<Grain>
  {
    private RenderTexture m_GrainLookupRT;
    private const int k_SampleCount = 1024;
    private int m_SampleIndex;

    public override void Render(PostProcessRenderContext context)
    {
      float realtimeSinceStartup = Time.realtimeSinceStartup;
      float z = HaltonSeq.Get(this.m_SampleIndex & 1023, 2);
      float w = HaltonSeq.Get(this.m_SampleIndex & 1023, 3);
      if (++this.m_SampleIndex >= 1024)
        this.m_SampleIndex = 0;
      if ((Object) this.m_GrainLookupRT == (Object) null || !this.m_GrainLookupRT.IsCreated())
      {
        RuntimeUtilities.Destroy((Object) this.m_GrainLookupRT);
        RenderTexture renderTexture = new RenderTexture(128, 128, 0, this.GetLookupFormat());
        renderTexture.filterMode = FilterMode.Bilinear;
        renderTexture.wrapMode = TextureWrapMode.Repeat;
        renderTexture.anisoLevel = 0;
        renderTexture.name = "Grain Lookup Texture";
        this.m_GrainLookupRT = renderTexture;
        this.m_GrainLookupRT.Create();
      }
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.grainBaker);
      propertySheet.properties.Clear();
      propertySheet.properties.SetFloat(ShaderIDs.Phase, realtimeSinceStartup % 10f);
      context.command.BeginSample("GrainLookup");
      context.command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.m_GrainLookupRT, propertySheet, !this.settings.colored.value ? 0 : 1);
      context.command.EndSample("GrainLookup");
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("GRAIN");
      uberSheet.properties.SetTexture(ShaderIDs.GrainTex, (Texture) this.m_GrainLookupRT);
      uberSheet.properties.SetVector(ShaderIDs.Grain_Params1, (Vector4) new Vector2(this.settings.lumContrib.value, this.settings.intensity.value * 20f));
      uberSheet.properties.SetVector(ShaderIDs.Grain_Params2, new Vector4((float) context.width / (float) this.m_GrainLookupRT.width / this.settings.size.value, (float) context.height / (float) this.m_GrainLookupRT.height / this.settings.size.value, z, w));
    }

    private RenderTextureFormat GetLookupFormat() => SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;

    public override void Release()
    {
      RuntimeUtilities.Destroy((Object) this.m_GrainLookupRT);
      this.m_GrainLookupRT = (RenderTexture) null;
      this.m_SampleIndex = 0;
    }
  }
}
