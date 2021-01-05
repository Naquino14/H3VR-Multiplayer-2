using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_HoldChallenge", menuName = "TNH/TNH_HoldChallenge", order = 0)]
	public class TNH_HoldChallenge : ScriptableObject
	{
		[Serializable]
		public class Phase
		{
			public TNH_EncryptionType Encryption;

			public int MinTargets;

			public int MaxTargets;

			[SearchableEnum]
			public SosigEnemyID EType;

			[SearchableEnum]
			public SosigEnemyID LType;

			public int MinEnemies;

			public int MaxEnemies;

			public float SpawnCadence;

			public int MaxEnemiesAlive;

			public int MaxDirections;

			public float ScanTime;

			public float WarmUp = 7f;

			public int IFFUsed = 1;
		}

		public List<Phase> Phases;
	}
}
