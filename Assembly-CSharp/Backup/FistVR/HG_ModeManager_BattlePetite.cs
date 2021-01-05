// Decompiled with JetBrains decompiler
// Type: FistVR.HG_ModeManager_BattlePetite
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_ModeManager_BattlePetite : HG_ModeManager
  {
    public List<ZosigEnemyTemplate> EnemyTemplates;
    public ZosigEnemyTemplate CivvieTemplate;
    public List<SosigEnemyTemplate> STemplates_Ranged;
    public List<SosigEnemyTemplate> STemplates_Melee;
    public SosigEnemyTemplate STemplate_Civvie;
    public List<FVRObject> SpawnedEquipment_Sosigguns;
    public List<FVRObject> SpawnedEquipment_Sosigmelee;
    public List<FVRObject> StartingSpawnedEquipment_Player;
    public List<Transform> BR_SpawnZones_Player;
    public List<Transform> BR_SpawnZones_Equipment;
    public List<Transform> BR_SpawnZones_Civvies;
    private List<Sosig> m_activeSosigs = new List<Sosig>();
    private List<Sosig> m_civvieSosigs = new List<Sosig>();
    private List<GameObject> m_spawnedEquipment = new List<GameObject>();
    [Header("Audio")]
    public AudioEvent AudEvent_ZoneCompleted;
    public AudioEvent AudEvent_SequenceCompleted;
    private float m_timer;
    private float m_sosigScore;
    private int m_numSosigsKilled;
    private int m_numCivvieKills;
    public Transform RallyPoint;
    public Vector3 StripSearch_Center;
    public Vector3 StripSearch_HalfExtents;
    public LayerMask SearchLM;

    public override void InitMode(HG_ModeManager.HG_Mode mode)
    {
      this.CleanArena();
      this.m_mode = mode;
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      this.m_activeSosigs.Clear();
      this.m_civvieSosigs.Clear();
      this.BR_SpawnZones_Player.Shuffle<Transform>();
      this.BR_SpawnZones_Player.Shuffle<Transform>();
      this.BR_SpawnZones_Player.Shuffle<Transform>();
      Transform transform = this.BR_SpawnZones_Player[0];
      double point = (double) GM.CurrentMovementManager.TeleportToPoint(transform.position, true, transform.forward);
      this.InitialRespawnPos = GM.CurrentSceneSettings.DeathResetPoint.position;
      this.InitialRespawnRot = GM.CurrentSceneSettings.DeathResetPoint.rotation;
      GM.CurrentSceneSettings.DeathResetPoint.position = transform.position;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = transform.rotation;
      this.StripEquipment(mode);
      if (mode == HG_ModeManager.HG_Mode.BattlePetite_Sosiggun)
      {
        this.m_spawnedEquipment.Add(Object.Instantiate<GameObject>(this.StartingSpawnedEquipment_Player[Random.Range(0, this.StartingSpawnedEquipment_Player.Count)].GetGameObject(), this.BR_SpawnZones_Player[0].position + this.BR_SpawnZones_Player[0].forward + Vector3.up, Random.rotation));
        this.m_spawnedEquipment.Add(Object.Instantiate<GameObject>(this.SpawnedEquipment_Sosigmelee[Random.Range(0, this.SpawnedEquipment_Sosigmelee.Count)].GetGameObject(), this.BR_SpawnZones_Player[0].position + this.BR_SpawnZones_Player[0].right + Vector3.up, Random.rotation));
      }
      else if (mode == HG_ModeManager.HG_Mode.BattlePetite_Melee)
        this.m_spawnedEquipment.Add(Object.Instantiate<GameObject>(this.SpawnedEquipment_Sosigmelee[Random.Range(0, this.SpawnedEquipment_Sosigmelee.Count)].GetGameObject(), this.BR_SpawnZones_Player[0].position + this.BR_SpawnZones_Player[0].forward + Vector3.up, Random.rotation));
      if (mode == HG_ModeManager.HG_Mode.BattlePetite_Melee)
      {
        for (int index = 1; index < this.BR_SpawnZones_Player.Count; ++index)
          this.SpawnEnemy(this.STemplates_Melee[Random.Range(0, this.STemplates_Melee.Count)], this.BR_SpawnZones_Player[index], index, false);
      }
      else
      {
        for (int index = 1; index < this.BR_SpawnZones_Player.Count; ++index)
          this.SpawnEnemy(this.STemplates_Ranged[Random.Range(0, this.STemplates_Ranged.Count)], this.BR_SpawnZones_Player[index], index, false);
      }
      this.BR_SpawnZones_Equipment.Shuffle<Transform>();
      this.BR_SpawnZones_Equipment.Shuffle<Transform>();
      this.BR_SpawnZones_Equipment.Shuffle<Transform>();
      switch (mode)
      {
        case HG_ModeManager.HG_Mode.BattlePetite_Open:
        case HG_ModeManager.HG_Mode.BattlePetite_Sosiggun:
          for (int index = 0; index < 30; ++index)
            this.m_spawnedEquipment.Add(Object.Instantiate<GameObject>(this.SpawnedEquipment_Sosigguns[Random.Range(0, this.SpawnedEquipment_Sosigguns.Count)].GetGameObject(), this.BR_SpawnZones_Equipment[index].position + Vector3.up, Random.rotation));
          break;
        case HG_ModeManager.HG_Mode.BattlePetite_Melee:
          for (int index = 0; index < 30; ++index)
            this.m_spawnedEquipment.Add(Object.Instantiate<GameObject>(this.SpawnedEquipment_Sosigmelee[Random.Range(0, this.SpawnedEquipment_Sosigmelee.Count)].GetGameObject(), this.BR_SpawnZones_Equipment[index].position + Vector3.up, Random.rotation));
          break;
      }
      this.m_timer = 0.0f;
      this.m_sosigScore = 0.0f;
      this.m_numSosigsKilled = 0;
      this.m_numCivvieKills = 0;
      this.SpawnCivvies();
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
      this.IsPlaying = true;
    }

    public override void HandlePlayerDeath() => this.EndMode(false, true);

    public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
    {
      this.IsPlaying = false;
      if (this.m_activeSosigs.Count > 0)
      {
        for (int index = this.m_activeSosigs.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_activeSosigs[index] != (Object) null)
            this.m_activeSosigs[index].ClearSosig();
        }
      }
      this.m_activeSosigs.Clear();
      if (this.m_civvieSosigs.Count > 0)
      {
        for (int index = this.m_civvieSosigs.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_civvieSosigs[index] != (Object) null)
            this.m_civvieSosigs[index].ClearSosig();
        }
      }
      this.m_civvieSosigs.Clear();
      for (int index = this.m_spawnedEquipment.Count - 1; index >= 0; --index)
        Object.Destroy((Object) this.m_spawnedEquipment[index]);
      this.m_spawnedEquipment.Clear();
      GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      GM.CurrentSceneSettings.DeathResetPoint.position = this.InitialRespawnPos;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.InitialRespawnRot;
      this.M.Case();
      base.EndMode(doesInvokeTeleport, immediateTeleportBackAndScore);
    }

    private void StripEquipment(HG_ModeManager.HG_Mode mode)
    {
      Collider[] colliderArray = Physics.OverlapBox(this.StripSearch_Center, this.StripSearch_HalfExtents, Quaternion.identity, (int) this.SearchLM, QueryTriggerInteraction.Collide);
      if (mode != HG_ModeManager.HG_Mode.BattlePetite_Sosiggun && mode != HG_ModeManager.HG_Mode.BattlePetite_Melee)
        return;
      for (int index = 0; index < colliderArray.Length; ++index)
      {
        Collider collider = colliderArray[index];
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

    private void SpawnCivvies()
    {
      this.BR_SpawnZones_Civvies.Shuffle<Transform>();
      this.BR_SpawnZones_Civvies.Shuffle<Transform>();
      this.BR_SpawnZones_Civvies.Shuffle<Transform>();
      for (int index = 0; index < 5; ++index)
        this.SpawnEnemy(this.STemplate_Civvie, this.BR_SpawnZones_Civvies[index], -3, true);
    }

    private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF, bool IsCivvie)
    {
      GameObject weaponPrefab = (GameObject) null;
      if (t.WeaponOptions.Count > 0)
        weaponPrefab = t.WeaponOptions[Random.Range(0, t.WeaponOptions.Count)].GetGameObject();
      GameObject weaponPrefab2 = (GameObject) null;
      if (t.WeaponOptions_Secondary.Count > 0)
        weaponPrefab2 = t.WeaponOptions_Secondary[Random.Range(0, t.WeaponOptions_Secondary.Count)].GetGameObject();
      Sosig sosig = this.SpawnEnemySosig(t.SosigPrefabs[Random.Range(0, t.SosigPrefabs.Count)].GetGameObject(), weaponPrefab, weaponPrefab2, point.position, point.rotation, t.ConfigTemplates[Random.Range(0, t.ConfigTemplates.Count)], t.OutfitConfig[Random.Range(0, t.OutfitConfig.Count)], IFF, IsCivvie);
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
        componentInChildren.CommandAssaultPoint(this.RallyPoint.position);
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
      else if (this.m_civvieSosigs.Contains(s))
      {
        if (this.IsPlaying && s.GetDiedFromIFF() == GM.CurrentPlayerBody.GetPlayerIFF())
          ++this.m_numCivvieKills;
        s.TickDownToClear(5f);
        this.m_civvieSosigs.Remove(s);
      }
      if (this.m_activeSosigs.Count >= 1 || !this.IsPlaying)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_SequenceCompleted, this.transform.position);
      this.EndMode(true, false);
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

    public override int GetScore()
    {
      float num = 0.0f;
      if (this.m_numSosigsKilled > 0)
        num = this.m_sosigScore / (float) this.m_numSosigsKilled;
      return this.m_numSosigsKilled * 500 + (int) num * 150 - this.m_numCivvieKills * 200;
    }

    public override List<string> GetScoringReadOuts()
    {
      float num = 0.0f;
      if (this.m_numSosigsKilled > 0)
        num = this.m_sosigScore / (float) this.m_numSosigsKilled;
      return new List<string>()
      {
        "Base Score: " + (this.m_numSosigsKilled * 500).ToString(),
        "Style Bonus: " + ((int) num * 150).ToString(),
        "Friendly Fire Penalty: " + (this.m_numCivvieKills * -200).ToString(),
        "Final Score: " + this.GetScore().ToString()
      };
    }
  }
}
