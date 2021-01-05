using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Pose : SteamVR_Action_Pose_Base<SteamVR_Action_Pose_Source_Map<SteamVR_Action_Pose_Source>, SteamVR_Action_Pose_Source>, ISerializationCallbackReceiver
	{
		public delegate void ActiveChangeHandler(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource);

		public delegate void UpdateHandler(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource);

		public delegate void TrackingChangeHandler(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, bool deviceConnected);

		public event ActiveChangeHandler onActiveChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
			}
		}

		public event ActiveChangeHandler onActiveBindingChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
			}
		}

		public event ChangeHandler onChange
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onChange += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
			}
		}

		public event UpdateHandler onUpdate
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
			}
		}

		public event TrackingChangeHandler onTrackingChanged
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= value;
			}
		}

		public event ValidPoseChangeHandler onValidPoseChanged
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= value;
			}
		}

		public event DeviceConnectedChangeHandler onDeviceConnectedChanged
		{
			add
			{
				sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += value;
			}
			remove
			{
				sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= value;
			}
		}

		public void AddOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, DeviceConnectedChangeHandler functionToCall)
		{
			sourceMap[inputSource].onDeviceConnectedChanged += functionToCall;
		}

		public void RemoveOnDeviceConnectedChanged(SteamVR_Input_Sources inputSource, DeviceConnectedChangeHandler functionToStopCalling)
		{
			sourceMap[inputSource].onDeviceConnectedChanged -= functionToStopCalling;
		}

		public void AddOnTrackingChanged(SteamVR_Input_Sources inputSource, TrackingChangeHandler functionToCall)
		{
			sourceMap[inputSource].onTrackingChanged += functionToCall;
		}

		public void RemoveOnTrackingChanged(SteamVR_Input_Sources inputSource, TrackingChangeHandler functionToStopCalling)
		{
			sourceMap[inputSource].onTrackingChanged -= functionToStopCalling;
		}

		public void AddOnValidPoseChanged(SteamVR_Input_Sources inputSource, ValidPoseChangeHandler functionToCall)
		{
			sourceMap[inputSource].onValidPoseChanged += functionToCall;
		}

		public void RemoveOnValidPoseChanged(SteamVR_Input_Sources inputSource, ValidPoseChangeHandler functionToStopCalling)
		{
			sourceMap[inputSource].onValidPoseChanged -= functionToStopCalling;
		}

		public void AddOnActiveChangeListener(SteamVR_Input_Sources inputSource, ActiveChangeHandler functionToCall)
		{
			sourceMap[inputSource].onActiveChange += functionToCall;
		}

		public void RemoveOnActiveChangeListener(SteamVR_Input_Sources inputSource, ActiveChangeHandler functionToStopCalling)
		{
			sourceMap[inputSource].onActiveChange -= functionToStopCalling;
		}

		public void AddOnChangeListener(SteamVR_Input_Sources inputSource, ChangeHandler functionToCall)
		{
			sourceMap[inputSource].onChange += functionToCall;
		}

		public void RemoveOnChangeListener(SteamVR_Input_Sources inputSource, ChangeHandler functionToStopCalling)
		{
			sourceMap[inputSource].onChange -= functionToStopCalling;
		}

		public void AddOnUpdateListener(SteamVR_Input_Sources inputSource, UpdateHandler functionToCall)
		{
			sourceMap[inputSource].onUpdate += functionToCall;
		}

		public void RemoveOnUpdateListener(SteamVR_Input_Sources inputSource, UpdateHandler functionToStopCalling)
		{
			sourceMap[inputSource].onUpdate -= functionToStopCalling;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			InitAfterDeserialize();
		}

		public static void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newOrigin)
		{
			SteamVR_Action_Pose_Base<SteamVR_Action_Pose_Source_Map<SteamVR_Action_Pose_Source>, SteamVR_Action_Pose_Source>.SetUniverseOrigin(newOrigin);
			OpenVR.Compositor.SetTrackingSpace(newOrigin);
		}
	}
}
