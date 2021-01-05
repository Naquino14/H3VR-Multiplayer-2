namespace Valve.VR
{
	public interface ISteamVR_Action_In : ISteamVR_Action, ISteamVR_Action_In_Source, ISteamVR_Action_Source
	{
		void UpdateValues();

		string GetRenderModelComponentName(SteamVR_Input_Sources inputSource);

		SteamVR_Input_Sources GetActiveDevice(SteamVR_Input_Sources inputSource);

		uint GetDeviceIndex(SteamVR_Input_Sources inputSource);

		bool GetChanged(SteamVR_Input_Sources inputSource);

		string GetLocalizedOriginPart(SteamVR_Input_Sources inputSource, params EVRInputStringBits[] localizedParts);

		string GetLocalizedOrigin(SteamVR_Input_Sources inputSource);
	}
}
