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
			RenderSettings.ambientSkyColor = Ambient_Sky;
			RenderSettings.ambientEquatorColor = Ambient_Equator;
			RenderSettings.ambientGroundColor = Ambient_Ground;
			RenderSettings.sun.color = Direct_Color;
			RenderSettings.sun.intensity = Direct_Intensity;
			RenderSettings.sun.shadowStrength = Direct_ShadowIntensity;
			RenderSettings.fogColor = Fog_Color;
			RenderSettings.fogDensity = Fog_Density;
			RenderSettings.skybox.SetColor("_Tint", Fog_Color);
		}
	}
}
