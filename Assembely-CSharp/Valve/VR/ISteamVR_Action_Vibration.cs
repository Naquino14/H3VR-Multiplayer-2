namespace Valve.VR
{
	public interface ISteamVR_Action_Vibration : ISteamVR_Action_Out, ISteamVR_Action, ISteamVR_Action_Out_Source, ISteamVR_Action_Source
	{
		void Execute(float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource);
	}
}
