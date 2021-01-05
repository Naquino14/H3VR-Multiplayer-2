// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.WaveformMonitor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class WaveformMonitor : Monitor
  {
    public float exposure = 0.12f;
    public int height = 256;
    private ComputeBuffer m_Data;
    private int m_ThreadGroupSize;
    private int m_ThreadGroupSizeX;
    private int m_ThreadGroupSizeY;

    internal override void OnEnable()
    {
      this.m_ThreadGroupSizeX = 16;
      if (RuntimeUtilities.isAndroidOpenGL)
      {
        this.m_ThreadGroupSize = 128;
        this.m_ThreadGroupSizeY = 8;
      }
      else
      {
        this.m_ThreadGroupSize = 256;
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
      int num = Mathf.FloorToInt((float) this.height * (float) ((double) context.width / 2.0 / ((double) context.height / 2.0)));
      this.CheckOutput(num, this.height);
      this.exposure = Mathf.Max(0.0f, this.exposure);
      int count = num * this.height;
      if (this.m_Data == null)
        this.m_Data = new ComputeBuffer(count, 16);
      else if (this.m_Data.count < count)
      {
        this.m_Data.Release();
        this.m_Data = new ComputeBuffer(count, 16);
      }
      ComputeShader waveform = context.resources.computeShaders.waveform;
      CommandBuffer command = context.command;
      command.BeginSample("Waveform");
      Vector4 val = new Vector4((float) num, (float) this.height, !RuntimeUtilities.isLinearColorSpace ? 0.0f : 1f, 0.0f);
      int kernel1 = waveform.FindKernel("KWaveformClear");
      command.SetComputeBufferParam(waveform, kernel1, "_WaveformBuffer", this.m_Data);
      command.SetComputeVectorParam(waveform, "_Params", val);
      command.DispatchCompute(waveform, kernel1, Mathf.CeilToInt((float) num / (float) this.m_ThreadGroupSizeX), Mathf.CeilToInt((float) this.height / (float) this.m_ThreadGroupSizeY), 1);
      command.GetTemporaryRT(ShaderIDs.WaveformSource, num, this.height, 0, FilterMode.Bilinear, context.sourceFormat);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) ShaderIDs.HalfResFinalCopy, (RenderTargetIdentifier) ShaderIDs.WaveformSource);
      int kernel2 = waveform.FindKernel("KWaveformGather");
      command.SetComputeBufferParam(waveform, kernel2, "_WaveformBuffer", this.m_Data);
      command.SetComputeTextureParam(waveform, kernel2, "_Source", (RenderTargetIdentifier) ShaderIDs.WaveformSource);
      command.SetComputeVectorParam(waveform, "_Params", val);
      command.DispatchCompute(waveform, kernel2, num, Mathf.CeilToInt((float) this.height / (float) this.m_ThreadGroupSize), 1);
      command.ReleaseTemporaryRT(ShaderIDs.WaveformSource);
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.waveform);
      propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float) num, (float) this.height, this.exposure, 0.0f));
      propertySheet.properties.SetBuffer(ShaderIDs.WaveformBuffer, this.m_Data);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.output, propertySheet, 0);
      command.EndSample("Waveform");
    }
  }
}
