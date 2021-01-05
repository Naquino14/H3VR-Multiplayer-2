using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Behaviour_Pose : MonoBehaviour
	{
		public delegate void ActiveChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, bool active);

		public delegate void ChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource);

		public delegate void UpdateHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource);

		public delegate void TrackingChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, bool deviceConnected);

		public delegate void DeviceIndexChangedHandler(SteamVR_Behaviour_Pose fromAction, SteamVR_Input_Sources fromSource, int newDeviceIndex);

		public SteamVR_Action_Pose poseAction = SteamVR_Input.GetAction<SteamVR_Action_Pose>("Pose");

		[Tooltip("The device this action should apply to. Any if the action is not device specific.")]
		public SteamVR_Input_Sources inputSource;

		[Tooltip("If not set, relative to parent")]
		public Transform origin;

		public SteamVR_Behaviour_PoseEvent onTransformUpdated;

		public SteamVR_Behaviour_PoseEvent onTransformChanged;

		public SteamVR_Behaviour_Pose_ConnectedChangedEvent onConnectedChanged;

		public SteamVR_Behaviour_Pose_TrackingChangedEvent onTrackingChanged;

		public SteamVR_Behaviour_Pose_DeviceIndexChangedEvent onDeviceIndexChanged;

		public UpdateHandler onTransformUpdatedEvent;

		public ChangeHandler onTransformChangedEvent;

		public DeviceConnectedChangeHandler onConnectedChangedEvent;

		public TrackingChangeHandler onTrackingChangedEvent;

		public DeviceIndexChangedHandler onDeviceIndexChangedEvent;

		[Tooltip("Can be disabled to stop broadcasting bound device status changes")]
		public bool broadcastDeviceChanges = true;

		protected int deviceIndex = -1;

		protected SteamVR_HistoryBuffer historyBuffer = new SteamVR_HistoryBuffer(30);

		protected int lastFrameUpdated;

		public bool isValid => poseAction[inputSource].poseIsValid;

		public bool isActive => poseAction[inputSource].active;

		protected virtual void Start()
		{
			if (poseAction == null)
			{
				Debug.LogError("<b>[SteamVR]</b> No pose action set for this component");
				return;
			}
			CheckDeviceIndex();
			if (origin == null)
			{
				origin = base.transform.parent;
			}
		}

		protected virtual void OnEnable()
		{
			SteamVR.Initialize();
			if (poseAction != null)
			{
				poseAction[inputSource].onUpdate += SteamVR_Behaviour_Pose_OnUpdate;
				poseAction[inputSource].onDeviceConnectedChanged += OnDeviceConnectedChanged;
				poseAction[inputSource].onTrackingChanged += OnTrackingChanged;
				poseAction[inputSource].onChange += SteamVR_Behaviour_Pose_OnChange;
			}
		}

		protected virtual void OnDisable()
		{
			if (poseAction != null)
			{
				poseAction[inputSource].onUpdate -= SteamVR_Behaviour_Pose_OnUpdate;
				poseAction[inputSource].onDeviceConnectedChanged -= OnDeviceConnectedChanged;
				poseAction[inputSource].onTrackingChanged -= OnTrackingChanged;
				poseAction[inputSource].onChange -= SteamVR_Behaviour_Pose_OnChange;
			}
			historyBuffer.Clear();
		}

		private void SteamVR_Behaviour_Pose_OnUpdate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
		{
			UpdateHistoryBuffer();
			UpdateTransform();
			if (onTransformUpdated != null)
			{
				onTransformUpdated.Invoke(this, inputSource);
			}
			if (onTransformUpdatedEvent != null)
			{
				onTransformUpdatedEvent(this, inputSource);
			}
		}

		protected virtual void UpdateTransform()
		{
			CheckDeviceIndex();
			if (!(poseAction == null) && poseAction.active)
			{
				if (origin != null)
				{
					base.transform.position = origin.transform.TransformPoint(poseAction[inputSource].localPosition);
					base.transform.rotation = origin.rotation * poseAction[inputSource].localRotation;
				}
				else
				{
					base.transform.localPosition = poseAction[inputSource].localPosition;
					base.transform.localRotation = poseAction[inputSource].localRotation;
				}
			}
		}

		private void SteamVR_Behaviour_Pose_OnChange(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
		{
			if (onTransformChanged != null)
			{
				onTransformChanged.Invoke(this, fromSource);
			}
			if (onTransformChangedEvent != null)
			{
				onTransformChangedEvent(this, fromSource);
			}
		}

		protected virtual void OnDeviceConnectedChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, bool connected)
		{
			CheckDeviceIndex();
			if (onConnectedChanged != null)
			{
				onConnectedChanged.Invoke(this, inputSource, connected);
			}
			if (onConnectedChangedEvent != null)
			{
				onConnectedChangedEvent(this, inputSource, connected);
			}
		}

		protected virtual void OnTrackingChanged(SteamVR_Action_Pose changedAction, SteamVR_Input_Sources changedSource, ETrackingResult trackingChanged)
		{
			if (onTrackingChanged != null)
			{
				onTrackingChanged.Invoke(this, inputSource, trackingChanged);
			}
			if (onTrackingChangedEvent != null)
			{
				onTrackingChangedEvent(this, inputSource, trackingChanged);
			}
		}

		protected virtual void CheckDeviceIndex()
		{
			if (!poseAction[inputSource].active || !poseAction[inputSource].deviceIsConnected)
			{
				return;
			}
			int trackedDeviceIndex = (int)poseAction[inputSource].trackedDeviceIndex;
			if (deviceIndex != trackedDeviceIndex)
			{
				deviceIndex = trackedDeviceIndex;
				if (broadcastDeviceChanges)
				{
					base.gameObject.BroadcastMessage("SetInputSource", inputSource, SendMessageOptions.DontRequireReceiver);
					base.gameObject.BroadcastMessage("SetDeviceIndex", deviceIndex, SendMessageOptions.DontRequireReceiver);
				}
				if (onDeviceIndexChanged != null)
				{
					onDeviceIndexChanged.Invoke(this, inputSource, deviceIndex);
				}
				if (onDeviceIndexChangedEvent != null)
				{
					onDeviceIndexChangedEvent(this, inputSource, deviceIndex);
				}
			}
		}

		public int GetDeviceIndex()
		{
			if (deviceIndex == -1)
			{
				CheckDeviceIndex();
			}
			return deviceIndex;
		}

		public Vector3 GetVelocity()
		{
			return poseAction[inputSource].velocity;
		}

		public Vector3 GetAngularVelocity()
		{
			return poseAction[inputSource].angularVelocity;
		}

		public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
		{
			return poseAction[inputSource].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
		}

		public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
		{
			int topVelocity = historyBuffer.GetTopVelocity(10, 1);
			historyBuffer.GetAverageVelocities(out velocity, out angularVelocity, 2, topVelocity);
		}

		protected void UpdateHistoryBuffer()
		{
			int frameCount = Time.frameCount;
			if (lastFrameUpdated != frameCount)
			{
				historyBuffer.Update(poseAction[inputSource].localPosition, poseAction[inputSource].localRotation, poseAction[inputSource].velocity, poseAction[inputSource].angularVelocity);
				lastFrameUpdated = frameCount;
			}
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			if (poseAction != null)
			{
				return poseAction.GetLocalizedOriginPart(inputSource, localizedParts);
			}
			return null;
		}
	}
}
