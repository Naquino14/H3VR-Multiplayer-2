using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class MF_PlayAreaConfig
	{
		public MF_PlayArea PlayArea;

		public MF_Zone PlayerSpawnZone_Red;

		public MF_Zone PlayerSpawnZone_Blue;

		public List<MF_Zone> SpawnZones_RedTeam;

		public List<MF_Zone> SpawnZones_BlueTeam;

		public List<MF_Zone> PlayZones_RedTeam;

		public List<MF_Zone> PlayZones_BlueTeam;

		public List<MF_Zone> PlayZones_Neutral;

		public List<MF_Zone> GetZoneSet(MF_ZoneCategory cat)
		{
			return cat switch
			{
				MF_ZoneCategory.RedSide => PlayZones_RedTeam, 
				MF_ZoneCategory.BlueSide => PlayZones_BlueTeam, 
				MF_ZoneCategory.Neutral => PlayZones_Neutral, 
				MF_ZoneCategory.SpawnRed => SpawnZones_RedTeam, 
				MF_ZoneCategory.SpawnBlue => SpawnZones_BlueTeam, 
				_ => null, 
			};
		}
	}
}
