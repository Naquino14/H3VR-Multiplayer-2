namespace Valve.VR
{
	public static class SteamVR_Skeleton_JointIndexes
	{
		public const int root = 0;

		public const int wrist = 1;

		public const int thumbMetacarpal = 2;

		public const int thumbProximal = 2;

		public const int thumbMiddle = 3;

		public const int thumbDistal = 4;

		public const int thumbTip = 5;

		public const int indexMetacarpal = 6;

		public const int indexProximal = 7;

		public const int indexMiddle = 8;

		public const int indexDistal = 9;

		public const int indexTip = 10;

		public const int middleMetacarpal = 11;

		public const int middleProximal = 12;

		public const int middleMiddle = 13;

		public const int middleDistal = 14;

		public const int middleTip = 15;

		public const int ringMetacarpal = 16;

		public const int ringProximal = 17;

		public const int ringMiddle = 18;

		public const int ringDistal = 19;

		public const int ringTip = 20;

		public const int pinkyMetacarpal = 21;

		public const int pinkyProximal = 22;

		public const int pinkyMiddle = 23;

		public const int pinkyDistal = 24;

		public const int pinkyTip = 25;

		public const int thumbAux = 26;

		public const int indexAux = 27;

		public const int middleAux = 28;

		public const int ringAux = 29;

		public const int pinkyAux = 30;

		public static int GetFingerForBone(int boneIndex)
		{
			switch (boneIndex)
			{
			case 0:
			case 1:
				return -1;
			case 2:
			case 3:
			case 4:
			case 5:
			case 26:
				return 0;
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 27:
				return 1;
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 28:
				return 2;
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 29:
				return 3;
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 30:
				return 4;
			default:
				return -1;
			}
		}
	}
}
