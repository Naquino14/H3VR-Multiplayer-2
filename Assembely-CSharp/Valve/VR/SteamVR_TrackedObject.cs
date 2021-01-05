using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_TrackedObject : MonoBehaviour
	{
		public enum EIndex
		{
			None = -1,
			Hmd,
			Device1,
			Device2,
			Device3,
			Device4,
			Device5,
			Device6,
			Device7,
			Device8,
			Device9,
			Device10,
			Device11,
			Device12,
			Device13,
			Device14,
			Device15
		}

		public EIndex index;

		[Tooltip("If not set, relative to parent")]
		public Transform origin;

		private SteamVR_Events.Action newPosesAction;

		public bool isValid
		{
			get;
			private set;
		}

		private SteamVR_TrackedObject()
		{
			newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
		}

		private void OnNewPoses(TrackedDevicePose_t[] poses)
		{
			if (index == EIndex.None)
			{
				return;
			}
			int num = (int)index;
			isValid = false;
			if (poses.Length > num && poses[num].bDeviceIsConnected && poses[num].bPoseIsValid)
			{
				isValid = true;
				SteamVR_Utils.RigidTransform rigidTransform = new SteamVR_Utils.RigidTransform(poses[num].mDeviceToAbsoluteTracking);
				if (origin != null)
				{
					base.transform.position = origin.transform.TransformPoint(rigidTransform.pos);
					base.transform.rotation = origin.rotation * rigidTransform.rot;
				}
				else
				{
					base.transform.localPosition = rigidTransform.pos;
					base.transform.localRotation = rigidTransform.rot;
				}
			}
		}

		private void Awake()
		{
			OnEnable();
		}

		private void OnEnable()
		{
			SteamVR_Render instance = SteamVR_Render.instance;
			if (instance == null)
			{
				base.enabled = false;
			}
			else
			{
				newPosesAction.enabled = true;
			}
		}

		private void OnDisable()
		{
			newPosesAction.enabled = false;
			isValid = false;
		}

		public void SetDeviceIndex(int index)
		{
			if (Enum.IsDefined(typeof(EIndex), index))
			{
				this.index = (EIndex)index;
			}
		}
	}
}
