// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.LightMeterMonitor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class LightMeterMonitor : Monitor
  {
    public int width = 512;
    public int height = 256;
    public bool showCurves = true;

    internal override void Render(PostProcessRenderContext context)
    {
      this.CheckOutput(this.width, this.height);
      LogHistogram logHistogram = context.logHistogram;
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lightMeter);
      propertySheet.ClearKeywords();
      propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, logHistogram.data);
      Vector4 histogramScaleOffsetRes = logHistogram.GetHistogramScaleOffsetRes(context);
      histogramScaleOffsetRes.z = 1f / (float) this.width;
      histogramScaleOffsetRes.w = 1f / (float) this.height;
      propertySheet.properties.SetVector(ShaderIDs.ScaleOffsetRes, histogramScaleOffsetRes);
      if ((UnityEngine.Object) context.logLut != (UnityEngine.Object) null && this.showCurves)
      {
        propertySheet.EnableKeyword("COLOR_GRADING_HDR");
        propertySheet.properties.SetTexture(ShaderIDs.Lut3D, context.logLut);
      }
      AutoExposure autoExposure = context.autoExposure;
      if ((UnityEngine.Object) autoExposure != (UnityEngine.Object) null)
      {
        float x = autoExposure.filtering.value.x;
        float num = Mathf.Clamp(autoExposure.filtering.value.y, 1.01f, 99f);
        Vector4 vector4 = new Vector4(Mathf.Clamp(x, 1f, num - 0.01f) * 0.01f, num * 0.01f, RuntimeUtilities.Exp2(autoExposure.minLuminance.value), RuntimeUtilities.Exp2(autoExposure.maxLuminance.value));
        propertySheet.EnableKeyword("AUTO_EXPOSURE");
        propertySheet.properties.SetVector(ShaderIDs.Params, vector4);
      }
      CommandBuffer command = context.command;
      command.BeginSample("LightMeter");
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.output, propertySheet, 0);
      command.EndSample("LightMeter");
    }
  }
}
