namespace Valve.VR
{
	public interface ISteamVR_Action : ISteamVR_Action_Source
	{
		bool GetActive(SteamVR_Input_Sources inputSource);

		string GetShortName();
	}
}
