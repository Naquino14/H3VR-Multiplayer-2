using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Prefab Dir", menuName = "AudioPooling/TailsDef", order = 0)]
	public class FVRSoundTailsDirectory : ScriptableObject
	{
		[Serializable]
		public class TailSoundSet
		{
			public FVRSoundEnvironment Environment;

			public AudioEvent AudioEvent;
		}

		public FVRTailSoundClass SoundClass;

		public List<TailSoundSet> SoundSets;

		[ContextMenu("Migrate")]
		public void Migrate()
		{
			for (int i = 0; i < SoundSets.Count; i++)
			{
				if (SoundSets[i].Environment == FVRSoundEnvironment.InsideMedium)
				{
					SoundSets[i].AudioEvent.Clips = SoundSets[2].AudioEvent.Clips;
				}
				else if (SoundSets[i].Environment == FVRSoundEnvironment.OutsideEnclosedNarrow)
				{
					SoundSets[i].AudioEvent.Clips = SoundSets[5].AudioEvent.Clips;
				}
				else if (SoundSets[i].Environment == FVRSoundEnvironment.ShootingRange)
				{
					SoundSets[i].AudioEvent.Clips = SoundSets[1].AudioEvent.Clips;
				}
			}
		}
	}
}
