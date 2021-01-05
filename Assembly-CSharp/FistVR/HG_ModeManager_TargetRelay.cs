// Decompiled with JetBrains decompiler
// Type: FistVR.HG_ModeManager_TargetRelay
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HG_ModeManager_TargetRelay : HG_ModeManager
  {
    public List<GameObject> Targets;
    public GameObject Indicator;
    private GameObject m_spawnedIndicator;
    private Vector3 m_spawnedIndicatorBasePos;
    public List<int> ZoneSequence_Sprint;
    public List<int> ZoneSequence_Jog;
    public List<int> ZoneSequence_Marathon;
    public List<Transform> CivvieSpawnPoints;
    private List<int> m_curZoneSequence;
    private int m_curZoneInSequence;
    private float m_timer;
    private int m_numShotsFired;
    private int m_numTargetsDestroyedByGunFire;
    private List<HG_Target> m_activeTargets = new List<HG_Target>();
    [Header("Audio")]
    public AudioEvent AudEvent_ZoneCompleted;
    public AudioEvent AudEvent_SequenceCompleted;

    public override void InitMode(HG_ModeManager.HG_Mode mode)
    {
      this.m_mode = mode;
      GM.CurrentSceneSettings.ShotFiredEvent += new FVRSceneSettings.ShotFired(this.GunShotFired);
      this.m_activeTargets.Clear();
      switch (mode)
      {
        case HG_ModeManager.HG_Mode.TargetRelay_Sprint:
          this.m_curZoneSequence = this.ZoneSequence_Sprint;
          break;
        case HG_ModeManager.HG_Mode.TargetRelay_Jog:
          this.m_curZoneSequence = this.ZoneSequence_Jog;
          break;
        case HG_ModeManager.HG_Mode.TargetRelay_Marathon:
          this.m_curZoneSequence = this.ZoneSequence_Marathon;
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
      this.ConfigureCurrentZone();
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, this.transform.position);
      this.m_timer = 0.0f;
      this.m_numShotsFired = 0;
      this.m_numTargetsDestroyedByGunFire = 0;
      this.IsPlaying = true;
    }

    public override int GetScore()
    {
      float num = 0.0f;
      if (this.m_numShotsFired > 0)
        num = (float) this.m_numTargetsDestroyedByGunFire / (float) this.m_numShotsFired;
      return (this.m_curZoneSequence.Count - 1) * 500 + Mathf.Max((this.m_curZoneSequence.Count * 30 - (int) this.m_timer) * 10, 0) + (int) ((double) (this.m_curZoneSequence.Count - 1) * (double) num * 500.0);
    }

    public override List<string> GetScoringReadOuts()
    {
      List<string> stringList = new List<string>();
      stringList.Add("Base Score: " + ((this.m_curZoneSequence.Count - 1) * 500).ToString());
      stringList.Add("Time Bonus: " + Mathf.Max((this.m_curZoneSequence.Count * 30 - (int) this.m_timer) * 10, 0).ToString());
      float num = 0.0f;
      if (this.m_numShotsFired > 0)
        num = (float) this.m_numTargetsDestroyedByGunFire / (float) this.m_numShotsFired;
      stringList.Add("Accuracy Bonus: " + ((int) ((double) (this.m_curZoneSequence.Count - 1) * (double) num * 500.0)).ToString());
      stringList.Add("Final Score: " + this.GetScore().ToString());
      return stringList;
    }

    public void Update()
    {
      if (!this.IsPlaying)
        return;
      this.m_timer += Time.deltaTime;
      if (!((Object) this.m_spawnedIndicator != (Object) null))
        return;
      this.m_spawnedIndicator.transform.position = this.m_spawnedIndicatorBasePos + Vector3.up * (Mathf.Sin(Time.time * 4f) * 1.5f);
    }

    private bool AreTargetsActive() => this.m_activeTargets.Count > 0;

    private void ConfigureCurrentZone()
    {
      HG_Zone zone = this.M.Zones[this.m_curZoneSequence[this.m_curZoneInSequence]];
      zone.TargetPoints.Shuffle<Transform>();
      zone.TargetPoints.Shuffle<Transform>();
      for (int index = 0; index < 5; ++index)
      {
        HG_Target component = Object.Instantiate<GameObject>(this.Targets[Random.Range(0, this.Targets.Count)], zone.TargetPoints[index].position + Vector3.up * Random.Range(1.5f, 2.5f), Random.rotation).GetComponent<HG_Target>();
        component.Init((HG_ModeManager) this, zone);
        this.m_activeTargets.Add(component);
      }
      this.m_spawnedIndicator = Object.Instantiate<GameObject>(this.Indicator, zone.transform.position + Vector3.up * 14f, Quaternion.identity);
      this.m_spawnedIndicatorBasePos = zone.transform.position + Vector3.up * 14f;
      SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_ZoneCompleted, zone.transform.position);
    }

    private void DeactivateCurrentZone()
    {
      if (this.m_activeTargets.Count > 0)
      {
        for (int index = this.m_activeTargets.Count - 1; index >= 0; --index)
          Object.Destroy((Object) this.m_activeTargets[index].gameObject);
        this.m_activeTargets.Clear();
      }
      if (!((Object) this.m_spawnedIndicator != (Object) null))
        return;
      Object.Destroy((Object) this.m_spawnedIndicator);
    }

    public override void EndMode(bool doesInvokeTeleport, bool immediateTeleportBackAndScore)
    {
      this.DeactivateCurrentZone();
      GM.CurrentSceneSettings.ShotFiredEvent -= new FVRSceneSettings.ShotFired(this.GunShotFired);
      GM.CurrentSceneSettings.DeathResetPoint.position = this.InitialRespawnPos;
      GM.CurrentSceneSettings.DeathResetPoint.rotation = this.InitialRespawnRot;
      this.IsPlaying = false;
      base.EndMode(true, false);
    }

    public void GunShotFired(FVRFireArm firearm) => ++this.m_numShotsFired;

    public override void TargetDestroyed(HG_Target t)
    {
      if (this.m_activeTargets.Contains(t))
        this.m_activeTargets.Remove(t);
      if (t.GetClassThatKilledMe() == Damage.DamageClass.Projectile)
        ++this.m_numTargetsDestroyedByGunFire;
      if (this.AreTargetsActive())
        return;
      this.DeactivateCurrentZone();
      ++this.m_curZoneInSequence;
      if (this.m_curZoneInSequence < this.m_curZoneSequence.Count)
      {
        this.ConfigureCurrentZone();
      }
      else
      {
        this.M.Case();
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_SequenceCompleted, this.transform.position);
        this.EndMode(true, false);
      }
    }
  }
}
