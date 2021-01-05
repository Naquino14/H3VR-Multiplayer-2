// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner_IPSC
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniSpawner_IPSC : OmniSpawner
  {
    private OmniSpawnDef_IPSC m_def;
    public GameObject[] PlinthPrefabs;
    public GameObject[] IPSCPrefabs;
    private OmniIPSCPlinth m_plinth;
    private bool m_canSpawn;
    private Vector2 m_spawnCadence = new Vector2(0.25f, 0.25f);
    private float m_spawnTick = 0.25f;
    private int m_targetIndex;
    private List<OmniSpawnDef_IPSC.IPSCType> m_targetList;
    private List<OmniIPSC> m_activeIPSC = new List<OmniIPSC>();
    private List<Transform> m_availableSpawnPoints = new List<Transform>();
    private List<Transform> m_usedSpawnPoints = new List<Transform>();

    public override void Configure(OmniSpawnDef def, OmniWaveEngagementRange range)
    {
      base.Configure(def, range);
      this.m_def = def as OmniSpawnDef_IPSC;
      this.m_targetList = this.m_def.TargetList;
      this.m_spawnCadence = this.m_def.SpawnCadence;
      this.m_spawnTick = 0.01f;
      GameObject gameObject = Object.Instantiate<GameObject>(this.PlinthPrefabs[(int) range], this.transform.position, this.transform.rotation);
      gameObject.transform.SetParent(this.transform);
      this.m_plinth = gameObject.GetComponent<OmniIPSCPlinth>();
      for (int index = 0; index < this.m_plinth.SpawnPoints.Length; ++index)
        this.m_availableSpawnPoints.Add(this.m_plinth.SpawnPoints[index]);
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

    public override void Activate() => base.Activate();

    public override int Deactivate()
    {
      if (this.m_activeIPSC.Count > 0)
      {
        for (int index = this.m_activeIPSC.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_activeIPSC[index] != (Object) null)
            Object.Destroy((Object) this.m_activeIPSC[index].gameObject);
        }
        this.m_activeIPSC.Clear();
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
      if (!this.m_canSpawn)
        return;
      if ((double) this.m_spawnTick > 0.0)
        this.m_spawnTick -= Time.deltaTime;
      else if (this.m_targetIndex < this.m_targetList.Count)
      {
        if (this.m_availableSpawnPoints.Count <= 0)
          return;
        this.SpawnTarget();
      }
      else
      {
        this.m_isDoneSpawning = true;
        if (this.m_activeIPSC.Count > 0)
          return;
        this.m_isReadyForWaveEnd = true;
      }
    }

    private void SpawnTarget()
    {
      this.m_spawnTick = Random.Range(this.m_spawnCadence.x, this.m_spawnCadence.y);
      Transform availableSpawnPoint = this.m_availableSpawnPoints[Random.Range(0, this.m_availableSpawnPoints.Count)];
      Vector3 position = availableSpawnPoint.position;
      OmniIPSC component = Object.Instantiate<GameObject>(this.IPSCPrefabs[(int) this.m_targetList[this.m_targetIndex]], new Vector3(position.x, -3f, position.z), availableSpawnPoint.rotation).GetComponent<OmniIPSC>();
      this.m_activeIPSC.Add(component);
      this.m_usedSpawnPoints.Add(availableSpawnPoint);
      this.m_availableSpawnPoints.Remove(availableSpawnPoint);
      component.Configure(this, availableSpawnPoint, this.m_def.TimeActivated);
      ++this.m_targetIndex;
    }

    public void TargetDeactivating(OmniIPSC targ)
    {
      this.m_usedSpawnPoints.Remove(targ.SpawnPoint);
      this.m_availableSpawnPoints.Add(targ.SpawnPoint);
      this.m_activeIPSC.Remove(targ);
      Object.Destroy((Object) targ.gameObject);
    }
  }
}
