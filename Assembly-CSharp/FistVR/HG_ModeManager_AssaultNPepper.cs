// Decompiled with JetBrains decompiler
// Type: FistVR.HG_ModeManager_AssaultNPepper
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_ModeManager_AssaultNPepper : HG_ModeManager
  {
    public List<SosigEnemyTemplate> Templates_Enemy;
    public SosigEnemyTemplate Template_Civvie;
    public GameObject SpawningCore_Prefab;
    public GameObject Indicator_Prefab;
    public List<int> ZoneSequence_Skirmish;
    public List<int> ZoneSequence_Brawl;
    public List<int> ZoneSequence_Maelstrom;
    public List<Transform> CivvieSpawnPoints;
    private List<int> m_curZoneSequence;
    private int m_curZoneInSequence;
    private int m_numZonesActiveAtOnce;
    private List<HG_Target> m_activeTargets = new List<HG_Target>();
    private List<HG_Zone> m_activeSpawningZones = new List<HG_Zone>();
    private List<Sosig> m_activeSosigs = new List<Sosig>();
    private List<Sosig> m_civvieSosigs = new List<Sosig>();
    private int m_maxSosigs = 8;
    private float m_TickToSpawn = 20f;
    private int m_difficulty;
    [Header("Audio")]
    public AudioEvent AudEvent_ZoneCompleted;
    public AudioEvent AudEvent_SequenceCompleted;
    private float m_timer;
    private float m_sosigScore;
    private int m_numSosigsKilled;
    private int m_numCivvieKills;

    public override void InitMode(HG_ModeManager.HG_Mode mode)
    {
      this.m_mode = mode;
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      this.m_activeTargets.Clear();
      this.m_activeSpawningZones.Clear();
      switch (mode)
      {
        case HG_ModeManager.HG_Mode.AssaultNPepper_Skirmish:
          this.m_curZoneSequence = this.ZoneSequence_Skirmish;
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Brawl:
          this.m_curZoneSequence = this.ZoneSequence_Brawl;
          break;
        case HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom:
          this.m_curZoneSequence = this.ZoneSequence_Maelstrom;
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Neophyte:
          this.m_curZoneSequence = this.ZoneSequence_Skirmish;
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Warrior:
          this.m_curZoneSequence = this.ZoneSequence_Brawl;
          break;
        case HG_ModeManager.HG_Mode.MeatNMetal_Veteran:
          this.m_curZoneSequence = this.ZoneSequence_Maelstrom;
          break;
      }
      this.m_curZoneInSequence = 0;
      Transform playerSpawnPoint = this.M.Zones[this.m_curZoneSequence[this.m_curZoneInSequence]].PlayerSpawnPoint;
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(playerSpawnPoint.position, true, playerSpawnPoint.forward);
      this.InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
      this.InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
      GM.CurrentSceneSettings.DeathResetPoint.position = playerSpawnPoint.position;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = playerSpawnPoint.rotation;
      ++this.m_curZoneInSequence;
      this.ConfigureCurrentZone(true);
      if (mode == HG_ModeManager.HG_Mode.AssaultNPepper_Brawl || mode == HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom || (mode == HG_ModeManager.HG_Mode.MeatNMetal_Warrior || mode == HG_ModeManager.HG_Mode.MeatNMetal_Veteran))
      {
        ++this.m_curZoneInSequence;
        this.ConfigureCurrentZone(true);
      }
      if (mode == HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom || mode == HG_ModeManager.HG_Mode.MeatNMetal_Veteran)
      {
        ++this.m_curZoneInSequence;
        this.ConfigureCurrentZone(true);
      }
      this.m_timer = 0.0f;
      this.m_sosigScore = 0.0f;
      this.m_numSosigsKilled = 0;
      this.m_numCivvieKills = 0;
      this.SpawnCivvies();
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
      this.m_TickToSpawn = 10f;
      this.IsPlaying = true;
    }

    private void SpawnCivvies()
    {
      this.CivvieSpawnPoints.Shuffle<Transform>();
      this.CivvieSpawnPoints.Shuffle<Transform>();
      this.CivvieSpawnPoints.Shuffle<Transform>();
      for (int index = 0; index < 5; ++index)
        this.SpawnEnemy(this.Template_Civvie, this.CivvieSpawnPoints[index], 1, true);
    }

    public void Update()
    {
      if (!this.IsPlaying)
        return;
      this.m_timer += Time.deltaTime;
      this.m_TickToSpawn -= Time.deltaTime;
      if ((double) this.m_TickToSpawn < 0.0)
      {
        this.m_TickToSpawn = Random.Range(18f, 22f);
        this.SpawnRoutine();
      }
      for (int index = 0; index < this.m_activeSpawningZones.Count; ++index)
      {
        if ((Object) this.m_activeSpawningZones[index].Indicator != (Object) null)
          this.m_activeSpawningZones[index].Indicator.transform.position = this.m_activeSpawningZones[index].transform.position + Vector3.up * 14f + Vector3.up * (Mathf.Sin(Time.time * 4f) * 1.5f);
      }
    }

    private void SpawnRoutine()
    {
      int num1 = 2;
      if (this.m_mode == HG_ModeManager.HG_Mode.AssaultNPepper_Brawl)
        num1 += 2;
      else if (this.m_mode == HG_ModeManager.HG_Mode.AssaultNPepper_Maelstrom)
        num1 += 4;
      int b = Mathf.Clamp(this.m_maxSosigs - this.m_activeSosigs.Count, 0, this.m_maxSosigs);
      int num2 = num1;
      int num3 = Mathf.Min(this.m_activeSpawningZones.Count <= 2 ? (this.m_activeSpawningZones.Count <= 1 ? 2 : (num2 <= 2 ? 1 : 2)) : (num2 <= 4 ? 1 : 2), b);
      if (num3 <= 0 || this.m_activeSpawningZones.Count <= 0)
        return;
      for (int index1 = 0; index1 < this.m_activeSpawningZones.Count; ++index1)
      {
        this.m_activeSpawningZones[index1].SpawnPoints_Defense.Shuffle<Transform>();
        if ((double) Vector3.Distance(GM.CurrentPlayerBody.transform.position, this.m_activeSpawningZones[index1].transform.position) >= 10.0)
        {
          for (int index2 = 0; index2 < num3; ++index2)
            this.SpawnEnemyAtPoint(this.m_activeSpawningZones[index1].SpawnPoints_Defense[index2]);
        }
      }
    }

    private void SpawnEnemyAtPoint(Transform point) => this.SpawnEnemy(this.Templates_Enemy[Random.Range(0, Mathf.Min(this.m_difficulty, this.Templates_Enemy.Count))], point, 1, false);

    public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
    {
      if (!this.IsPlaying)
        return;
      this.IsPlaying = false;
      for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        this.m_activeSosigs[index].ClearSosig();
      this.m_activeSosigs.Clear();
      for (int index = this.m_civvieSosigs.Count - 1; index >= 0; --index)
        this.m_civvieSosigs[index].ClearSosig();
      this.m_civvieSosigs.Clear();
      GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      GM.CurrentSceneSettings.DeathResetPoint.position = this.InitialRespawnPos;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.InitialRespawnRot;
      this.M.Case();
      base.EndMode(true, false);
    }

    private void ConfigureCurrentZone(bool isBeginning)
    {
      HG_Zone zone = this.M.Zones[this.m_curZoneSequence[this.m_curZoneInSequence]];
      HG_Target component = Object.Instantiate<GameObject>(this.SpawningCore_Prefab, zone.PlayerSpawnPoint.position + Vector3.up * 2f, Quaternion.identity).GetComponent<HG_Target>();
      component.Init((HG_ModeManager) this, zone);
      this.m_activeTargets.Add(component);
      this.m_activeSpawningZones.Add(zone);
      zone.Indicator = Object.Instantiate<GameObject>(this.Indicator_Prefab, zone.transform.position + Vector3.up * 14f, Quaternion.identity).transform;
      this.m_TickToSpawn = Mathf.Max(this.m_TickToSpawn, 5f);
      if (isBeginning)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, zone.transform.position);
    }

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (this.m_activeSosigs.Contains(s))
      {
        if (this.IsPlaying)
          this.ScoreSosid(s);
        s.TickDownToClear(5f);
        this.m_activeSosigs.Remove(s);
      }
      else
      {
        if (!this.m_civvieSosigs.Contains(s))
          return;
        if (this.IsPlaying)
          ++this.m_numCivvieKills;
        s.TickDownToClear(5f);
        this.m_civvieSosigs.Remove(s);
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

    public override void TargetDestroyed(HG_Target t)
    {
      HG_Zone zone = t.GetZone();
      if ((Object) zone.Indicator != (Object) null)
        Object.Destroy((Object) zone.Indicator.gameObject);
      this.m_activeSpawningZones.Remove(zone);
      ++this.m_difficulty;
      ++this.m_curZoneInSequence;
      if (this.m_curZoneInSequence < this.m_curZoneSequence.Count)
        this.ConfigureCurrentZone(false);
      if (this.m_activeSpawningZones.Count >= 1)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_SequenceCompleted, this.transform.position);
      this.EndMode(true, false);
    }

    private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsCivvie)
    {
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0)
        weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      Sosig sosig = this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF);
      if (IsCivvie)
        this.m_civvieSosigs.Add(sosig);
      else
        this.m_activeSosigs.Add(sosig);
    }

    private Sosig SpawnEnemySosig(
      GameObject prefab,
      GameObject weaponPrefab,
      GameObject weaponPrefab2,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig w,
      int IFF)
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
      componentInChildren.CurrentOrder = Sosig.SosigOrder.Wander;
      componentInChildren.FallbackOrder = Sosig.SosigOrder.Wander;
      componentInChildren.SetDominantGuardDirection(Random.onUnitSphere);
      return componentInChildren;
    }

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(gs[Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    public override int GetScore()
    {
      float num = 0.0f;
      if (this.m_numSosigsKilled > 0)
        num = this.m_sosigScore / (float) this.m_numSosigsKilled;
      return (this.m_curZoneSequence.Count - 1) * 750 + this.m_numSosigsKilled * 10 + Mathf.Max((this.m_curZoneSequence.Count * 60 - (int) this.m_timer) * 10, 0) + (int) num * 200 - this.m_numCivvieKills * 200;
    }

    public override List<string> GetScoringReadOuts()
    {
      float num = 0.0f;
      if (this.m_numSosigsKilled > 0)
        num = this.m_sosigScore / (float) this.m_numSosigsKilled;
      return new List<string>()
      {
        "Base Score: " + ((this.m_curZoneSequence.Count - 1) * 750 + this.m_numSosigsKilled * 10).ToString(),
        "Time Bonus: " + Mathf.Max((this.m_curZoneSequence.Count * 60 - (int) this.m_timer) * 10, 0).ToString(),
        "Style Bonus: " + ((int) num * 200).ToString(),
        "Friendly Fire Penalty: " + (this.m_numCivvieKills * -200).ToString(),
        "Final Score: " + this.GetScore().ToString()
      };
    }
  }
}
