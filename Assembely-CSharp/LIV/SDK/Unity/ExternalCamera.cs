using System.Linq;
using System.Text;
using UnityEngine;
using Valve.VR;

namespace LIV.SDK.Unity
{
	[AddComponentMenu("LIV/ExternalCamera")]
	public class ExternalCamera : MonoBehaviour
	{
		private TrackedDevicePose_t _trackedDevicePose;

		private TrackedDevicePose_t[] _devices = new TrackedDevicePose_t[64];

		private TrackedDevicePose_t[] _gameDevices = new TrackedDevicePose_t[64];

		public bool IsValid => OpenVRTrackedDeviceId != uint.MaxValue;

		public uint OpenVRTrackedDeviceId
		{
			get;
			protected set;
		}

		private void OnEnable()
		{
			InvokeRepeating("UpdateOpenVRDevice", 0.5f, 0.5f);
			UpdateOpenVRDevice();
		}

		private void OnDisable()
		{
			CancelInvoke("UpdateOpenVRDevice");
		}

		private void LateUpdate()
		{
			UpdateCamera();
		}

		private void OnPreCull()
		{
			UpdateCamera();
		}

		private void UpdateCamera()
		{
			if (OpenVRTrackedDeviceId == uint.MaxValue)
			{
				return;
			}
			UpdateOpenVRPose();
			if (!_trackedDevicePose.bDeviceIsConnected)
			{
				UpdateOpenVRDevice();
				if (OpenVRTrackedDeviceId == uint.MaxValue)
				{
					return;
				}
				UpdateOpenVRPose();
			}
			UpdateTransform(_trackedDevicePose.mDeviceToAbsoluteTracking);
		}

		private void UpdateOpenVRPose()
		{
			if (OpenVR.Compositor.GetLastPoses(_devices, _gameDevices) == EVRCompositorError.None)
			{
				_trackedDevicePose = _devices[OpenVRTrackedDeviceId];
			}
		}

		private void UpdateTransform(HmdMatrix34_t deviceToAbsolute)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity[0, 0] = deviceToAbsolute.m0;
			identity[0, 1] = deviceToAbsolute.m1;
			identity[0, 2] = 0f - deviceToAbsolute.m2;
			identity[0, 3] = deviceToAbsolute.m3;
			identity[1, 0] = deviceToAbsolute.m4;
			identity[1, 1] = deviceToAbsolute.m5;
			identity[1, 2] = 0f - deviceToAbsolute.m6;
			identity[1, 3] = deviceToAbsolute.m7;
			identity[2, 0] = 0f - deviceToAbsolute.m8;
			identity[2, 1] = 0f - deviceToAbsolute.m9;
			identity[2, 2] = deviceToAbsolute.m10;
			identity[2, 3] = 0f - deviceToAbsolute.m11;
			base.transform.localPosition = identity.GetPosition();
			base.transform.localRotation = identity.GetRotation();
		}

		private void UpdateOpenVRDevice()
		{
			OpenVRTrackedDeviceId = IdentifyExternalCameraDevice();
		}

		private uint IdentifyExternalCameraDevice()
		{
			EVRCompositorError lastPoses = OpenVR.Compositor.GetLastPoses(_devices, _gameDevices);
			if (lastPoses != 0)
			{
				Debug.Log("Encountered an error whilst looking for the external camera: " + lastPoses);
				return uint.MaxValue;
			}
			var source = (from candidate in _devices.Select((TrackedDevicePose_t pose, int index) => new
				{
					pose = pose,
					index = (uint)index
				})
				where candidate.pose.bDeviceIsConnected
				select new
				{
					pose = candidate.pose,
					index = candidate.index,
					deviceClass = OpenVR.System.GetTrackedDeviceClass(candidate.index)
				} into candidate
				where candidate.deviceClass == ETrackedDeviceClass.Controller || candidate.deviceClass == ETrackedDeviceClass.GenericTracker
				select new
				{
					pose = candidate.pose,
					index = candidate.index,
					deviceClass = candidate.deviceClass,
					deviceRole = OpenVR.System.GetControllerRoleForTrackedDeviceIndex(candidate.index),
					modelNumber = GetStringTrackedDeviceProperty(candidate.index, ETrackedDeviceProperty.Prop_ModelNumber_String),
					renderModel = GetStringTrackedDeviceProperty(candidate.index, ETrackedDeviceProperty.Prop_RenderModelName_String)
				}).OrderByDescending(candidate =>
			{
				if (candidate.modelNumber == "LIV Virtual Camera")
				{
					return 10;
				}
				if (candidate.modelNumber == "Virtual Controller Device")
				{
					return 9;
				}
				if (candidate.deviceClass == ETrackedDeviceClass.GenericTracker)
				{
					return 5;
				}
				if (candidate.deviceClass == ETrackedDeviceClass.Controller)
				{
					if (candidate.renderModel == "{htc}vr_tracker_vive_1_0")
					{
						return 8;
					}
					if (candidate.deviceRole == ETrackedControllerRole.OptOut)
					{
						return 7;
					}
					if (candidate.deviceRole == ETrackedControllerRole.Invalid)
					{
						return 6;
					}
					return 1;
				}
				return 0;
			});
			return (uint)(((int?)source.FirstOrDefault()?.index) ?? (-1));
		}

		private static string GetStringTrackedDeviceProperty(uint device, ETrackedDeviceProperty property)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			ETrackedPropertyError pError = ETrackedPropertyError.TrackedProp_Success;
			OpenVR.System.GetStringTrackedDeviceProperty(device, property, stringBuilder, 1024u, ref pError);
			switch (pError)
			{
			case ETrackedPropertyError.TrackedProp_Success:
				return stringBuilder.ToString();
			case ETrackedPropertyError.TrackedProp_UnknownProperty:
				return string.Empty;
			default:
				Debug.Log("Encountered an error whilst reading a tracked device property: " + pError);
				return null;
			}
		}
	}
}
