using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Skeleton_Pose : ScriptableObject
	{
		public SteamVR_Skeleton_Pose_Hand leftHand = new SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources.LeftHand);

		public SteamVR_Skeleton_Pose_Hand rightHand = new SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources.RightHand);

		protected const int leftHandInputSource = 1;

		protected const int rightHandInputSource = 2;

		public SteamVR_Skeleton_Pose_Hand GetHand(int hand)
		{
			return hand switch
			{
				1 => leftHand, 
				2 => rightHand, 
				_ => null, 
			};
		}

		public SteamVR_Skeleton_Pose_Hand GetHand(SteamVR_Input_Sources hand)
		{
			return hand switch
			{
				SteamVR_Input_Sources.LeftHand => leftHand, 
				SteamVR_Input_Sources.RightHand => rightHand, 
				_ => null, 
			};
		}
	}
}
