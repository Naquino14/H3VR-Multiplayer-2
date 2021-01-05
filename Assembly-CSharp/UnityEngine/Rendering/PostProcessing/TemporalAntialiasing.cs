// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.TemporalAntialiasing
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine.VR;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public sealed class TemporalAntialiasing
  {
    [Tooltip("The diameter (in texels) inside which jitter samples are spread. Smaller values result in crisper but more aliased output, while larger values result in more stable but blurrier output.")]
    [Range(0.1f, 1f)]
    public float jitterSpread = 0.75f;
    [Tooltip("Controls the amount of sharpening applied to the color buffer. High values may introduce dark-border artifacts.")]
    [Range(0.0f, 3f)]
    public float sharpness = 0.25f;
    [Tooltip("The blend coefficient for a stationary fragment. Controls the percentage of history sample blended into the final color.")]
    [Range(0.0f, 0.99f)]
    public float stationaryBlending = 0.95f;
    [Tooltip("The blend coefficient for a fragment with significant motion. Controls the percentage of history sample blended into the final color.")]
    [Range(0.0f, 0.99f)]
    public float motionBlending = 0.85f;
    public Func<Camera, Vector2, Matrix4x4> jitteredMatrixFunc;
    private readonly RenderTargetIdentifier[] m_Mrt = new RenderTargetIdentifier[2];
    private bool m_ResetHistory = true;
    private const int k_SampleCount = 8;
    private int m_SampleIndex;
    private const int k_NumEyes = 2;
    private const int k_NumHistoryTextures = 2;
    private readonly RenderTexture[][] m_HistoryTextures = new RenderTexture[2][];
    private int[] m_HistoryPingPong = new int[2];

    public Vector2 jitter { get; private set; }

    public bool IsSupported() => SystemInfo.supportedRenderTargetCount >= 2 && SystemInfo.supportsMotionVectors && !RuntimeUtilities.isVREnabled && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2;

    internal DepthTextureMode GetCameraFlags() => DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

    internal void ResetHistory() => this.m_ResetHistory = true;

    private Vector2 GenerateRandomOffset()
    {
      Vector2 vector2 = new Vector2(HaltonSeq.Get(this.m_SampleIndex & 1023, 2), HaltonSeq.Get(this.m_SampleIndex & 1023, 3));
      if (++this.m_SampleIndex >= 8)
        this.m_SampleIndex = 0;
      return vector2;
    }

    public Matrix4x4 GetJitteredProjectionMatrix(Camera camera)
    {
      this.jitter = this.GenerateRandomOffset();
      this.jitter *= this.jitterSpread;
      Matrix4x4 matrix4x4 = this.jitteredMatrixFunc == null ? (!camera.orthographic ? RuntimeUtilities.GetJitteredPerspectiveProjectionMatrix(camera, this.jitter) : RuntimeUtilities.GetJitteredOrthographicProjectionMatrix(camera, this.jitter)) : this.jitteredMatrixFunc(camera, this.jitter);
      this.jitter = new Vector2(this.jitter.x / (float) camera.pixelWidth, this.jitter.y / (float) camera.pixelHeight);
      return matrix4x4;
    }

    public void ConfigureJitteredProjectionMatrix(PostProcessRenderContext context)
    {
      Camera camera = context.camera;
      camera.nonJitteredProjectionMatrix = camera.projectionMatrix;
      camera.projectionMatrix = this.GetJitteredProjectionMatrix(camera);
      camera.useJitteredProjectionMatrixForTransparentRendering = false;
    }

    public void ConfigureStereoJitteredProjectionMatrices(PostProcessRenderContext context)
    {
    }

    private void GenerateHistoryName(RenderTexture rt, int id, PostProcessRenderContext context)
    {
      rt.name = "Temporal Anti-aliasing History id #" + (object) id;
      if (!VRSettings.isDeviceActive)
        return;
      RenderTexture renderTexture = rt;
      renderTexture.name = renderTexture.name + " for eye " + (object) context.xrActiveEye;
    }

    private RenderTexture CheckHistory(int id, PostProcessRenderContext context)
    {
      int xrActiveEye = context.xrActiveEye;
      if (this.m_HistoryTextures[xrActiveEye] == null)
        this.m_HistoryTextures[xrActiveEye] = new RenderTexture[2];
      RenderTexture temp = this.m_HistoryTextures[xrActiveEye][id];
      if (this.m_ResetHistory || (UnityEngine.Object) temp == (UnityEngine.Object) null || !temp.IsCreated())
      {
        RenderTexture.ReleaseTemporary(temp);
        RenderTexture temporary = RenderTexture.GetTemporary(context.width, context.height, 0, context.sourceFormat);
        this.GenerateHistoryName(temporary, id, context);
        temporary.filterMode = FilterMode.Bilinear;
        this.m_HistoryTextures[xrActiveEye][id] = temporary;
        context.command.BlitFullscreenTriangle(context.source, (RenderTargetIdentifier) (Texture) temporary);
      }
      else if (temp.width != context.width || temp.height != context.height)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(context.width, context.height, 0, context.sourceFormat);
        this.GenerateHistoryName(temporary, id, context);
        temporary.filterMode = FilterMode.Bilinear;
        this.m_HistoryTextures[xrActiveEye][id] = temporary;
        context.command.BlitFullscreenTriangle((RenderTargetIdentifier) (Texture) temp, (RenderTargetIdentifier) (Texture) temporary);
        RenderTexture.ReleaseTemporary(temp);
      }
      return this.m_HistoryTextures[xrActiveEye][id];
    }

    internal void Render(PostProcessRenderContext context)
    {
      PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.temporalAntialiasing);
      CommandBuffer command = context.command;
      command.BeginSample(nameof (TemporalAntialiasing));
      int num1;
      RenderTexture renderTexture1 = this.CheckHistory((num1 = this.m_HistoryPingPong[context.xrActiveEye] + 1) % 2, context);
      int num2;
      RenderTexture renderTexture2 = this.CheckHistory((num2 = num1 + 1) % 2, context);
      int num3;
      this.m_HistoryPingPong[context.xrActiveEye] = (num3 = num2 + 1) % 2;
      propertySheet.properties.SetVector(ShaderIDs.Jitter, (Vector4) this.jitter);
      propertySheet.properties.SetFloat(ShaderIDs.Sharpness, this.sharpness);
      propertySheet.properties.SetVector(ShaderIDs.FinalBlendParameters, new Vector4(this.stationaryBlending, this.motionBlending, 6000f, 0.0f));
      propertySheet.properties.SetTexture(ShaderIDs.HistoryTex, (Texture) renderTexture1);
      int pass = !context.camera.orthographic ? 0 : 1;
      this.m_Mrt[0] = context.destination;
      this.m_Mrt[1] = (RenderTargetIdentifier) (Texture) renderTexture2;
      command.BlitFullscreenTriangle(context.source, this.m_Mrt, context.source, propertySheet, pass);
      command.EndSample(nameof (TemporalAntialiasing));
      this.m_ResetHistory = false;
    }

    internal void Release()
    {
      if (this.m_HistoryTextures != null)
      {
        for (int index1 = 0; index1 < this.m_HistoryTextures.Length; ++index1)
        {
          if (this.m_HistoryTextures[index1] != null)
          {
            for (int index2 = 0; index2 < this.m_HistoryTextures[index1].Length; ++index2)
            {
              RenderTexture.ReleaseTemporary(this.m_HistoryTextures[index1][index2]);
              this.m_HistoryTextures[index1][index2] = (RenderTexture) null;
            }
            this.m_HistoryTextures[index1] = (RenderTexture[]) null;
          }
        }
      }
      this.m_SampleIndex = 0;
      this.m_HistoryPingPong[0] = 0;
      this.m_HistoryPingPong[1] = 0;
      this.ResetHistory();
    }

    private enum Pass
    {
      SolverDilate,
      SolverNoDilate,
    }
  }
}
