// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.VectorscopeMonitor
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class VectorscopeMonitor : Monitor
  {
    public int size = 256;
    public float exposure = 0.12f;
    private ComputeBuffer m_Data;
    private int m_ThreadGroupSizeX;
    private int m_ThreadGroupSizeY;

    internal override void OnEnable()
    {
      this.m_ThreadGroupSizeX = 16;
      this.m_ThreadGroupSizeY = !RuntimeUtilities.isAndroidOpenGL ? 16 : 8;
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
      this.CheckOutput(this.size, this.size);
      this.exposure = Mathf.Max(0.0f, this.exposure);
      int count = this.size * this.size;
      if (this.m_Data == null)
        this.m_Data = new ComputeBuffer(count, 4);
      else if (this.m_Data.count != count)
      {
        this.m_Data.Release();
        this.m_Data = new ComputeBuffer(count, 4);
      }
      ComputeShader vectorscope = context.resources.computeShaders.vectorscope;
      CommandBuffer command = context.command;
      command.BeginSample("Vectorscope");
      Vector4 val = new Vector4((float) (context.width / 2), (float) (context.height / 2), (float) this.size, !RuntimeUtilities.isLinearColorSpace ? 0.0f : 1f);
      int kernel1 = vectorscope.FindKernel("KVectorscopeClear");
      command.SetComputeBufferParam(vectorscope, kernel1, "_VectorscopeBuffer", this.m_Data);
      command.SetComputeVectorParam(vectorscope, "_Params", val);
      command.DispatchCompute(vectorscope, kernel1, Mathf.CeilToInt((float) this.size / (float) this.m_ThreadGroupSizeX), Mathf.CeilToInt((float) this.size / (float) this.m_ThreadGroupSizeY), 1);
      int kernel2 = vectorscope.FindKernel("KVectorscopeGather");
      command.SetComputeBufferParam(vectorscope, kernel2, "_VectorscopeBuffer", this.m_Data);
      command.SetComputeTextureParam(vectorscope, kernel2, "_Source", (RenderTargetIdentifier) ShaderIDs.HalfResFinalCopy);
      command.DispatchCompute(vectorscope, kernel2, Mathf.CeilToInt(val.x / (float) this.m_ThreadGroupSizeX), Mathf.CeilToInt(val.y / (float) this.m_ThreadGroupSizeY), 1);
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.vectorscope);
      propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float) this.size, (float) this.size, this.exposure, 0.0f));
      propertySheet.properties.SetBuffer(ShaderIDs.VectorscopeBuffer, this.m_Data);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) (Texture) this.output, propertySheet, 0);
      command.EndSample("Vectorscope");
    }
  }
}
