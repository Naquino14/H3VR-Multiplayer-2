// Decompiled with JetBrains decompiler
// Type: FistVR.Kabot
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Kabot : MonoBehaviour, IFVRDamageable
  {
    [Header("Refs")]
    public Rigidbody RB;
    public Transform Center;
    public List<Kabot.KSpike> Spikes;
    public Kabot.KabotState State;
    [Header("GeneralParams")]
    public float Life = 30000f;
    [Header("Animation")]
    public AnimationCurve PulsingCurve;
    public Vector2 PulseScaleRange;
    private float m_pulseXTick;
    private float m_pulseYTick = 0.3333f;
    private float m_pulseZTick = 0.6666f;
    [Header("MovementLogic")]
    public Vector2 LingerRange;
    public float MoveLerpMult = 1f;
    [Header("SpikeLogic")]
    public float ScaleXY = 0.2f;
    public float MaxSpikeRange = 10f;
    public float SpikeSpeedFire = 3f;
    public float SpikeSpeedRetract = 1f;
    public LayerMask LM_Spike;
    public LayerMask LM_DamageCast;
    public AudioEvent AudEvent_SpikeHit;
    public PMaterialDefinition MatDefImpact;
    public DamageDealt DamageOnHit;
    public GameObject SpawnOnDie;
    public AudioSource Pulse;
    private bool m_isPulseActive;
    private float m_distCheckTick = 0.25f;
    private float m_timeToLinger;
    private float m_moveLerp;
    private Vector3 m_fromPos;
    private Vector3 m_toPos;
    private bool m_isDead;

    private void Start()
    {
      this.InitiateLingering();
      float num = UnityEngine.Random.Range(0.0f, 1f);
      this.m_pulseXTick += num;
      this.m_pulseYTick += num;
      this.m_pulseZTick += num;
      this.transform.rotation = Quaternion.LookRotation(UnityEngine.Random.onUnitSphere, Vector3.up);
      for (int index = 0; index < this.Spikes.Count; ++index)
        this.Spikes[index].Index = index;
      this.m_distCheckTick = UnityEngine.Random.Range(0.1f, 0.25f);
    }

    private void Update()
    {
      if (this.m_isDead)
      {
        bool flag = true;
        for (int index = 0; index < this.Spikes.Count; ++index)
        {
          if (this.Spikes[index].State != Kabot.KSpike.SpikeState.Retracted)
          {
            flag = false;
            break;
          }
        }
        if (flag)
          this.Explode();
      }
      else
      {
        this.m_distCheckTick -= Time.deltaTime;
        if ((double) this.m_distCheckTick <= 0.0)
        {
          this.m_distCheckTick = UnityEngine.Random.Range(0.1f, 0.25f);
          this.SoundCheck();
        }
      }
      this.KUpdate();
      this.m_pulseXTick += Time.deltaTime;
      this.m_pulseYTick += Time.deltaTime;
      this.m_pulseZTick += Time.deltaTime;
      this.m_pulseXTick = Mathf.Repeat(this.m_pulseXTick, 1f);
      this.m_pulseYTick = Mathf.Repeat(this.m_pulseYTick, 1f);
      this.m_pulseZTick = Mathf.Repeat(this.m_pulseZTick, 1f);
      this.Center.localScale = new Vector3(Mathf.Lerp(this.PulseScaleRange.x, this.PulseScaleRange.y, this.PulsingCurve.Evaluate(this.m_pulseXTick)), Mathf.Lerp(this.PulseScaleRange.x, this.PulseScaleRange.y, this.PulsingCurve.Evaluate(this.m_pulseYTick)), Mathf.Lerp(this.PulseScaleRange.x, this.PulseScaleRange.y, this.PulsingCurve.Evaluate(this.m_pulseZTick)));
    }

    private void SoundCheck()
    {
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
      if ((double) num < 6.0 && !this.m_isPulseActive)
      {
        this.m_isPulseActive = true;
        this.Pulse.Play();
      }
      else
      {
        if ((double) num <= 8.0 || !this.m_isPulseActive)
          return;
        this.m_isPulseActive = false;
        this.Pulse.Stop();
      }
    }

    private void KUpdate()
    {
      switch (this.State)
      {
        case Kabot.KabotState.Lingering:
          this.KUpdate_Lingering();
          break;
        case Kabot.KabotState.Locomoting:
          this.KUpdate_Locomoting();
          break;
      }
      float deltaTime = Time.deltaTime;
      for (int index = 0; index < this.Spikes.Count; ++index)
        this.Spikes[index].Tick(deltaTime);
    }

    private void InitiateLingering()
    {
      if (!this.Spikes[0].C.enabled)
        this.Spikes[0].C.enabled = true;
      this.State = Kabot.KabotState.Lingering;
      this.m_timeToLinger = UnityEngine.Random.Range(this.LingerRange.x, this.LingerRange.y);
      for (int index = 1; index <= 7; ++index)
        this.Spikes[index].FireOut(UnityEngine.Random.onUnitSphere);
      float num = Vector3.Distance(GM.CurrentPlayerRoot.position, this.transform.position);
      float delay = num / 343f;
      if ((double) num >= 40.0)
        return;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_SpikeHit, this.transform.position, delay);
    }

    private void InitiateLocomoting()
    {
      this.State = Kabot.KabotState.Locomoting;
      this.m_moveLerp = 0.0f;
      this.m_fromPos = this.transform.position;
      this.m_toPos = Vector3.Lerp(this.transform.position, this.Spikes[0].GetAnchorPoint(), UnityEngine.Random.Range(0.3f, 0.8f));
      for (int index = 1; index < this.Spikes.Count; ++index)
        this.Spikes[index].Retract();
    }

    private void KUpdate_Lingering()
    {
      this.m_timeToLinger -= Time.deltaTime;
      if ((double) this.m_timeToLinger > 0.0)
        return;
      if (this.Spikes[0].State == Kabot.KSpike.SpikeState.Anchored)
      {
        this.InitiateLocomoting();
      }
      else
      {
        if (this.Spikes[0].State != Kabot.KSpike.SpikeState.Retracted)
          return;
        this.Spikes[0].FireOut(UnityEngine.Random.onUnitSphere);
      }
    }

    private void KUpdate_Locomoting()
    {
      bool flag = true;
      for (int index = 1; index < this.Spikes.Count; ++index)
      {
        if (this.Spikes[index].State != Kabot.KSpike.SpikeState.Retracted)
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        return;
      if (this.Spikes[0].C.enabled)
        this.Spikes[0].C.enabled = false;
      if ((double) this.m_moveLerp < 1.0)
      {
        this.m_moveLerp += Time.deltaTime;
        Vector3 position1 = this.transform.position;
        Vector3 position2 = Vector3.Lerp(this.m_fromPos, this.m_toPos, this.m_moveLerp);
        Vector3 vector3 = position2 - position1;
        if (Physics.SphereCast(new Ray(position1, vector3.normalized), 0.36f, vector3.magnitude, (int) this.LM_Spike, QueryTriggerInteraction.Ignore))
        {
          this.m_moveLerp = 1f;
        }
        else
        {
          this.RB.MovePosition(position2);
          float num = Vector3.Distance(this.transform.position, this.Spikes[0].GetAnchorPoint());
          this.Spikes[0].T.localScale = new Vector3(this.ScaleXY, this.ScaleXY, num);
          this.Spikes[0].SetAnchorDistance(num);
        }
      }
      else if (this.Spikes[0].State == Kabot.KSpike.SpikeState.Anchored)
      {
        this.Spikes[0].Retract();
        this.InitiateLingering();
      }
      else if (this.Spikes[0].State != Kabot.KSpike.SpikeState.Retracted)
        ;
    }

    public void PlayHitSound(int i, Vector3 p)
    {
      if (i != 0 && i != 8 && i != 9)
        return;
      float num = Vector3.Distance(GM.CurrentPlayerRoot.position, this.transform.position);
      float delay = num / 343f;
      if ((double) num >= 40.0)
        return;
      SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, this.AudEvent_SpikeHit, p, delay);
    }

    public void Damage(FistVR.Damage d)
    {
      this.Life -= d.Dam_TotalKinetic;
      if ((double) this.Life <= 0.0)
        this.Die();
      else
        this.ResponseDir(-d.strikeDir);
    }

    private void Die()
    {
      if (this.m_isDead)
        return;
      this.m_isDead = true;
      for (int index = 0; index < this.Spikes.Count; ++index)
      {
        this.Spikes[index].CanFireOut = false;
        this.Spikes[index].Retract();
      }
      this.SpikeSpeedRetract = 10f;
    }

    private void Explode()
    {
      UnityEngine.Object.Instantiate<GameObject>(this.SpawnOnDie, this.transform.position, UnityEngine.Random.rotation);
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    private void ResponseDir(Vector3 v)
    {
      if (this.Spikes[8].State == Kabot.KSpike.SpikeState.Retracted)
        this.Spikes[8].FireOut(Vector3.Lerp(v, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(0.0f, 0.2f)));
      else if (this.Spikes[8].State == Kabot.KSpike.SpikeState.Anchored)
        this.Spikes[8].Retract();
      if (this.Spikes[9].State == Kabot.KSpike.SpikeState.Retracted)
      {
        this.Spikes[9].FireOut(Vector3.Lerp(v, UnityEngine.Random.onUnitSphere, UnityEngine.Random.Range(0.0f, 0.2f)));
      }
      else
      {
        if (this.Spikes[9].State != Kabot.KSpike.SpikeState.Anchored)
          return;
        this.Spikes[9].Retract();
      }
    }

    public enum KabotState
    {
      Lingering,
      Locomoting,
    }

    [Serializable]
    public class KSpike
    {
      public Kabot K;
      public Transform T;
      public Collider C;
      public Kabot.KSpike.SpikeState State;
      private float m_lerp;
      private bool m_isGoingToAnchor;
      private RaycastHit m_hit;
      private float m_anchorDistance = 1f;
      private Vector3 m_anchorPoint;
      private float m_fireSpeed = 1f;
      public int Index;
      public bool CanFireOut = true;

      public Vector3 GetAnchorPoint() => this.m_anchorPoint;

      public void SetAnchorDistance(float f) => this.m_anchorDistance = f;

      public void FireOut(Vector3 forward)
      {
        if (!this.CanFireOut)
          return;
        this.C.enabled = false;
        this.T.rotation = Quaternion.LookRotation(forward, Vector3.up);
        this.State = Kabot.KSpike.SpikeState.Anchoring;
        this.m_lerp = 0.0f;
        this.m_fireSpeed = UnityEngine.Random.Range(this.K.SpikeSpeedFire * 0.9f, this.K.SpikeSpeedFire * 1f);
        if (Physics.Raycast(this.T.position + this.T.forward * 0.32f, this.T.forward, out this.m_hit, this.K.MaxSpikeRange - 0.32f, (int) this.K.LM_Spike, QueryTriggerInteraction.Ignore))
        {
          this.m_anchorPoint = this.m_hit.point + this.T.forward * 0.15f;
          this.m_anchorDistance = this.m_hit.distance + 0.15f;
          if ((double) this.m_hit.distance > 0.699999988079071)
            this.m_isGoingToAnchor = true;
          else
            this.m_isGoingToAnchor = false;
        }
        else
        {
          this.m_anchorPoint = this.T.position + this.T.forward * this.K.MaxSpikeRange;
          this.m_anchorDistance = this.K.MaxSpikeRange;
          this.m_isGoingToAnchor = false;
        }
      }

      public void Retract()
      {
        this.C.enabled = false;
        this.State = Kabot.KSpike.SpikeState.Retracting;
      }

      public void Tick(float t)
      {
        switch (this.State)
        {
          case Kabot.KSpike.SpikeState.Retracting:
            this.m_lerp -= t * this.K.SpikeSpeedRetract;
            this.T.localScale = new Vector3(this.K.ScaleXY, this.K.ScaleXY, Mathf.Lerp(this.K.ScaleXY, this.m_anchorDistance, this.m_lerp));
            if ((double) this.m_lerp > 0.0)
              break;
            this.State = Kabot.KSpike.SpikeState.Retracted;
            break;
          case Kabot.KSpike.SpikeState.Anchoring:
            this.m_lerp += t * this.m_fireSpeed;
            this.T.localScale = new Vector3(this.K.ScaleXY, this.K.ScaleXY, Mathf.Lerp(this.K.ScaleXY, this.m_anchorDistance, this.m_lerp));
            if ((double) this.m_lerp < 1.0)
              break;
            if (Physics.Raycast(this.T.position + this.T.forward * 0.32f, this.T.forward, out this.m_hit, this.m_anchorDistance - 0.32f, (int) this.K.LM_DamageCast, QueryTriggerInteraction.Collide))
            {
              IFVRDamageable component = this.m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
              if (component == null && (UnityEngine.Object) this.m_hit.collider.attachedRigidbody != (UnityEngine.Object) null)
                component = this.m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
              if (component != null)
                component.Damage(new FistVR.Damage()
                {
                  Class = FistVR.Damage.DamageClass.Melee,
                  Source_IFF = 0,
                  strikeDir = this.T.forward,
                  Dam_Piercing = 1000f,
                  Dam_TotalKinetic = 1000f,
                  hitNormal = -this.T.forward,
                  point = this.m_hit.point
                });
            }
            if (this.m_isGoingToAnchor)
            {
              FXM.SpawnImpactEffect(this.m_anchorPoint, -this.T.forward, (int) this.K.MatDefImpact.impactCategory, ImpactEffectMagnitude.Large, false);
              this.State = Kabot.KSpike.SpikeState.Anchored;
              this.K.PlayHitSound(this.Index, this.m_anchorPoint);
              this.C.enabled = true;
              break;
            }
            this.State = Kabot.KSpike.SpikeState.Retracting;
            break;
        }
      }

      public enum SpikeState
      {
        Retracted,
        Retracting,
        Anchoring,
        Anchored,
      }
    }
  }
}
