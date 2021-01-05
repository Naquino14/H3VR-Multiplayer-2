// Decompiled with JetBrains decompiler
// Type: FistVR.StingerMissile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class StingerMissile : MonoBehaviour, IFVRDamageable
  {
    public ParticleSystem PFX_Fire;
    public ParticleSystem PFX_Smoke;
    private AIEntity m_targetEntity;
    private bool m_isFlying;
    private bool m_isMotorEngaged;
    private bool m_isDestroyedAndWaitingToCountDown;
    private float m_distanceTravelled;
    private Vector3 m_velocity;
    private Vector3 m_forwardDir;
    public LayerMask LM_Hitting;
    private RaycastHit m_hit;
    public GameObject TurnOffOnExplode;
    public GameObject DestroyOnExplode;
    public List<GameObject> SpawnOnExplode;
    public AudioSource RocketSound;
    public AudioEvent AudEvent_Launch;
    private bool m_hadPlayedLaunchEvent;
    private float m_motorPower = 200f;
    private float m_maxSpeed = 800f;
    private float m_turnSpeed = 1f;
    public GameObject PlaceMarker;
    private Vector3 m_lastPos;
    private Vector3 m_newPos;
    private Vector3 m_lastEntPos;
    private Vector3 m_curEntPos;
    private bool m_tickingDownToStopSound;
    private float m_tickDownToStopSound = 1f;

    public void Fire(AIEntity ent)
    {
      this.m_targetEntity = ent;
      this.m_isFlying = true;
      this.m_forwardDir = this.transform.forward;
      this.m_velocity = this.transform.forward * 12f;
      this.m_lastPos = this.transform.position;
      this.m_newPos = this.transform.position;
      this.m_curEntPos = ent.GetPos();
      this.m_lastEntPos = ent.GetPos();
    }

    public void Fire(Vector3 targPos, float InitialSpeed)
    {
      this.m_targetEntity = (AIEntity) null;
      this.m_isFlying = true;
      this.m_forwardDir = this.transform.forward;
      this.m_velocity = this.transform.forward * InitialSpeed;
      this.m_lastPos = this.transform.position;
      this.m_newPos = this.transform.position;
      this.m_curEntPos = targPos;
      this.m_lastEntPos = targPos;
    }

    public void Fire(AIEntity ent, float InitialSpeed)
    {
      this.m_targetEntity = ent;
      this.m_isFlying = true;
      this.m_forwardDir = this.transform.forward;
      this.m_velocity = this.transform.forward * InitialSpeed;
      this.m_lastPos = this.transform.position;
      this.m_newPos = this.transform.position;
      this.m_curEntPos = ent.GetPos();
      this.m_lastEntPos = ent.GetPos();
    }

    public void SetMotorPower(float f) => this.m_motorPower = f;

    public void SetMaxSpeed(float f) => this.m_maxSpeed = f;

    public void SetTurnSpeed(float f) => this.m_turnSpeed = f;

    public void Damage(FistVR.Damage d) => this.Explode();

    private void Update()
    {
      if (this.m_isFlying && !this.m_isDestroyedAndWaitingToCountDown)
      {
        float num = 9.81f;
        switch (GM.Options.SimulationOptions.BallisticGravityMode)
        {
          case SimulationOptions.GravityMode.Realistic:
            num = 9.81f;
            break;
          case SimulationOptions.GravityMode.Playful:
            num = 5f;
            break;
          case SimulationOptions.GravityMode.OnTheMoon:
            num = 1.622f;
            break;
          case SimulationOptions.GravityMode.None:
            num = 0.0f;
            break;
        }
        this.m_velocity += Vector3.down * num * Time.deltaTime;
        if (this.m_isMotorEngaged)
        {
          if (!this.RocketSound.isPlaying)
            this.RocketSound.Play();
          if (!this.m_hadPlayedLaunchEvent)
          {
            this.m_hadPlayedLaunchEvent = true;
            SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Launch, this.transform.position);
          }
          this.m_velocity += this.m_forwardDir * Time.deltaTime * this.m_motorPower;
          this.m_velocity = Vector3.ClampMagnitude(this.m_velocity, this.m_maxSpeed);
          if ((Object) this.m_targetEntity != (Object) null)
            this.m_curEntPos = this.m_targetEntity.GetPos();
          Vector3 vector3 = this.m_curEntPos - this.m_lastEntPos;
          this.m_forwardDir = Vector3.RotateTowards(this.m_forwardDir, this.m_curEntPos + vector3.normalized * (vector3.magnitude * (1f / Time.deltaTime) * (Vector3.Distance(this.transform.position, this.m_curEntPos) / (this.m_velocity.magnitude + 1f / 1000f))) + num * Time.deltaTime * Vector3.up * 0.5f - this.transform.position, Time.deltaTime * this.m_turnSpeed, 1f);
          this.transform.rotation = Quaternion.LookRotation(this.m_forwardDir);
          this.m_lastEntPos = this.m_curEntPos;
        }
        bool flag = false;
        if ((double) this.m_distanceTravelled > 1.0 && Physics.SphereCast(this.m_lastPos, 0.1f, this.m_velocity.normalized, out this.m_hit, this.m_velocity.magnitude * Time.deltaTime, (int) this.LM_Hitting, QueryTriggerInteraction.Collide))
        {
          this.m_newPos = this.m_hit.point;
          flag = true;
        }
        else
          this.m_newPos = this.m_lastPos + this.m_velocity * Time.deltaTime;
        this.transform.position = this.m_newPos;
        this.m_distanceTravelled += Vector3.Distance(this.m_newPos, this.m_lastPos);
        if ((double) this.m_distanceTravelled > 8.0 && !this.m_isMotorEngaged)
          this.m_isMotorEngaged = true;
        if (flag || (double) this.m_distanceTravelled > 4000.0)
          this.Explode();
        this.m_lastPos = this.m_newPos;
      }
      if (!this.m_isDestroyedAndWaitingToCountDown || !this.m_tickingDownToStopSound)
        return;
      this.m_tickDownToStopSound -= Time.deltaTime;
      if ((double) this.m_tickDownToStopSound <= 0.0)
        this.RocketSound.Stop();
      if ((double) this.m_tickDownToStopSound > -30.0)
        return;
      Object.Destroy((Object) this.gameObject);
    }

    private void Explode()
    {
      if (this.m_isDestroyedAndWaitingToCountDown)
        return;
      this.m_isDestroyedAndWaitingToCountDown = true;
      this.TurnOffOnExplode.SetActive(false);
      if ((Object) this.DestroyOnExplode != (Object) null)
        Object.Destroy((Object) this.DestroyOnExplode);
      this.m_tickDownToStopSound = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) / 343f;
      this.m_tickingDownToStopSound = true;
      this.PFX_Fire.emission.rateOverTimeMultiplier = 0.0f;
      ParticleSystem.EmissionModule emission = this.PFX_Smoke.emission;
      emission.rateOverTimeMultiplier = 0.0f;
      emission.rateOverDistanceMultiplier = 0.0f;
      for (int index = 0; index < this.SpawnOnExplode.Count; ++index)
        Object.Instantiate<GameObject>(this.SpawnOnExplode[index], this.transform.position, Quaternion.identity);
    }
  }
}
