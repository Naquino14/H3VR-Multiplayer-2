// Decompiled with JetBrains decompiler
// Type: CynCliff_GlobalShaderSettings
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class CynCliff_GlobalShaderSettings : MonoBehaviour
{
  [Header("Grass Settings")]
  public Texture2D breakupVariationTex;
  public float breakupVariationTiling = 3f / 1000f;
  [Range(0.0f, 1f)]
  public float breakupIntensity = 1f;
  [Range(0.0f, 1f)]
  public float variationIntensity = 1f;
  [ColorUsage(false, true, 0.0f, 3f, 1f, 1f)]
  public Color grassVariationTintDark = new Color(0.6f, 0.46f, 0.25f, 1f);
  [ColorUsage(false, true, 0.0f, 3f, 1f, 1f)]
  public Color grassVariationTintLight = new Color(1.05f, 1.05f, 0.87f, 1f);
  [Range(0.0f, 3f)]
  public float grassFuzzMultiplier = 1.2f;
  public float grassAngleMin = 0.92f;
  public float grassAngleMax = 1.45f;
  public float grassAnglePower = 5.61f;

  private void UpdateGlobalShaderParameters()
  {
    if ((Object) this.breakupVariationTex != (Object) null)
      Shader.SetGlobalTexture("_BreakupVariationTex", (Texture) this.breakupVariationTex);
    Shader.SetGlobalFloat("_BreakupVariationTiling", this.breakupVariationTiling);
    Shader.SetGlobalFloat("_BreakupIntensity", this.breakupIntensity);
    Shader.SetGlobalFloat("_VariationIntensity", this.variationIntensity);
    Shader.SetGlobalColor("_GrassVariationTintDark", this.grassVariationTintDark);
    Shader.SetGlobalColor("_GrassVariationTintLight", this.grassVariationTintLight);
    Shader.SetGlobalFloat("_GrassFuzzMultiplier", this.grassFuzzMultiplier);
    Shader.SetGlobalFloat("_GrassAngleMin", this.grassAngleMin);
    Shader.SetGlobalFloat("_GrassAngleMax", this.grassAngleMax);
    Shader.SetGlobalFloat("_GrassAnglePower", this.grassAnglePower);
  }

  private void Start() => this.UpdateGlobalShaderParameters();
}
