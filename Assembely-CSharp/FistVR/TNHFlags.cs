using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class TNHFlags
	{
		public TNH_Char Char;

		public TNHSetting_ProgressionType ProgressionTypeSetting;

		public TNHSetting_HealthMode HealthModeSetting;

		public TNHSetting_EquipmentMode EquipmentModeSetting;

		public TNHModifier_AIDifficulty AIDifficultyModifier;

		public TNHModifier_RadarMode RadarModeModifier;

		public TNH_ItemSpawnerMode ItemSpawnerMode;

		public TNH_BackpackMode BackpackMode;

		public TNH_HealthMult HealthMult = TNH_HealthMult.Meaty;

		public TNH_BGAudioMode BGAudioMode;

		public TNH_AIVoiceMode AIVoiceMode;

		public TNH_RadarHand RadarHand;

		public void InitializeFromSaveFile()
		{
			if (ES2.Exists("TNH_v2.txt"))
			{
				Debug.Log("TNH_v2.txt exists, initializing from it");
				using ES2Reader eS2Reader = ES2Reader.Create("TNH_v2.txt");
				if (eS2Reader.TagExists("Char"))
				{
					Char = eS2Reader.Read<TNH_Char>("Char");
				}
				if (eS2Reader.TagExists("ProgressionTypeSetting"))
				{
					ProgressionTypeSetting = eS2Reader.Read<TNHSetting_ProgressionType>("ProgressionTypeSetting");
				}
				if (eS2Reader.TagExists("HealthModeSetting"))
				{
					HealthModeSetting = eS2Reader.Read<TNHSetting_HealthMode>("HealthModeSetting");
				}
				if (eS2Reader.TagExists("EquipmentModeSetting"))
				{
					EquipmentModeSetting = eS2Reader.Read<TNHSetting_EquipmentMode>("EquipmentModeSetting");
				}
				if (eS2Reader.TagExists("AIDifficultyModifier"))
				{
					AIDifficultyModifier = eS2Reader.Read<TNHModifier_AIDifficulty>("AIDifficultyModifier");
				}
				if (eS2Reader.TagExists("RadarModeModifier"))
				{
					RadarModeModifier = eS2Reader.Read<TNHModifier_RadarMode>("RadarModeModifier");
				}
				if (eS2Reader.TagExists("ItemSpawnerMode"))
				{
					ItemSpawnerMode = eS2Reader.Read<TNH_ItemSpawnerMode>("ItemSpawnerMode");
				}
				if (eS2Reader.TagExists("BackpackMode"))
				{
					BackpackMode = eS2Reader.Read<TNH_BackpackMode>("BackpackMode");
				}
				if (eS2Reader.TagExists("HealthMult"))
				{
					HealthMult = eS2Reader.Read<TNH_HealthMult>("HealthMult");
				}
				if (eS2Reader.TagExists("BGAudioMode"))
				{
					BGAudioMode = eS2Reader.Read<TNH_BGAudioMode>("BGAudioMode");
				}
				if (eS2Reader.TagExists("AIVoiceMode"))
				{
					AIVoiceMode = eS2Reader.Read<TNH_AIVoiceMode>("AIVoiceMode");
				}
				if (eS2Reader.TagExists("RadarHand"))
				{
					RadarHand = eS2Reader.Read<TNH_RadarHand>("RadarHand");
				}
			}
			else
			{
				Debug.Log("TNH_v2.txt does not exist, creating it");
				SaveToFile();
				InitializeFromSaveFile();
			}
		}

		public void SaveToFile()
		{
			using ES2Writer eS2Writer = ES2Writer.Create("TNH_v2.txt");
			eS2Writer.Write(Char, "Char");
			eS2Writer.Write(ProgressionTypeSetting, "ProgressionTypeSetting");
			eS2Writer.Write(HealthModeSetting, "HealthModeSetting");
			eS2Writer.Write(EquipmentModeSetting, "EquipmentModeSetting");
			eS2Writer.Write(AIDifficultyModifier, "AIDifficultyModifier");
			eS2Writer.Write(RadarModeModifier, "RadarModeModifier");
			eS2Writer.Write(ItemSpawnerMode, "ItemSpawnerMode");
			eS2Writer.Write(BackpackMode, "BackpackMode");
			eS2Writer.Write(HealthMult, "HealthMult");
			eS2Writer.Write(BGAudioMode, "BGAudioMode");
			eS2Writer.Write(AIVoiceMode, "AIVoiceMode");
			eS2Writer.Write(RadarHand, "RadarHand");
			eS2Writer.Save();
		}
	}
}
