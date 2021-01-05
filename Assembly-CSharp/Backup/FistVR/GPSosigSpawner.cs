// Decompiled with JetBrains decompiler
// Type: FistVR.GPSosigSpawner
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class GPSosigSpawner : GPPlaceable
  {
    [Header("SosigSpawner")]
    public Transform SpawnPoint;
    public OptionsPanel_ButtonSet OBS_SpawnWhen;
    public OptionsPanel_ButtonSet OBS_InitialState;
    public OptionsPanel_ButtonSet OBS_Equipment;
    public OptionsPanel_ButtonSet OBS_Ammo;
    private GPSosigManager.SpawnWhen m_spawnWhen;
    private GPSosigManager.InitialState m_initialState;
    private GPSosigManager.Equipment m_equipment;
    private GPSosigManager.Ammo m_ammo;
    private int m_teamIFF;
    private int m_eventChannel;
    private int m_assaultGroup;
    private int m_patrolPath;
    public Text LBL_TeamIFF;
    public Text LBL_SpawnChannel;
    public Text LBL_AssaultGroup;
    public Text LBL_PatrolPath;
    private List<SosigEnemyCategory> m_categories = new List<SosigEnemyCategory>();
    private List<SosigEnemyID> m_curDisplayedIDsToAdd = new List<SosigEnemyID>();
    private List<SosigEnemyTemplate> m_curDisplayedTemplatesToAdd = new List<SosigEnemyTemplate>();
    private List<SosigEnemyTemplate> m_myTemplates = new List<SosigEnemyTemplate>();
    public List<Text> List_Categories;
    public List<Text> List_TemplatesToAdd;
    public List<Text> List_TemplatesToRemove;
    private float m_spawnStartTick;
    private bool m_isSpawnStartTickingDown;

    public override void Init(GPSceneMode mode)
    {
      this.m_categories = ManagerSingleton<IM>.Instance.olistSosigCats;
      base.Init(mode);
      switch (mode)
      {
        case GPSceneMode.Play:
          this.SetParamsPanel(false);
          if (this.m_spawnWhen == GPSosigManager.SpawnWhen.OnEvent)
          {
            GM.CurrentGamePlannerManager.AddReceiver(this.m_eventChannel, new GamePlannerManager.GPEventSignal(this.SpawnMySosig));
            break;
          }
          this.m_isSpawnStartTickingDown = true;
          this.m_spawnStartTick = UnityEngine.Random.Range(0.01f, 0.05f);
          break;
        case GPSceneMode.Design:
          this.BTN_Template_SetGroup(0);
          this.UpdateParamsDisplay();
          break;
      }
    }

    private void Update()
    {
      if (!this.m_isSpawnStartTickingDown)
        return;
      this.m_spawnStartTick -= Time.deltaTime;
      if ((double) this.m_spawnStartTick > 0.0)
        return;
      this.m_isSpawnStartTickingDown = false;
      this.SpawnMySosig();
    }

    public void SpawnMySosig()
    {
      if (!GM.CurrentGamePlannerManager.CanSpawnSosigs() || this.m_myTemplates.Count <= 0)
        return;
      GM.CurrentGamePlannerManager.SosigManager.SpawnSosig(this.m_myTemplates[UnityEngine.Random.Range(0, this.m_myTemplates.Count)], this.SpawnPoint, this.m_initialState, this.m_equipment, this.m_ammo, this.m_teamIFF, this.m_assaultGroup, this.m_patrolPath);
    }

    public override void UpdateParamsDisplay()
    {
      for (int index = 0; index < this.List_Categories.Count; ++index)
      {
        if (index < this.m_categories.Count)
        {
          this.List_Categories[index].gameObject.SetActive(true);
          this.List_Categories[index].text = this.m_categories[index].ToString();
        }
        else
          this.List_Categories[index].gameObject.SetActive(false);
      }
      for (int index = 0; index < this.List_TemplatesToAdd.Count; ++index)
      {
        if (index < this.m_curDisplayedTemplatesToAdd.Count)
        {
          this.List_TemplatesToAdd[index].gameObject.SetActive(true);
          this.List_TemplatesToAdd[index].text = this.m_curDisplayedTemplatesToAdd[index].DisplayName.ToString();
        }
        else
          this.List_TemplatesToAdd[index].gameObject.SetActive(false);
      }
      for (int index = 0; index < this.List_TemplatesToRemove.Count; ++index)
      {
        if (index < this.m_myTemplates.Count)
        {
          this.List_TemplatesToRemove[index].gameObject.SetActive(true);
          this.List_TemplatesToRemove[index].text = this.m_myTemplates[index].SosigEnemyCategory.ToString() + " > " + this.m_myTemplates[index].DisplayName;
        }
        else
          this.List_TemplatesToRemove[index].gameObject.SetActive(false);
      }
      this.LBL_TeamIFF.text = this.m_teamIFF.ToString();
      this.LBL_SpawnChannel.text = this.m_eventChannel.ToString();
      this.LBL_AssaultGroup.text = this.m_assaultGroup.ToString();
      this.LBL_PatrolPath.text = this.m_patrolPath.ToString();
      this.OBS_SpawnWhen.SetSelectedButton((int) this.m_spawnWhen);
      this.OBS_InitialState.SetSelectedButton((int) this.m_initialState);
      this.OBS_Equipment.SetSelectedButton((int) this.m_equipment);
      this.OBS_Ammo.SetSelectedButton((int) this.m_ammo);
    }

    public void BTN_Template_SetGroup(int i)
    {
      this.m_curDisplayedIDsToAdd.Clear();
      this.m_curDisplayedTemplatesToAdd.Clear();
      SosigEnemyCategory category = this.m_categories[i];
      this.m_curDisplayedIDsToAdd = ManagerSingleton<IM>.Instance.odicSosigIDsByCategory[category];
      this.m_curDisplayedTemplatesToAdd = ManagerSingleton<IM>.Instance.odicSosigObjsByCategory[category];
      this.UpdateParamsDisplay();
    }

    public void BTN_Template_Add(int i)
    {
      if (this.m_myTemplates.Count > 19)
        return;
      this.m_myTemplates.Add(this.m_curDisplayedTemplatesToAdd[i]);
      this.AddToInternalList(this.m_curDisplayedIDsToAdd[i].ToString(), true);
      this.UpdateParamsDisplay();
    }

    public void BTN_Template_Remove(int i)
    {
      if (this.m_myTemplates.Count >= i)
        return;
      this.m_myTemplates.RemoveAt(i);
      this.RemoveFromInternalList(this.m_curDisplayedIDsToAdd[i].ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_SpawnWhen(int i)
    {
      this.m_spawnWhen = (GPSosigManager.SpawnWhen) i;
      this.SetFlag("SpawnWhen", ((GPSosigManager.SpawnWhen) i).ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_InitialState(int i)
    {
      this.m_initialState = (GPSosigManager.InitialState) i;
      this.SetFlag("InitialState", ((GPSosigManager.InitialState) i).ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_Equipment(int i)
    {
      this.m_equipment = (GPSosigManager.Equipment) i;
      this.SetFlag("Equipment", ((GPSosigManager.Equipment) i).ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_Ammo(int i)
    {
      this.m_ammo = (GPSosigManager.Ammo) i;
      this.SetFlag("Ammo", ((GPSosigManager.Ammo) i).ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_ModTeamIFF(int i)
    {
      this.m_teamIFF += i;
      this.m_teamIFF = Mathf.Clamp(this.m_teamIFF, 0, 99);
      this.SetFlag("TeamIFF", this.m_teamIFF.ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_ModSpawnChannel(int i)
    {
      this.m_eventChannel += i;
      this.m_eventChannel = Mathf.Clamp(this.m_eventChannel, 0, 99);
      this.SetFlag("EventChannel", this.m_eventChannel.ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_ModAssaultGroup(int i)
    {
      this.m_assaultGroup += i;
      this.m_assaultGroup = Mathf.Clamp(this.m_assaultGroup, 0, 99);
      this.SetFlag("AssaultGroup", this.m_assaultGroup.ToString());
      this.UpdateParamsDisplay();
    }

    public void BTN_ModPatrolPath(int i)
    {
      this.m_patrolPath += i;
      this.m_patrolPath = Mathf.Clamp(this.m_patrolPath, 0, 99);
      this.SetFlag("PatrolPath", this.m_patrolPath.ToString());
      this.UpdateParamsDisplay();
    }

    public override void ConfigureFromSavedPlaceable(GPSavedPlaceable p)
    {
      base.ConfigureFromSavedPlaceable(p);
      for (int index = 0; index < this.InternalList.Count; ++index)
        this.m_myTemplates.Add(ManagerSingleton<IM>.Instance.odicSosigObjsByID[(SosigEnemyID) Enum.Parse(typeof (SosigEnemyID), this.InternalList[index])]);
      if (this.FlagExists("SpawnWhen"))
        this.m_spawnWhen = (GPSosigManager.SpawnWhen) Enum.Parse(typeof (GPSosigManager.SpawnWhen), this.Flags["SpawnWhen"]);
      if (this.FlagExists("InitialState"))
        this.m_initialState = (GPSosigManager.InitialState) Enum.Parse(typeof (GPSosigManager.InitialState), this.Flags["InitialState"]);
      if (this.FlagExists("Equipment"))
        this.m_equipment = (GPSosigManager.Equipment) Enum.Parse(typeof (GPSosigManager.Equipment), this.Flags["Equipment"]);
      if (this.FlagExists("Ammo"))
        this.m_ammo = (GPSosigManager.Ammo) Enum.Parse(typeof (GPSosigManager.Ammo), this.Flags["Ammo"]);
      if (this.FlagExists("TeamIFF"))
        this.m_teamIFF = int.Parse(this.Flags["TeamIFF"]);
      if (this.FlagExists("EventChannel"))
        this.m_eventChannel = int.Parse(this.Flags["EventChannel"]);
      if (this.FlagExists("AssaultGroup"))
        this.m_assaultGroup = int.Parse(this.Flags["AssaultGroup"]);
      if (!this.FlagExists("PatrolPath"))
        return;
      this.m_patrolPath = int.Parse(this.Flags["PatrolPath"]);
    }
  }
}
