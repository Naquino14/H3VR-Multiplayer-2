// Decompiled with JetBrains decompiler
// Type: FistVR.TAH_UIManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TAH_UIManager : MonoBehaviour
  {
    public TAH_Manager Manager;
    public OptionsPanel_ButtonSet OBS_PlayerHealth;
    public OptionsPanel_ButtonSet OBS_Difficulty;
    public OptionsPanel_ButtonSet OBS_BotBullets;
    public OptionsPanel_ButtonSet OBS_Music;
    public OptionsPanel_ButtonSet OBS_LootProgression;
    public OptionsPanel_ButtonSet OBS_ItemSpawner;

    private void Start()
    {
      this.OBS_PlayerHealth.SetSelectedButton(GM.TAHSettings.TAHOption_PlayerHealth);
      this.OBS_Difficulty.SetSelectedButton(GM.TAHSettings.TAHOption_DifficultyProgression);
      this.OBS_BotBullets.SetSelectedButton(GM.TAHSettings.TAHOption_BotBullets);
      this.OBS_Music.SetSelectedButton(GM.TAHSettings.TAHOption_Music);
      this.OBS_LootProgression.SetSelectedButton(GM.TAHSettings.TAHOption_LootProgression);
      this.OBS_ItemSpawner.SetSelectedButton(GM.TAHSettings.TAHOption_ItemSpawner);
    }

    public void SetPlayerHealth(int i)
    {
      GM.TAHSettings.TAHOption_PlayerHealth = i;
      this.Manager.UpdateHealth();
      GM.TAHSettings.SaveToFile();
    }

    public void SetDifficulty(int i)
    {
      GM.TAHSettings.TAHOption_DifficultyProgression = i;
      GM.TAHSettings.SaveToFile();
    }

    public void BotBullets(int i)
    {
      GM.TAHSettings.TAHOption_BotBullets = i;
      GM.TAHSettings.SaveToFile();
    }

    public void Music(int i)
    {
      if (i == 0)
        this.Manager.MusicState(false);
      else
        this.Manager.MusicState(true);
      GM.TAHSettings.TAHOption_Music = i;
      GM.TAHSettings.SaveToFile();
    }

    public void LootProgression(int i)
    {
      GM.TAHSettings.TAHOption_LootProgression = i;
      GM.TAHSettings.SaveToFile();
    }

    public void ItemSpawner(int i)
    {
      if (i == 0)
        this.Manager.ItemSpawnerState(false);
      else
        this.Manager.ItemSpawnerState(true);
      GM.TAHSettings.TAHOption_ItemSpawner = i;
      GM.TAHSettings.SaveToFile();
    }

    public void StartGame() => this.Manager.BeginGame();
  }
}
