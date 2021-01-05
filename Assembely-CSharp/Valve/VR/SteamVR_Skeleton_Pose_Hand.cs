using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Skeleton_Pose_Hand
	{
		public SteamVR_Input_Sources inputSource;

		public SteamVR_Skeleton_FingerExtensionTypes thumbFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes indexFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes middleFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes ringFingerMovementType;

		public SteamVR_Skeleton_FingerExtensionTypes pinkyFingerMovementType;

		public bool ignoreRootPoseData = true;

		public bool ignoreWristPoseData = true;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3[] bonePositions;

		public Quaternion[] boneRotations;

		public SteamVR_Skeleton_Pose_Hand(SteamVR_Input_Sources source)
		{
			inputSource = source;
		}

		public SteamVR_Skeleton_FingerExtensionTypes GetFingerExtensionType(int finger)
		{
			switch (finger)
			{
			case 0:
				return thumbFingerMovementType;
			case 1:
				return indexFingerMovementType;
			case 2:
				return middleFingerMovementType;
			case 3:
				return ringFingerMovementType;
			case 4:
				return pinkyFingerMovementType;
			default:
				Debug.LogWarning("Finger not in range!");
				return SteamVR_Skeleton_FingerExtensionTypes.Static;
			}
		}

		public SteamVR_Skeleton_FingerExtensionTypes GetMovementTypeForBone(int boneIndex)
		{
			return SteamVR_Skeleton_JointIndexes.GetFingerForBone(boneIndex) switch
			{
				0 => thumbFingerMovementType, 
				1 => indexFingerMovementType, 
				2 => middleFingerMovementType, 
				3 => ringFingerMovementType, 
				4 => pinkyFingerMovementType, 
				_ => SteamVR_Skeleton_FingerExtensionTypes.Static, 
			};
		}
	}
}
