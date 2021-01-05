// Decompiled with JetBrains decompiler
// Type: AlloyEffectsManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[RequireComponent(typeof (Camera))]
[AddComponentMenu("Alloy/Alloy Effects Manager")]
public class AlloyEffectsManager : MonoBehaviour
{
  private const float c_blurWdith = 0.15f;
  private const float c_blurDepthDifferenceMultiplier = 100f;
  private const string c_copyTransmissionBufferName = "AlloyCopyTransmission";
  private const string c_blurNormalsBufferName = "AlloyBlurNormals";
  private const CameraEvent c_copyTransmissionEvent = CameraEvent.AfterGBuffer;
  private const CameraEvent c_blurNormalsEvent = CameraEvent.BeforeLighting;
  public AlloyEffectsManager.SkinSettingsData SkinSettings = new AlloyEffectsManager.SkinSettingsData()
  {
    Enabled = true,
    Weight = 1f,
    MaskCutoff = 0.1f,
    Bias = 0.0f,
    Scale = 1f,
    BumpBlur = 0.7f,
    Absorption = new Vector3(-8f, -40f, -64f),
    AoColorBleed = new Vector3(0.4f, 0.15f, 0.13f)
  };
  public AlloyEffectsManager.TransmissionSettingsData TransmissionSettings = new AlloyEffectsManager.TransmissionSettingsData()
  {
    Enabled = true,
    Weight = 1f,
    ShadowWeight = 0.5f,
    BumpDistortion = 0.05f,
    Falloff = 1f
  };
  [HideInInspector]
  public Texture2D SkinLut;
  [HideInInspector]
  public Shader TransmissionBlitShader;
  [HideInInspector]
  public Shader BlurNormalsShader;
  private Material m_deferredTransmissionBlitMaterial;
  private Material m_deferredBlurredNormalsMaterial;
  private Camera m_camera;
  private bool m_isTransmissionEnabled;
  private bool m_isScatteringEnabled;
  private CommandBuffer m_copyTransmission;
  private CommandBuffer m_renderBlurredNormals;

  private void Awake() => this.m_camera = this.GetComponent<Camera>();

  private void Reset() => this.ResetCommandBuffers();

  private void OnEnable() => this.ResetCommandBuffers();

  private void OnDisable() => this.DestroyCommandBuffers();

  private void OnDestroy() => this.DestroyCommandBuffers();

  public void Refresh()
  {
    bool enabled = this.SkinSettings.Enabled;
    if (this.m_isTransmissionEnabled == (this.TransmissionSettings.Enabled || enabled) && this.m_isScatteringEnabled == enabled)
      this.RefreshProperties();
    else
      this.ResetCommandBuffers();
  }

  private void RefreshProperties()
  {
    if (!this.m_isTransmissionEnabled && !this.m_isScatteringEnabled)
      return;
    Shader.SetGlobalVector("_DeferredTransmissionParams", new Vector4(!this.m_isTransmissionEnabled ? 0.0f : Mathf.GammaToLinearSpace(this.TransmissionSettings.Weight), this.TransmissionSettings.Falloff, this.TransmissionSettings.BumpDistortion, this.TransmissionSettings.ShadowWeight));
    if (!this.m_isScatteringEnabled)
      return;
    float num = 1f / Mathf.Tan((float) Math.PI / 360f * this.m_camera.fieldOfView);
    Shader.SetGlobalVector("_DeferredBlurredNormalsParams", (Vector4) new Vector2(0.15f * num, 100f * num));
    Vector3 absorption = this.SkinSettings.Absorption;
    Vector3 aoColorBleed = this.SkinSettings.AoColorBleed;
    Shader.SetGlobalTexture("_DeferredSkinLut", (Texture) this.SkinSettings.Lut);
    Shader.SetGlobalVector("_DeferredSkinParams", (Vector4) new Vector3(this.SkinSettings.Weight, 1f / this.SkinSettings.MaskCutoff, this.SkinSettings.BumpBlur));
    Shader.SetGlobalVector("_DeferredSkinTransmissionAbsorption", new Vector4(absorption.x, absorption.y, absorption.z, this.SkinSettings.Bias));
    Shader.SetGlobalVector("_DeferredSkinColorBleedAoWeights", new Vector4(aoColorBleed.x, aoColorBleed.y, aoColorBleed.z, this.SkinSettings.Scale));
  }

