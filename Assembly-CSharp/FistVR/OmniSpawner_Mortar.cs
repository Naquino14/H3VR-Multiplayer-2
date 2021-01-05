// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner_Mortar
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniSpawner_Mortar : OmniSpawner
  {
    private OmniSpawnDef_Mortar m_def;
    public GameObject MortarPrefab;
    private bool m_canSpawn;
    private Vector2 m_spawnCadence = new Vector2(0.25f, 0.25f);
    private float m_spawnTick = 1f;
    private Vector2 m_velocityRange = new Vector2(10f, 10f);
    private int m_targetsLeftToFire;
    private List<GameObject> m_spawnedMortars = new List<GameObject>();
    private OmniSpawnDef_Mortar.MortarAngle m_angle;
    private Vector3 m_curPos = Vector3.zero;
    private Vector3 m_tarPos = Vector3.zero;
    private Quaternion m_curRot = Quaternion.identity;
    private Quaternion m_tarRot = Quaternion.identity;
    private float m_elevation;
    private float m_dist;

    public override void Configure(OmniSpawnDef def, OmniWaveEngagementRange range)
    {
      base.Configure(def, range);
      this.m_def = def as OmniSpawnDef_Mortar;
      this.m_spawnCadence = this.m_def.SpawnCadence;
      this.m_velocityRange = this.m_def.VelocityRange;
      this.m_targetsLeftToFire = this.m_def.NumShots;
      this.m_angle = this.m_def.Angle;
      this.m_elevation = this.transform.position.y;
      this.m_dist = this.transform.position.z;
    }

    public override void BeginSpawning()
    {
      base.BeginSpawning();
      this.m_canSpawn = true;
      this.m_curPos = this.transform.position;
      this.m_tarPos = this.transform.position;
      this.m_curRot = this.transform.rotation;
      this.m_tarRot = this.transform.rotation;
      this.GenerateNewPose();
    }

    public override void EndSpawning()
    {
      base.EndSpawning();
      this.m_canSpawn = false;
    }

    public override void Activate() => base.Activate();

    public override int Deactivate()
    {
      if (this.m_spawnedMortars.Count > 0)
      {
        for (int index = this.m_spawnedMortars.Count - 1; index >= 0; --index)
        {
          if ((Object) this.m_spawnedMortars[index] != (Object) null)
            Object.Destroy((Object) this.m_spawnedMortars[index]);
        }
        this.m_spawnedMortars.Clear();
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
      if (!this.m_canSpawn || this.m_state != OmniSpawner.SpawnerState.Activated)
        return;
      this.m_curPos = Vector3.Lerp(this.m_curPos, this.m_tarPos, Time.deltaTime * 2f);
      this.m_curRot = Quaternion.Slerp(this.m_curRot, this.m_tarRot, Time.deltaTime * 10f);
      this.transform.position = this.m_curPos;
      this.transform.rotation = this.m_curRot;
      if ((double) this.m_spawnTick > 0.0)
        this.m_spawnTick -= Time.deltaTime;
      else if (this.m_targetsLeftToFire > 0)
      {
        this.SpawnMortar();
      }
      else
      {
        this.m_isDoneSpawning = true;
        if (this.m_spawnedMortars.Count > 0)
          return;
        this.m_isReadyForWaveEnd = true;
      }
    }

    private void GenerateNewPose()
    {
      Vector3 position1 = this.transform.position;
      Vector3 position2 = this.transform.position;
      position1.x = Random.Range(15f, -15f);
      position2.x = -position2.x;
      position2.x += Random.Range(5f, -5f);
      position2.y = this.transform.position.y + 45f;
      switch (this.m_angle)
      {
        case OmniSpawnDef_Mortar.MortarAngle.DownRange:
          position2.z = 35f;
          position2.z += Random.Range(0.0f, 10f);
          break;
        case OmniSpawnDef_Mortar.MortarAngle.Centered:
          position2.z = 1f;
          break;
        case OmniSpawnDef_Mortar.MortarAngle.UpRange:
          position2.z = -35f;
          position2.z -= Random.Range(0.0f, 10f);
          break;
      }
      this.m_tarPos = position1;
      this.m_tarRot = Quaternion.LookRotation(position2, Vector3.up);
    }

    private void SpawnMortar()
    {
      this.m_spawnTick = Random.Range(this.m_spawnCadence.x, this.m_spawnCadence.y);
      GameObject gameObject = Object.Instantiate<GameObject>(this.MortarPrefab, this.transform.position, Quaternion.identity);
      gameObject.transform.localScale = new Vector3(this.m_def.MortarSize, this.m_def.MortarSize, this.m_def.MortarSize);
      OmniMortar component = gameObject.GetComponent<OmniMortar>();
      this.m_spawnedMortars.Add(gameObject);
      component.Configure(this, this.transform.position, this.transform.forward, Random.Range(this.m_velocityRange.x, this.m_velocityRange.y));
      this.GenerateNewPose();
      this.PlaySpawnSound();
      --this.m_targetsLeftToFire;
    }

    public void MortarExpired(OmniMortar mortar)
    {
      this.m_spawnedMortars.Remove(mortar.gameObject);
      Object.Destroy((Object) mortar.gameObject);
    }

    public void MortarHit(OmniMortar mortar)
    {
      this.m_spawnedMortars.Remove(mortar.gameObject);
      this.Invoke("PlaySuccessSound", 0.15f);
      this.AddPoints(100);
      Object.Destroy((Object) mortar.gameObject);
    }
  }
}
