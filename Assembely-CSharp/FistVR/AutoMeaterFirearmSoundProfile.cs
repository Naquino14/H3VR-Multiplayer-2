using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "AutoMeaterGunSoundProfile", menuName = "Agents/Automeater/BotSoundConfig", order = 0)]
	public class AutoMeaterFirearmSoundProfile : ScriptableObject
	{
		[Serializable]
		public class GunShotSet
		{
			public string SampleName;

			public List<FVRSoundEnvironment> EnvironmentsUsed = new List<FVRSoundEnvironment>();

			public AudioEvent ShotSet_Near;

			public AudioEvent ShotSet_Far;

			public AudioEvent ShotSet_Distant;
		}

		public List<GunShotSet> ShotSets;

		public AudioEvent EjectionBack;

		public AudioEvent EjectionForward;

		public AudioEvent GoingToReload;

		public AudioEvent Reloading;

		public AudioEvent RecoveringFromReload;

		public wwBotWurstGunSoundConfig SoundConfig;

		[ContextMenu("MigrateBotSetToThis")]
		public void MigrateBotSetToThis()
		{
			for (int i = 0; i < SoundConfig.ShotSets.Count; i++)
			{
				wwBotWurstGunSoundConfig.BotGunShotSet botGunShotSet = SoundConfig.ShotSets[i];
				ShotSets[i].SampleName = botGunShotSet.SampleName;
				for (int j = 0; j < botGunShotSet.EnvironmentsUsed.Count; j++)
				{
					ShotSets[i].EnvironmentsUsed.Add(botGunShotSet.EnvironmentsUsed[j]);
				}
				ShotSets[i].ShotSet_Near = botGunShotSet.ShotSet_Near;
				ShotSets[i].ShotSet_Far = botGunShotSet.ShotSet_Far;
				ShotSets[i].ShotSet_Distant = botGunShotSet.ShotSet_Distant;
			}
		}
	}
}
