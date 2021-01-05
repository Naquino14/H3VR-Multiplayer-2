using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New AudioImpactMaterialGroup", menuName = "AudioPooling/AudioImpactMaterialGroup", order = 0)]
	public class AudioImpactMaterialGroup : ScriptableObject
	{
		public List<AudioClip> Clips_Hard;

		public List<AudioClip> Clips_Medium;

		public List<AudioClip> Clips_Light;

		public Vector3 Volumes = new Vector3(1f, 1f, 1f);

		public List<AudioClip> Clips_Temp;

		[ContextMenu("SortMe")]
		public void SortMe()
		{
			for (int i = 0; i < Clips_Temp.Count; i++)
			{
				if (Clips_Temp[i].name.Contains("Light"))
				{
					Clips_Light.Add(Clips_Temp[i]);
				}
				else if (Clips_Temp[i].name.Contains("Medium"))
				{
					Clips_Medium.Add(Clips_Temp[i]);
				}
				else if (Clips_Temp[i].name.Contains("Hard"))
				{
					Clips_Hard.Add(Clips_Temp[i]);
				}
			}
			Clips_Temp.Clear();
		}
	}
}
