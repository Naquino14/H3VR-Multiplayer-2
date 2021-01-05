using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_TrapsChallenge", menuName = "TNH/TNH_TrapsChallenge", order = 0)]
	public class TNH_TrapsChallenge : ScriptableObject
	{
		[Serializable]
		public class TrapOccurance
		{
			public TNH_TrapType Type;

			public int MinNumber;

			public int MaxNumber;
		}

		public List<TrapOccurance> Traps;
	}
}
