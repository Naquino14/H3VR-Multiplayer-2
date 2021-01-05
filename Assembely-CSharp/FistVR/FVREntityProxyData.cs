using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class FVREntityProxyData
	{
		public string UniqueID = "unassigned";

		public string EntityID = "unassigned";

		public Vector3 Position = Vector3.zero;

		public Vector3 EulerAngles = Vector3.zero;

		public bool[] StoredBools;

		public int[] StoredInts;

		public float[] StoredFloats;

		public Vector4[] StoredVector4s;

		public string[] StoredStrings;

		public void PrimeDataLists(FVREntityFlagUsage flags)
		{
			StoredBools = new bool[flags.BoolFlags.Length];
			StoredInts = new int[flags.IntFlags.Length];
			StoredFloats = new float[flags.FloatFlags.Length];
			StoredVector4s = new Vector4[flags.Vector4Flags.Length];
			StoredStrings = new string[flags.StringFlags.Length];
		}

		public void Init(FVREntityFlagUsage flags)
		{
			for (int i = 0; i < StoredBools.Length; i++)
			{
				StoredBools[i] = flags.BoolFlags[i].DefaultValue;
			}
			for (int j = 0; j < StoredInts.Length; j++)
			{
				StoredInts[j] = flags.IntFlags[j].DefaultValue;
			}
			for (int k = 0; k < StoredFloats.Length; k++)
			{
				StoredFloats[k] = flags.FloatFlags[k].DefaultValue;
			}
			for (int l = 0; l < StoredVector4s.Length; l++)
			{
				ref Vector4 reference = ref StoredVector4s[l];
				reference = flags.Vector4Flags[l].DefaultValue;
			}
			for (int m = 0; m < StoredStrings.Length; m++)
			{
				StoredStrings[m] = flags.StringFlags[m].DefaultValue;
			}
		}
	}
}
