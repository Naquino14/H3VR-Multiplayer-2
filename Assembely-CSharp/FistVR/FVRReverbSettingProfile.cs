using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New ReverbSetting Profile", menuName = "Audio/ReverbProfile", order = 0)]
	public class FVRReverbSettingProfile : ScriptableObject
	{
		public SM.ReverbSettings Settings;
	}
}
