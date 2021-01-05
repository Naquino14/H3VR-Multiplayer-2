using System;

namespace Valve.VR
{
	[Serializable]
	public abstract class SteamVR_Action_In<SourceMap, SourceElement> : SteamVR_Action<SourceMap, SourceElement>, ISteamVR_Action_In, ISteamVR_Action, ISteamVR_Action_In_Source, ISteamVR_Action_Source where SourceMap : SteamVR_Action_In_Source_Map<SourceElement>, new()where SourceElement : SteamVR_Action_In_Source, new()
	{
		public bool changed
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.changed;
			}
		}

		public bool lastChanged
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.changed;
			}
		}

		public float changedTime
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.changedTime;
			}
		}

		public float updateTime
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.updateTime;
			}
		}

		public ulong activeOrigin
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.activeOrigin;
			}
		}

		public ulong lastActiveOrigin
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.lastActiveOrigin;
			}
		}

		public SteamVR_Input_Sources activeDevice
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.activeDevice;
			}
		}

		public uint trackedDeviceIndex
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.trackedDeviceIndex;
			}
		}

		public string renderModelComponentName
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.renderModelComponentName;
			}
		}

		public string localizedOriginName
		{
			get
			{
				SourceElement val = sourceMap[SteamVR_Input_Sources.Any];
				return val.localizedOriginName;
			}
		}

		public virtual void UpdateValues()
		{
			sourceMap.UpdateValues();
		}

		public virtual string GetRenderModelComponentName(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.renderModelComponentName;
		}

		public virtual SteamVR_Input_Sources GetActiveDevice(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.activeDevice;
		}

		public virtual uint GetDeviceIndex(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.trackedDeviceIndex;
		}

		public virtual bool GetChanged(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.changed;
		}

		public override float GetTimeLastChanged(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.changedTime;
		}

		public string GetLocalizedOriginPart(SteamVR_Input_Sources inputSource, params EVRInputStringBits[] localizedParts)
		{
			SourceElement val = sourceMap[inputSource];
			return val.GetLocalizedOriginPart(localizedParts);
		}

		public string GetLocalizedOrigin(SteamVR_Input_Sources inputSource)
		{
			SourceElement val = sourceMap[inputSource];
			return val.GetLocalizedOrigin();
		}

		public override bool IsUpdating(SteamVR_Input_Sources inputSource)
		{
			return sourceMap.IsUpdating(inputSource);
		}

		public void ForceAddSourceToUpdateList(SteamVR_Input_Sources inputSource)
		{
			sourceMap.ForceAddSourceToUpdateList(inputSource);
		}
	}
}
