using System.Runtime.InteropServices;
using UnityEngine;

namespace Valve.VR
{
	public abstract class SteamVR_Action_In_Source : SteamVR_Action_Source, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		protected static uint inputOriginInfo_size;

		protected InputOriginInfo_t inputOriginInfo = default(InputOriginInfo_t);

		protected InputOriginInfo_t lastInputOriginInfo = default(InputOriginInfo_t);

		public bool isUpdating
		{
			get;
			set;
		}

		public float updateTime
		{
			get;
			protected set;
		}

		public abstract ulong activeOrigin
		{
			get;
		}

		public abstract ulong lastActiveOrigin
		{
			get;
		}

		public abstract bool changed
		{
			get;
			protected set;
		}

		public abstract bool lastChanged
		{
			get;
			protected set;
		}

		public SteamVR_Input_Sources activeDevice
		{
			get
			{
				UpdateOriginTrackedDeviceInfo();
				return SteamVR_Input_Source.GetSource(inputOriginInfo.devicePath);
			}
		}

		public uint trackedDeviceIndex
		{
			get
			{
				UpdateOriginTrackedDeviceInfo();
				return inputOriginInfo.trackedDeviceIndex;
			}
		}

		public string renderModelComponentName
		{
			get
			{
				UpdateOriginTrackedDeviceInfo();
				return inputOriginInfo.rchRenderModelComponentName;
			}
		}

		public string localizedOriginName
		{
			get
			{
				UpdateOriginTrackedDeviceInfo();
				return GetLocalizedOrigin();
			}
		}

		public float changedTime
		{
			get;
			protected set;
		}

		protected int lastOriginGetFrame
		{
			get;
			set;
		}

		public abstract void UpdateValue();

		public override void Initialize()
		{
			base.Initialize();
			if (inputOriginInfo_size == 0)
			{
				inputOriginInfo_size = (uint)Marshal.SizeOf(typeof(InputOriginInfo_t));
			}
		}

		protected void UpdateOriginTrackedDeviceInfo()
		{
			if (lastOriginGetFrame != Time.frameCount)
			{
				EVRInputError originTrackedDeviceInfo = OpenVR.Input.GetOriginTrackedDeviceInfo(activeOrigin, ref inputOriginInfo, inputOriginInfo_size);
				if (originTrackedDeviceInfo != 0)
				{
					Debug.LogError("<b>[SteamVR]</b> GetOriginTrackedDeviceInfo error (" + base.fullPath + "): " + originTrackedDeviceInfo.ToString() + " handle: " + base.handle.ToString() + " activeOrigin: " + activeOrigin.ToString() + " active: " + active);
				}
				lastInputOriginInfo = inputOriginInfo;
				lastOriginGetFrame = Time.frameCount;
			}
		}

		public string GetLocalizedOriginPart(params EVRInputStringBits[] localizedParts)
		{
			UpdateOriginTrackedDeviceInfo();
			if (active)
			{
				return SteamVR_Input.GetLocalizedName(activeOrigin, localizedParts);
			}
			return null;
		}

		public string GetLocalizedOrigin()
		{
			UpdateOriginTrackedDeviceInfo();
			if (active)
			{
				return SteamVR_Input.GetLocalizedName(activeOrigin, EVRInputStringBits.VRInputString_All);
			}
			return null;
		}
	}
}
