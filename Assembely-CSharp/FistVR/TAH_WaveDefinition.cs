using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New WaveDefinition", menuName = "TakeAndHold/TAHWaveDefinition", order = 0)]
	public class TAH_WaveDefinition : ScriptableObject
	{
		public int NumSpawnPointsToUse = 1;

		public float WarmUpToSpawnTime = 3f;

		public float TimeForWave = 10f;

		public float SpawnCooldownTime = 1f;

		public int NumBots = 3;

		public int WaveIntensity = 1;

		public List<TAH_BotSpawnProfile> BotSpawnProfiles;
	}
}
