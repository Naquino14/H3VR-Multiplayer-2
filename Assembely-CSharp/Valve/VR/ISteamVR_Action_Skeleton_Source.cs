using UnityEngine;

namespace Valve.VR
{
	public interface ISteamVR_Action_Skeleton_Source
	{
		EVRSkeletalTrackingLevel skeletalTrackingLevel
		{
			get;
		}

		Vector3[] bonePositions
		{
			get;
		}

		Quaternion[] boneRotations
		{
			get;
		}

		Vector3[] lastBonePositions
		{
			get;
		}

		Quaternion[] lastBoneRotations
		{
			get;
		}

		EVRSkeletalMotionRange rangeOfMotion
		{
			get;
			set;
		}

		EVRSkeletalTransformSpace skeletalTransformSpace
		{
			get;
			set;
		}

		bool onlyUpdateSummaryData
		{
			get;
			set;
		}

		float thumbCurl
		{
			get;
		}

		float indexCurl
		{
			get;
		}

		float middleCurl
		{
			get;
		}

		float ringCurl
		{
			get;
		}

		float pinkyCurl
		{
			get;
		}

		float thumbIndexSplay
		{
			get;
		}

		float indexMiddleSplay
		{
			get;
		}

		float middleRingSplay
		{
			get;
		}

		float ringPinkySplay
		{
			get;
		}

		float lastThumbCurl
		{
			get;
		}

		float lastIndexCurl
		{
			get;
		}

		float lastMiddleCurl
		{
			get;
		}

		float lastRingCurl
		{
			get;
		}

		float lastPinkyCurl
		{
			get;
		}

		float lastThumbIndexSplay
		{
			get;
		}

		float lastIndexMiddleSplay
		{
			get;
		}

		float lastMiddleRingSplay
		{
			get;
		}

		float lastRingPinkySplay
		{
			get;
		}

		float[] fingerCurls
		{
			get;
		}

		float[] fingerSplays
		{
			get;
		}

		float[] lastFingerCurls
		{
			get;
		}

		float[] lastFingerSplays
		{
			get;
		}
	}
}
