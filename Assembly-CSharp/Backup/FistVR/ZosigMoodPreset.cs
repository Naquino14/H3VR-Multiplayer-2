// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigMoodPreset
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Mood Preset", menuName = "Zosig/MoodPreset", order = 0)]
  public class ZosigMoodPreset : ScriptableObject
  {
    public Color Ambient_Sky;
    public Color Ambient_Equator;
    public Color Ambient_Ground;
    public Color Direct_Color;
    public float Direct_Intensity;
    public float Direct_ShadowIntensity;
    public Color Fog_Color;
    public float Fog_Density;
    public float TransitionSpeed = 0.2f;

    [ContextMenu("SetToThis")]
    public void SetToThis()
    {
      RenderSettings.ambientSkyColor = this.Ambient_Sky;
      RenderSettings.ambientEquatorColor = this.Ambient_Equator;
      RenderSettings.ambientGroundColor = this.Ambient_Ground;
      RenderSettings.sun.color = this.Direct_Color;
      RenderSettings.sun.intensity = this.Direct_Intensity;
      RenderSettings.sun.shadowStrength = this.Direct_ShadowIntensity;
      RenderSettings.fogColor = this.Fog_Color;
      RenderSettings.fogDensity = this.Fog_Density;
      RenderSettings.skybox.SetColor("_Tint", this.Fog_Color);
    }
  }
}
