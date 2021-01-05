namespace Valve.VR
{
	public interface ISteamVR_Action_In_Source : ISteamVR_Action_Source
	{
		bool changed
		{
			get;
		}

		bool lastChanged
		{
			get;
		}

		float changedTime
		{
			get;
		}

		float updateTime
		{
			get;
		}

		ulong activeOrigin
		{
			get;
		}

		ulong lastActiveOrigin
		{
			get;
		}

		SteamVR_Input_Sources activeDevice
		{
			get;
		}

		uint trackedDeviceIndex
		{
			get;
		}

		string renderModelComponentName
		{
			get;
		}

		string localizedOriginName
		{
			get;
		}
	}
}
