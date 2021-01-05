// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.LogHistogram
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class LogHistogram
  {
    public const int rangeMin = -9;
    public const int rangeMax = 9;
    private const int k_Bins = 128;
    private int m_ThreadX;
    private int m_ThreadY;

    public ComputeBuffer data { get; private set; }

    public void Generate(PostProcessRenderContext context)
    {
      if (this.data == null)
      {
        this.m_ThreadX = 16;
        this.m_ThreadY = !RuntimeUtilities.isAndroidOpenGL ? 16 : 8;
        this.data = new ComputeBuffer(128, 4);
      }
      Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes(context);
      ComputeShader exposureHistogram = context.resources.computeShaders.exposureHistogram;
      CommandBuffer command = context.command;
      command.BeginSample(nameof (LogHistogram));
      int kernel1 = exposureHistogram.FindKernel("KEyeHistogramClear");
      command.SetComputeBufferParam(exposureHistogram, kernel1, "_HistogramBuffer", this.data);
      command.DispatchCompute(exposureHistogram, kernel1, Mathf.CeilToInt(128f / (float) this.m_ThreadX), 1, 1);
      int kernel2 = exposureHistogram.FindKernel("KEyeHistogram");
      command.SetComputeBufferParam(exposureHistogram, kernel2, "_HistogramBuffer", this.data);
      command.SetComputeTextureParam(exposureHistogram, kernel2, "_Source", context.source);
      command.SetComputeVectorParam(exposureHistogram, "_ScaleOffsetRes", histogramScaleOffsetRes);
      command.DispatchCompute(exposureHistogram, kernel2, Mathf.CeilToInt(histogramScaleOffsetRes.z / (float) this.m_ThreadX), Mathf.CeilToInt(histogramScaleOffsetRes.w / (float) this.m_ThreadY), 1);
      command.EndSample(nameof (LogHistogram));
    }

    public Vector4 GetHistogramScaleOffsetRes(PostProcessRenderContext context)
    {
      float x = 1f / 18f;
      float y = 9f * x;
      return new Vector4(x, y, (float) context.width, (float) context.height);
    }

    public void Release()
    {
      if (this.data != null)
        this.data.Release();
      this.data = (ComputeBuffer) null;
    }
  }
}
