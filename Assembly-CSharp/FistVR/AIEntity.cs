// Decompiled with JetBrains decompiler
// Type: FistVR.AIEntity
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class AIEntity : MonoBehaviour
  {
    public int IFFCode = -1;
    public bool ReceivesEvent_Damage;
    public float ManagerCheckFrequency = 0.2f;
    private float m_checkTick;
    [Header("Core Connections")]
    public Transform FacingTransform;
    private bool m_hasFacingTransform;
    public Transform GroundTransform;
    private bool m_hasGroundTransform;
    public List<AIEntityIFFBeacon> Beacons;
    public bool UsesFakedPosition;
    public Vector3 FakePos;
    [Header("Visual System")]
    public bool ReceivesEvent_Visual;
    public float MaximumSightRange = 100f;
    public float MaximumSightFOV = 45f;
    public AnimationCurve SightDistanceByFOVMultiplier;
    public bool IsVisualCheckOmni;
    public Transform SensoryFrame;
    public LayerMask LM_VisualOcclusionCheck;
    [Header("Sonic System")]
    public bool ReceivesEvent_Sonic;
    public float SonicThreshold;
    public float MaxHearingDistance = 300f;
    [Header("Reported Values")]
    public float DangerMultiplier = 1f;
    public float VisibilityMultiplier = 1f;
    public float MaxDistanceVisibleFrom = 300f;

    public void Start()
    {
      if ((Object) this.SensoryFrame == (Object) null)
        this.SensoryFrame = this.transform;
      if ((Object) this.FacingTransform != (Object) null)
        this.m_hasFacingTransform = true;
      if ((Object) this.GroundTransform != (Object) null)
        this.m_hasGroundTransform = true;
      this.m_checkTick = Random.Range(0.0f, this.ManagerCheckFrequency);
      if (!((Object) GM.CurrentAIManager != (Object) null))
        return;
      GM.CurrentAIManager.RegisterAIEntity(this);
    }

    public void OnDestroy()
    {
      if (!((Object) GM.CurrentAIManager != (Object) null))
        return;
      GM.CurrentAIManager.DeRegisterAIEntity(this);
    }

    public bool ReadyForManagerCheck() => (double) this.m_checkTick <= 0.0;

    public Vector3 GetPos() => this.UsesFakedPosition ? this.FakePos : this.transform.position;

    public Vector3 GetGroundPos() => this.m_hasGroundTransform ? this.GroundTransform.position : this.transform.position;

    public Vector3 GetThreatFacing() => this.m_hasFacingTransform ? this.FacingTransform.forward : this.transform.forward;

    public void Tick(float t)
    {
      if ((double) this.m_checkTick >= 0.0)
        this.m_checkTick -= t;
      if (this.Beacons.Count > 0)
      {
        for (int index = 0; index < this.Beacons.Count; ++index)
        {
          if ((Object) this.Beacons[index] != (Object) null && this.Beacons[index].IFF != this.IFFCode)
            this.Beacons[index].IFF = this.IFFCode;
        }
      }
      if (!this.ReceivesEvent_Sonic)
        return;
      this.SonicThreshold = Mathf.MoveTowards(this.SonicThreshold, 0.0f, t * GM.CurrentAIManager.SonicThresholdDecayCurve.Evaluate(this.SonicThreshold));
    }

    public void ResetTick() => this.m_checkTick = this.ManagerCheckFrequency;

    public void ProcessLoudness(float loudness)
    {
      if ((double) loudness >= (double) this.SonicThreshold)
        this.SonicThreshold = Random.Range(loudness * 1.1f, loudness * 1.25f);
      this.SonicThreshold = Mathf.Clamp(this.SonicThreshold, 0.0f, 200f);
    }

    public event AIEntity.AIEventReceive AIEventReceiveEvent;

    public void OnAIEventReceive(AIEvent e)
    {
      if (this.AIEventReceiveEvent == null)
        return;
      this.AIEventReceiveEvent(e);
    }

    public event AIEntity.AIReceiveSuppression AIReceiveSuppressionEvent;

    public void OnAIReceiveSuppression(
      Vector3 pos,
      Vector3 dir,
      int iffcode,
      float intensity,
      float range)
    {
      if (this.AIReceiveSuppressionEvent == null)
        return;
      this.AIReceiveSuppressionEvent(pos, dir, iffcode, intensity, range);
    }

    public delegate void AIEventReceive(AIEvent e);

    public delegate void AIReceiveSuppression(
      Vector3 pos,
      Vector3 dir,
      int iffcode,
      float intensity,
      float range);
  }
}
