// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.MultiScaleVO
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class MultiScaleVO : IAmbientOcclusionMethod
  {
    private AmbientOcclusion m_Settings;
    private PropertySheet m_PropertySheet;
    private MultiScaleVO.RTHandle m_DepthCopy;
    private MultiScaleVO.RTHandle m_LinearDepth;
    private MultiScaleVO.RTHandle m_LowDepth1;
    private MultiScaleVO.RTHandle m_LowDepth2;
    private MultiScaleVO.RTHandle m_LowDepth3;
    private MultiScaleVO.RTHandle m_LowDepth4;
    private MultiScaleVO.RTHandle m_TiledDepth1;
    private MultiScaleVO.RTHandle m_TiledDepth2;
    private MultiScaleVO.RTHandle m_TiledDepth3;
    private MultiScaleVO.RTHandle m_TiledDepth4;
    private MultiScaleVO.RTHandle m_Occlusion1;
    private MultiScaleVO.RTHandle m_Occlusion2;
    private MultiScaleVO.RTHandle m_Occlusion3;
    private MultiScaleVO.RTHandle m_Occlusion4;
    private MultiScaleVO.RTHandle m_Combined1;
    private MultiScaleVO.RTHandle m_Combined2;
    private MultiScaleVO.RTHandle m_Combined3;
    private MultiScaleVO.RTHandle m_Result;
    private readonly float[] m_SampleThickness = new float[12]
    {
      Mathf.Sqrt(0.96f),
      Mathf.Sqrt(0.84f),
      Mathf.Sqrt(0.64f),
      Mathf.Sqrt(0.36f),
      Mathf.Sqrt(0.92f),
      Mathf.Sqrt(0.8f),
      Mathf.Sqrt(0.6f),
      Mathf.Sqrt(0.32f),
      Mathf.Sqrt(0.68f),
      Mathf.Sqrt(0.48f),
      Mathf.Sqrt(0.2f),
      Mathf.Sqrt(0.2799999f)
    };
    private readonly float[] m_InvThicknessTable = new float[12];
    private readonly float[] m_SampleWeightTable = new float[12];
    private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[2]
    {
      (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0,
      (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget
    };

    public MultiScaleVO(AmbientOcclusion settings) => this.m_Settings = settings;

    public DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth;

    private void DoLazyInitialization(PostProcessRenderContext context)
    {
      Shader multiScaleAo = context.resources.shaders.multiScaleAO;
      this.m_PropertySheet = context.propertySheets.Get(multiScaleAo);
      this.m_PropertySheet.ClearKeywords();
      if (this.m_Result != null)
        return;
      this.m_DepthCopy = new MultiScaleVO.RTHandle("DepthCopy", MultiScaleVO.TextureType.Float, MultiScaleVO.MipLevel.Original);
      this.m_LinearDepth = new MultiScaleVO.RTHandle("LinearDepth", MultiScaleVO.TextureType.HalfUAV, MultiScaleVO.MipLevel.Original);
      this.m_LowDepth1 = new MultiScaleVO.RTHandle("LowDepth1", MultiScaleVO.TextureType.FloatUAV, MultiScaleVO.MipLevel.L1);
      this.m_LowDepth2 = new MultiScaleVO.RTHandle("LowDepth2", MultiScaleVO.TextureType.FloatUAV, MultiScaleVO.MipLevel.L2);
      this.m_LowDepth3 = new MultiScaleVO.RTHandle("LowDepth3", MultiScaleVO.TextureType.FloatUAV, MultiScaleVO.MipLevel.L3);
      this.m_LowDepth4 = new MultiScaleVO.RTHandle("LowDepth4", MultiScaleVO.TextureType.FloatUAV, MultiScaleVO.MipLevel.L4);
      this.m_TiledDepth1 = new MultiScaleVO.RTHandle("TiledDepth1", MultiScaleVO.TextureType.HalfTiledUAV, MultiScaleVO.MipLevel.L3);
      this.m_TiledDepth2 = new MultiScaleVO.RTHandle("TiledDepth2", MultiScaleVO.TextureType.HalfTiledUAV, MultiScaleVO.MipLevel.L4);
      this.m_TiledDepth3 = new MultiScaleVO.RTHandle("TiledDepth3", MultiScaleVO.TextureType.HalfTiledUAV, MultiScaleVO.MipLevel.L5);
      this.m_TiledDepth4 = new MultiScaleVO.RTHandle("TiledDepth4", MultiScaleVO.TextureType.HalfTiledUAV, MultiScaleVO.MipLevel.L6);
      this.m_Occlusion1 = new MultiScaleVO.RTHandle("Occlusion1", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L1);
      this.m_Occlusion2 = new MultiScaleVO.RTHandle("Occlusion2", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L2);
      this.m_Occlusion3 = new MultiScaleVO.RTHandle("Occlusion3", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L3);
      this.m_Occlusion4 = new MultiScaleVO.RTHandle("Occlusion4", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L4);
      this.m_Combined1 = new MultiScaleVO.RTHandle("Combined1", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L1);
      this.m_Combined2 = new MultiScaleVO.RTHandle("Combined2", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L2);
      this.m_Combined3 = new MultiScaleVO.RTHandle("Combined3", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.L3);
      this.m_Result = new MultiScaleVO.RTHandle("AmbientOcclusion", MultiScaleVO.TextureType.FixedUAV, MultiScaleVO.MipLevel.Original);
    }

    private void RebuildCommandBuffers(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      MultiScaleVO.RTHandle.SetBaseDimensions(context.width, context.height);
      this.m_PropertySheet.properties.SetVector(ShaderIDs.AOColor, (Vector4) (Color.white - this.m_Settings.color.value));
      this.m_TiledDepth1.AllocateNow();
      this.m_TiledDepth2.AllocateNow();
      this.m_TiledDepth3.AllocateNow();
      this.m_TiledDepth4.AllocateNow();
      this.m_Result.AllocateNow();
      this.PushDownsampleCommands(context, command);
      this.m_Occlusion1.PushAllocationCommand(command);
      this.m_Occlusion2.PushAllocationCommand(command);
      this.m_Occlusion3.PushAllocationCommand(command);
      this.m_Occlusion4.PushAllocationCommand(command);
      float tanHalfFovHeight = this.CalculateTanHalfFovHeight(context);
      this.PushRenderCommands(context, command, this.m_TiledDepth1, this.m_Occlusion1, tanHalfFovHeight);
      this.PushRenderCommands(context, command, this.m_TiledDepth2, this.m_Occlusion2, tanHalfFovHeight);
      this.PushRenderCommands(context, command, this.m_TiledDepth3, this.m_Occlusion3, tanHalfFovHeight);
      this.PushRenderCommands(context, command, this.m_TiledDepth4, this.m_Occlusion4, tanHalfFovHeight);
      this.m_Combined1.PushAllocationCommand(command);
      this.m_Combined2.PushAllocationCommand(command);
      this.m_Combined3.PushAllocationCommand(command);
      this.PushUpsampleCommands(context, command, this.m_LowDepth4, this.m_Occlusion4, this.m_LowDepth3, this.m_Occlusion3, this.m_Combined3);
      this.PushUpsampleCommands(context, command, this.m_LowDepth3, this.m_Combined3, this.m_LowDepth2, this.m_Occlusion2, this.m_Combined2);
      this.PushUpsampleCommands(context, command, this.m_LowDepth2, this.m_Combined2, this.m_LowDepth1, this.m_Occlusion1, this.m_Combined1);
      this.PushUpsampleCommands(context, command, this.m_LowDepth1, this.m_Combined1, this.m_LinearDepth, (MultiScaleVO.RTHandle) null, this.m_Result);
      if (!context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
        return;
      context.PushDebugOverlay(command, this.m_Result.id, this.m_PropertySheet, 3);
    }

    private Vector4 CalculateZBufferParams(Camera camera)
    {
      float y = camera.farClipPlane / camera.nearClipPlane;
      return SystemInfo.usesReversedZBuffer ? new Vector4(y - 1f, 1f, 0.0f, 0.0f) : new Vector4(1f - y, y, 0.0f, 0.0f);
    }

    private float CalculateTanHalfFovHeight(PostProcessRenderContext context) => 1f / context.camera.projectionMatrix[0, 0];

    private void PushDownsampleCommands(PostProcessRenderContext context, CommandBuffer cmd)
    {
      bool flag = !RuntimeUtilities.IsResolvedDepthAvailable(context.camera);
      if (flag)
      {
        this.m_DepthCopy.PushAllocationCommand(cmd);
        cmd.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, this.m_DepthCopy.id, this.m_PropertySheet, 0);
      }
      this.m_LinearDepth.PushAllocationCommand(cmd);
      this.m_LowDepth1.PushAllocationCommand(cmd);
      this.m_LowDepth2.PushAllocationCommand(cmd);
      this.m_LowDepth3.PushAllocationCommand(cmd);
      this.m_LowDepth4.PushAllocationCommand(cmd);
      ComputeShader scaleAoDownsample1 = context.resources.computeShaders.multiScaleAODownsample1;
      int kernel1 = scaleAoDownsample1.FindKernel("main");
      cmd.SetComputeTextureParam(scaleAoDownsample1, kernel1, "LinearZ", this.m_LinearDepth.id);
      cmd.SetComputeTextureParam(scaleAoDownsample1, kernel1, "DS2x", this.m_LowDepth1.id);
      cmd.SetComputeTextureParam(scaleAoDownsample1, kernel1, "DS4x", this.m_LowDepth2.id);
      cmd.SetComputeTextureParam(scaleAoDownsample1, kernel1, "DS2xAtlas", this.m_TiledDepth1.id);
      cmd.SetComputeTextureParam(scaleAoDownsample1, kernel1, "DS4xAtlas", this.m_TiledDepth2.id);
      cmd.SetComputeVectorParam(scaleAoDownsample1, "ZBufferParams", this.CalculateZBufferParams(context.camera));
      cmd.SetComputeTextureParam(scaleAoDownsample1, kernel1, "Depth", !flag ? (RenderTargetIdentifier) BuiltinRenderTextureType.ResolvedDepth : this.m_DepthCopy.id);
      cmd.DispatchCompute(scaleAoDownsample1, kernel1, this.m_TiledDepth2.width, this.m_TiledDepth2.height, 1);
      if (flag)
        cmd.ReleaseTemporaryRT(this.m_DepthCopy.nameID);
      ComputeShader scaleAoDownsample2 = context.resources.computeShaders.multiScaleAODownsample2;
      int kernel2 = scaleAoDownsample2.FindKernel("main");
      cmd.SetComputeTextureParam(scaleAoDownsample2, kernel2, "DS4x", this.m_LowDepth2.id);
      cmd.SetComputeTextureParam(scaleAoDownsample2, kernel2, "DS8x", this.m_LowDepth3.id);
      cmd.SetComputeTextureParam(scaleAoDownsample2, kernel2, "DS16x", this.m_LowDepth4.id);
      cmd.SetComputeTextureParam(scaleAoDownsample2, kernel2, "DS8xAtlas", this.m_TiledDepth3.id);
      cmd.SetComputeTextureParam(scaleAoDownsample2, kernel2, "DS16xAtlas", this.m_TiledDepth4.id);
      cmd.DispatchCompute(scaleAoDownsample2, kernel2, this.m_TiledDepth4.width, this.m_TiledDepth4.height, 1);
    }

    private void PushRenderCommands(
      PostProcessRenderContext context,
      CommandBuffer cmd,
      MultiScaleVO.RTHandle source,
      MultiScaleVO.RTHandle dest,
      float tanHalfFovH)
    {
      float num1 = (float) (2.0 * (double) tanHalfFovH * 10.0) / (float) source.width;
      if (!source.isTiled)
        num1 *= 2f;
      if (RuntimeUtilities.isSinglePassStereoEnabled)
        num1 *= 2f;
      float num2 = 1f / num1;
      for (int index = 0; index < 12; ++index)
        this.m_InvThicknessTable[index] = num2 / this.m_SampleThickness[index];
      this.m_SampleWeightTable[0] = 4f * this.m_SampleThickness[0];
      this.m_SampleWeightTable[1] = 4f * this.m_SampleThickness[1];
      this.m_SampleWeightTable[2] = 4f * this.m_SampleThickness[2];
      this.m_SampleWeightTable[3] = 4f * this.m_SampleThickness[3];
      this.m_SampleWeightTable[4] = 4f * this.m_SampleThickness[4];
      this.m_SampleWeightTable[5] = 8f * this.m_SampleThickness[5];
      this.m_SampleWeightTable[6] = 8f * this.m_SampleThickness[6];
      this.m_SampleWeightTable[7] = 8f * this.m_SampleThickness[7];
      this.m_SampleWeightTable[8] = 4f * this.m_SampleThickness[8];
      this.m_SampleWeightTable[9] = 8f * this.m_SampleThickness[9];
      this.m_SampleWeightTable[10] = 8f * this.m_SampleThickness[10];
      this.m_SampleWeightTable[11] = 4f * this.m_SampleThickness[11];
      this.m_SampleWeightTable[0] = 0.0f;
      this.m_SampleWeightTable[2] = 0.0f;
      this.m_SampleWeightTable[5] = 0.0f;
      this.m_SampleWeightTable[7] = 0.0f;
      this.m_SampleWeightTable[9] = 0.0f;
      float num3 = 0.0f;
      foreach (float num4 in this.m_SampleWeightTable)
        num3 += num4;
      for (int index = 0; index < this.m_SampleWeightTable.Length; ++index)
        this.m_SampleWeightTable[index] /= num3;
      ComputeShader multiScaleAoRender = context.resources.computeShaders.multiScaleAORender;
      int kernel = multiScaleAoRender.FindKernel("main_interleaved");
      cmd.SetComputeFloatParams(multiScaleAoRender, "gInvThicknessTable", this.m_InvThicknessTable);
      cmd.SetComputeFloatParams(multiScaleAoRender, "gSampleWeightTable", this.m_SampleWeightTable);
      cmd.SetComputeVectorParam(multiScaleAoRender, "gInvSliceDimension", (Vector4) source.inverseDimensions);
      cmd.SetComputeVectorParam(multiScaleAoRender, "AdditionalParams", (Vector4) new Vector2(-1f / this.m_Settings.thicknessModifier.value, this.m_Settings.intensity.value));
      cmd.SetComputeTextureParam(multiScaleAoRender, kernel, "DepthTex", source.id);
      cmd.SetComputeTextureParam(multiScaleAoRender, kernel, "Occlusion", dest.id);
      uint x;
      uint y;
      uint z;
      multiScaleAoRender.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
      cmd.DispatchCompute(multiScaleAoRender, kernel, (source.width + (int) x - 1) / (int) x, (source.height + (int) y - 1) / (int) y, (source.depth + (int) z - 1) / (int) z);
    }

    private void PushUpsampleCommands(
      PostProcessRenderContext context,
      CommandBuffer cmd,
      MultiScaleVO.RTHandle lowResDepth,
      MultiScaleVO.RTHandle interleavedAO,
      MultiScaleVO.RTHandle highResDepth,
      MultiScaleVO.RTHandle highResAO,
      MultiScaleVO.RTHandle dest)
    {
      ComputeShader multiScaleAoUpsample = context.resources.computeShaders.multiScaleAOUpsample;
      int kernel = multiScaleAoUpsample.FindKernel(highResAO != null ? "main_blendout" : "main");
      float y = 1920f / (float) lowResDepth.width;
      float num = (float) (1.0 - (double) Mathf.Pow(10f, this.m_Settings.blurTolerance.value) * (double) y);
      float z = num * num;
      float w = Mathf.Pow(10f, this.m_Settings.upsampleTolerance.value);
      float x = (float) (1.0 / ((double) Mathf.Pow(10f, this.m_Settings.noiseFilterTolerance.value) + (double) w));
      cmd.SetComputeVectorParam(multiScaleAoUpsample, "InvLowResolution", (Vector4) lowResDepth.inverseDimensions);
      cmd.SetComputeVectorParam(multiScaleAoUpsample, "InvHighResolution", (Vector4) highResDepth.inverseDimensions);
      cmd.SetComputeVectorParam(multiScaleAoUpsample, "AdditionalParams", new Vector4(x, y, z, w));
      cmd.SetComputeTextureParam(multiScaleAoUpsample, kernel, "LoResDB", lowResDepth.id);
      cmd.SetComputeTextureParam(multiScaleAoUpsample, kernel, "HiResDB", highResDepth.id);
      cmd.SetComputeTextureParam(multiScaleAoUpsample, kernel, "LoResAO1", interleavedAO.id);
      if (highResAO != null)
        cmd.SetComputeTextureParam(multiScaleAoUpsample, kernel, "HiResAO", highResAO.id);
      cmd.SetComputeTextureParam(multiScaleAoUpsample, kernel, "AoResult", dest.id);
      int threadGroupsX = (highResDepth.width + 17) / 16;
      int threadGroupsY = (highResDepth.height + 17) / 16;
      cmd.DispatchCompute(multiScaleAoUpsample, kernel, threadGroupsX, threadGroupsY, 1);
    }

    public void RenderAfterOpaque(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Ambient Occlusion");
      this.DoLazyInitialization(context);
      PropertySheet propertySheet = this.m_PropertySheet;
      if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
      {
        propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
        propertySheet.properties.SetVector(ShaderIDs.FogParams, (Vector4) new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
      }
      this.RebuildCommandBuffers(context);
      command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, this.m_Result.id);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 2);
      command.EndSample("Ambient Occlusion");
    }

    public void RenderAmbientOnly(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Ambient Occlusion Render");
      this.DoLazyInitialization(context);
      this.RebuildCommandBuffers(context);
      command.EndSample("Ambient Occlusion Render");
    }

    public void CompositeAmbientOnly(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Ambient Occlusion Composite");
      command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, this.m_Result.id);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, this.m_MRT, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 1);
      command.EndSample("Ambient Occlusion Composite");
    }

    public void Release()
    {
      if (this.m_Result != null)
      {
        this.m_TiledDepth1.Destroy();
        this.m_TiledDepth2.Destroy();
        this.m_TiledDepth3.Destroy();
        this.m_TiledDepth4.Destroy();
        this.m_Result.Destroy();
      }
      this.m_TiledDepth1 = (MultiScaleVO.RTHandle) null;
      this.m_TiledDepth2 = (MultiScaleVO.RTHandle) null;
      this.m_TiledDepth3 = (MultiScaleVO.RTHandle) null;
      this.m_TiledDepth4 = (MultiScaleVO.RTHandle) null;
      this.m_Result = (MultiScaleVO.RTHandle) null;
    }

    internal enum MipLevel
    {
      Original,
      L1,
      L2,
      L3,
      L4,
      L5,
      L6,
    }

    internal enum TextureType
    {
      Fixed,
      Half,
      Float,
      FixedUAV,
      HalfUAV,
      FloatUAV,
      FixedTiledUAV,
      HalfTiledUAV,
      FloatTiledUAV,
    }

    private enum Pass
    {
      DepthCopy,
      CompositionDeferred,
      CompositionForward,
      DebugOverlay,
    }

    internal class RTHandle
    {
      private static int s_BaseWidth;
      private static int s_BaseHeight;
      private RenderTexture m_RT;
      private MultiScaleVO.TextureType m_Type;
      private MultiScaleVO.MipLevel m_Level;

      public RTHandle(string name, MultiScaleVO.TextureType type, MultiScaleVO.MipLevel level)
      {
        this.nameID = Shader.PropertyToID(name);
        this.m_Type = type;
        this.m_Level = level;
      }

      public static void SetBaseDimensions(int w, int h)
      {
        MultiScaleVO.RTHandle.s_BaseWidth = w;
        MultiScaleVO.RTHandle.s_BaseHeight = h;
      }

      public int nameID { get; private set; }

      public int width { get; private set; }

      public int height { get; private set; }

      public int depth => this.isTiled ? 16 : 1;

      public bool isTiled => this.m_Type > MultiScaleVO.TextureType.FloatUAV;

      public bool hasUAV => this.m_Type > MultiScaleVO.TextureType.Float;

      public RenderTargetIdentifier id => (UnityEngine.Object) this.m_RT != (UnityEngine.Object) null ? new RenderTargetIdentifier((Texture) this.m_RT) : new RenderTargetIdentifier(this.nameID);

      public Vector2 inverseDimensions => new Vector2(1f / (float) this.width, 1f / (float) this.height);

      public void AllocateNow()
      {
        this.CalculateDimensions();
        bool flag = false;
        if ((UnityEngine.Object) this.m_RT == (UnityEngine.Object) null || !this.m_RT.IsCreated())
        {
          RenderTexture renderTexture = new RenderTexture(this.width, this.height, 0, this.renderTextureFormat, RenderTextureReadWrite.Linear);
          renderTexture.hideFlags = HideFlags.DontSave;
          this.m_RT = renderTexture;
          flag = true;
        }
        else if (this.m_RT.width != this.width || this.m_RT.height != this.height)
        {
          this.m_RT.Release();
          this.m_RT.width = this.width;
          this.m_RT.height = this.height;
          this.m_RT.format = this.renderTextureFormat;
          flag = true;
        }
        if (!flag)
          return;
        this.m_RT.filterMode = FilterMode.Point;
        this.m_RT.enableRandomWrite = this.hasUAV;
        if (this.isTiled)
        {
          this.m_RT.dimension = TextureDimension.Tex2DArray;
          this.m_RT.volumeDepth = this.depth;
        }
        this.m_RT.Create();
      }

      public void PushAllocationCommand(CommandBuffer cmd)
      {
        this.CalculateDimensions();
        cmd.GetTemporaryRT(this.nameID, this.width, this.height, 0, FilterMode.Point, this.renderTextureFormat, RenderTextureReadWrite.Linear, 1, this.hasUAV);
      }

      public void Destroy()
      {
        RuntimeUtilities.Destroy((UnityEngine.Object) this.m_RT);
        this.m_RT = (RenderTexture) null;
      }

      private RenderTextureFormat renderTextureFormat
      {
        get
        {
          switch ((int) this.m_Type % 3)
          {
            case 0:
              return RenderTextureFormat.R8;
            case 1:
              return RenderTextureFormat.RHalf;
            default:
              return RenderTextureFormat.RFloat;
          }
        }
      }

      private void CalculateDimensions()
      {
        int num = 1 << (int) (this.m_Level & (MultiScaleVO.MipLevel) 31);
        this.width = (MultiScaleVO.RTHandle.s_BaseWidth + (num - 1)) / num;
        this.height = (MultiScaleVO.RTHandle.s_BaseHeight + (num - 1)) / num;
      }
    }
  }
}
