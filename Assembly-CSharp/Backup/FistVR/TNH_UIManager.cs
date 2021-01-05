// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_UIManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TNH_UIManager : MonoBehaviour
  {
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
    public List<TNH_UIManager.CharacterCategory> Categories;
    [Header("Scoring")]
    public TNH_ScoreDisplay ScoreDisplay;

    private void Start()
    {
      this.ConfigureButtonStateFromOptions();
      for (int index = 0; index < this.LBL_CategoryName.Count; ++index)
      {
        if (index < this.Categories.Count)
        {
          this.LBL_CategoryName[index].gameObject.SetActive(true);
          this.LBL_CategoryName[index].text = (index + 1).ToString() + ". " + this.Categories[index].CategoryName;
        }
        else
          this.LBL_CategoryName[index].gameObject.SetActive(false);
      }
      this.OBS_CharCategory.SetSelectedButton(0);
      this.SetSelectedCategory(0);
      this.UpdateOptionVis();
    }

    private void ConfigureButtonStateFromOptions()
    {
      this.OBS_Progression.SetSelectedButton((int) GM.TNHOptions.ProgressionTypeSetting);
      this.OBS_EquipmentMode.SetSelectedButton((int) GM.TNHOptions.EquipmentModeSetting);
      this.OBS_HealthMode.SetSelectedButton((int) GM.TNHOptions.HealthModeSetting);
      this.OBS_AIDifficulty.SetSelectedButton((int) GM.TNHOptions.AIDifficultyModifier);
      this.OBS_AIRadarMode.SetSelectedButton((int) GM.TNHOptions.RadarModeModifier);
      this.OBS_HealthMult.SetSelectedButton((int) GM.TNHOptions.HealthMult);
      this.OBS_ItemSpawner.SetSelectedButton((int) GM.TNHOptions.ItemSpawnerMode);
      this.OBS_Backpack.SetSelectedButton((int) GM.TNHOptions.BackpackMode);
      this.OBS_BGAudio.SetSelectedButton((int) GM.TNHOptions.BGAudioMode);
      this.OBS_AINarration.SetSelectedButton((int) GM.TNHOptions.AIVoiceMode);
      this.OBS_RadarHand.SetSelectedButton((int) GM.TNHOptions.RadarHand);
      this.SetCharacter(GM.TNHOptions.Char);
      this.UpdateTableBasedOnOptions();
    }

    private void UpdateTableBasedOnOptions() => this.ScoreDisplay.SwitchToModeID(this.ScoreDisplay.GetTableID(this.LevelName, this.CharDatabase.GetDef(GM.TNHOptions.Char).TableID, GM.TNHOptions.ProgressionTypeSetting, GM.TNHOptions.EquipmentModeSetting, GM.TNHOptions.HealthModeSetting));

    private void UpdateOptionVis()
    {
      if (GM.TNHOptions.HealthModeSetting == TNHSetting_HealthMode.CustomHealth)
        this.CustomHealthOptions.SetActive(true);
      else
        this.CustomHealthOptions.SetActive(false);
    }

    public void SetOBS_Progression(int i)
    {
      GM.TNHOptions.ProgressionTypeSetting = (TNHSetting_ProgressionType) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(0);
      this.UpdateTableBasedOnOptions();
    }

    public void SetOBS_EquipmentMode(int i)
    {
      GM.TNHOptions.EquipmentModeSetting = (TNHSetting_EquipmentMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(0);
      this.UpdateTableBasedOnOptions();
    }

    public void SetOBS_HealthMode(int i)
    {
      GM.TNHOptions.HealthModeSetting = (TNHSetting_HealthMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(0);
      this.UpdateTableBasedOnOptions();
      this.UpdateOptionVis();
    }

    public void SetOBS_AIDifficulty(int i)
    {
      GM.TNHOptions.AIDifficultyModifier = (TNHModifier_AIDifficulty) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(1);
    }

    public void SetOBS_AIRadarMode(int i)
    {
      GM.TNHOptions.RadarModeModifier = (TNHModifier_RadarMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(1);
    }

    public void SetOBS_HealthMult(int i)
    {
      GM.TNHOptions.HealthMult = (TNH_HealthMult) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(0);
    }

    public void SetOBS_ItemSpawner(int i)
    {
      GM.TNHOptions.ItemSpawnerMode = (TNH_ItemSpawnerMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(2);
    }

    public void SetOBS_Backpack(int i)
    {
      GM.TNHOptions.BackpackMode = (TNH_BackpackMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(2);
    }

    public void SetOBS_BGAudio(int i)
    {
      GM.TNHOptions.BGAudioMode = (TNH_BGAudioMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(2);
    }

    public void SetOBS_AINarration(int i)
    {
      GM.TNHOptions.AIVoiceMode = (TNH_AIVoiceMode) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(2);
    }

    public void SetOBS_RadarHand(int i)
    {
      GM.TNHOptions.RadarHand = (TNH_RadarHand) i;
      GM.TNHOptions.SaveToFile();
      this.PlayButtonSound(2);
    }

    private void PlayButtonSound(int i)
    {
      if (this.ButtonSoundEvents.Count <= i)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.ButtonSoundEvents[i], this.transform.position);
    }

    private TNH_CharacterDef SetCharacter(TNH_Char c)
    {
      GM.TNHOptions.Char = c;
      GM.TNHOptions.SaveToFile();
      TNH_CharacterDef def = this.CharDatabase.GetDef(c);
      this.SelectedCharacter_Image.sprite = def.Picture;
      this.SelectedCharacter_Title.text = def.DisplayName;
      this.SelectedCharacter_Description.text = def.Description;
      this.UpdateTableBasedOnOptions();
      return def;
    }

    public void SetSelectedCategory(int cat)
    {
      this.m_selectedCategory = cat;
      this.PlayButtonSound(0);
      for (int index = 0; index < this.LBL_CharacterName.Count; ++index)
      {
        if (index < this.Categories[cat].Characters.Count)
        {
          this.LBL_CharacterName[index].gameObject.SetActive(true);
          TNH_CharacterDef def = this.CharDatabase.GetDef(this.Categories[cat].Characters[index]);
          this.LBL_CharacterName[index].text = (index + 1).ToString() + ". " + def.DisplayName;
        }
        else
          this.LBL_CharacterName[index].gameObject.SetActive(false);
      }
    }

    public void SetSelectedCharacter(int i)
    {
      this.m_selectedCharacter = i;
      this.SetCharacter(this.Categories[this.m_selectedCategory].Characters[this.m_selectedCharacter]);
      this.PlayButtonSound(1);
    }

    [Serializable]
    public class CharacterCategory
    {
      public string CategoryName;
      public List<TNH_Char> Characters;
    }
  }
}
