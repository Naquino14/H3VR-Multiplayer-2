// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigMoodManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigMoodManager : MonoBehaviour
  {
    public List<ZosigMoodTransitionVolume> VolumeList = new List<ZosigMoodTransitionVolume>();
    public Light Sunlight;
    public int PlayerAmbientIndex;
    private int m_startingIndex;
    private ZosigMoodPreset m_presetTransitioningTo;
    private Color m_cur_ambient_Sky;
    private Color m_cur_ambient_Equator;
    private Color m_cur_ambient_Ground;
    private Color m_cur_direct_Color;
    private float m_cur_direct_Intensity;
    private float m_cur_direct_ShadowIntensity;
    private Color m_cur_fog_Color;
    private float m_cur_fog_Density;
    private Color m_from_ambient_Sky;
    private Color m_from_ambient_Equator;
    private Color m_from_ambient_Ground;
    private Color m_from_direct_Color;
    private float m_from_direct_Intensity;
    private float m_from_direct_ShadowIntensity;
    private Color m_from_fog_Color;
    private float m_from_fog_Density;
    private Color m_to_ambient_Sky;
    private Color m_to_ambient_Equator;
    private Color m_to_ambient_Ground;
    private Color m_to_direct_Color;
    private float m_to_direct_Intensity;
    private float m_to_direct_ShadowIntensity;
    private Color m_to_fog_Color;
    private float m_to_fog_Density;
    private bool m_isTransitioning;
    private float m_transitionLerp;
    private float m_transitionSpeed = 0.2f;

    private void Start() => this.SetFromMoodPreset(this.VolumeList[0].MoodPreset);

    private void Update()
    {
      ++this.m_startingIndex;
      if (this.m_startingIndex >= this.VolumeList.Count)
        this.m_startingIndex = 0;
      if (this.TestVolumeBool(this.VolumeList[this.m_startingIndex], GM.CurrentPlayerBody.Head.position) && (Object) this.VolumeList[this.m_startingIndex].MoodPreset != (Object) this.m_presetTransitioningTo)
        this.StartTransitionTo(this.VolumeList[this.m_startingIndex]);
      if (!this.m_isTransitioning)
        return;
      if ((double) this.m_transitionLerp >= 1.0)
      {
        this.m_transitionLerp = 1f;
        this.m_isTransitioning = false;
      }
      else
        this.m_transitionLerp += Time.deltaTime * this.m_transitionSpeed;
      this.UpdateLerpingMoodValues(this.m_transitionLerp);
      this.SetActualLightValuesBasedOnCur();
    }

    private void UpdateLerpingMoodValues(float l)
    {
      this.m_cur_ambient_Sky = Color.Lerp(this.m_from_ambient_Sky, this.m_to_ambient_Sky, l);
      this.m_cur_ambient_Equator = Color.Lerp(this.m_from_ambient_Equator, this.m_to_ambient_Equator, l);
      this.m_cur_ambient_Ground = Color.Lerp(this.m_from_ambient_Ground, this.m_to_ambient_Ground, l);
      this.m_cur_direct_Color = Color.Lerp(this.m_from_direct_Color, this.m_to_direct_Color, l);
      this.m_cur_direct_Intensity = Mathf.Lerp(this.m_from_direct_Intensity, this.m_to_direct_Intensity, l);
      this.m_cur_direct_ShadowIntensity = Mathf.Lerp(this.m_from_direct_ShadowIntensity, this.m_to_direct_ShadowIntensity, l);
      this.m_cur_fog_Color = Color.Lerp(this.m_from_fog_Color, this.m_to_fog_Color, l);
      this.m_cur_fog_Density = Mathf.Lerp(this.m_from_fog_Density, this.m_to_fog_Density, l);
    }

    private void SetActualLightValuesBasedOnCur()
    {
      RenderSettings.ambientSkyColor = this.m_cur_ambient_Sky;
      RenderSettings.ambientEquatorColor = this.m_cur_ambient_Equator;
      RenderSettings.ambientGroundColor = this.m_cur_ambient_Ground;
      this.Sunlight.color = this.m_cur_direct_Color;
      this.Sunlight.intensity = this.m_cur_direct_Intensity;
      this.Sunlight.shadowStrength = this.m_cur_direct_ShadowIntensity;
      RenderSettings.fogColor = this.m_cur_fog_Color;
      RenderSettings.fogDensity = this.m_cur_fog_Density;
      RenderSettings.skybox.SetColor("_Tint", this.m_cur_fog_Color);
    }

    private void SetFromMoodPreset(ZosigMoodPreset p)
    {
      this.m_cur_ambient_Sky = p.Ambient_Sky;
      this.m_cur_ambient_Equator = p.Ambient_Equator;
      this.m_cur_ambient_Ground = p.Ambient_Ground;
      this.m_cur_direct_Color = p.Direct_Color;
      this.m_cur_direct_Intensity = p.Direct_Intensity;
      this.m_cur_direct_ShadowIntensity = p.Direct_ShadowIntensity;
      this.m_cur_fog_Color = p.Fog_Color;
      this.m_cur_fog_Density = p.Fog_Density;
      this.m_from_ambient_Sky = p.Ambient_Sky;
      this.m_from_ambient_Equator = p.Ambient_Equator;
      this.m_from_ambient_Ground = p.Ambient_Ground;
      this.m_from_direct_Color = p.Direct_Color;
      this.m_from_direct_Intensity = p.Direct_Intensity;
      this.m_from_direct_ShadowIntensity = p.Direct_ShadowIntensity;
      this.m_from_fog_Color = p.Fog_Color;
      this.m_from_fog_Density = p.Fog_Density;
      this.SetActualLightValuesBasedOnCur();
      this.m_presetTransitioningTo = p;
    }

    public void StartTransitionTo(ZosigMoodTransitionVolume v)
    {
      this.m_from_ambient_Sky = this.m_cur_ambient_Sky;
      this.m_from_ambient_Equator = this.m_cur_ambient_Equator;
      this.m_from_ambient_Ground = this.m_cur_ambient_Ground;
      this.m_from_direct_Color = this.m_cur_direct_Color;
      this.m_from_direct_Intensity = this.m_cur_direct_Intensity;
      this.m_from_direct_ShadowIntensity = this.m_cur_direct_ShadowIntensity;
      this.m_from_fog_Color = this.m_cur_fog_Color;
      this.m_from_fog_Density = this.m_cur_fog_Density;
      this.m_to_ambient_Sky = v.MoodPreset.Ambient_Sky;
      this.m_to_ambient_Equator = v.MoodPreset.Ambient_Equator;
      this.m_to_ambient_Ground = v.MoodPreset.Ambient_Ground;
      this.m_to_direct_Color = v.MoodPreset.Direct_Color;
      this.m_to_direct_Intensity = v.MoodPreset.Direct_Intensity;
      this.m_to_direct_ShadowIntensity = v.MoodPreset.Direct_ShadowIntensity;
      this.m_to_fog_Color = v.MoodPreset.Fog_Color;
      this.m_to_fog_Density = v.MoodPreset.Fog_Density;
      this.m_transitionLerp = 0.0f;
      this.m_isTransitioning = true;
      this.m_transitionSpeed = v.MoodPreset.TransitionSpeed;
      this.m_presetTransitioningTo = v.MoodPreset;
    }

    public bool TestVolumeBool(ZosigMoodTransitionVolume z, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = z.t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }
  }
}
