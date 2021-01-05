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

		private void Start()
		{
			SetFromMoodPreset(VolumeList[0].MoodPreset);
		}

		private void Update()
		{
			m_startingIndex++;
			if (m_startingIndex >= VolumeList.Count)
			{
				m_startingIndex = 0;
			}
			if (TestVolumeBool(VolumeList[m_startingIndex], GM.CurrentPlayerBody.Head.position) && VolumeList[m_startingIndex].MoodPreset != m_presetTransitioningTo)
			{
				StartTransitionTo(VolumeList[m_startingIndex]);
			}
			if (m_isTransitioning)
			{
				if (m_transitionLerp >= 1f)
				{
					m_transitionLerp = 1f;
					m_isTransitioning = false;
				}
				else
				{
					m_transitionLerp += Time.deltaTime * m_transitionSpeed;
				}
				UpdateLerpingMoodValues(m_transitionLerp);
				SetActualLightValuesBasedOnCur();
			}
		}

		private void UpdateLerpingMoodValues(float l)
		{
			m_cur_ambient_Sky = Color.Lerp(m_from_ambient_Sky, m_to_ambient_Sky, l);
			m_cur_ambient_Equator = Color.Lerp(m_from_ambient_Equator, m_to_ambient_Equator, l);
			m_cur_ambient_Ground = Color.Lerp(m_from_ambient_Ground, m_to_ambient_Ground, l);
			m_cur_direct_Color = Color.Lerp(m_from_direct_Color, m_to_direct_Color, l);
			m_cur_direct_Intensity = Mathf.Lerp(m_from_direct_Intensity, m_to_direct_Intensity, l);
			m_cur_direct_ShadowIntensity = Mathf.Lerp(m_from_direct_ShadowIntensity, m_to_direct_ShadowIntensity, l);
			m_cur_fog_Color = Color.Lerp(m_from_fog_Color, m_to_fog_Color, l);
			m_cur_fog_Density = Mathf.Lerp(m_from_fog_Density, m_to_fog_Density, l);
		}

		private void SetActualLightValuesBasedOnCur()
		{
			RenderSettings.ambientSkyColor = m_cur_ambient_Sky;
			RenderSettings.ambientEquatorColor = m_cur_ambient_Equator;
			RenderSettings.ambientGroundColor = m_cur_ambient_Ground;
			Sunlight.color = m_cur_direct_Color;
			Sunlight.intensity = m_cur_direct_Intensity;
			Sunlight.shadowStrength = m_cur_direct_ShadowIntensity;
			RenderSettings.fogColor = m_cur_fog_Color;
			RenderSettings.fogDensity = m_cur_fog_Density;
			RenderSettings.skybox.SetColor("_Tint", m_cur_fog_Color);
		}

		private void SetFromMoodPreset(ZosigMoodPreset p)
		{
			m_cur_ambient_Sky = p.Ambient_Sky;
			m_cur_ambient_Equator = p.Ambient_Equator;
			m_cur_ambient_Ground = p.Ambient_Ground;
			m_cur_direct_Color = p.Direct_Color;
			m_cur_direct_Intensity = p.Direct_Intensity;
			m_cur_direct_ShadowIntensity = p.Direct_ShadowIntensity;
			m_cur_fog_Color = p.Fog_Color;
			m_cur_fog_Density = p.Fog_Density;
			m_from_ambient_Sky = p.Ambient_Sky;
			m_from_ambient_Equator = p.Ambient_Equator;
			m_from_ambient_Ground = p.Ambient_Ground;
			m_from_direct_Color = p.Direct_Color;
			m_from_direct_Intensity = p.Direct_Intensity;
			m_from_direct_ShadowIntensity = p.Direct_ShadowIntensity;
			m_from_fog_Color = p.Fog_Color;
			m_from_fog_Density = p.Fog_Density;
			SetActualLightValuesBasedOnCur();
			m_presetTransitioningTo = p;
		}

		public void StartTransitionTo(ZosigMoodTransitionVolume v)
		{
			m_from_ambient_Sky = m_cur_ambient_Sky;
			m_from_ambient_Equator = m_cur_ambient_Equator;
			m_from_ambient_Ground = m_cur_ambient_Ground;
			m_from_direct_Color = m_cur_direct_Color;
			m_from_direct_Intensity = m_cur_direct_Intensity;
			m_from_direct_ShadowIntensity = m_cur_direct_ShadowIntensity;
			m_from_fog_Color = m_cur_fog_Color;
			m_from_fog_Density = m_cur_fog_Density;
			m_to_ambient_Sky = v.MoodPreset.Ambient_Sky;
			m_to_ambient_Equator = v.MoodPreset.Ambient_Equator;
			m_to_ambient_Ground = v.MoodPreset.Ambient_Ground;
			m_to_direct_Color = v.MoodPreset.Direct_Color;
			m_to_direct_Intensity = v.MoodPreset.Direct_Intensity;
			m_to_direct_ShadowIntensity = v.MoodPreset.Direct_ShadowIntensity;
			m_to_fog_Color = v.MoodPreset.Fog_Color;
			m_to_fog_Density = v.MoodPreset.Fog_Density;
			m_transitionLerp = 0f;
			m_isTransitioning = true;
			m_transitionSpeed = v.MoodPreset.TransitionSpeed;
			m_presetTransitioningTo = v.MoodPreset;
		}

		public bool TestVolumeBool(ZosigMoodTransitionVolume z, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = z.t.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}
	}
}
