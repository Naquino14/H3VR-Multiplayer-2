using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_Progression", menuName = "TNH/TNH_Progression", order = 0)]
	public class TNH_Progression : ScriptableObject
	{
		[Serializable]
		public class Level
		{
			public TNH_TakeChallenge TakeChallenge;

			public TNH_HoldChallenge HoldChallenge;

			public TNH_TakeChallenge SupplyChallenge;

			public TNH_PatrolChallenge PatrolChallenge;

			public TNH_TrapsChallenge TrapsChallenge;

			public int NumOverrideTokensForHold;
		}

		public List<Level> Levels;
	}
}
