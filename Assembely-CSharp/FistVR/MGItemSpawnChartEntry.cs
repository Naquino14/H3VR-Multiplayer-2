using System;

namespace FistVR
{
	[Serializable]
	public class MGItemSpawnChartEntry
	{
		public FVRObject Obj;

		public FVRObject Mag;

		public float Incidence;

		public float FinalWeight;

		public int Amount;

		public FireArmRoundClass FireArmRoundClass;

		public float FireArmMagFillPercentage;
	}
}
