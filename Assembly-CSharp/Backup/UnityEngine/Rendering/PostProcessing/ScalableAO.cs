// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ScalableAO
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class ScalableAO : IAmbientOcclusionMethod
  {
    private RenderTexture m_Result;
    private PropertySheet m_PropertySheet;
    private AmbientOcclusion m_Settings;
    private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[2]
    {
      (RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer0,
      (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget
    };
    private readonly int[] m_SampleCount = new int[5]
    {
      4,
      6,
      10,
      8,
      12
    };

    public ScalableAO(AmbientOcclusion settings) => this.m_Settings = settings;

    public DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth | DepthTextureMode.DepthNormals;

    private void DoLazyInitialization(PostProcessRenderContext context)
    {
      this.m_PropertySheet = context.propertySheets.Get(context.resources.shaders.scalableAO);
      bool flag = false;
      if ((UnityEngine.Object) this.m_Result == (UnityEngine.Object) null || !this.m_Result.IsCreated())
      {
        RenderTexture renderTexture = new RenderTexture(context.width, context.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        renderTexture.hideFlags = HideFlags.DontSave;
        renderTexture.filterMode = FilterMode.Bilinear;
        this.m_Result = renderTexture;
        flag = true;
      }
      else if (this.m_Result.width != context.width || this.m_Result.height != context.height)
      {
        this.m_Result.Release();
        this.m_Result.width = context.width;
        this.m_Result.height = context.height;
        flag = true;
      }
      if (!flag)
        return;
      this.m_Result.Create();
    }

    private void Render(PostProcessRenderContext context, CommandBuffer cmd, int occlusionSource)
    {
      this.DoLazyInitialization(context);
      this.m_Settings.radius.value = Mathf.Max(this.m_Settings.radius.value, 0.0001f);
      bool flag = this.m_Settings.quality.value < AmbientOcclusionQuality.High;
      float x = this.m_Settings.intensity.value;
      float y = this.m_Settings.radius.value;
      float z = !flag ? 1f : 0.5f;
      float w = (float) this.m_SampleCount[(int) this.m_Settings.quality.value];
      PropertySheet propertySheet = this.m_PropertySheet;
      propertySheet.ClearKeywords();
      propertySheet.properties.SetVector(ShaderIDs.AOParams, new Vector4(x, y, z, w));
      propertySheet.properties.SetVector(ShaderIDs.AOColor, (Vector4) (Color.white - this.m_Settings.color.value));
      if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
      {
        propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
        propertySheet.properties.SetVector(ShaderIDs.FogParams, (Vector4) new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
      }
      int width = context.width;
      int height = context.height;
      int num = !flag ? 1 : 2;
      int occlusionTexture1 = ShaderIDs.OcclusionTexture1;
      cmd.GetTemporaryRT(occlusionTexture1, width / num, height / num, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      cmd.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) occlusionTexture1, propertySheet, occlusionSource);
      int occlusionTexture2 = ShaderIDs.OcclusionTexture2;
      cmd.GetTemporaryRT(occlusionTexture2, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
      cmd.BlitFullscreenTriangle((RenderTargetIdentifier) occlusionTexture1, (RenderTargetIdentifier) occlusionTexture2, propertySheet, 2 + occlusionSource);
      cmd.ReleaseTemporaryRT(occlusionTexture1);
      cmd.BlitFullscreenTriangle((RenderTargetIdentifier) occlusionTexture2, (RenderTargetIdentifier) (Texture) this.m_Result, propertySheet, 4);
      cmd.ReleaseTemporaryRT(occlusionTexture2);
      if (!context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
        return;
      context.PushDebugOverlay(cmd, (RenderTargetIdentifier) (Texture) this.m_Result, propertySheet, 7);
    }

    public void RenderAfterOpaque(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Ambient Occlusion");
      this.Render(context, command, 0);
      command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, (RenderTargetIdentifier) (Texture) this.m_Result);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 5);
      command.EndSample("Ambient Occlusion");
    }

    public void RenderAmbientOnly(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Ambient Occlusion Render");
      this.Render(context, command, 1);
      command.EndSample("Ambient Occlusion Render");
    }

    public void CompositeAmbientOnly(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Ambient Occlusion Composite");
      command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, (RenderTargetIdentifier) (Texture) this.m_Result);
      command.BlitFullscreenTriangle((RenderTargetIdentifier) BuiltinRenderTextureType.None, this.m_MRT, (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 6);
      command.EndSample("Ambient Occlusion Composite");
    }

    public void Release()
    {
      RuntimeUtilities.Destroy((UnityEngine.Object) this.m_Result);
      this.m_Result = (RenderTexture) null;
    }

    private enum Pass
    {
      OcclusionEstimationForward,
      OcclusionEstimationDeferred,
      HorizontalBlurForward,
      HorizontalBlurDeferred,
      VerticalBlur,
      CompositionForward,
      CompositionDeferred,
      DebugOverlay,
    }
  }
}
