using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class MF_ZoneModeConfig
	{
		public MF_GameMode Mode = MF_GameMode.TeamDM;

		public List<MF_PlayAreaConfig> PlayAreaConfigs;

		public MF_PlayAreaConfig GetPlayAreaConfig(MF_PlayArea area)
		{
			for (int i = 0; i < PlayAreaConfigs.Count; i++)
			{
				if (PlayAreaConfigs[i].PlayArea == area)
				{
					return PlayAreaConfigs[i];
				}
			}
			return null;
		}
	}
}
