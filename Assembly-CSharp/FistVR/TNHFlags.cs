// Decompiled with JetBrains decompiler
// Type: FistVR.TNHFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
        Debug.Log((object) "TNH_v2.txt exists, initializing from it");
        using (ES2Reader es2Reader = ES2Reader.Create("TNH_v2.txt"))
        {
          if (es2Reader.TagExists("Char"))
            this.Char = es2Reader.Read<TNH_Char>("Char");
          if (es2Reader.TagExists("ProgressionTypeSetting"))
            this.ProgressionTypeSetting = es2Reader.Read<TNHSetting_ProgressionType>("ProgressionTypeSetting");
          if (es2Reader.TagExists("HealthModeSetting"))
            this.HealthModeSetting = es2Reader.Read<TNHSetting_HealthMode>("HealthModeSetting");
          if (es2Reader.TagExists("EquipmentModeSetting"))
            this.EquipmentModeSetting = es2Reader.Read<TNHSetting_EquipmentMode>("EquipmentModeSetting");
          if (es2Reader.TagExists("AIDifficultyModifier"))
            this.AIDifficultyModifier = es2Reader.Read<TNHModifier_AIDifficulty>("AIDifficultyModifier");
          if (es2Reader.TagExists("RadarModeModifier"))
            this.RadarModeModifier = es2Reader.Read<TNHModifier_RadarMode>("RadarModeModifier");
          if (es2Reader.TagExists("ItemSpawnerMode"))
            this.ItemSpawnerMode = es2Reader.Read<TNH_ItemSpawnerMode>("ItemSpawnerMode");
          if (es2Reader.TagExists("BackpackMode"))
            this.BackpackMode = es2Reader.Read<TNH_BackpackMode>("BackpackMode");
          if (es2Reader.TagExists("HealthMult"))
            this.HealthMult = es2Reader.Read<TNH_HealthMult>("HealthMult");
          if (es2Reader.TagExists("BGAudioMode"))
            this.BGAudioMode = es2Reader.Read<TNH_BGAudioMode>("BGAudioMode");
          if (es2Reader.TagExists("AIVoiceMode"))
            this.AIVoiceMode = es2Reader.Read<TNH_AIVoiceMode>("AIVoiceMode");
          if (!es2Reader.TagExists("RadarHand"))
            return;
          this.RadarHand = es2Reader.Read<TNH_RadarHand>("RadarHand");
        }
      }
      else
      {
        Debug.Log((object) "TNH_v2.txt does not exist, creating it");
        this.SaveToFile();
        this.InitializeFromSaveFile();
      }
    }

    public void SaveToFile()
    {
      using (ES2Writer es2Writer = ES2Writer.Create("TNH_v2.txt"))
      {
        es2Writer.Write<TNH_Char>(this.Char, "Char");
        es2Writer.Write<TNHSetting_ProgressionType>(this.ProgressionTypeSetting, "ProgressionTypeSetting");
        es2Writer.Write<TNHSetting_HealthMode>(this.HealthModeSetting, "HealthModeSetting");
        es2Writer.Write<TNHSetting_EquipmentMode>(this.EquipmentModeSetting, "EquipmentModeSetting");
        es2Writer.Write<TNHModifier_AIDifficulty>(this.AIDifficultyModifier, "AIDifficultyModifier");
        es2Writer.Write<TNHModifier_RadarMode>(this.RadarModeModifier, "RadarModeModifier");
        es2Writer.Write<TNH_ItemSpawnerMode>(this.ItemSpawnerMode, "ItemSpawnerMode");
        es2Writer.Write<TNH_BackpackMode>(this.BackpackMode, "BackpackMode");
        es2Writer.Write<TNH_HealthMult>(this.HealthMult, "HealthMult");
        es2Writer.Write<TNH_BGAudioMode>(this.BGAudioMode, "BGAudioMode");
        es2Writer.Write<TNH_AIVoiceMode>(this.AIVoiceMode, "AIVoiceMode");
        es2Writer.Write<TNH_RadarHand>(this.RadarHand, "RadarHand");
        es2Writer.Save();
      }
    }
  }
}
