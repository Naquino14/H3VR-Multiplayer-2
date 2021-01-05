// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.HistogramMonitor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class HistogramMonitor : Monitor
  {
    public int width = 512;
    public int height = 256;
    public HistogramMonitor.Channel channel = HistogramMonitor.Channel.Master;
    private ComputeBuffer m_Data;
    private int m_NumBins;
    private int m_ThreadGroupSizeX;
    private int m_ThreadGroupSizeY;

    internal override void OnEnable()
    {
      this.m_ThreadGroupSizeX = 16;
      if (RuntimeUtilities.isAndroidOpenGL)
      {
        this.m_NumBins = 128;
        this.m_ThreadGroupSizeY = 8;
      }
      else
      {
        this.m_NumBins = 256;
        this.m_ThreadGroupSizeY = 16;
      }
    }

    internal override void OnDisable()
    {
      base.OnDisable();
      if (this.m_Data != null)
        this.m_Data.Release();
      this.m_Data = (ComputeBuffer) null;
    }

    internal override bool NeedsHalfRes() => true;

    internal override void Render(PostProcessRenderContext context)
    {
      this.CheckOutput(this.width, this.height);
      if (this.m_Data == null)
        this.m_Data = new ComputeBuffer(this.m_NumBins, 4);
      ComputeShader gammaHistogram = context.resources.computeShaders.gammaHistogram;
      CommandBuffer command = context.command;
      command.BeginSample("GammaHistogram");
      int kernel1 = gammaHistogram.FindKernel("KHistogramClear");
      command.SetComputeBufferParam(gammaHistogram, kernel1, "_HistogramBuffer", this.m_Data);
      command.DispatchCompute(gammaHistogram, kernel1, Mathf.CeilToInt((float) this.m_NumBins / (float) this.m_ThreadGroupSizeX), 1, 1);
      int kernel2 = gammaHistogram.FindKernel("KHistogramGather");
      Vector4 val = new Vector4((float) (context.width / 2), (float) (context.height / 2), !RuntimeUtilities.isLinearColorSpace ? 0.0f : 1f, (float) this.channel);
      command.SetComputeVectorParam(gammaHistogram, "_Params", val);
      command.SetComputeTextureParam(gammaHistogram, kernel2, "_Source", (RenderTargetIdentifier) ShaderIDs.HalfResFinalCopy);
      command.SetComputeBufferParam(gammaHistogram, kernel2, "_HistogramBuffer", this.m_Data);
      command.DispatchCompute(gammaHistogram, kernel2, Mathf.CeilToInt(val.x / (float) this.m_ThreadGroupSizeX), Mathf.CeilToInt(val.y / (float) this.m_ThreadGroupSizeY), 1);
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.gammaHistogram);
      propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float) this.width, (float) this.height, 0.0f, 0.0f));
      propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, this.m_Data);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.output, propertySheet, 0);
      command.EndSample("GammaHistogram");
    }

    public enum Channel
    {
      Red,
      Green,
      Blue,
      Master,
    }
  }
}
