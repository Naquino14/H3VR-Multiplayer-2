// Decompiled with JetBrains decompiler
// Type: FistVR.Chainsaw
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Chainsaw : FVRPhysicalObject
  {
    [Header("ChainsawParms")]
    public FVRAlternateGrip Foregrip;
    public AudioSource SawAudio;
    public AudioSource StartingAudio;
    public AudioClip AudClip_Start;
    public AudioClip AudClip_Idle;
    public AudioClip AudClip_Buzzing;
    public AudioClip AudClip_Hitting;
    private bool m_isRunning;
    private float m_currentCableLength;
    private float m_lastCableLength;
    private float m_motorSpeed;
    private float triggerAmount;
    private float m_sawingIntesity;
    public bool UsesBladeSolidBits = true;
    public Renderer BladeSolid;
    public Renderer BladeBits;
    private Material m_matBladeSolid;
    private Material m_matBladeBits;
    public Collider[] BladeCols;
    private HashSet<Collider> m_bladeCols = new HashSet<Collider>();
    public ParticleSystem Sparks;
    private ParticleSystem.EmitParams emitParams;
    public Transform BladePoint1;
    public Transform BladePoint2;
    private List<IFVRDamageable> DamageablesToDo;
    private HashSet<IFVRDamageable> DamageablesToDoHS;
    private List<Vector3> DamageableHitPoints;
    private List<Vector3> DamageableHitNormals;
    private float TimeSinceDamageDealing = 0.2f;
    public ParticleSystem EngineSmoke;
    public bool UsesEngineRot = true;
    public Transform EngineRot;
    public float PerceptibleEventVolume = 50f;
    public float PerceptibleEventRange = 30f;
    private float m_timeTilPerceptibleEventTick = 0.2f;
    private float timeSinceCollision = 1f;
    private int framesTilFlash;

    protected override void Awake()
    {
      base.Awake();
      this.emitParams = new ParticleSystem.EmitParams();
      if (this.UsesBladeSolidBits)
      {
        this.m_matBladeSolid = this.BladeSolid.materials[0];
        this.m_matBladeBits = this.BladeBits.material;
      }
      for (int index = 0; index < this.BladeCols.Length; ++index)
        this.m_bladeCols.Add(this.BladeCols[index]);
      this.DamageablesToDo = new List<IFVRDamageable>();
      this.DamageablesToDoHS = new HashSet<IFVRDamageable>();
      this.DamageableHitPoints = new List<Vector3>();
      this.DamageableHitNormals = new List<Vector3>();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin || this.IsAltHeld)
        return;
      this.triggerAmount = hand.Input.TriggerFloat;
      if ((!hand.IsInStreamlinedMode || !hand.Input.BYButtonDown) && (hand.IsInStreamlinedMode || !hand.Input.TouchpadDown))
        return;
      this.m_isRunning = false;
      this.m_motorSpeed = 0.98f;
    }

    private void OnCollisionStay(Collision col)
    {
      if (!this.m_isRunning || (double) this.m_sawingIntesity <= 0.100000001490116)
        return;
      int num = 0;
      for (int index = 0; index < col.contacts.Length; ++index)
      {
        if (this.m_bladeCols.Contains(col.contacts[index].thisCollider))
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
            Vector3 closestValidPoint = this.GetClosestValidPoint(this.BladePoint1.position, this.BladePoint2.position, col.contacts[index].point);
            Vector3 vector3 = Vector3.ClampMagnitude(col.contacts[index].point - closestValidPoint, 0.04f);
            Vector3 pos = closestValidPoint + vector3;
            this.emitParams.position = pos;
            this.emitParams.velocity = Vector3.Cross(vector3.normalized, this.transform.right) * Random.Range(1f, 10f) + Random.onUnitSphere * 3f + vector3 * 2f;
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

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
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
          Dam_Blunt = 100f * Mathf.Clamp(this.RootRigidbody.velocity.magnitude, 1f, 3f),
          Dam_Cutting = 250f * Mathf.Clamp(this.RootRigidbody.velocity.magnitude, 1f, 2f)
        };
        dam.Dam_TotalKinetic = dam.Dam_Cutting + dam.Dam_Blunt;
        dam.Class = Damage.DamageClass.Melee;
        for (int index = 0; index < this.DamageablesToDo.Count; ++index)
        {
          if ((Object) this.DamageablesToDo[index] != (Object) null)
          {
            dam.hitNormal = this.DamageableHitNormals[index];
            dam.point = this.DamageableHitPoints[index];
            dam.strikeDir = -dam.hitNormal;
            this.DamageablesToDo[index].Damage(dam);
          }
        }
        this.DamageablesToDo.Clear();
        this.DamageablesToDoHS.Clear();
        this.DamageableHitPoints.Clear();
        this.DamageableHitNormals.Clear();
        this.TimeSinceDamageDealing = 0.1f;
      }
      if (!this.m_isRunning)
      {
        this.SawAudio.volume = this.m_motorSpeed * 0.7f;
        this.StartingAudio.volume = this.m_motorSpeed;
        if ((double) this.m_motorSpeed <= 0.0 && this.StartingAudio.isPlaying)
          this.StartingAudio.Stop();
        if (this.UsesBladeSolidBits)
        {
          this.m_matBladeSolid.SetVector("_MainTexVelocity", (Vector4) new Vector2(0.0f, 0.0f));
          this.m_matBladeBits.SetVector("_MainTexVelocity", (Vector4) new Vector2(0.0f, 0.0f));
        }
        ParticleSystem.EmissionModule emission = this.EngineSmoke.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = 0.0f;
        rate.constantMin = 0.0f;
        emission.rate = rate;
      }
      else
      {
        if (!this.SawAudio.isPlaying)
          this.SawAudio.Play();
        this.triggerAmount += Random.Range(-0.05f, 0.05f);
        this.m_sawingIntesity = !this.IsHeld ? Mathf.Lerp(this.m_sawingIntesity, 0.0f, Time.deltaTime * 2f) : Mathf.Lerp(this.m_sawingIntesity, this.triggerAmount, Time.deltaTime * 5f);
        if ((double) this.m_sawingIntesity > 0.100000001490116)
        {
          this.SawAudio.volume = (float) ((0.800000011920929 + (double) this.m_sawingIntesity * 0.5) * 0.300000011920929);
          this.SawAudio.pitch = (float) (0.600000023841858 + (double) this.m_sawingIntesity * 0.699999988079071);
          if ((double) this.timeSinceCollision < 0.2)
          {
            if ((Object) this.SawAudio.clip != (Object) this.AudClip_Hitting)
              this.SawAudio.clip = this.AudClip_Hitting;
          }
          else if ((Object) this.SawAudio.clip != (Object) this.AudClip_Buzzing)
            this.SawAudio.clip = this.AudClip_Buzzing;
          if (this.UsesBladeSolidBits)
          {
            this.m_matBladeSolid.SetVector("_MainTexVelocity", (Vector4) new Vector2(this.m_sawingIntesity, 0.0f));
            this.m_matBladeBits.SetVector("_MainTexVelocity", (Vector4) new Vector2(this.m_sawingIntesity, 0.0f));
          }
        }
        else
        {
          this.SawAudio.volume = 0.25f;
          this.SawAudio.pitch = 1f;
          if ((Object) this.SawAudio.clip != (Object) this.AudClip_Idle)
            this.SawAudio.clip = this.AudClip_Idle;
          if (this.UsesBladeSolidBits)
          {
            this.m_matBladeSolid.SetVector("_MainTexVelocity", (Vector4) new Vector2(0.01f, 0.0f));
            this.m_matBladeBits.SetVector("_MainTexVelocity", (Vector4) new Vector2(0.01f, 0.0f));
          }
        }
        ParticleSystem.EmissionModule emission = this.EngineSmoke.emission;
        ParticleSystem.MinMaxCurve rate = emission.rate;
        rate.mode = ParticleSystemCurveMode.Constant;
        rate.constantMax = (float) ((double) this.m_motorSpeed * 2.0 + (double) this.m_sawingIntesity * 20.0);
        rate.constantMin = (float) ((double) this.m_motorSpeed * 2.0 + (double) this.m_sawingIntesity * 20.0);
        emission.rate = rate;
      }
      if ((double) this.m_motorSpeed >= 1.0)
      {
        this.m_isRunning = true;
      }
      else
      {
        this.m_motorSpeed -= Time.deltaTime * 3f;
        this.m_motorSpeed = Mathf.Clamp(this.m_motorSpeed, 0.0f, 1f);
      }
      if (this.UsesEngineRot)
      {
        float x = this.EngineRot.localEulerAngles.x;
        this.EngineRot.localEulerAngles = new Vector3(Mathf.Repeat((double) this.m_sawingIntesity <= 0.0 ? x + Time.deltaTime * (360f * this.m_motorSpeed) : x + Time.deltaTime * (float) (360.0 + (double) this.m_sawingIntesity * 1200.0), 360f), 0.0f, 0.0f);
      }
      if (!this.m_isRunning)
        return;
      this.m_timeTilPerceptibleEventTick -= Time.deltaTime;
      if ((double) this.m_timeTilPerceptibleEventTick > 0.0)
        return;
      this.m_timeTilPerceptibleEventTick = Random.Range(0.2f, 0.3f);
      GM.CurrentSceneSettings.OnPerceiveableSound((float) ((double) this.PerceptibleEventVolume * (double) this.m_motorSpeed * (double) this.m_sawingIntesity * 0.5), (float) ((double) this.PerceptibleEventRange * (double) this.m_motorSpeed * (double) this.m_sawingIntesity * 0.5), this.transform.position, GM.CurrentPlayerBody.GetPlayerIFF());
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.m_isRunning)
        return;
      float num = 0.1f + this.m_sawingIntesity * 0.3f;
      this.RootRigidbody.velocity += Random.onUnitSphere * num;
      this.RootRigidbody.angularVelocity += Random.onUnitSphere * num;
    }

    public void SetCableLength(float f)
    {
      if (!this.m_isRunning && (double) f > (double) this.m_currentCableLength)
      {
        if (!this.StartingAudio.isPlaying)
          this.StartingAudio.Play();
        this.m_motorSpeed += (float) (((double) f - (double) this.m_currentCableLength) * 1.5);
      }
      this.m_currentCableLength = f;
    }
  }
}
