// Decompiled with JetBrains decompiler
// Type: FistVR.HG_ModeManager_MeatleGear
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_ModeManager_MeatleGear : HG_ModeManager
  {
    public Transform PlayerSpawnPoint;
    public GameObject GronchMasterPrefab;
    private RonchMaster m_spawnedGronchMaster;
    public Transform GronchMasterSpawnPoint;
    [Header("Audio")]
    public AudioEvent AudEvent_ZoneCompleted;
    public AudioEvent AudEvent_SequenceCompleted;
    private bool m_isGronchAlive;
    private bool m_playerKilledGronch;
    private float m_timer;
    public Transform SpawnPoint_Knife;
    public Transform SpawnPoint_HealthPowerup;
    public Transform SpawnPoint_RegenPowerup;
    public FVRObject Prefab_Knife;
    public FVRObject Prefab_HealthPowerup;
    public FVRObject Prefab_RegenPowerup;
    [Header("LootSpawning")]
    public GameObject LootCrate;
    public ZosigItemSpawnTable SpawnTable;
    public List<Transform> LootboxSpawnPoints;
    private List<GameObject> m_spawnedCrates = new List<GameObject>();
    public Vector3 StripSearch_Center;
    public Vector3 StripSearch_HalfExtents;
    public LayerMask SearchLM;

    public override void InitMode(HG_ModeManager.HG_Mode mode)
    {
      this.CleanArena();
      this.m_mode = mode;
      this.m_playerKilledGronch = false;
      Transform playerSpawnPoint = this.PlayerSpawnPoint;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(playerSpawnPoint.position, true, playerSpawnPoint.forward);
      this.InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
      this.InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
      GM.CurrentSceneSettings.DeathResetPoint.position = playerSpawnPoint.position;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = playerSpawnPoint.rotation;
      this.StripEquipment(mode);
      this.m_isGronchAlive = true;
      this.m_timer = 0.0f;
      Object.Instantiate<GameObject>(this.Prefab_Knife.GetGameObject(), this.SpawnPoint_Knife.position, this.SpawnPoint_Knife.rotation);
      Object.Instantiate<GameObject>(this.Prefab_HealthPowerup.GetGameObject(), this.SpawnPoint_HealthPowerup.position, this.SpawnPoint_HealthPowerup.rotation);
      Object.Instantiate<GameObject>(this.Prefab_RegenPowerup.GetGameObject(), this.SpawnPoint_RegenPowerup.position, this.SpawnPoint_RegenPowerup.rotation);
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
      if (mode == HG_ModeManager.HG_Mode.MeatleGear_ScavengingSnake)
        this.SpawnLoot();
      this.m_spawnedGronchMaster = Object.Instantiate<GameObject>(this.GronchMasterPrefab, this.GronchMasterSpawnPoint.position, this.GronchMasterSpawnPoint.rotation).GetComponent<RonchMaster>();
      this.m_spawnedGronchMaster.SetModeManager(this);
      this.IsPlaying = true;
    }

    public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
    {
      this.IsPlaying = false;
      GM.CurrentSceneSettings.DeathResetPoint.position = this.InitialRespawnPos;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.InitialRespawnRot;
      if (this.m_spawnedCrates.Count > 0)
      {
        for (int index = this.m_spawnedCrates.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_spawnedCrates[index] != (Object) null)
            Object.Destroy((Object) this.m_spawnedCrates[index]);
        }
      }
      this.m_spawnedCrates.Clear();
      base.EndMode(doesInvokeTeleport, immediateTeleportBackAndScore);
    }

    public void GronchDied()
    {
      this.m_isGronchAlive = false;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_SequenceCompleted, this.transform.position);
      this.m_playerKilledGronch = true;
      this.M.Case();
      this.EndMode(true, false);
    }

    public override void HandlePlayerDeath() => this.EndMode(false, true);

    private void Update()
    {
      if (!this.IsPlaying)
        return;
      this.m_timer += Time.deltaTime;
      if (!this.m_isGronchAlive)
        ;
    }

    private void SpawnLoot()
    {
      this.LootboxSpawnPoints.Shuffle<Transform>();
      this.LootboxSpawnPoints.Shuffle<Transform>();
      for (int index1 = 0; index1 < 12; ++index1)
      {
        GameObject gameObject = Object.Instantiate<GameObject>(this.LootCrate, this.LootboxSpawnPoints[index1].position, Random.rotation);
        MM_LootCrate component = gameObject.GetComponent<MM_LootCrate>();
        int index2 = Random.Range(0, this.SpawnTable.Objects.Count);
        FVRObject fvrObject = (FVRObject) null;
        if (this.SpawnTable.Objects.Count > 0)
          fvrObject = this.SpawnTable.Objects[index2];
        component.Init(fvrObject, (FVRObject) null, (FVRObject) null, (FVRObject) null);
        this.m_spawnedCrates.Add(gameObject);
      }
    }

    public override int GetScore()
    {
      int num = 0;
      if (this.m_playerKilledGronch)
        num = 1;
      return 50000 * num + Mathf.Max((9000 - (int) this.m_timer) * 30, 0) * num + (int) ((double) GM.CurrentPlayerBody.GetPlayerHealth() * 50000.0) * num;
    }

    public override List<string> GetScoringReadOuts()
    {
      if ((Object) this.m_spawnedGronchMaster != (Object) null)
        this.m_spawnedGronchMaster.Dispose();
      int num = 0;
      if (this.m_playerKilledGronch)
        num = 1;
      return new List<string>()
      {
        "Base Score: " + (50000 * num).ToString(),
        "Time Bonus: " + (Mathf.Max((9000 - (int) this.m_timer) * 30, 0) * num).ToString(),
        "Health Bonus: " + ((int) ((double) GM.CurrentPlayerBody.GetPlayerHealth() * 50000.0) * num).ToString(),
        "Final Score: " + this.GetScore().ToString()
      };
    }

    private void CleanArena()
    {
      foreach (Collider collider in Physics.OverlapBox(this.StripSearch_Center, this.StripSearch_HalfExtents, Quaternion.identity, (int) this.SearchLM, QueryTriggerInteraction.Collide))
      {
        if ((Object) collider.attachedRigidbody != (Object) null)
        {
          FVRPhysicalObject component = collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
          if ((Object) component != (Object) null)
          {
            if ((Object) component.QuickbeltSlot != (Object) null)
              component.ClearQuickbeltState();
            if (component.IsHeld)
              component.ForceBreakInteraction();
            Object.Destroy((Object) component.gameObject);
          }
        }
      }
    }

    private void StripEquipment(HG_ModeManager.HG_Mode mode)
    {
      foreach (Collider collider in Physics.OverlapBox(this.StripSearch_Center, this.StripSearch_HalfExtents, Quaternion.identity, (int) this.SearchLM, QueryTriggerInteraction.Collide))
      {
        if ((Object) collider.attachedRigidbody != (Object) null)
        {
          FVRPhysicalObject component = collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
          if ((Object) component != (Object) null && (Object) component.ObjectWrapper != (Object) null)
          {
            if (component.ObjectWrapper.Category == FVRObject.ObjectCategory.Powerup)
            {
              if ((Object) component.QuickbeltSlot != (Object) null)
                component.ClearQuickbeltState();
              if (component.IsHeld)
                component.ForceBreakInteraction();
              Object.Destroy((Object) component.gameObject);
            }
            else if (mode != HG_ModeManager.HG_Mode.MeatleGear_Open && component.ObjectWrapper.Category != FVRObject.ObjectCategory.MeleeWeapon)
            {
              if ((Object) component.QuickbeltSlot != (Object) null)
                component.ClearQuickbeltState();
              if (component.IsHeld)
                component.ForceBreakInteraction();
              Object.Destroy((Object) component.gameObject);
            }
          }
        }
      }
    }

    public void CleanUpScene()
    {
      FVRFireArmMagazine[] objectsOfType1 = Object.FindObjectsOfType<FVRFireArmMagazine>();
      for (int index = objectsOfType1.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType1[index].IsHeld && (Object) objectsOfType1[index].QuickbeltSlot == (Object) null && ((Object) objectsOfType1[index].FireArm == (Object) null && objectsOfType1[index].m_numRounds == 0))
          Object.Destroy((Object) objectsOfType1[index].gameObject);
      }
      FVRFireArmClip[] objectsOfType2 = Object.FindObjectsOfType<FVRFireArmClip>();
      for (int index = objectsOfType2.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType2[index].IsHeld && (Object) objectsOfType2[index].QuickbeltSlot == (Object) null && ((Object) objectsOfType2[index].FireArm == (Object) null && objectsOfType2[index].m_numRounds == 0))
          Object.Destroy((Object) objectsOfType2[index].gameObject);
      }
      Speedloader[] objectsOfType3 = Object.FindObjectsOfType<Speedloader>();
      for (int index = objectsOfType3.Length - 1; index >= 0; --index)
      {
        if (!objectsOfType3[index].IsHeld && (Object) objectsOfType3[index].QuickbeltSlot == (Object) null)
          Object.Destroy((Object) objectsOfType3[index].gameObject);
      }
    }
  }
}
