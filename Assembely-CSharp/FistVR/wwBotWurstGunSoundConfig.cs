using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "BotWurstGunSoundConfig", menuName = "Agents/Wurstwurld/BotSoundConfig", order = 0)]
	public class wwBotWurstGunSoundConfig : ScriptableObject
	{
		[Serializable]
		public class BotGunShotSet
		{
			public string SampleName;

			public List<FVRSoundEnvironment> EnvironmentsUsed = new List<FVRSoundEnvironment>();

			public AudioEvent ShotSet_Near;

			public AudioEvent ShotSet_Far;

			public AudioEvent ShotSet_Distant;
		}

		public List<BotGunShotSet> ShotSets;

		public AudioEvent EjectionBack;

		public AudioEvent EjectionForward;

		public AudioEvent GoingToReload;

		public AudioEvent Reloading;

		public AudioEvent RecoveringFromReload;

		public AutoMeaterFirearmSoundProfile MigrateFrom;

		[ContextMenu("MigrateBotSetToThis")]
		public void MigrateBotSetToThis()
		{
			for (int i = 0; i < MigrateFrom.ShotSets.Count; i++)
			{
				AutoMeaterFirearmSoundProfile.GunShotSet gunShotSet = MigrateFrom.ShotSets[i];
				ShotSets[i].SampleName = gunShotSet.SampleName;
				for (int j = 0; j < gunShotSet.EnvironmentsUsed.Count; j++)
				{
					ShotSets[i].EnvironmentsUsed.Add(gunShotSet.EnvironmentsUsed[j]);
				}
				ShotSets[i].ShotSet_Near = gunShotSet.ShotSet_Near;
				ShotSets[i].ShotSet_Far = gunShotSet.ShotSet_Far;
				ShotSets[i].ShotSet_Distant = gunShotSet.ShotSet_Distant;
			}
		}
	}
}
