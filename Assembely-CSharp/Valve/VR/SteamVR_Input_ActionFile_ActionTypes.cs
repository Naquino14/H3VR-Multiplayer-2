namespace Valve.VR
{
	public static class SteamVR_Input_ActionFile_ActionTypes
	{
		public static string boolean = "boolean";

		public static string vector1 = "vector1";

		public static string vector2 = "vector2";

		public static string vector3 = "vector3";

		public static string vibration = "vibration";

		public static string pose = "pose";

		public static string skeleton = "skeleton";

		public static string skeletonLeftPath = "\\skeleton\\hand\\left";

		public static string skeletonRightPath = "\\skeleton\\hand\\right";

		public static string[] listAll = new string[7]
		{
			boolean,
			vector1,
			vector2,
			vector3,
			vibration,
			pose,
			skeleton
		};

		public static string[] listIn = new string[6]
		{
			boolean,
			vector1,
			vector2,
			vector3,
			pose,
			skeleton
		};

		public static string[] listOut = new string[1]
		{
			vibration
		};

		public static string[] listSkeletons = new string[2]
		{
			skeletonLeftPath,
			skeletonRightPath
		};
	}
}
