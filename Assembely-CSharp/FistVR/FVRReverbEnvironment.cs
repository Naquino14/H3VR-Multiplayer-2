using UnityEngine;

namespace FistVR
{
	public class FVRReverbEnvironment : MonoBehaviour
	{
		public FVRSoundEnvironment Environment;

		public Vector3 Size;

		public int Priority;

		public void Awake()
		{
			Size = base.transform.localScale * 2f;
		}

		public void SetPriorityBasedOnType()
		{
			if (Environment == FVRSoundEnvironment.InsideNarrowSmall)
			{
				Priority = 0;
			}
			else if (Environment == FVRSoundEnvironment.InsideSmall)
			{
				Priority = 1;
			}
			else if (Environment == FVRSoundEnvironment.InsideNarrow)
			{
				Priority = 2;
			}
			else if (Environment == FVRSoundEnvironment.InsideMedium)
			{
				Priority = 3;
			}
			else if (Environment == FVRSoundEnvironment.InsideLarge)
			{
				Priority = 4;
			}
			else if (Environment == FVRSoundEnvironment.InsideLargeHighCeiling)
			{
				Priority = 5;
			}
			else if (Environment == FVRSoundEnvironment.ShootingRange)
			{
				Priority = 6;
			}
			else if (Environment == FVRSoundEnvironment.SniperRange)
			{
				Priority = 7;
			}
			else if (Environment == FVRSoundEnvironment.InsideWarehouseSmall)
			{
				Priority = 8;
			}
			else if (Environment == FVRSoundEnvironment.InsideWarehouse)
			{
				Priority = 9;
			}
			else if (Environment == FVRSoundEnvironment.OutsideEnclosedNarrow)
			{
				Priority = 10;
			}
			else if (Environment == FVRSoundEnvironment.OutsideEnclosed)
			{
				Priority = 11;
			}
			else if (Environment == FVRSoundEnvironment.Forest)
			{
				Priority = 12;
			}
			else if (Environment == FVRSoundEnvironment.OutsideOpen)
			{
				Priority = 13;
			}
		}
	}
}
