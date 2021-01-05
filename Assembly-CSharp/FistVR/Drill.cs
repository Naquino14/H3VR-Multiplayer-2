// Decompiled with JetBrains decompiler
// Type: FistVR.Drill
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Drill : FVRPhysicalObject
  {
    private float m_spinnyZ;
    public Transform SpinnyBit;
    public Transform Trigger;
    public AudioSource DrillAudio;
    public AudioClip AudClipRevving;
    public AudioClip AudClipDrilling;
    private float m_triggerFloat;
    private float m_motorSpeed;
    public ParticleSystem Sparks;
    private ParticleSystem.EmitParams emitParams;
    public Collider DrillBit;
    private List<IFVRDamageable> DamageablesToDo;
    private HashSet<IFVRDamageable> DamageablesToDoHS;
    private List<Vector3> DamageableHitPoints;
    private List<Vector3> DamageableHitNormals;
    private float TimeSinceDamageDealing = 0.2f;
    public ParticleSystem EngineSmoke;
    public float PerceptibleEventVolume = 50f;
    public float PerceptibleEventRange = 30f;
    private float m_timeTilPerceptibleEventTick = 0.2f;
    private float timeSinceCollision = 1f;
    private int framesTilFlash;

    protected override void Awake()
    {
      base.Awake();
      this.emitParams = new ParticleSystem.EmitParams();
      this.DamageablesToDo = new List<IFVRDamageable>();
      this.DamageablesToDoHS = new HashSet<IFVRDamageable>();
      this.DamageableHitPoints = new List<Vector3>();
      this.DamageableHitNormals = new List<Vector3>();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin)
        return;
      this.m_triggerFloat = hand.Input.TriggerFloat;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.IsHeld)
        this.m_triggerFloat = 0.0f;
      this.Trigger.localPosition = new Vector3(0.0f, 0.0f, this.m_triggerFloat * -0.0117f);
      if (this.framesTilFlash > 0)
        --this.framesTilFlash;
      if ((double) this.timeSinceCollision < 1.0)
        this.timeSinceCollision += Time.deltaTime;
      if ((double) this.TimeSinceDamageDealing > 0.0)
      {
        this.TimeSinceDamageDealing -= Time.deltaTime;
      }
      else
      {
        Damage dam = new Damage()
        {
          Dam_Blunt = 50f * Mathf.Clamp(this.RootRigidbody.velocity.magnitude, 1f, 3f),
          Dam_Piercing = 250f * Mathf.Clamp(this.RootRigidbody.velocity.magnitude, 1f, 2f)
        };
        dam.Dam_TotalKinetic = dam.Dam_Cutting + dam.Dam_Blunt;
        dam.Class = Damage.DamageClass.Melee;
        for (int index = 0; index < this.DamageablesToDo.Count; ++index)
        {
          if ((Object) this.DamageablesToDo[index] != (Object) null)
          {
            dam.hitNormal = this.DamageableHitNormals[index];
            dam.point = this.DamageableHitPoints[index];
            dam.strikeDir = -this.transform.forward;
            this.DamageablesToDo[index].Damage(dam);
          }
        }
        this.DamageablesToDo.Clear();
        this.DamageablesToDoHS.Clear();
        this.DamageableHitPoints.Clear();
        this.DamageableHitNormals.Clear();
        this.TimeSinceDamageDealing = 0.1f;
      }
      this.m_motorSpeed = !this.IsHeld ? Mathf.Lerp(this.m_motorSpeed, 0.0f, Time.deltaTime * 4f) : Mathf.Lerp(this.m_motorSpeed, this.m_triggerFloat, Time.deltaTime * 4f);
      if ((double) this.m_motorSpeed <= 0.025000000372529 && this.DrillAudio.isPlaying)
        this.DrillAudio.Stop();
      else if ((double) this.m_motorSpeed > 0.025000000372529 && !this.DrillAudio.isPlaying)
        this.DrillAudio.Play();
      if ((double) this.timeSinceCollision < 0.200000002980232)
      {
        this.DrillAudio.volume = (float) (0.100000001490116 + (double) this.m_motorSpeed * 0.150000005960464);
        this.DrillAudio.pitch = (float) (0.800000011920929 + (double) this.m_motorSpeed * 0.200000002980232);
      }
      else
      {
        this.DrillAudio.volume = (float) (0.100000001490116 + (double) this.m_motorSpeed * 0.150000005960464);
        this.DrillAudio.pitch = (float) (0.300000011920929 + (double) this.m_motorSpeed * 0.699999988079071);
      }
      if ((double) this.timeSinceCollision < 0.200000002980232 && (Object) this.DrillAudio.clip != (Object) this.AudClipDrilling)
      {
        this.DrillAudio.clip = this.AudClipDrilling;
        ParticleSystem.EmissionModule emission = this.EngineSmoke.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 10f;
        rate.constantMin = 10f;
        emission.rate = rate;
      }
      else if ((double) this.timeSinceCollision >= 0.200000002980232 && (Object) this.DrillAudio.clip != (Object) this.AudClipRevving)
      {
        this.DrillAudio.clip = this.AudClipRevving;
        ParticleSystem.EmissionModule emission = this.EngineSmoke.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 0.0f;
        rate.constantMin = 0.0f;
        emission.rate = rate;
      }
      if ((double) this.m_motorSpeed <= 0.100000001490116)
        return;
      this.m_spinnyZ += Time.deltaTime * (6000f * this.m_motorSpeed);
      this.m_spinnyZ = Mathf.Repeat(this.m_spinnyZ, 360f);
      this.SpinnyBit.localEulerAngles = new Vector3(0.0f, 0.0f, -this.m_spinnyZ);
      this.m_timeTilPerceptibleEventTick -= Time.deltaTime;
      if ((double) this.m_timeTilPerceptibleEventTick > 0.0)
        return;
      this.m_timeTilPerceptibleEventTick = Random.Range(0.2f, 0.3f);
      GM.CurrentSceneSettings.OnPerceiveableSound(this.PerceptibleEventVolume * this.m_motorSpeed, this.PerceptibleEventRange * this.m_motorSpeed, this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
    }

    private void OnCollisionStay(Collision col)
    {
      if ((double) this.m_motorSpeed <= 0.100000001490116)
        return;
      int num = 0;
      for (int index = 0; index < col.contacts.Length; ++index)
      {
        if ((Object) col.contacts[index].thisCollider == (Object) this.DrillBit)
        {
          IFVRDamageable component1 = col.contacts[index].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
          if (component1 != null && this.DamageablesToDoHS.Add(component1))
          {
            this.DamageablesToDo.Add(component1);
            this.DamageableHitPoints.Add(col.contacts[index].point);
            this.DamageableHitNormals.Add(col.contacts[index].normal);
          }
          if (component1 == null && (Object) col.contacts[index].otherCollider.attachedRigidbody != (Object) null)
          {
            IFVRDamageable component2 = col.contacts[index].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
            if (this.DamageablesToDoHS.Add(component2))
            {
              this.DamageablesToDo.Add(component2);
              this.DamageableHitPoints.Add(col.contacts[index].point);
              this.DamageableHitNormals.Add(col.contacts[index].normal);
            }
          }
          if (num < 2)
          {
            this.timeSinceCollision = 0.0f;
            ++num;
            Vector3 point = col.contacts[index].point;
            Vector3 vector3 = -this.SpinnyBit.forward;
            Vector3 pos = point;
            this.emitParams.position = pos;
            this.emitParams.velocity = vector3 * Random.Range(0.02f, 10f) * this.m_motorSpeed + Random.onUnitSphere * 3f * this.m_motorSpeed;
            this.Sparks.Emit(this.emitParams, 1);
            if (this.framesTilFlash <= 0)
            {
              this.framesTilFlash = Random.Range(3, 7);
              FXM.InitiateMuzzleFlash(pos, col.contacts[index].normal, Random.Range(0.25f, 2f), Color.white, Random.Range(0.5f, 1f));
            }
          }
        }
      }
    }
  }
}
