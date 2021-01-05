// Decompiled with JetBrains decompiler
// Type: SSAOPro
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[HelpURL("http://www.thomashourdel.com/ssaopro/doc/")]
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/SSAO Pro")]
[RequireComponent(typeof (Camera))]
public class SSAOPro : MonoBehaviour
{
  public Texture2D NoiseTexture;
  public bool UseHighPrecisionDepthMap;
  public SSAOPro.SampleCount Samples = SSAOPro.SampleCount.Medium;
  [Range(1f, 4f)]
  public int Downsampling = 1;
  [Range(0.01f, 1.25f)]
  public float Radius = 0.125f;
  [Range(0.0f, 16f)]
  public float Intensity = 2f;
  [Range(0.0f, 10f)]
  public float Distance = 1f;
  [Range(0.0f, 1f)]
  public float Bias = 0.1f;
  [Range(0.0f, 1f)]
  public float LumContribution = 0.5f;
  [ColorUsage(false)]
  public Color OcclusionColor = Color.black;
  public float CutoffDistance = 150f;
  public float CutoffFalloff = 50f;
  public SSAOPro.BlurMode Blur;
  public bool BlurDownsampling;
  [Range(1f, 4f)]
  public int BlurPasses = 1;
  [Range(0.05f, 1f)]
  public float BlurBilateralThreshold = 0.1f;
  public bool DebugAO;
  protected Shader m_ShaderSSAO_v2;
  protected Shader m_ShaderHighPrecisionDepth;
  protected Material m_Material_v2;
  protected Camera m_Camera;
  protected Camera m_RWSCamera;
  protected RenderTextureFormat m_RTFormat = RenderTextureFormat.RFloat;
  private string[] keywords = new string[2];

  public Material Material
  {
    get
    {
      if ((Object) this.m_Material_v2 == (Object) null)
      {
        this.m_Material_v2 = new Material(this.ShaderSSAO);
        this.m_Material_v2.hideFlags = HideFlags.HideAndDontSave;
      }
      return this.m_Material_v2;
    }
  }

  public Shader ShaderSSAO
  {
    get
    {
      if ((Object) this.m_ShaderSSAO_v2 == (Object) null)
        this.m_ShaderSSAO_v2 = Shader.Find("Hidden/SSAO Pro V2");
      return this.m_ShaderSSAO_v2;
    }
  }

  private void OnEnable()
  {
    this.m_Camera = this.GetComponent<Camera>();
    if (!SystemInfo.supportsImageEffects)
    {
      Debug.LogWarning((object) "Image Effects are not supported on this platform.");
      this.enabled = false;
    }
    else if (!SystemInfo.supportsRenderTextures)
    {
      Debug.LogWarning((object) "RenderTextures are not supported on this platform.");
      this.enabled = false;
    }
    else
    {
      if (!((Object) this.ShaderSSAO != (Object) null) || this.ShaderSSAO.isSupported)
        return;
      Debug.LogWarning((object) "Unsupported shader (SSAO).");
      this.enabled = false;
    }
  }

  private void OnDestroy()
  {
    if ((Object) this.m_Material_v2 != (Object) null)
      Object.DestroyImmediate((Object) this.m_Material_v2);
    if ((Object) this.m_RWSCamera != (Object) null)
      Object.DestroyImmediate((Object) this.m_RWSCamera.gameObject);
    this.m_Material_v2 = (Material) null;
    this.m_RWSCamera = (Camera) null;
  }

