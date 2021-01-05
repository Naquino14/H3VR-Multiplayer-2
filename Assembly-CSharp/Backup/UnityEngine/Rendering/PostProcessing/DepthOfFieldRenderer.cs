// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.DepthOfFieldRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class DepthOfFieldRenderer : PostProcessEffectRenderer<DepthOfField>
  {
    private readonly RenderTexture[] m_CoCHistoryTextures = new RenderTexture[2];
    private int m_HistoryPingPong;
    private const float k_FilmHeight = 0.024f;

    public override DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth;

    private RenderTextureFormat SelectFormat(
      RenderTextureFormat primary,
      RenderTextureFormat secondary)
    {
      if (SystemInfo.SupportsRenderTextureFormat(primary))
        return primary;
      return SystemInfo.SupportsRenderTextureFormat(secondary) ? secondary : RenderTextureFormat.Default;
    }

    private float CalculateMaxCoCRadius(int screenHeight) => Mathf.Min(0.05f, (float) ((double) this.settings.kernelSize.value * 4.0 + 6.0) / (float) screenHeight);

    private RenderTexture CheckHistory(
      int id,
      int width,
      int height,
      RenderTextureFormat format)
    {
      RenderTexture temp = this.m_CoCHistoryTextures[id];
      if (this.m_ResetHistory || (Object) temp == (Object) null || (!temp.IsCreated() || temp.width != width) || temp.height != height)
      {
        RenderTexture.ReleaseTemporary(temp);
        temp = RenderTexture.GetTemporary(width, height, 0, format);
        temp.name = "CoC History";
        temp.filterMode = FilterMode.Bilinear;
        temp.Create();
        this.m_CoCHistoryTextures[id] = temp;
      }
      return temp;
    }

    public override void Render(PostProcessRenderContext context)
    {
      RenderTextureFormat format1 = RenderTextureFormat.DefaultHDR;
      RenderTextureFormat format2 = this.SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
      float b = this.settings.focalLength.value / 1000f;
      float num1 = Mathf.Max(this.settings.focusDistance.value, b);
      float num2 = (float) context.width / (float) context.height;
      float num3 = (float) ((double) b * (double) b / ((double) this.settings.aperture.value * ((double) num1 - (double) b) * 0.0240000002086163 * 2.0));
      float maxCoCradius = this.CalculateMaxCoCRadius(context.height);
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.depthOfField);
      propertySheet.properties.Clear();
      propertySheet.properties.SetFloat(ShaderIDs.Distance, num1);
      propertySheet.properties.SetFloat(ShaderIDs.LensCoeff, num3);
      propertySheet.properties.SetFloat(ShaderIDs.MaxCoC, maxCoCradius);
      propertySheet.properties.SetFloat(ShaderIDs.RcpMaxCoC, 1f / maxCoCradius);
      propertySheet.properties.SetFloat(ShaderIDs.RcpAspect, 1f / num2);
      CommandBuffer command = context.command;
      command.BeginSample("DepthOfField");
      command.GetTemporaryRT(ShaderIDs.CoCTex, context.width, context.height, 0, FilterMode.Bilinear, format2, RenderTextureReadWrite.Linear);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) ShaderIDs.CoCTex, propertySheet, 0);
      if (context.IsTemporalAntialiasingActive())
      {
        float motionBlending = context.temporalAntialiasing.motionBlending;
        float z = !this.m_ResetHistory ? motionBlending : 0.0f;
        Vector2 jitter = context.temporalAntialiasing.jitter;
        propertySheet.properties.SetVector(ShaderIDs.TaaParams, (Vector4) new Vector3(jitter.x, jitter.y, z));
        int num4;
        RenderTexture renderTexture1 = this.CheckHistory((num4 = this.m_HistoryPingPong + 1) % 2, context.width, context.height, format2);
        int num5;
        RenderTexture renderTexture2 = this.CheckHistory((num5 = num4 + 1) % 2, context.width, context.height, format2);
        int num6;
        this.m_HistoryPingPong = (num6 = num5 + 1) % 2;
        command.BlitFullscreenTriangle((RenderTargetIdentifier) (Texture) renderTexture1, (RenderTargetIdentifier) (Texture) renderTexture2, propertySheet, 1);
        command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
        command.SetGlobalTexture(ShaderIDs.CoCTex, (RenderTargetIdentifier) (Texture) renderTexture2);
      }
      command.GetTemporaryRT(ShaderIDs.DepthOfFieldTex, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, format1);
      command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) ShaderIDs.DepthOfFieldTex, propertySheet, 2);
      command.GetTemporaryRT(ShaderIDs.DepthOfFieldTemp, context.width / 2, context.height / 2, 0, FilterMode.Bilinear, format1);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) ShaderIDs.DepthOfFieldTex, (RenderTargetIdentifier) ShaderIDs.DepthOfFieldTemp, propertySheet, (int) (3 + this.settings.kernelSize.value));
      command.BlitFullscreenTriangle((RenderTargetIdentifier) ShaderIDs.DepthOfFieldTemp, (RenderTargetIdentifier) ShaderIDs.DepthOfFieldTex, propertySheet, 7);
      command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTemp);
      if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
        context.PushDebugOverlay(command, context.source, propertySheet, 9);
      command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 8);
      command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTex);
      if (!context.IsTemporalAntialiasingActive())
        command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
      command.EndSample("DepthOfField");
      this.m_ResetHistory = false;
    }

    public override void Release()
    {
      for (int index = 0; index < this.m_CoCHistoryTextures.Length; ++index)
      {
        RenderTexture.ReleaseTemporary(this.m_CoCHistoryTextures[index]);
        this.m_CoCHistoryTextures[index] = (RenderTexture) null;
      }
      this.m_HistoryPingPong = 0;
      this.ResetHistory();
    }

    private enum Pass
    {
      CoCCalculation,
      CoCTemporalFilter,
      DownsampleAndPrefilter,
      BokehSmallKernel,
      BokehMediumKernel,
      BokehLargeKernel,
      BokehVeryLargeKernel,
      PostFilter,
      Combine,
      DebugOverlay,
    }
  }
}
