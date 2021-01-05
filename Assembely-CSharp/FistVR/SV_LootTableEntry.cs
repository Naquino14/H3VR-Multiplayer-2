using System;

namespace FistVR
{
	[Serializable]
	public class SV_LootTableEntry
	{
		public FVRObject MainObj;

		public FVRObject SecondaryObj;

		public float Incidence;

		public float FinalWeight;
	}
}