  [ImageEffectOpaque]
  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if ((Object) this.ShaderSSAO == (Object) null)
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      int pass1 = this.SetShaderStates();
      this.Material.SetMatrix("_InverseViewProject", (this.m_Camera.projectionMatrix * this.m_Camera.worldToCameraMatrix).inverse);
      this.Material.SetMatrix("_CameraModelView", this.m_Camera.cameraToWorldMatrix);
      this.Material.SetTexture("_NoiseTex", (Texture) this.NoiseTexture);
      this.Material.SetVector("_Params1", new Vector4(!((Object) this.NoiseTexture == (Object) null) ? (float) this.NoiseTexture.width : 0.0f, this.Radius, this.Intensity, this.Distance));
      this.Material.SetVector("_Params2", new Vector4(this.Bias, this.LumContribution, this.CutoffDistance, this.CutoffFalloff));
      this.Material.SetColor("_OcclusionColor", this.OcclusionColor);
      if (this.Blur == SSAOPro.BlurMode.None)
      {
        RenderTexture temporary = RenderTexture.GetTemporary(source.width / this.Downsampling, source.height / this.Downsampling, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit((Texture) temporary, temporary, this.Material, 0);
        if (this.DebugAO)
        {
          Graphics.Blit((Texture) source, temporary, this.Material, pass1);
          Graphics.Blit((Texture) temporary, destination);
          RenderTexture.ReleaseTemporary(temporary);
        }
        else
        {
          Graphics.Blit((Texture) source, temporary, this.Material, pass1);
          this.Material.SetTexture("_SSAOTex", (Texture) temporary);
          Graphics.Blit((Texture) source, destination, this.Material, 8);
          RenderTexture.ReleaseTemporary(temporary);
        }
      }
      else
      {
        int pass2 = 5;
        if (this.Blur == SSAOPro.BlurMode.Bilateral)
          pass2 = 6;
        else if (this.Blur == SSAOPro.BlurMode.HighQualityBilateral)
          pass2 = 7;
        int num = !this.BlurDownsampling ? 1 : this.Downsampling;
        RenderTexture temporary1 = RenderTexture.GetTemporary(source.width / num, source.height / num, 0, RenderTextureFormat.ARGB32);
        RenderTexture temporary2 = RenderTexture.GetTemporary(source.width / this.Downsampling, source.height / this.Downsampling, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit((Texture) temporary1, temporary1, this.Material, 0);
        Graphics.Blit((Texture) source, temporary1, this.Material, pass1);
        if (this.Blur == SSAOPro.BlurMode.HighQualityBilateral)
          this.Material.SetFloat("_BilateralThreshold", this.BlurBilateralThreshold / 10000f);
        for (int index = 0; index < this.BlurPasses; ++index)
        {
          this.Material.SetVector("_Direction", (Vector4) new Vector2(1f / (float) source.width, 0.0f));
          Graphics.Blit((Texture) temporary1, temporary2, this.Material, pass2);
          this.Material.SetVector("_Direction", (Vector4) new Vector2(0.0f, 1f / (float) source.height));
          Graphics.Blit((Texture) temporary2, temporary1, this.Material, pass2);
        }
        if (!this.DebugAO)
        {
          this.Material.SetTexture("_SSAOTex", (Texture) temporary1);
          Graphics.Blit((Texture) source, destination, this.Material, 8);
        }
        else
          Graphics.Blit((Texture) temporary1, destination);
        RenderTexture.ReleaseTemporary(temporary1);
        RenderTexture.ReleaseTemporary(temporary2);
      }
    }
  }

  private int SetShaderStates()
  {
    this.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
    this.m_Camera.depthTextureMode |= DepthTextureMode.DepthNormals;
    this.keywords[0] = this.Samples != SSAOPro.SampleCount.Low ? (this.Samples != SSAOPro.SampleCount.Medium ? (this.Samples != SSAOPro.SampleCount.High ? (this.Samples != SSAOPro.SampleCount.Ultra ? "SAMPLES_VERY_LOW" : "SAMPLES_ULTRA") : "SAMPLES_HIGH") : "SAMPLES_MEDIUM") : "SAMPLES_LOW";
    this.keywords[1] = "HIGH_PRECISION_DEPTHMAP_OFF";
    this.Material.shaderKeywords = this.keywords;
    int num = 0;
    if ((Object) this.NoiseTexture != (Object) null)
      num = 1;
    if ((double) this.LumContribution >= 1.0 / 1000.0)
      num += 2;
    return 1 + num;
  }

  public enum BlurMode
  {
    None,
    Gaussian,
    Bilateral,
    HighQualityBilateral,
  }

  public enum SampleCount
  {
    VeryLow,
    Low,
    Medium,
    High,
    Ultra,
  }
}
