// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.AutoExposureRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class AutoExposureRenderer : PostProcessEffectRenderer<AutoExposure>
  {
    private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];
    private int m_AutoExposurePingPong;
    private RenderTexture m_CurrentAutoExposure;

    private void CheckTexture(int id)
    {
      if (!((Object) this.m_AutoExposurePool[id] == (Object) null) && this.m_AutoExposurePool[id].IsCreated())
        return;
      this.m_AutoExposurePool[id] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
      this.m_AutoExposurePool[id].Create();
    }

    public override void Render(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("AutoExposureLookup");
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.autoExposure);
      propertySheet.ClearKeywords();
      this.CheckTexture(0);
      this.CheckTexture(1);
      float x = this.settings.filtering.value.x;
      float num1 = Mathf.Clamp(this.settings.filtering.value.y, 1.01f, 99f);
      float num2 = Mathf.Clamp(x, 1f, num1 - 0.01f);
      float a = this.settings.minLuminance.value;
      float b = this.settings.maxLuminance.value;
      this.settings.minLuminance.value = Mathf.Min(a, b);
      this.settings.maxLuminance.value = Mathf.Max(a, b);
      propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, context.logHistogram.data);
      propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(num2 * 0.01f, num1 * 0.01f, RuntimeUtilities.Exp2(this.settings.minLuminance.value), RuntimeUtilities.Exp2(this.settings.maxLuminance.value)));
      propertySheet.properties.SetVector(ShaderIDs.Speed, (Vector4) new Vector2(this.settings.speedDown.value, this.settings.speedUp.value));
      propertySheet.properties.SetVector(ShaderIDs.ScaleOffsetRes, context.logHistogram.GetHistogramScaleOffsetRes(context));
      propertySheet.properties.SetFloat(ShaderIDs.ExposureCompensation, this.settings.keyValue.value);
      if (this.m_ResetHistory || !Application.isPlaying)
      {
        this.m_CurrentAutoExposure = this.m_AutoExposurePool[0];
        command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.m_CurrentAutoExposure, propertySheet, 1);
        RuntimeUtilities.CopyTexture(command, (RenderTargetIdentifier) (Texture) this.m_AutoExposurePool[0], (RenderTargetIdentifier) (Texture) this.m_AutoExposurePool[1]);
        this.m_ResetHistory = false;
      }
      else
      {
        int exposurePingPong = this.m_AutoExposurePingPong;
        int num3;
        RenderTexture renderTexture1 = this.m_AutoExposurePool[(num3 = exposurePingPong + 1) % 2];
        int num4;
        RenderTexture renderTexture2 = this.m_AutoExposurePool[(num4 = num3 + 1) % 2];
        command.BlitFullscreenTriangle((RenderTargetIdentifier) (Texture) renderTexture1, (RenderTargetIdentifier) (Texture) renderTexture2, propertySheet, (int) this.settings.eyeAdaptation.value);
        int num5;
        this.m_AutoExposurePingPong = (num5 = num4 + 1) % 2;
        this.m_CurrentAutoExposure = renderTexture2;
      }
      command.EndSample("AutoExposureLookup");
      context.autoExposureTexture = (Texture) this.m_CurrentAutoExposure;
      context.autoExposure = this.settings;
    }

    public override void Release()
    {
      foreach (Object @object in this.m_AutoExposurePool)
        RuntimeUtilities.Destroy(@object);
    }
  }
}
