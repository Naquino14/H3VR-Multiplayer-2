using UnityEngine;

namespace Valve.VR
{
	public interface ISteamVR_Action_Pose : ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		Vector3 localPosition
		{
			get;
		}

		Quaternion localRotation
		{
			get;
		}

		ETrackingResult trackingState
		{
			get;
		}

		Vector3 velocity
		{
			get;
		}

		Vector3 angularVelocity
		{
			get;
		}

		bool poseIsValid
		{
			get;
		}

		bool deviceIsConnected
		{
			get;
		}

		Vector3 lastLocalPosition
		{
			get;
		}

		Quaternion lastLocalRotation
		{
			get;
		}

		ETrackingResult lastTrackingState
		{
			get;
		}

		Vector3 lastVelocity
		{
			get;
		}

		Vector3 lastAngularVelocity
		{
			get;
		}

		bool lastPoseIsValid
		{
			get;
		}

		bool lastDeviceIsConnected
		{
			get;
		}
	}
}
