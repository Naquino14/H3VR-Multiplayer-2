// Decompiled with JetBrains decompiler
// Type: FistVR.MF_GameManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class MF_GameManager : MonoBehaviour
  {
    public FVRFMODController FMODController;
    public MF_TeamManager TeamManager;
    private AnvilPrewarmCallback LoadingCallback;
    [Header("UI - Panels")]
    public GameObject Panel_Mode_TDM;
    public GameObject Panel_RespawnReset;
    [Header("UI - PlayerSettings")]
    public OptionsPanel_ButtonSet OP_PlayerSettings_Health;
    public OptionsPanel_ButtonSet OP_PlayerSettings_BlastJumping;
    public OptionsPanel_ButtonSet OP_PlayerSettings_BlastJumpSelfDamage;
    [Header("UI - TeamDM")]
    public OptionsPanel_ButtonSet OP_TDM_PlayerTeam;
    public OptionsPanel_ButtonSet OP_TDM_RedTeamSize;
    public OptionsPanel_ButtonSet OP_TDM_BlueTeamSize;
    public OptionsPanel_ButtonSet OP_TDM_RedSpawnSpeed;
    public OptionsPanel_ButtonSet OP_TDM_BlueSpawnSpeed;
    public OptionsPanel_ButtonSet OP_TDM_PlayArea;
    private float curVol;
    private float tarVol = 0.25f;

    private void Awake() => this.FMODController.SetMasterVolume(0.0f);

    private void Start()
    {
      this.LoadingCallback = AnvilManager.PreloadBundleAsync("assets_resources_objectids_weaponry_mf2");
      this.LoadingCallback.AddCallback((Action) (() => Debug.Log((object) "Prewarm Completed MF")));
      this.SetPlayerIFF(1);
      this.InitSound();
      this.InitPage_PlayerSettings();
      this.InitPage_TDM();
      this.InitPlayerSettings();
    }

    private void InitSound() => this.FMODController.SwitchTo(0, 2f, false, false);

    private void Update()
    {
      if ((double) this.curVol >= (double) this.tarVol)
        return;
      this.curVol = Mathf.MoveTowards(this.curVol, this.tarVol, Time.deltaTime * 0.25f);
      this.FMODController.SetMasterVolume(this.curVol);
    }

    public void OnPlayerDeath() => this.FMODController.SetIntParameterByIndex(0, "Intensity", 1f);

    private void SetPlayerHealthToIndex(int index) => GM.CurrentPlayerBody.SetHealthThreshold(GM.MFFlags.PlayerSetting_HealthSettings[index]);

    private void SetPlayerIFF(int i)
    {
      GM.CurrentSceneSettings.DefaultPlayerIFF = i;
      GM.CurrentPlayerBody.SetPlayerIFF(i);
    }

    public void Respawn()
    {
      Transform targetPointByClass = this.TeamManager.PlayerRespawnZone.GetTargetPointByClass(MF_Class.Soldier);
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(targetPointByClass.position, true, targetPointByClass.forward);
      this.FMODController.SetIntParameterByIndex(0, "Intensity", 2f);
    }

    public void ResetGame()
    {
    }

    private void InitPage_PlayerSettings()
    {
      this.OP_PlayerSettings_Health.SetSelectedButton(GM.MFFlags.PlayerSetting_HealthIndex);
      this.OP_PlayerSettings_BlastJumping.SetSelectedButton((int) GM.MFFlags.PlayerSetting_BlastJumping);
      this.OP_PlayerSettings_BlastJumpSelfDamage.SetSelectedButton((int) GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage);
      this.SetPlayerHealthToIndex(GM.MFFlags.PlayerSetting_HealthIndex);
    }

    private void InitPage_TDM()
    {
      this.OP_TDM_PlayerTeam.SetSelectedButton((int) GM.MFFlags.TDMOption_PlayerTeam);
      this.OP_TDM_RedTeamSize.SetSelectedButton(GM.MFFlags.TDMOption_RedTeamSizeIndex);
      this.OP_TDM_BlueTeamSize.SetSelectedButton(GM.MFFlags.TDMOption_BlueTeamSizeIndex);
      this.OP_TDM_RedSpawnSpeed.SetSelectedButton((int) GM.MFFlags.TDMOption_RedTeamSpeed);
      this.OP_TDM_BlueSpawnSpeed.SetSelectedButton((int) GM.MFFlags.TDMOption_BlueTeamSpeed);
      this.OP_TDM_PlayArea.SetSelectedButton((int) GM.MFFlags.TDMOption_PlayArea);
    }

    private void InitPlayerSettings()
    {
      GM.MFFlags.PlayerTeam = GM.MFFlags.TDMOption_PlayerTeam;
      if (GM.MFFlags.TDMOption_PlayerTeam == MF_TeamColor.Red)
        this.SetPlayerIFF(this.TeamManager.IFF_Red);
      else
        this.SetPlayerIFF(this.TeamManager.IFF_Blue);
    }

    public void SetMusicOnOff(int i)
    {
      if (i == 0)
      {
        this.FMODController.SwitchTo(0, 0.0f, true, false);
        this.FMODController.SetIntParameterByIndex(0, "Intensity", 1f);
        this.FMODController.SetMasterVolume(0.25f);
      }
      else
        this.FMODController.SetIntParameterByIndex(0, "Intensity", 0.0f);
    }

    public void SetOption_Player_Health(int i)
    {
      GM.MFFlags.PlayerSetting_HealthIndex = i;
      this.SetPlayerHealthToIndex(GM.MFFlags.PlayerSetting_HealthIndex);
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_Player_BlastJumping(int i)
    {
      GM.MFFlags.PlayerSetting_BlastJumping = (MF_BlastJumping) i;
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_Player_BlastJumpingSelfDamage(int i)
    {
      GM.MFFlags.PlayerSetting_BlastJumpingSelfDamage = (MF_BlastJumpingSelfDamage) i;
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_TDM_PlayerTeam(int i)
    {
      GM.MFFlags.TDMOption_PlayerTeam = (MF_TeamColor) i;
      GM.MFFlags.SaveToFile();
      if (GM.MFFlags.TDMOption_PlayerTeam == MF_TeamColor.Red)
        this.SetPlayerIFF(this.TeamManager.IFF_Red);
      else
        this.SetPlayerIFF(this.TeamManager.IFF_Blue);
    }

    public void SetOption_TDM_RedTeamSize(int i)
    {
      GM.MFFlags.TDMOption_RedTeamSizeIndex = i;
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_TDM_BlueTeamSize(int i)
    {
      GM.MFFlags.TDMOption_BlueTeamSizeIndex = i;
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_TDM_RedSpawnSpeed(int i)
    {
      GM.MFFlags.TDMOption_RedTeamSpeed = (MF_SpawnSpeed) i;
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_TDM_BlueSpawnSpeed(int i)
    {
      GM.MFFlags.TDMOption_BlueTeamSpeed = (MF_SpawnSpeed) i;
      GM.MFFlags.SaveToFile();
    }

    public void SetOption_TDM_PlayArea(int i)
    {
      GM.MFFlags.TDMOption_PlayArea = (MF_PlayArea) i;
      GM.MFFlags.SaveToFile();
    }

    public void StartGame()
    {
      if (this.LoadingCallback != null && !this.LoadingCallback.IsCompleted)
        return;
      float RedTeamSpawnCadence = 6f;
      float BlueTeamSpawnCadence = 6f;
      MF_SpawnSpeed optionRedTeamSpeed = GM.MFFlags.TDMOption_RedTeamSpeed;
      MF_SpawnSpeed optionBlueTeamSpeed = GM.MFFlags.TDMOption_BlueTeamSpeed;
      switch (optionRedTeamSpeed)
      {
        case MF_SpawnSpeed.Slow:
          RedTeamSpawnCadence = 9f;
          break;
        case MF_SpawnSpeed.Fast:
          RedTeamSpawnCadence = 3f;
          break;
      }
      switch (optionBlueTeamSpeed)
      {
        case MF_SpawnSpeed.Slow:
          BlueTeamSpawnCadence = 9f;
          break;
        case MF_SpawnSpeed.Fast:
          BlueTeamSpawnCadence = 3f;
          break;
      }
      this.Panel_Mode_TDM.SetActive(false);
      this.Panel_RespawnReset.SetActive(true);
      this.TeamManager.InitGameMode(MF_GameMode.TeamDM, GM.MFFlags.TDM_TeamSizes[GM.MFFlags.TDMOption_RedTeamSizeIndex], GM.MFFlags.TDM_TeamSizes[GM.MFFlags.TDMOption_BlueTeamSizeIndex], GM.MFFlags.TDMOption_PlayerTeam, RedTeamSpawnCadence, BlueTeamSpawnCadence, GM.MFFlags.TDMOption_PlayArea);
      this.Respawn();
    }

    private void TestStart()
    {
      float RedTeamSpawnCadence = 6f;
      float BlueTeamSpawnCadence = 6f;
      MF_SpawnSpeed optionRedTeamSpeed = GM.MFFlags.TDMOption_RedTeamSpeed;
      MF_SpawnSpeed optionBlueTeamSpeed = GM.MFFlags.TDMOption_BlueTeamSpeed;
      switch (optionRedTeamSpeed)
      {
        case MF_SpawnSpeed.Slow:
          RedTeamSpawnCadence = 9f;
          break;
        case MF_SpawnSpeed.Fast:
          RedTeamSpawnCadence = 3f;
          break;
      }
      switch (optionBlueTeamSpeed)
      {
        case MF_SpawnSpeed.Slow:
          BlueTeamSpawnCadence = 9f;
          break;
        case MF_SpawnSpeed.Fast:
          BlueTeamSpawnCadence = 3f;
          break;
      }
      this.Panel_Mode_TDM.SetActive(false);
      this.Panel_RespawnReset.SetActive(true);
      this.TeamManager.InitGameMode(MF_GameMode.TeamDM, 16, 16, MF_TeamColor.Red, RedTeamSpawnCadence, BlueTeamSpawnCadence, MF_PlayArea.FullMap);
      this.Respawn();
    }
  }
}
