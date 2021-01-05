using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class MF2Flags
	{
		public MF_TeamColor PlayerTeam;

		public int PlayerSetting_HealthIndex = 4;

		public float[] PlayerSetting_HealthSettings = new float[6]
		{
			1000f,
			2500f,
			5000f,
			10000f,
			50000f,
			100000f
		};

		public MF_BlastJumping PlayerSetting_BlastJumping;

		public MF_BlastJumpingSelfDamage PlayerSetting_BlastJumpingSelfDamage = MF_BlastJumpingSelfDamage.Arcade;

		public int[] TDM_TeamSizes = new int[6]
		{
			0,
			1,
			3,
			6,
			10,
			16
		};

		public MF_TeamColor TDMOption_PlayerTeam;

		public int TDMOption_RedTeamSizeIndex = 4;

		public int TDMOption_BlueTeamSizeIndex = 4;

		public MF_SpawnSpeed TDMOption_RedTeamSpeed = MF_SpawnSpeed.Standard;

		public MF_SpawnSpeed TDMOption_BlueTeamSpeed = MF_SpawnSpeed.Standard;

		public MF_PlayArea TDMOption_PlayArea;

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("MF.txt"))
			{
				Debug.Log("MF.txt exists, initializing from it");
				using ES2Reader eS2Reader = ES2Reader.Create("MF.txt");
				if (eS2Reader.TagExists("PlayerSetting_HealthIndex"))
				{
					PlayerSetting_HealthIndex = eS2Reader.Read<int>("PlayerSetting_HealthIndex");
				}
				if (eS2Reader.TagExists("PlayerSetting_BlastJumping"))
				{
					PlayerSetting_BlastJumping = eS2Reader.Read<MF_BlastJumping>("PlayerSetting_BlastJumping");
				}
				if (eS2Reader.TagExists("PlayerSetting_BlastJumpingSelfDamage"))
				{
					PlayerSetting_BlastJumpingSelfDamage = eS2Reader.Read<MF_BlastJumpingSelfDamage>("PlayerSetting_BlastJumpingSelfDamage");
				}
				if (eS2Reader.TagExists("TDMOption_PlayerTeam"))
				{
					TDMOption_PlayerTeam = eS2Reader.Read<MF_TeamColor>("TDMOption_PlayerTeam");
				}
				if (eS2Reader.TagExists("TDMOption_RedTeamSizeIndex"))
				{
					TDMOption_RedTeamSizeIndex = eS2Reader.Read<int>("TDMOption_RedTeamSizeIndex");
				}
				if (eS2Reader.TagExists("TDMOption_BlueTeamSizeIndex"))
				{
					TDMOption_BlueTeamSizeIndex = eS2Reader.Read<int>("TDMOption_BlueTeamSizeIndex");
				}
				if (eS2Reader.TagExists("TDMOption_RedTeamSpeed"))
				{
					TDMOption_RedTeamSpeed = eS2Reader.Read<MF_SpawnSpeed>("TDMOption_RedTeamSpeed");
				}
				if (eS2Reader.TagExists("TDMOption_BlueTeamSpeed"))
				{
					TDMOption_BlueTeamSpeed = eS2Reader.Read<MF_SpawnSpeed>("TDMOption_BlueTeamSpeed");
				}
				if (eS2Reader.TagExists("TDMOption_PlayArea"))
				{
					TDMOption_PlayArea = eS2Reader.Read<MF_PlayArea>("TDMOption_PlayArea");
				}
			}
			else
			{
				Debug.Log("MF.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("MF.txt");
			eS2Writer.Write(PlayerSetting_HealthIndex, "PlayerSetting_HealthIndex");
			eS2Writer.Write(PlayerSetting_BlastJumping, "PlayerSetting_BlastJumping");
			eS2Writer.Write(PlayerSetting_BlastJumpingSelfDamage, "PlayerSetting_BlastJumpingSelfDamage");
			eS2Writer.Write(TDMOption_PlayerTeam, "TDMOption_PlayerTeam");
			eS2Writer.Write(TDMOption_RedTeamSizeIndex, "TDMOption_RedTeamSizeIndex");
			eS2Writer.Write(TDMOption_BlueTeamSizeIndex, "TDMOption_BlueTeamSizeIndex");
			eS2Writer.Write(TDMOption_RedTeamSpeed, "TDMOption_RedTeamSpeed");
			eS2Writer.Write(TDMOption_BlueTeamSpeed, "TDMOption_BlueTeamSpeed");
			eS2Writer.Write(TDMOption_PlayArea, "TDMOption_PlayArea");
			eS2Writer.Save();
		}
	}
}
