// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner_Cleric
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniSpawner_Cleric : OmniSpawner
  {
    private OmniSpawnDef_Cleric m_def;
    public GameObject TargetPrefab;
    private bool m_canSpawn;
    private int m_setIndex;
    private float m_timeBetweenSets = 1f;
    private float m_timeTilNextSet = 1f;
    private bool m_waitingForNextSet;
    private OmniSpawnDef_Cleric.ClericSet m_curSet;
    private int m_clericIndex;
    private float m_timeTilNextCleric = 1f;
    private bool m_shouldSpawnNextOnHitCleric = true;
    private List<int> m_randIndexes;
    private List<OmniCleric> m_activeClerics = new List<OmniCleric>();
    public GameObject ClericRingPrefab;
    private OmniClericRing m_ring;

    public override void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
    {
      base.Configure(Def, Range);
      this.m_def = Def as OmniSpawnDef_Cleric;
      this.m_timeBetweenSets = this.m_def.TimeBetweenSets;
      this.m_timeTilNextSet = 0.0f;
      this.m_curSet = this.m_def.Sets[0];
      this.m_clericIndex = 0;
      this.m_timeTilNextCleric = 0.0f;
      this.m_shouldSpawnNextOnHitCleric = true;
    }

    public override void BeginSpawning()
    {
      base.BeginSpawning();
      this.m_canSpawn = true;
    }

    public override void EndSpawning()
    {
      base.EndSpawning();
      this.m_canSpawn = false;
    }

    public override void Activate()
    {
      base.Activate();
      if (!((Object) this.m_ring == (Object) null))
        return;
      this.m_ring = Object.Instantiate<GameObject>(this.ClericRingPrefab, Vector3.zero, Quaternion.identity).GetComponent<OmniClericRing>();
      for (int index = 0; index < this.m_ring.Indicators.Count; ++index)
        this.m_ring.Indicators[index].enabled = false;
    }

    public override int Deactivate()
    {
      if (this.m_activeClerics.Count > 0)
      {
        for (int index = this.m_activeClerics.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_activeClerics[index] != (Object) null)
            Object.Destroy((Object) this.m_activeClerics[index].gameObject);
        }
        this.m_activeClerics.Clear();
      }
      if ((Object) this.m_ring != (Object) null)
      {
        Object.Destroy((Object) this.m_ring.gameObject);
        this.m_ring = (OmniClericRing) null;
      }
      return base.Deactivate();
    }

    private void Update() => this.UpdateMe();

    private void UpdateMe()
    {
      if (!this.m_isConfigured)
        return;
      switch (this.m_state)
      {
        case OmniSpawner.SpawnerState.Deactivating:
          this.Deactivating();
          break;
        case OmniSpawner.SpawnerState.Activated:
          this.SpawningLoop();
          break;
        case OmniSpawner.SpawnerState.Activating:
          this.Activating();
          break;
      }
    }

    private void SpawningLoop()
    {
      if (this.m_canSpawn && !this.m_waitingForNextSet)
      {
        switch (this.m_curSet.SpawnStyle)
        {
          case OmniSpawnDef_Cleric.TargetSpawnStyle.AllAtOnce:
            while (this.m_clericIndex < this.m_curSet.TargetSet.Count)
              this.SpawnCleric(this.m_clericIndex, this.m_curSet.TargetSet[this.m_clericIndex]);
            break;
          case OmniSpawnDef_Cleric.TargetSpawnStyle.Sequential:
            if (this.m_clericIndex < this.m_curSet.TargetSet.Count)
            {
              if ((double) this.m_timeTilNextCleric <= 0.0)
              {
                this.SpawnCleric(this.m_clericIndex, this.m_curSet.TargetSet[this.m_clericIndex]);
                this.m_timeTilNextCleric = this.m_curSet.SequentialTiming;
                break;
              }
              this.m_timeTilNextCleric -= Time.deltaTime;
              break;
            }
            break;
          case OmniSpawnDef_Cleric.TargetSpawnStyle.OnHit:
            if (this.m_clericIndex < this.m_curSet.TargetSet.Count && this.m_shouldSpawnNextOnHitCleric)
            {
              this.m_shouldSpawnNextOnHitCleric = false;
              this.SpawnCleric(this.m_clericIndex, this.m_curSet.TargetSet[this.m_clericIndex]);
              break;
            }
            break;
        }
      }
      if (this.m_waitingForNextSet)
      {
        if ((double) this.m_timeTilNextSet > 0.0)
        {
          this.m_timeTilNextSet -= Time.deltaTime;
        }
        else
        {
          this.m_timeTilNextSet = 0.0f;
          this.m_waitingForNextSet = false;
        }
      }
      if (this.m_clericIndex < this.m_curSet.TargetSet.Count || this.m_activeClerics.Count > 0)
        return;
      if (this.m_setIndex < this.m_def.Sets.Count - 1)
      {
        this.m_clericIndex = 0;
        ++this.m_setIndex;
        this.m_curSet = this.m_def.Sets[this.m_setIndex];
        this.m_waitingForNextSet = true;
        this.m_timeTilNextSet = this.m_timeBetweenSets;
        this.m_timeTilNextCleric = this.m_curSet.SequentialTiming;
      }
      else
      {
        this.m_isDoneSpawning = true;
        if (this.m_activeClerics.Count > 0)
          return;
        this.m_isReadyForWaveEnd = true;
      }
    }

    public void SpawnCleric(int index, OmniSpawnDef_Cleric.TargetLocation loc)
    {
      OmniCleric component = Object.Instantiate<GameObject>(this.TargetPrefab, this.m_ring.SpawnPoints[(int) loc].position, this.m_ring.SpawnPoints[(int) loc].rotation).GetComponent<OmniCleric>();
      component.Init(this, this.m_ring.SpawnPoints[(int) loc], this.m_curSet.FiresBack, this.m_curSet.FiringTime, loc);
      this.m_activeClerics.Add(component);
      ++this.m_clericIndex;
      this.PlaySpawnSound();
      this.m_ring.Indicators[(int) loc].enabled = true;
    }

    public void ClearCleric(OmniCleric cleric, bool isHeadShot)
    {
      this.m_ring.Indicators[(int) cleric.m_pos].enabled = false;
      if ((double) new Vector2(GM.CurrentPlayerBody.Head.position.x, GM.CurrentPlayerBody.Head.position.z).magnitude > 1.29999995231628)
      {
        this.PlayFailureSound();
        this.AddPoints(-200);
      }
      else
      {
        this.PlaySuccessSound();
        if (isHeadShot)
          this.AddPoints(200);
        else
          this.AddPoints(100);
      }
      this.m_activeClerics.Remove(cleric);
      this.m_shouldSpawnNextOnHitCleric = true;
      Object.Destroy((Object) cleric.gameObject);
    }
  }
}
