// Decompiled with JetBrains decompiler
// Type: FistVR.WinterEnemySpawnZone
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class WinterEnemySpawnZone : MonoBehaviour
  {
    [Header("Volumes And Spawn Points")]
    public List<Transform> VolumesToCheck;
    public List<Transform> SpawnPoints;
    public Sosig.SosigOrder DefaultOrder;
    [Header("Spawn Setting Params")]
    public Vector2 SpawnRange = new Vector2(20f, 200f);
    public int MaxCanBeAlive = 5;
    public float DespawnRange = 500f;
    public bool DoesSpawnOnEntry;
    public Vector2 OnEntryCooldown = new Vector2(120f, 180f);
    private float m_onEntryCooldownTick;
    public bool CanSpawnInView;
    [Header("Max TOTAL Count Params")]
    public bool UsesMaxTotalSpawnedCount;
    public int MaxToSpawnEver;
    private int m_spawnedSofar;
    private int m_numOfMineWhoveDied;
    private bool m_hasHitMaxDiedOfMineThreshold;
    public Vector2 RefireTickRangeAfterSpawnFailure = new Vector2(30f, 300f);
    public Vector2 RefireTickRangeAfterSpawnSuccess = new Vector2(30f, 300f);
    [Header("Patrol Settings")]
    public bool IsPatrolZone;
    public List<Transform> PatrolPoints;
    public List<Transform> PatrolSpawnPoints;
    private int m_curPatrolPoint;
    private bool m_isPatrollingUp = true;
    private float m_timeUntilSpawnCheck = 100f;
    private List<Sosig> m_spawnedSosigs = new List<Sosig>();
    public WinterEnemySpawnZone.SpawnGroup Group;
    private bool m_wasIn;

    private void Start()
    {
      GM.CurrentSceneSettings.SosigKillEvent += new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);
      if (!this.IsPatrolZone)
        return;
      this.PatrolSpawnPoints.Shuffle<Transform>();
      this.PatrolSpawnPoints.Shuffle<Transform>();
    }

    private void OnDestroy() => GM.CurrentSceneSettings.SosigKillEvent -= new FVRSceneSettings.SosigKill(this.CheckIfDeadSosigWasMine);

    private void Update()
    {
      if ((double) this.m_timeUntilSpawnCheck >= 0.0)
        this.m_timeUntilSpawnCheck -= Time.deltaTime;
      if ((double) this.m_onEntryCooldownTick >= 0.0)
        this.m_onEntryCooldownTick -= Time.deltaTime;
      if (this.IsPatrolZone)
      {
        if (this.m_spawnedSosigs.Count > 0)
        {
          bool flag = true;
          for (int index = 0; index < this.m_spawnedSosigs.Count; ++index)
          {
            if ((double) Vector3.Distance(this.m_spawnedSosigs[index].transform.position, this.PatrolPoints[this.m_curPatrolPoint].position) > 10.0)
              flag = false;
          }
          if (flag)
          {
            if (this.m_curPatrolPoint + 1 >= this.PatrolPoints.Count && this.m_isPatrollingUp)
              this.m_isPatrollingUp = false;
            if (this.m_curPatrolPoint == 0)
              this.m_isPatrollingUp = true;
            if (this.m_isPatrollingUp)
              ++this.m_curPatrolPoint;
            else
              --this.m_curPatrolPoint;
            for (int index = 0; index < this.m_spawnedSosigs.Count; ++index)
              this.m_spawnedSosigs[index].CommandAssaultPoint(this.PatrolPoints[this.m_curPatrolPoint].position);
          }
          for (int index = 0; index < this.m_spawnedSosigs.Count; ++index)
          {
            if (this.m_spawnedSosigs[index].CurrentOrder == Sosig.SosigOrder.Wander)
              this.m_spawnedSosigs[index].CurrentOrder = Sosig.SosigOrder.Assault;
            this.m_spawnedSosigs[index].FallbackOrder = Sosig.SosigOrder.Assault;
          }
        }
        else
        {
          this.m_curPatrolPoint = 0;
          this.m_isPatrollingUp = true;
        }
      }
      this.Check();
    }

    private void Check()
    {
      this.CheckDespawn();
      Vector3 position = GM.CurrentPlayerBody.Head.position;
      bool flag1 = false;
      if (this.IsPlayerInAnyVolumes(position))
      {
        if (!this.m_wasIn)
        {
          this.m_wasIn = true;
          if (this.DoesSpawnOnEntry && (double) this.m_onEntryCooldownTick <= 0.0)
          {
            flag1 = true;
            this.m_onEntryCooldownTick = UnityEngine.Random.Range(this.OnEntryCooldown.x, this.OnEntryCooldown.y);
          }
        }
      }
      else
        this.m_wasIn = false;
      if ((double) this.m_timeUntilSpawnCheck > 0.0 && !flag1)
        return;
      this.m_timeUntilSpawnCheck = UnityEngine.Random.Range(this.RefireTickRangeAfterSpawnFailure.x, this.RefireTickRangeAfterSpawnFailure.y);
      if (this.m_spawnedSosigs.Count >= this.MaxCanBeAlive || this.UsesMaxTotalSpawnedCount && this.m_spawnedSofar >= this.MaxToSpawnEver || !this.IsPlayerInAnyVolumes(position))
        return;
      Vector3 forward = GM.CurrentPlayerBody.Head.forward;
      bool flag2 = false;
      WinterEnemySpawnZone.SpawnGroup group = this.Group;
      int num = UnityEngine.Random.Range(group.MinSpawnedInGroup, group.MaxSpawnedInGroup + 1);
      List<Transform> ts = new List<Transform>();
      if (this.IsPatrolZone)
      {
        for (int index = 0; index < this.PatrolSpawnPoints.Count; ++index)
          ts.Add(this.PatrolSpawnPoints[index]);
      }
      else
      {
        for (int index = 0; index < this.SpawnPoints.Count; ++index)
          ts.Add(this.SpawnPoints[index]);
      }
      if (ts.Count > 0)
        ts.Shuffle<Transform>();
      for (int index1 = 0; index1 < num && ts.Count >= 1 && (!this.UsesMaxTotalSpawnedCount || this.m_spawnedSofar < this.MaxToSpawnEver); ++index1)
      {
        bool flag3 = false;
        int index2 = 0;
        for (int index3 = ts.Count - 1; index3 >= 0; --index3)
        {
          if (this.IsUsefulPoint(ts[index3].position, position, forward))
          {
            flag3 = true;
            index2 = index3;
            break;
          }
        }
        if (flag3)
        {
          Transform point = ts[index2];
          ts.RemoveAt(index2);
          this.SpawnEnemy(group.GetTemplate(), point, group.IFF);
          flag2 = true;
          ++this.m_spawnedSofar;
        }
      }
      ts.Clear();
      if (!flag2)
        return;
      this.m_timeUntilSpawnCheck = UnityEngine.Random.Range(this.RefireTickRangeAfterSpawnSuccess.x, this.RefireTickRangeAfterSpawnSuccess.y);
    }

    private void CheckDespawn()
    {
      if (this.m_spawnedSosigs.Count < 1)
        return;
      float a = 0.0f;
      for (int index = 0; index < this.VolumesToCheck.Count; ++index)
        a = Mathf.Max(a, this.DistanceFromClosestPointOnZone(this.VolumesToCheck[index]));
      if ((double) a < (double) this.DespawnRange)
        return;
      for (int index = this.m_spawnedSosigs.Count - 1; index >= 0; --index)
      {
        if (this.m_spawnedSosigs[index].CurrentOrder != Sosig.SosigOrder.Skirmish && this.m_spawnedSosigs[index].CurrentOrder != Sosig.SosigOrder.Investigate)
        {
          Vector3 from = this.m_spawnedSosigs[index].transform.position - GM.CurrentPlayerBody.transform.position;
          if ((double) from.magnitude > 100.0 && (double) Vector3.Angle(from, GM.CurrentPlayerBody.Head.forward) > 90.0)
          {
            this.DespawnSosig(this.m_spawnedSosigs[index]);
            this.m_spawnedSosigs.RemoveAt(index);
          }
        }
      }
    }

    public void DespawnAll()
    {
      if (this.m_spawnedSosigs.Count <= 0)
        return;
      for (int index = this.m_spawnedSosigs.Count - 1; index >= 0; --index)
      {
        if ((UnityEngine.Object) this.m_spawnedSosigs[index] != (UnityEngine.Object) null)
          this.DespawnSosig(this.m_spawnedSosigs[index]);
      }
    }

    private void DespawnSosig(Sosig s)
    {
      for (int index = 0; index < s.Links.Count; ++index)
      {
        s.DestroyAllHeldObjects();
        if ((UnityEngine.Object) s.Links[index] != (UnityEngine.Object) null)
          UnityEngine.Object.Destroy((UnityEngine.Object) s.Links[index].gameObject);
      }
      UnityEngine.Object.Destroy((UnityEngine.Object) s.gameObject);
      --this.m_spawnedSofar;
    }

    public void CheckIfDeadSosigWasMine(Sosig s)
    {
      if (this.m_spawnedSosigs.Count < 1 || !this.m_spawnedSosigs.Contains(s))
        return;
      s.TickDownToClear(5f);
      this.m_spawnedSosigs.Remove(s);
    }

    private float DistanceFromClosestPointOnZone(Transform volume) => Vector3.Distance(new Bounds(volume.position, volume.localScale).ClosestPoint(GM.CurrentPlayerBody.transform.position), GM.CurrentPlayerBody.transform.position);

    private bool IsUsefulPoint(Vector3 p, Vector3 playerPos, Vector3 facing)
    {
      float num = Vector3.Distance(p, playerPos);
      return (double) num >= (double) this.SpawnRange.x && (double) num <= (double) this.SpawnRange.y && (this.CanSpawnInView || (double) Vector3.Angle(p - playerPos, facing) >= 60.0);
    }

    public bool IsPlayerInAnyVolumes(Vector3 p)
    {
      bool flag = false;
      for (int index = 0; index < this.VolumesToCheck.Count; ++index)
      {
        if (this.TestVolumeBool(this.VolumesToCheck[index], p))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public bool TestVolumeBool(Transform t, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }

    private void SpawnEnemy(SosigEnemyTemplate t, Transform point, int IFF) => this.m_spawnedSosigs.Add(this.SpawnEnemySosig(t, point.position, point.forward, IFF));

    private Sosig SpawnEnemySosig(
      SosigEnemyTemplate template,
      Vector3 position,
      Vector3 forward,
      int IFF)
    {
      FVRObject sosigPrefab = template.SosigPrefabs[UnityEngine.Random.Range(0, template.SosigPrefabs.Count)];
      SosigConfigTemplate configTemplate = template.ConfigTemplates[UnityEngine.Random.Range(0, template.ConfigTemplates.Count)];
      SosigOutfitConfig w1 = template.OutfitConfig[UnityEngine.Random.Range(0, template.OutfitConfig.Count)];
      Sosig sosig = this.SpawnSosigAndConfigureSosig(sosigPrefab.GetGameObject(), position, Quaternion.LookRotation(forward, Vector3.up), configTemplate, w1);
      sosig.InitHands();
      sosig.Inventory.Init();
      sosig.Inventory.FillAllAmmo();
      sosig.E.IFFCode = IFF;
      if (template.WeaponOptions.Count > 0)
      {
        SosigWeapon w2 = this.SpawnWeapon(template.WeaponOptions);
        w2.SetAutoDestroy(true);
        w2.SetAmmoClamping(true);
        w2.O.SpawnLockable = false;
        sosig.ForceEquip(w2);
      }
      bool flag1 = false;
      if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) template.SecondaryChance)
        flag1 = true;
      if (template.WeaponOptions_Secondary.Count > 0 && flag1)
      {
        SosigWeapon w2 = this.SpawnWeapon(template.WeaponOptions_Secondary);
        w2.SetAutoDestroy(true);
        w2.SetAmmoClamping(true);
        w2.O.SpawnLockable = false;
        sosig.ForceEquip(w2);
      }
      bool flag2 = false;
      if ((double) UnityEngine.Random.Range(0.0f, 1f) <= (double) template.TertiaryChance)
        flag2 = true;
      if (template.WeaponOptions_Tertiary.Count > 0 && flag2)
      {
        SosigWeapon w2 = this.SpawnWeapon(template.WeaponOptions_Tertiary);
        w2.SetAutoDestroy(true);
        w2.SetAmmoClamping(true);
        w2.O.SpawnLockable = false;
        sosig.ForceEquip(w2);
      }
      if (this.IsPatrolZone)
      {
        sosig.CurrentOrder = Sosig.SosigOrder.Assault;
        sosig.FallbackOrder = Sosig.SosigOrder.Assault;
        sosig.CommandAssaultPoint(this.PatrolPoints[0].position);
      }
      else
      {
        sosig.CurrentOrder = this.DefaultOrder;
        sosig.FallbackOrder = this.DefaultOrder;
        if (this.DefaultOrder == Sosig.SosigOrder.GuardPoint)
        {
          sosig.SetGuardInvestigateDistanceThreshold(40f);
          sosig.SetDominantGuardDirection(forward);
        }
      }
      return sosig;
    }

    private Sosig SpawnSosigAndConfigureSosig(
      GameObject prefab,
      Vector3 pos,
      Quaternion rot,
      SosigConfigTemplate t,
      SosigOutfitConfig w)
    {
      Sosig componentInChildren = UnityEngine.Object.Instantiate<GameObject>(prefab, pos, rot).GetComponentInChildren<Sosig>();
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Headwear)
        this.SpawnAccesoryToLink(w.Headwear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Facewear)
        this.SpawnAccesoryToLink(w.Facewear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Eyewear)
        this.SpawnAccesoryToLink(w.Eyewear, componentInChildren.Links[0]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Torsowear)
        this.SpawnAccesoryToLink(w.Torsowear, componentInChildren.Links[1]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear)
        this.SpawnAccesoryToLink(w.Pantswear, componentInChildren.Links[2]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Pantswear_Lower)
        this.SpawnAccesoryToLink(w.Pantswear_Lower, componentInChildren.Links[3]);
      if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) w.Chance_Backpacks)
        this.SpawnAccesoryToLink(w.Backpacks, componentInChildren.Links[1]);
      if (t.UsesLinkSpawns)
      {
        for (int index = 0; index < componentInChildren.Links.Count; ++index)
        {
          if ((double) UnityEngine.Random.Range(0.0f, 1f) < (double) t.LinkSpawnChance[index])
            componentInChildren.Links[index].RegisterSpawnOnDestroy(t.LinkSpawns[index]);
        }
      }
      componentInChildren.Configure(t);
      return componentInChildren;
    }

    private SosigWeapon SpawnWeapon(List<FVRObject> o) => UnityEngine.Object.Instantiate<GameObject>(o[UnityEngine.Random.Range(0, o.Count)].GetGameObject(), new Vector3(0.0f, 30f, 0.0f), Quaternion.identity).GetComponent<SosigWeapon>();

    private void SpawnAccesoryToLink(List<FVRObject> gs, SosigLink l)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gs[UnityEngine.Random.Range(0, gs.Count)].GetGameObject(), l.transform.position, l.transform.rotation);
      gameObject.transform.SetParent(l.transform);
      gameObject.GetComponent<SosigWearable>().RegisterWearable(l);
    }

    [Serializable]
    public class SpawnGroup
    {
      public int MinSpawnedInGroup = 1;
      public int MaxSpawnedInGroup = 1;
      public int IFF = 1;
      [SearchableEnum]
      public List<SosigEnemyID> IDSelection;

      public SosigEnemyTemplate GetTemplate() => ManagerSingleton<IM>.Instance.odicSosigObjsByID[this.IDSelection[UnityEngine.Random.Range(0, this.IDSelection.Count)]];
    }
  }
}
