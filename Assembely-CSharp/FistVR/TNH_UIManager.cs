using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class TNH_UIManager : MonoBehaviour
	{
		[Serializable]
		public class CharacterCategory
		{
			public string CategoryName;

			public List<TNH_Char> Characters;
		}

		[Header("Level Options")]
		public string LevelName = "Classic";

		[Header("Category/Character Section")]
		public List<Text> LBL_CategoryName;

		public List<Text> LBL_CharacterName;

		public Image SelectedCharacter_Image;

		public Text SelectedCharacter_Title;

		public Text SelectedCharacter_Description;

		private int m_selectedCategory;

		private int m_selectedCharacter;

		public OptionsPanel_ButtonSet OBS_CharCategory;

		public GameObject CustomHealthOptions;

		[Header("Main Options")]
		public OptionsPanel_ButtonSet OBS_Progression;

		public OptionsPanel_ButtonSet OBS_EquipmentMode;

		public OptionsPanel_ButtonSet OBS_HealthMode;

		public OptionsPanel_ButtonSet OBS_AIDifficulty;

		public OptionsPanel_ButtonSet OBS_AIRadarMode;

		public OptionsPanel_ButtonSet OBS_HealthMult;

		public OptionsPanel_ButtonSet OBS_ItemSpawner;

		public OptionsPanel_ButtonSet OBS_Backpack;

		public OptionsPanel_ButtonSet OBS_BGAudio;

		public OptionsPanel_ButtonSet OBS_AINarration;

		public OptionsPanel_ButtonSet OBS_RadarHand;

		[Header("Audio")]
		public List<AudioEvent> ButtonSoundEvents;

		public TNH_CharacterDatabase CharDatabase;

		public List<CharacterCategory> Categories;

		[Header("Scoring")]
		public TNH_ScoreDisplay ScoreDisplay;

		private void Start()
		{
			ConfigureButtonStateFromOptions();
			for (int i = 0; i < LBL_CategoryName.Count; i++)
			{
				if (i < Categories.Count)
				{
					LBL_CategoryName[i].gameObject.SetActive(value: true);
					LBL_CategoryName[i].text = i + 1 + ". " + Categories[i].CategoryName;
				}
				else
				{
					LBL_CategoryName[i].gameObject.SetActive(value: false);
				}
			}
			OBS_CharCategory.SetSelectedButton(0);
			SetSelectedCategory(0);
			UpdateOptionVis();
		}

		private void ConfigureButtonStateFromOptions()
		{
			OBS_Progression.SetSelectedButton((int)GM.TNHOptions.ProgressionTypeSetting);
			OBS_EquipmentMode.SetSelectedButton((int)GM.TNHOptions.EquipmentModeSetting);
			OBS_HealthMode.SetSelectedButton((int)GM.TNHOptions.HealthModeSetting);
			OBS_AIDifficulty.SetSelectedButton((int)GM.TNHOptions.AIDifficultyModifier);
			OBS_AIRadarMode.SetSelectedButton((int)GM.TNHOptions.RadarModeModifier);
			OBS_HealthMult.SetSelectedButton((int)GM.TNHOptions.HealthMult);
			OBS_ItemSpawner.SetSelectedButton((int)GM.TNHOptions.ItemSpawnerMode);
			OBS_Backpack.SetSelectedButton((int)GM.TNHOptions.BackpackMode);
			OBS_BGAudio.SetSelectedButton((int)GM.TNHOptions.BGAudioMode);
			OBS_AINarration.SetSelectedButton((int)GM.TNHOptions.AIVoiceMode);
			OBS_RadarHand.SetSelectedButton((int)GM.TNHOptions.RadarHand);
			SetCharacter(GM.TNHOptions.Char);
			UpdateTableBasedOnOptions();
		}

		private void UpdateTableBasedOnOptions()
		{
			TNH_Char @char = GM.TNHOptions.Char;
			TNH_CharacterDef def = CharDatabase.GetDef(@char);
			string tableID = ScoreDisplay.GetTableID(LevelName, def.TableID, GM.TNHOptions.ProgressionTypeSetting, GM.TNHOptions.EquipmentModeSetting, GM.TNHOptions.HealthModeSetting);
			ScoreDisplay.SwitchToModeID(tableID);
		}

		private void UpdateOptionVis()
		{
			if (GM.TNHOptions.HealthModeSetting == TNHSetting_HealthMode.CustomHealth)
			{
				CustomHealthOptions.SetActive(value: true);
			}
			else
			{
				CustomHealthOptions.SetActive(value: false);
			}
		}

		public void SetOBS_Progression(int i)
		{
			GM.TNHOptions.ProgressionTypeSetting = (TNHSetting_ProgressionType)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(0);
			UpdateTableBasedOnOptions();
		}

		public void SetOBS_EquipmentMode(int i)
		{
			GM.TNHOptions.EquipmentModeSetting = (TNHSetting_EquipmentMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(0);
			UpdateTableBasedOnOptions();
		}

		public void SetOBS_HealthMode(int i)
		{
			GM.TNHOptions.HealthModeSetting = (TNHSetting_HealthMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(0);
			UpdateTableBasedOnOptions();
			UpdateOptionVis();
		}

		public void SetOBS_AIDifficulty(int i)
		{
			GM.TNHOptions.AIDifficultyModifier = (TNHModifier_AIDifficulty)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(1);
		}

		public void SetOBS_AIRadarMode(int i)
		{
			GM.TNHOptions.RadarModeModifier = (TNHModifier_RadarMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(1);
		}

		public void SetOBS_HealthMult(int i)
		{
			GM.TNHOptions.HealthMult = (TNH_HealthMult)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(0);
		}

		public void SetOBS_ItemSpawner(int i)
		{
			GM.TNHOptions.ItemSpawnerMode = (TNH_ItemSpawnerMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(2);
		}

		public void SetOBS_Backpack(int i)
		{
			GM.TNHOptions.BackpackMode = (TNH_BackpackMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(2);
		}

		public void SetOBS_BGAudio(int i)
		{
			GM.TNHOptions.BGAudioMode = (TNH_BGAudioMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(2);
		}

		public void SetOBS_AINarration(int i)
		{
			GM.TNHOptions.AIVoiceMode = (TNH_AIVoiceMode)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(2);
		}

		public void SetOBS_RadarHand(int i)
		{
			GM.TNHOptions.RadarHand = (TNH_RadarHand)i;
			GM.TNHOptions.SaveToFile();
			PlayButtonSound(2);
		}

		private void PlayButtonSound(int i)
		{
			if (ButtonSoundEvents.Count > i)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, ButtonSoundEvents[i], base.transform.position);
			}
		}

		private TNH_CharacterDef SetCharacter(TNH_Char c)
		{
			GM.TNHOptions.Char = c;
			GM.TNHOptions.SaveToFile();
			TNH_CharacterDef def = CharDatabase.GetDef(c);
			SelectedCharacter_Image.sprite = def.Picture;
			SelectedCharacter_Title.text = def.DisplayName;
			SelectedCharacter_Description.text = def.Description;
			UpdateTableBasedOnOptions();
			return def;
		}

		public void SetSelectedCategory(int cat)
		{
			m_selectedCategory = cat;
			PlayButtonSound(0);
			for (int i = 0; i < LBL_CharacterName.Count; i++)
			{
				if (i < Categories[cat].Characters.Count)
				{
					LBL_CharacterName[i].gameObject.SetActive(value: true);
					TNH_CharacterDef def = CharDatabase.GetDef(Categories[cat].Characters[i]);
					LBL_CharacterName[i].text = i + 1 + ". " + def.DisplayName;
				}
				else
				{
					LBL_CharacterName[i].gameObject.SetActive(value: false);
				}
			}
		}

		public void SetSelectedCharacter(int i)
		{
			m_selectedCharacter = i;
			SetCharacter(Categories[m_selectedCategory].Characters[m_selectedCharacter]);
			PlayButtonSound(1);
		}
	}
}
