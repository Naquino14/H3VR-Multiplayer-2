using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Accuacy Chart", menuName = "Ballistics/FireArmMechanicalAccuracyChart", order = 0)]
	public class FVRFireArmMechanicalAccuracyChart : ScriptableObject
	{
		[Serializable]
		public class MechanicalAccuracyEntry
		{
			public FVRFireArmMechanicalAccuracyClass Class;

			public float MinMOA;

			public float MaxMOA;

			public float MinDegrees;

			public float MaxDegrees;

			public float DropMult;

			public float DriftMult;

			public float RecoilMult = 1f;
		}

		public List<MechanicalAccuracyEntry> Entries = new List<MechanicalAccuracyEntry>();

		[ContextMenu("Calc")]
		public void Calc()
		{
			for (int i = 0; i < Entries.Count; i++)
			{
				Entries[i].MinDegrees = Entries[i].MinMOA * 0.0166667f;
				Entries[i].MaxDegrees = Entries[i].MaxMOA * 0.0166667f;
			}
		}
	}
}
