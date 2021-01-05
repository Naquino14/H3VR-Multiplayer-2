namespace Valve.VR
{
	public class SteamVR_Action_Vibration_Source_Map : SteamVR_Action_Source_Map<SteamVR_Action_Vibration_Source>
	{
		public bool IsUpdating(SteamVR_Input_Sources inputSource)
		{
			return sources[inputSource].timeLastExecuted != 0f;
		}
	}
}
