using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_PatrolChallenge", menuName = "TNH/TNH_PatrolChallenge", order = 0)]
	public class TNH_PatrolChallenge : ScriptableObject
	{
		[Serializable]
		public class Patrol
		{
			[SearchableEnum]
			public SosigEnemyID EType;

			[SearchableEnum]
			public SosigEnemyID LType;

			public int PatrolSize;

			public int MaxPatrols;

			public int MaxPatrols_LimitedAmmo;

			public float TimeTilRegen;

			public float TimeTilRegen_LimitedAmmo;

			public int IFFUsed = 1;
		}

		public List<Patrol> Patrols;
	}
}
