using System;

namespace Valve.VR
{
	public class SteamVR_Skeleton_FingerIndexes
	{
		public const int thumb = 0;

		public const int index = 1;

		public const int middle = 2;

		public const int ring = 3;

		public const int pinky = 4;

		public static SteamVR_Skeleton_FingerIndexEnum[] enumArray = (SteamVR_Skeleton_FingerIndexEnum[])Enum.GetValues(typeof(SteamVR_Skeleton_FingerIndexEnum));
	}
}
