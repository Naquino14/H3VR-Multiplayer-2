// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ScreenSpaceReflectionsRenderer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class ScreenSpaceReflectionsRenderer : PostProcessEffectRenderer<ScreenSpaceReflections>
  {
    private RenderTexture m_Resolve;
    private RenderTexture m_History;
    private int[] m_MipIDs;
    private readonly ScreenSpaceReflectionsRenderer.QualityPreset[] m_Presets = new ScreenSpaceReflectionsRenderer.QualityPreset[7]
    {
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 10,
        thickness = 32f,
        downsampling = ScreenSpaceReflectionResolution.Downsampled
      },
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 16,
        thickness = 32f,
        downsampling = ScreenSpaceReflectionResolution.Downsampled
      },
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 32,
        thickness = 16f,
        downsampling = ScreenSpaceReflectionResolution.Downsampled
      },
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 48,
        thickness = 8f,
        downsampling = ScreenSpaceReflectionResolution.Downsampled
      },
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 16,
        thickness = 32f,
        downsampling = ScreenSpaceReflectionResolution.FullSize
      },
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 48,
        thickness = 16f,
        downsampling = ScreenSpaceReflectionResolution.FullSize
      },
      new ScreenSpaceReflectionsRenderer.QualityPreset()
      {
        maximumIterationCount = 128,
        thickness = 12f,
        downsampling = ScreenSpaceReflectionResolution.Supersampled
      }
    };

    public override DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

    internal void CheckRT(
      ref RenderTexture rt,
      int width,
      int height,
      RenderTextureFormat format,
      FilterMode filterMode,
      bool useMipMap)
    {
      if (!((Object) rt == (Object) null) && rt.IsCreated() && (rt.width == width && rt.height == height))
        return;
      if ((Object) rt != (Object) null)
        rt.Release();
      ref RenderTexture local = ref rt;
      RenderTexture renderTexture1 = new RenderTexture(width, height, 0, format);
      renderTexture1.filterMode = filterMode;
      renderTexture1.useMipMap = useMipMap;
      renderTexture1.autoGenerateMips = false;
      renderTexture1.hideFlags = HideFlags.HideAndDontSave;
      RenderTexture renderTexture2 = renderTexture1;
      local = renderTexture2;
      rt.Create();
    }

    public override void Render(PostProcessRenderContext context)
    {
      CommandBuffer command = context.command;
      command.BeginSample("Screen-space Reflections");
      if (this.settings.preset.value != ScreenSpaceReflectionPreset.Custom)
      {
        int index = (int) this.settings.preset.value;
        this.settings.maximumIterationCount.value = this.m_Presets[index].maximumIterationCount;
        this.settings.thickness.value = this.m_Presets[index].thickness;
        this.settings.resolution.value = this.m_Presets[index].downsampling;
      }
      this.settings.maximumMarchDistance.value = Mathf.Max(0.0f, this.settings.maximumMarchDistance.value);
      int num1 = Mathf.ClosestPowerOfTwo(Mathf.Min(context.width, context.height));
      if (this.settings.resolution.value == ScreenSpaceReflectionResolution.Downsampled)
        num1 >>= 1;
      else if (this.settings.resolution.value == ScreenSpaceReflectionResolution.Supersampled)
        num1 <<= 1;
      int num2 = Mathf.Min(Mathf.FloorToInt(Mathf.Log((float) num1, 2f) - 3f), 12);
      this.CheckRT(ref this.m_Resolve, num1, num1, context.sourceFormat, FilterMode.Trilinear, true);
      Texture2D texture2D = context.resources.blueNoise256[0];
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.screenSpaceReflections);
      propertySheet.properties.SetTexture(ShaderIDs.Noise, (Texture) texture2D);
      Matrix4x4 matrix4x4 = new Matrix4x4();
      matrix4x4.SetRow(0, new Vector4((float) num1 * 0.5f, 0.0f, 0.0f, (float) num1 * 0.5f));
      matrix4x4.SetRow(1, new Vector4(0.0f, (float) num1 * 0.5f, 0.0f, (float) num1 * 0.5f));
      matrix4x4.SetRow(2, new Vector4(0.0f, 0.0f, 1f, 0.0f));
      matrix4x4.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1f));
      Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);
      matrix4x4 *= projectionMatrix;
      propertySheet.properties.SetMatrix(ShaderIDs.ViewMatrix, context.camera.worldToCameraMatrix);
      propertySheet.properties.SetMatrix(ShaderIDs.InverseViewMatrix, context.camera.worldToCameraMatrix.inverse);
      propertySheet.properties.SetMatrix(ShaderIDs.InverseProjectionMatrix, projectionMatrix.inverse);
      propertySheet.properties.SetMatrix(ShaderIDs.ScreenSpaceProjectionMatrix, matrix4x4);
      propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(this.settings.attenuation.value, this.settings.distanceFade.value, this.settings.maximumMarchDistance.value, (float) num2));
      propertySheet.properties.SetVector(ShaderIDs.Params2, new Vector4((float) context.width / (float) context.height, (float) num1 / (float) texture2D.width, this.settings.thickness.value, (float) this.settings.maximumIterationCount.value));
      command.GetTemporaryRT(ShaderIDs.Test, num1, num1, 0, FilterMode.Point, context.sourceFormat);
      command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) ShaderIDs.Test, propertySheet, 0);
      if (context.isSceneView)
      {
        command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) (Texture) this.m_Resolve, propertySheet, 1);
      }
      else
      {
        this.CheckRT(ref this.m_History, num1, num1, context.sourceFormat, FilterMode.Bilinear, false);
        if (this.m_ResetHistory)
        {
          context.command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) (Texture) this.m_History);
          this.m_ResetHistory = false;
        }
        command.GetTemporaryRT(ShaderIDs.SSRResolveTemp, num1, num1, 0, FilterMode.Bilinear, context.sourceFormat);
        command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) ShaderIDs.SSRResolveTemp, propertySheet, 1);
        propertySheet.properties.SetTexture(ShaderIDs.History, (Texture) this.m_History);
        command.BlitFullscreenTriangle((RenderTargetIdentifier) ShaderIDs.SSRResolveTemp, (RenderTargetIdentifier) (Texture) this.m_Resolve, propertySheet, 2);
        command.CopyTexture((RenderTargetIdentifier) (Texture) this.m_Resolve, 0, 0, (RenderTargetIdentifier) (Texture) this.m_History, 0, 0);
        command.ReleaseTemporaryRT(ShaderIDs.SSRResolveTemp);
      }
      command.ReleaseTemporaryRT(ShaderIDs.Test);
      if (this.m_MipIDs == null || this.m_MipIDs.Length == 0)
      {
        this.m_MipIDs = new int[12];
        for (int index = 0; index < 12; ++index)
          this.m_MipIDs[index] = Shader.PropertyToID("_SSRGaussianMip" + (object) index);
      }
      ComputeShader gaussianDownsample = context.resources.computeShaders.gaussianDownsample;
      int kernel = gaussianDownsample.FindKernel("KMain");
      RenderTargetIdentifier rt = new RenderTargetIdentifier((Texture) this.m_Resolve);
      for (int index = 0; index < num2; ++index)
      {
        num1 >>= 1;
        command.GetTemporaryRT(this.m_MipIDs[index], num1, num1, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Default, 1, true);
        command.SetComputeTextureParam(gaussianDownsample, kernel, "_Source", rt);
        command.SetComputeTextureParam(gaussianDownsample, kernel, "_Result", (RenderTargetIdentifier) this.m_MipIDs[index]);
        command.SetComputeVectorParam(gaussianDownsample, "_Size", new Vector4((float) num1, (float) num1, 1f / (float) num1, 1f / (float) num1));
        command.DispatchCompute(gaussianDownsample, kernel, num1 / 8, num1 / 8, 1);
        command.CopyTexture((RenderTargetIdentifier) this.m_MipIDs[index], 0, 0, (RenderTargetIdentifier) (Texture) this.m_Resolve, 0, index + 1);
        rt = (RenderTargetIdentifier) this.m_MipIDs[index];
      }
      for (int index = 0; index < num2; ++index)
        command.ReleaseTemporaryRT(this.m_MipIDs[index]);
      propertySheet.properties.SetTexture(ShaderIDs.Resolve, (Texture) this.m_Resolve);
      command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 3);
      command.EndSample("Screen-space Reflections");
    }

    public override void Release()
    {
      RuntimeUtilities.Destroy((Object) this.m_Resolve);
      RuntimeUtilities.Destroy((Object) this.m_History);
      this.m_Resolve = (RenderTexture) null;
      this.m_History = (RenderTexture) null;
    }

    private class QualityPreset
    {
      public int maximumIterationCount;
      public float thickness;
      public ScreenSpaceReflectionResolution downsampling;
    }

    private enum Pass
    {
      Test,
      Resolve,
      Reproject,
      Composite,
    }
  }
}
