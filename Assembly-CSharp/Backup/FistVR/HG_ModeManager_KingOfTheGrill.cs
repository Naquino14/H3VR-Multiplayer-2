// Decompiled with JetBrains decompiler
// Type: FistVR.HG_ModeManager_KingOfTheGrill
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class HG_ModeManager_KingOfTheGrill : HG_ModeManager
  {
    public Transform PlayerSpawnPoint;
    public List<FVRObject> StartingSpawns;
    public List<Transform> StartingSpawnPoints;
    public ZosigItemSpawnTable StartingMeleeWeapon;
    public Transform StartingMeleeSpot;
    public GameObject Table;
    public List<Text> Labels_Wave;
    public List<Text> Labels_Time;
    [Header("Enemy Spawn Types")]
    public List<SosigEnemyTemplate> SosigEnemyTemplates_Invasion;
    public List<SosigEnemyTemplate> SosigEnemyTemplates_Ressurection;
    public List<SosigEnemyTemplate> SosigEnemyTemplates_Anachronism;
    private List<SosigEnemyTemplate> m_curEnemyTemplateList;
    [Header("Enemy Zones")]
    public List<HG_SpawnPointGroup> GrillSpawnZones;
    public List<Transform> EnemyEndPoints;
    private List<Sosig> m_activeSosigs = new List<Sosig>();
    private List<GameObject> m_spawnedEquipment = new List<GameObject>();
    [Header("LootSpawning")]
    public List<Transform> LootCrateSpawns;
    public GameObject LootCrate;
    public List<ZosigItemSpawnTable> LootTables;
    public FVRObject UnCookedPowerup;
    [Header("Audio")]
    public AudioEvent AudEvent_ZoneCompleted;
    public AudioEvent AudEvent_SequenceCompleted;
    private int m_waveNumber;
    private float m_tickDownToWave = 10f;
    private float m_timer;
    private float m_sosigScore;
    private int m_numSosigsKilled;
    private int m_numCivvieKills;
    private int m_numPowerUpsCooked;
    public Vector3 StripSearch_Center;
    public Vector3 StripSearch_HalfExtents;
    public LayerMask SearchLM;
    private bool m_isWaitingForWave;

    public override void InitMode(HG_ModeManager.HG_Mode mode)
    {
      this.CleanArena();
      this.m_mode = mode;
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      this.m_activeSosigs.Clear();
      Transform playerSpawnPoint = this.PlayerSpawnPoint;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(playerSpawnPoint.position, true, playerSpawnPoint.forward);
      this.InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
      this.InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
      GM.CurrentSceneSettings.DeathResetPoint.position = playerSpawnPoint.position;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = playerSpawnPoint.rotation;
      this.StripEquipment(mode);
      this.m_waveNumber = 0;
      this.m_timer = 0.0f;
      this.m_sosigScore = 0.0f;
      this.m_numSosigsKilled = 0;
      this.m_numCivvieKills = 0;
      this.m_numPowerUpsCooked = 0;
      this.m_tickDownToWave = 10f;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
      this.Table.SetActive(true);
      for (int index = 0; index < this.StartingSpawnPoints.Count; ++index)
        this.m_spawnedEquipment.Add(Object.Instantiate<GameObject>(this.StartingSpawns[index].GetGameObject(), this.StartingSpawnPoints[index].position, this.StartingSpawnPoints[index].rotation));
      Object.Instantiate<GameObject>(this.StartingMeleeWeapon.Objects[Random.Range(0, this.StartingMeleeWeapon.Objects.Count)].GetGameObject(), this.StartingMeleeSpot.position, this.StartingMeleeSpot.rotation);
      switch (mode)
      {
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Invasion:
          this.m_curEnemyTemplateList = this.SosigEnemyTemplates_Invasion;
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Resurrection:
          this.m_curEnemyTemplateList = this.SosigEnemyTemplates_Ressurection;
          break;
        case HG_ModeManager.HG_Mode.KingOfTheGrill_Anachronism:
          this.m_curEnemyTemplateList = this.SosigEnemyTemplates_Anachronism;
          break;
      }
      this.IsPlaying = true;
      this.m_tickDownToWave = 59f;
      this.m_isWaitingForWave = true;
      this.SpawnLoot();
    }

    public override void HandlePlayerDeath() => this.EndMode(false, true);

    public void Update()
    {
      if (this.IsPlaying)
      {
        for (int index = 0; index < this.Labels_Wave.Count; ++index)
          this.Labels_Wave[index].text = "Wave - " + (this.m_waveNumber + 1).ToString();
        for (int index = 0; index < this.Labels_Time.Count; ++index)
          this.Labels_Time[index].text = this.FloatToTime(this.m_tickDownToWave, "00.00");
      }
      if (!this.IsPlaying || !this.m_isWaitingForWave)
        return;
      this.m_tickDownToWave -= Time.deltaTime;
      if ((double) this.m_tickDownToWave > 0.0)
        return;
      this.SpawnWave();
    }

    public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
    {
      this.IsPlaying = false;
      this.Table.SetActive(false);
      if (this.m_activeSosigs.Count > 0)
      {
        for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_activeSosigs[index] != (Object) null)
            this.m_activeSosigs[index].ClearSosig();
        }
      }
      this.m_activeSosigs.Clear();
      for (int index = this.m_spawnedEquipment.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.m_spawnedEquipment[index]);
      this.m_spawnedEquipment.Clear();
      GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      GM.CurrentSceneSettings.DeathResetPoint.position = this.InitialRespawnPos;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.InitialRespawnRot;
      base.EndMode(doesInvokeTeleport, immediateTeleportBackAndScore);
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
          if ((Object) component != (Object) null && (Object) component.ObjectWrapper != (Object) null && component.ObjectWrapper.Category != FVRObject.ObjectCategory.MeleeWeapon)
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

    private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsCivvie)
    {
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0)
        weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      this.m_activeSosigs.Add(this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF, IsCivvie));
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      GameObject weaponPrefab2,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig w,
      int IFF,
      bool ShouldWander)
    {
      Sosig componentInChildren = Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      componentInChildren.Configure(t);
      componentInChildren.E.IFFCode = IFF;
      if ((Object) weaponPrefab != (Object) null)
      {
        SosigWeapon component1 = Object.Instantiate<GameObject>(weaponPrefab, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
        component1.SetAutoDestroy(true);
        component1.O.SpawnLockable = false;
        if (component1.Type == SosigWeapon.SosigWeaponType.Gun)
        {
          component1.FlightVelocityMultiplier = 0.15f;
          componentInChildren.Inventory.FillAmmoWithType(component1.AmmoType);
        }
        componentInChildren.Inventory.FillAllAmmo();
        if ((Object) component1 != (Object) null)
        {
          componentInChildren.InitHands();
          componentInChildren.ForceEquip(component1);
        }
        if ((Object) weaponPrefab2 != (Object) null)
        {
          SosigWeapon component2 = Object.Instantiate<GameObject>(weaponPrefab2, pos + Vector3.up * 0.1f, rot).GetComponent<SosigWeapon>();
          component2.SetAutoDestroy(true);
          component2.O.SpawnLockable = false;
          if (component2.Type == SosigWeapon.SosigWeaponType.Gun)
          {
            component2.FlightVelocityMultiplier = 0.15f;
            componentInChildren.Inventory.FillAmmoWithType(component2.AmmoType);
          }
          if ((Object) component2 != (Object) null)
            componentInChildren.ForceEquip(component2);
        }
      }
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Headwear)
        this.SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Facewear)
        this.SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Torsowear)
        this.SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear)
        this.SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) Random.Range(0.0f, 1f) < (double) w.Chance_Backpacks)
        this.SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          if ((double) Random.Range(0.0f, 1f) < (double) t.LinkSpawnChance[index])
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
        }
      }
      if (ShouldWander)
      {
        componentInChildren.CurrentOrder = Sosig.SosigOrder.Wander;
        componentInChildren.FallbackOrder = Sosig.SosigOrder.Wander;
      }
      else
      {
        componentInChildren.CurrentOrder = Sosig.SosigOrder.Assault;
        componentInChildren.FallbackOrder = Sosig.SosigOrder.Assault;
        componentInChildren.CommandAssaultPoint(this.EnemyEndPoints[Random.Range(0, this.EnemyEndPoints.Count)].position);
      }
      componentInChildren.SetDominantGuardDirection(Random.onUnitSphere);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (this.m_activeSosigs.Contains(s))
      {
        if (this.IsPlaying && s.GetDiedFromIFF() == GM.CurrentPlayerBody.GetPlayerIFF())
          this.ScoreSosid(s);
        s.TickDownToClear(5f);
        this.m_activeSosigs.Remove(s);
      }
      if (this.m_activeSosigs.Count >= 1 || !this.IsPlaying || this.m_isWaitingForWave)
        return;
      this.AdvanceWave();
    }

    private void SpawnWave()
    {
      this.m_isWaitingForWave = false;
      HG_SpawnPointGroup grillSpawnZone = this.GrillSpawnZones[this.m_waveNumber];
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
      for (int index = 0; index < grillSpawnZone.Points.Count; ++index)
      {
        if (this.m_mode == HG_ModeManager.HG_Mode.KingOfTheGrill_Anachronism)
        {
          float num = Random.Range(0.0f, 1f);
          if ((double) num < 0.300000011920929)
            this.SpawnEnemy(this.SosigEnemyTemplates_Ressurection[this.m_waveNumber], grillSpawnZone.Points[index], 1, false);
          else if ((double) num < 0.5)
            this.SpawnEnemy(this.SosigEnemyTemplates_Anachronism[this.m_waveNumber], grillSpawnZone.Points[index], 1, false);
          else
            this.SpawnEnemy(this.SosigEnemyTemplates_Invasion[this.m_waveNumber], grillSpawnZone.Points[index], 1, false);
        }
        else
        {
          int waveNumber = this.m_waveNumber;
          int min = this.m_waveNumber - 2;
          if (min < 0)
            min = 0;
          this.SpawnEnemy(this.m_curEnemyTemplateList[Random.Range(min, waveNumber + 1)], grillSpawnZone.Points[index], 1, false);
        }
      }
    }

    private void SpawnLoot()
    {
      MM_LootCrate component = Object.Instantiate<GameObject>(this.LootCrate, this.LootCrateSpawns[Random.Range(0, this.LootCrateSpawns.Count)].position, Random.rotation).GetComponent<MM_LootCrate>();
      int waveNumber = this.m_waveNumber;
      if (waveNumber >= this.LootTables.Count)
      {
        component.Init((FVRObject) null, (FVRObject) null, this.UnCookedPowerup, this.UnCookedPowerup);
      }
      else
      {
        int index = Random.Range(0, this.LootTables[waveNumber].Objects.Count);
        FVRObject fvrObject = (FVRObject) null;
        if (this.LootTables[waveNumber].Objects.Count > 0)
          fvrObject = this.LootTables[waveNumber].Objects[index];
        component.Init(fvrObject, (FVRObject) null, this.UnCookedPowerup, this.UnCookedPowerup);
      }
    }

    private void AdvanceWave()
    {
      ++this.m_waveNumber;
      if (this.m_waveNumber >= 15)
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_SequenceCompleted, this.transform.position);
        this.M.Case();
        this.EndMode(true, false);
      }
      else
      {
        this.CleanUpScene();
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
        this.m_isWaitingForWave = true;
        this.m_tickDownToWave = 59f;
        this.SpawnLoot();
      }
    }

    public void DepositPowerUp(PowerupType type)
    {
      ++this.m_numPowerUpsCooked;
      switch (type)
      {
        case PowerupType.Health:
          this.m_numPowerUpsCooked += 3;
          break;
        case PowerupType.InfiniteAmmo:
          this.m_numPowerUpsCooked += 2;
          break;
        case PowerupType.Regen:
          this.m_numPowerUpsCooked += 3;
          break;
      }
    }

    private void ScoreSosid(Sosig s)
    {
      ++this.m_numSosigsKilled;
      float num = 5f;
      if (s.GetDiedFromIFF() == GM.CurrentPlayerBody.GetPlayerIFF())
      {
        if (s.GetDiedFromHeadShot())
          num += 15f;
        Damage.DamageClass diedFromClass = s.GetDiedFromClass();
        Sosig.SosigDeathType diedFromType = s.GetDiedFromType();
        switch (diedFromClass)
        {
          case Damage.DamageClass.Projectile:
            num += 10f;
            break;
          case Damage.DamageClass.Explosive:
            num += 5f;
            break;
          case Damage.DamageClass.Melee:
            num += 25f;
            break;
          case Damage.DamageClass.Environment:
            num += 30f;
            break;
        }
        switch (diedFromType)
        {
          case Sosig.SosigDeathType.BleedOut:
            num += 10f;
            break;
          case Sosig.SosigDeathType.JointSever:
            num += 35f;
            break;
          case Sosig.SosigDeathType.JointExplosion:
            num += 5f;
            break;
          case Sosig.SosigDeathType.JointBreak:
            num += 15f;
            break;
          case Sosig.SosigDeathType.JointPullApart:
            num += 15f;
            break;
        }
      }
      this.m_sosigScore += num;
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

    public override int GetScore()
    {
      float num = 0.0f;
      if (this.m_numSosigsKilled > 0)
        num = this.m_sosigScore / (float) this.m_numSosigsKilled;
      return this.m_numSosigsKilled * 1000 + (int) num * 500 + this.m_numPowerUpsCooked * 6000;
    }

    public override List<string> GetScoringReadOuts()
    {
      float num = 0.0f;
      if (this.m_numSosigsKilled > 0)
        num = this.m_sosigScore / (float) this.m_numSosigsKilled;
      return new List<string>()
      {
        "Base Score: " + (this.m_numPowerUpsCooked * 6000).ToString(),
        "Sosig Bonus: " + (this.m_numSosigsKilled * 1000).ToString(),
        "Style Bonus: " + ((int) num * 500).ToString(),
        "Final Score: " + this.GetScore().ToString()
      };
    }

    public string FloatToTime(float toConvert, string format)
    {
      if (format != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (HG_ModeManager_KingOfTheGrill.\u003C\u003Ef__switch\u0024map5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          HG_ModeManager_KingOfTheGrill.\u003C\u003Ef__switch\u0024map5 = new Dictionary<string, int>(13)
          {
            {
              "00.0",
              0
            },
            {
              "#0.0",
              1
            },
            {
              "00.00",
              2
            },
            {
              "00.000",
              3
            },
            {
              "#00.000",
              4
            },
            {
              "#0:00",
              5
            },
            {
              "#00:00",
              6
            },
            {
              "0:00.0",
              7
            },
            {
              "#0:00.0",
              8
            },
            {
              "0:00.00",
              9
            },
            {
              "#0:00.00",
              10
            },
            {
              "0:00.000",
              11
            },
            {
              "#0:00.000",
              12
            }
          };
        }
        int num;
        // ISSUE: reference to a compiler-generated field
        if (HG_ModeManager_KingOfTheGrill.\u003C\u003Ef__switch\u0024map5.TryGetValue(format, out num))
        {
          switch (num)
          {
            case 0:
              return string.Format("{0:00}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 1:
              return string.Format("{0:#0}:{1:0}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 2:
              return string.Format("{0:00}:{1:00}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 3:
              return string.Format("{0:00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 4:
              return string.Format("{0:#00}:{1:000}", (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 5:
              return string.Format("{0:#0}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 6:
              return string.Format("{0:#00}:{1:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0));
            case 7:
              return string.Format("{0:0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 8:
              return string.Format("{0:#0}:{1:00}.{2:0}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 10.0 % 10.0)));
            case 9:
              return string.Format("{0:0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 10:
              return string.Format("{0:#0}:{1:00}.{2:00}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 100.0 % 100.0)));
            case 11:
              return string.Format("{0:0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
            case 12:
              return string.Format("{0:#0}:{1:00}.{2:000}", (object) Mathf.Floor(toConvert / 60f), (object) (float) ((double) Mathf.Floor(toConvert) % 60.0), (object) Mathf.Floor((float) ((double) toConvert * 1000.0 % 1000.0)));
          }
        }
      }
      return "error";
    }
  }
}