  private void ResetCommandBuffers()
  {
    this.m_isScatteringEnabled = this.SkinSettings.Enabled;
    this.m_isTransmissionEnabled = this.TransmissionSettings.Enabled || this.m_isScatteringEnabled;
    if ((UnityEngine.Object) this.SkinSettings.Lut == (UnityEngine.Object) null)
      this.SkinSettings.Lut = this.SkinLut;
    this.DestroyCommandBuffers();
    if ((this.m_isTransmissionEnabled || this.m_isScatteringEnabled) && ((UnityEngine.Object) this.m_camera != (UnityEngine.Object) null && (UnityEngine.Object) this.TransmissionBlitShader != (UnityEngine.Object) null))
    {
      int id1 = Shader.PropertyToID("_DeferredPlusBuffer");
      this.m_deferredTransmissionBlitMaterial = new Material(this.TransmissionBlitShader);
      this.m_deferredTransmissionBlitMaterial.hideFlags = HideFlags.HideAndDontSave;
      this.m_copyTransmission = new CommandBuffer();
      this.m_copyTransmission.name = "AlloyCopyTransmission";
      if (!this.m_isScatteringEnabled)
      {
        this.m_copyTransmission.GetTemporaryRT(id1, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        this.m_copyTransmission.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget, (RenderTargetIdentifier) id1, this.m_deferredTransmissionBlitMaterial);
        this.m_copyTransmission.ReleaseTemporaryRT(id1);
      }
      else if ((UnityEngine.Object) this.BlurNormalsShader != (UnityEngine.Object) null)
      {
        int width = this.m_camera.pixelWidth / 2;
        int height = this.m_camera.pixelHeight / 2;
        int id2 = Shader.PropertyToID("_DeferredBlurredNormalPingBuffer");
        int id3 = Shader.PropertyToID("_DeferredBlurredNormalPongBuffer");
        this.m_copyTransmission.SetGlobalTexture("_DeferredTransmissionBuffer", (RenderTargetIdentifier) BuiltinRenderTextureType.CameraTarget);
        this.m_deferredBlurredNormalsMaterial = new Material(this.BlurNormalsShader);
        this.m_deferredBlurredNormalsMaterial.hideFlags = HideFlags.HideAndDontSave;
        this.m_renderBlurredNormals = new CommandBuffer();
        this.m_renderBlurredNormals.name = "AlloyBlurNormals";
        this.m_renderBlurredNormals.GetTemporaryRT(id1, -1, -1, 0, FilterMode.Point, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        this.m_renderBlurredNormals.GetTemporaryRT(id2, width, height, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        this.m_renderBlurredNormals.GetTemporaryRT(id3, width, height, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        this.m_renderBlurredNormals.Blit((RenderTargetIdentifier) BuiltinRenderTextureType.GBuffer2, (RenderTargetIdentifier) id2, this.m_deferredBlurredNormalsMaterial, 0);
        this.m_renderBlurredNormals.Blit((RenderTargetIdentifier) id2, (RenderTargetIdentifier) id3, this.m_deferredBlurredNormalsMaterial, 1);
        this.m_renderBlurredNormals.Blit((RenderTargetIdentifier) id3, (RenderTargetIdentifier) id2, this.m_deferredBlurredNormalsMaterial, 2);
        this.m_renderBlurredNormals.Blit((RenderTargetIdentifier) id2, (RenderTargetIdentifier) id1, this.m_deferredBlurredNormalsMaterial, 3);
        this.m_renderBlurredNormals.ReleaseTemporaryRT(id1);
        this.m_renderBlurredNormals.ReleaseTemporaryRT(id2);
        this.m_renderBlurredNormals.ReleaseTemporaryRT(id3);
        this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
        this.m_camera.AddCommandBuffer(CameraEvent.BeforeLighting, this.m_renderBlurredNormals);
      }
      this.m_camera.AddCommandBuffer(CameraEvent.AfterGBuffer, this.m_copyTransmission);
    }
    this.RefreshProperties();
  }

  private void DestroyCommandBuffers()
  {
    if (this.m_copyTransmission != null)
      this.m_camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, this.m_copyTransmission);
    if (this.m_renderBlurredNormals != null)
      this.m_camera.RemoveCommandBuffer(CameraEvent.BeforeLighting, this.m_renderBlurredNormals);
    if ((UnityEngine.Object) this.m_deferredTransmissionBlitMaterial != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_deferredTransmissionBlitMaterial);
    if ((UnityEngine.Object) this.m_deferredBlurredNormalsMaterial != (UnityEngine.Object) null)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_deferredBlurredNormalsMaterial);
    this.m_copyTransmission = (CommandBuffer) null;
    this.m_renderBlurredNormals = (CommandBuffer) null;
    this.m_deferredTransmissionBlitMaterial = (Material) null;
    this.m_deferredBlurredNormalsMaterial = (Material) null;
  }

  [Serializable]
  public struct SkinSettingsData
  {
    public bool Enabled;
    public Texture2D Lut;
    [Range(0.0f, 1f)]
    public float Weight;
    [Range(0.01f, 1f)]
    public float MaskCutoff;
    [Range(0.0f, 1f)]
    public float Bias;
    [Range(0.0f, 1f)]
    public float Scale;
    [Range(0.0f, 1f)]
    public float BumpBlur;
    public Vector3 Absorption;
    public Vector3 AoColorBleed;
  }

  [Serializable]
  public struct TransmissionSettingsData
  {
    public bool Enabled;
    [Range(0.0f, 1f)]
    public float Weight;
    [Range(0.0f, 1f)]
    public float ShadowWeight;
    [Range(0.0f, 1f)]
    [Tooltip("Amount that the transmission is distorted by surface normals.")]
    public float BumpDistortion;
    [MinValue(1f)]
    public float Falloff;
  }
}
