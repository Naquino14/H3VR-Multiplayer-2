// Decompiled with JetBrains decompiler
// Type: InstaOld
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class InstaOld : MonoBehaviour
{
  public Texture2D Lut;
  public float VignetteStrength;
  public Color VignetteColor;
  public ShaderVariantCollection Collection;
  [Header("Grain")]
  [Tooltip("Enable the use of colored grain.")]
  public bool colored;
  [Range(0.0f, 1f)]
  [Tooltip("Grain strength. Higher means more visible grain.")]
  public float intensity;
  [Range(0.3f, 3f)]
  [Tooltip("Grain particle size.")]
  public float size;
  [Range(0.0f, 1f)]
  [Tooltip("Controls the noisiness response curve based on scene luminance. Lower values mean less noise in dark areas.")]
  public float luminanceContribution;
  private RenderTexture m_GrainLookupRT;
  private Material m_grainGenMat;

  private void OnEnable()
  {
    this.m_grainGenMat = new Material(Shader.Find("Hidden/Post FX/Grain Generator"));
    this.m_grainGenMat.hideFlags = HideFlags.HideAndDontSave;
  }

  public void OnDisable()
  {
    Object.DestroyImmediate((Object) this.m_GrainLookupRT);
    this.m_GrainLookupRT = (RenderTexture) null;
  }

  public void BindMaterial(Material mat)
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    float z = Random.value;
    float w = Random.value;
    if ((Object) this.m_GrainLookupRT == (Object) null || !this.m_GrainLookupRT.IsCreated())
    {
      Object.DestroyImmediate((Object) this.m_GrainLookupRT);
      RenderTexture renderTexture = new RenderTexture(192, 192, 0, RenderTextureFormat.ARGBHalf);
      renderTexture.filterMode = FilterMode.Bilinear;
      renderTexture.wrapMode = TextureWrapMode.Repeat;
      renderTexture.anisoLevel = 0;
      renderTexture.name = "Grain Lookup Texture";
      this.m_GrainLookupRT = renderTexture;
      this.m_GrainLookupRT.Create();
    }
    this.m_grainGenMat.SetFloat(InstaOld.Uniforms._Phase, realtimeSinceStartup / 20f);
    Graphics.Blit((Texture) null, this.m_GrainLookupRT, this.m_grainGenMat, !this.colored ? 0 : 1);
    mat.SetTexture(InstaOld.Uniforms._GrainTex, (Texture) this.m_GrainLookupRT);
    mat.SetVector(InstaOld.Uniforms._Grain_Params1, (Vector4) new Vector2(this.luminanceContribution, this.intensity * 20f));
    mat.SetVector(InstaOld.Uniforms._Grain_Params2, new Vector4((float) Camera.current.pixelWidth / (float) this.m_GrainLookupRT.width / this.size, (float) Camera.current.pixelHeight / (float) this.m_GrainLookupRT.height / this.size, z, w));
    mat.SetTexture("_InstaOldLUT", (Texture) this.Lut);
    mat.SetFloat("_VignetteStrength", this.VignetteStrength);
    mat.SetColor("_VignetteColor", this.VignetteColor);
  }

  private static class Uniforms
  {
    internal static readonly int _Grain_Params1 = Shader.PropertyToID(nameof (_Grain_Params1));
    internal static readonly int _Grain_Params2 = Shader.PropertyToID(nameof (_Grain_Params2));
    internal static readonly int _GrainTex = Shader.PropertyToID(nameof (_GrainTex));
    internal static readonly int _Phase = Shader.PropertyToID(nameof (_Phase));
  }
}
