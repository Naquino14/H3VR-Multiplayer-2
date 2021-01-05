using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public abstract class SteamVR_Action_Pose_Base<SourceMap, SourceElement> : SteamVR_Action_In<SourceMap, SourceElement>, ISteamVR_Action_Pose, ISteamVR_Action_In_Source, ISteamVR_Action_Source where SourceMap : SteamVR_Action_Pose_Source_Map<SourceElement>, new()where SourceElement : SteamVR_Action_Pose_Source, new()
	{
		public Vector3 localPosition
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.localPosition;
			}
		}

		public Quaternion localRotation
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.localRotation;
			}
		}

		public ETrackingResult trackingState
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.trackingState;
			}
		}

		public Vector3 velocity
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.velocity;
			}
		}

		public Vector3 angularVelocity
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.angularVelocity;
			}
		}

		public bool poseIsValid
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.poseIsValid;
			}
		}

		public bool deviceIsConnected
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.deviceIsConnected;
			}
		}

		public Vector3 lastLocalPosition
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastLocalPosition;
			}
		}

		public Quaternion lastLocalRotation
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastLocalRotation;
			}
		}

		public ETrackingResult lastTrackingState
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastTrackingState;
			}
		}

		public Vector3 lastVelocity
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastVelocity;
			}
		}

		public Vector3 lastAngularVelocity
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastAngularVelocity;
			}
		}

		public bool lastPoseIsValid
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastPoseIsValid;
			}
		}

		public bool lastDeviceIsConnected
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastDeviceIsConnected;
			}
		}

		public SteamVR_Action_Pose_Base()
		{
		}

		protected static void SetUniverseOrigin(ETrackingUniverseOrigin newOrigin)
		{
			for (int i = 0; i < SteamVR_Input.actionsPose.Length; i++)
			{
				SteamVR_Input.actionsPose[i].sourceMap.SetTrackingUniverseOrigin(newOrigin);
			}
			for (int j = 0; j < SteamVR_Input.actionsSkeleton.Length; j++)
			{
				SteamVR_Input.actionsSkeleton[j].sourceMap.SetTrackingUniverseOrigin(newOrigin);
			}
		}

		public virtual void UpdateValues(bool skipStateAndEventUpdates)
		{
			sourceMap.UpdateValues(skipStateAndEventUpdates);
		}

		public bool GetVelocitiesAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
		{
			SourceElement val = sourceMap[inputSource];
			return val.GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
		}

		public bool GetPoseAtTimeOffset(SteamVR_Input_Sources inputSource, float secondsFromNow, out Vector3 localPosition, out Quaternion localRotation, out Vector3 velocity, out Vector3 angularVelocity)
		{
			SourceElement val = sourceMap[inputSource];
			return val.GetPoseAtTimeOffset(secondsFromNow, out localPosition, out localRotation, out velocity, out angularVelocity);
		}

		public virtual void UpdateTransform(SteamVR_Input_Sources inputSource, Transform transformToUpdate)
		{
			SourceElement val = sourceMap[inputSource];
			val.UpdateTransform(transformToUpdate);
		}

		public Vector3 GetLocalPosition(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.localPosition;
		}

		public Quaternion GetLocalRotation(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.localRotation;
		}

		public Vector3 GetVelocity(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.velocity;
		}

		public Vector3 GetAngularVelocity(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.angularVelocity;
		}

		public bool GetDeviceIsConnected(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.deviceIsConnected;
		}

		public bool GetPoseIsValid(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.poseIsValid;
		}

		public ETrackingResult GetTrackingResult(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.trackingState;
		}

		public Vector3 GetLastLocalPosition(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastLocalPosition;
		}

		public Quaternion GetLastLocalRotation(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastLocalRotation;
		}

		public Vector3 GetLastVelocity(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastVelocity;
		}

		public Vector3 GetLastAngularVelocity(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastAngularVelocity;
		}

		public bool GetLastDeviceIsConnected(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastDeviceIsConnected;
		}

		public bool GetLastPoseIsValid(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastPoseIsValid;
		}

		public ETrackingResult GetLastTrackingResult(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.lastTrackingState;
		}
	}
}
