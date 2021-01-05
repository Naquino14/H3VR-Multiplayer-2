// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.BloomRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
  {
    private BloomRenderer.Level[] m_Pyramid;
    private const int k_MaxPyramidSize = 16;

    public override void Init()
    {
      this.m_Pyramid = new BloomRenderer.Level[16];
      for (int index = 0; index < 16; ++index)
        this.m_Pyramid[index] = new BloomRenderer.Level()
        {
          down = Shader.PropertyToID("_BloomMipDown" + (object) index),
          up = Shader.PropertyToID("_BloomMipUp" + (object) index)
        };
    }

    public override void Render(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("BloomPyramid");
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.bloom);
      propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
      float num1 = Mathf.Clamp((float) (ParameterOverride<float>) this.settings.anamorphicRatio, -1f, 1f);
      float num2 = (double) num1 >= 0.0 ? 0.0f : -num1;
      float num3 = (double) num1 <= 0.0 ? 0.0f : num1;
      int width = Mathf.FloorToInt((float) context.width / (2f - num2));
      int num4 = Mathf.FloorToInt((float) context.height / (2f - num3));
      float f = (float) ((double) Mathf.Log((float) Mathf.Max(width / (!RuntimeUtilities.isSinglePassStereoEnabled ? 1 : 2), num4), 2f) + (double) Mathf.Min(this.settings.diffusion.value, 10f) - 10.0);
      int num5 = Mathf.FloorToInt(f);
      int num6 = Mathf.Clamp(num5, 1, 16);
      float x = 0.5f + f - (float) num5;
      propertySheet.properties.SetFloat(ShaderIDs.SampleScale, x);
      float linearSpace = Mathf.GammaToLinearSpace(this.settings.threshold.value);
      float num7 = (float) ((double) linearSpace * (double) this.settings.softKnee.value + 9.99999974737875E-06);
      Vector4 vector4_1 = new Vector4(linearSpace, linearSpace - num7, num7 * 2f, 0.25f / num7);
      propertySheet.properties.SetVector(ShaderIDs.Threshold, vector4_1);
      int num8 = !(bool) (ParameterOverride<bool>) this.settings.mobileOptimized ? 0 : 1;
      RenderTargetIdentifier source1 = context.source;
      for (int index = 0; index < num6; ++index)
      {
        int down = this.m_Pyramid[index].down;
        int up = this.m_Pyramid[index].up;
        int pass = index != 0 ? 2 + num8 : num8;
        command.GetTemporaryRT(down, width, num4, 0, FilterMode.Bilinear, context.sourceFormat);
        command.GetTemporaryRT(up, width, num4, 0, FilterMode.Bilinear, context.sourceFormat);
        command.BlitFullscreenTriangle(source1, (RenderTargetIdentifier) down, propertySheet, pass);
        source1 = (RenderTargetIdentifier) down;
        width = Mathf.Max(width / 2, 1);
        num4 = Mathf.Max(num4 / 2, 1);
      }
      RenderTargetIdentifier source2 = (RenderTargetIdentifier) this.m_Pyramid[num6 - 1].down;
      for (int index = num6 - 2; index >= 0; --index)
      {
        int down = this.m_Pyramid[index].down;
        int up = this.m_Pyramid[index].up;
        command.SetGlobalTexture(ShaderIDs.BloomTex, (RenderTargetIdentifier) down);
        command.BlitFullscreenTriangle(source2, (RenderTargetIdentifier) up, propertySheet, 4 + num8);
        source2 = (RenderTargetIdentifier) up;
      }
      Color linear = this.settings.color.value.linear;
      float num9 = RuntimeUtilities.Exp2(this.settings.intensity.value / 10f) - 1f;
      Vector4 vector4_2 = new Vector4(x, num9, this.settings.dirtIntensity.value, (float) num6);
      if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
        context.PushDebugOverlay(command, context.source, propertySheet, 6);
      else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
      {
        propertySheet.properties.SetVector(ShaderIDs.ColorIntensity, new Vector4(linear.r, linear.g, linear.b, num9));
        context.PushDebugOverlay(command, (RenderTargetIdentifier) this.m_Pyramid[0].up, propertySheet, 7 + num8);
      }
      Texture texture = !((Object) this.settings.dirtTexture.value == (Object) null) ? this.settings.dirtTexture.value : (Texture) RuntimeUtilities.blackTexture;
      float num10 = (float) texture.width / (float) texture.height;
      float num11 = (float) context.width / (float) context.height;
      Vector4 vector4_3 = new Vector4(1f, 1f, 0.0f, 0.0f);
      if ((double) num10 > (double) num11)
      {
        vector4_3.x = num11 / num10;
        vector4_3.z = (float) ((1.0 - (double) vector4_3.x) * 0.5);
      }
      else if ((double) num11 > (double) num10)
      {
        vector4_3.y = num10 / num11;
        vector4_3.w = (float) ((1.0 - (double) vector4_3.y) * 0.5);
      }
      PropertySheet uberSheet = context.uberSheet;
      uberSheet.EnableKeyword("BLOOM");
      uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, vector4_3);
      uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, vector4_2);
      uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linear);
      uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, texture);
      command.SetGlobalTexture(ShaderIDs.BloomTex, (RenderTargetIdentifier) this.m_Pyramid[0].up);
      for (int index = 0; index < num6; ++index)
      {
        command.ReleaseTemporaryRT(this.m_Pyramid[index].down);
        command.ReleaseTemporaryRT(this.m_Pyramid[index].up);
      }
      command.EndSample("BloomPyramid");
    }

    private enum Pass
    {
      Prefilter13,
      Prefilter4,
      Downsample13,
      Downsample4,
      UpsampleTent,
      UpsampleBox,
      DebugOverlayThreshold,
      DebugOverlayTent,
      DebugOverlayBox,
    }

    private struct Level
    {
      internal int down;
      internal int up;
    }
  }
}
