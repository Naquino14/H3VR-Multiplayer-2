// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotStealthMine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotStealthMine : MonoBehaviour, IFVRDamageable
  {
    public AIEntity E;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> SpawnOnDestroyPoints;
    public Rigidbody RB;
    public EncryptionBotStealthMine.BotState State;
    private float m_activateTick;
    public float ActivateSpeed = 1f;
    public float DeactivateSpeed = 1f;
    public float CooldownSpeed = 1f;
    private float m_cooldownTick = 1f;
    private float m_explodingTick;
    public float DetonationRange = 2f;
    public float MoveSpeed = 10f;
    [Header("Audio")]
    public AudioEvent AudEvent_Passive;
    public AudioEvent AudEvent_Activating;
    public AudioEvent AudEvent_Deactivating;
    public AudioEvent AudEvent_Scream;
    public AudioEvent AudEvent_TP;
    public ParticleSystem ActivatedParticles;
    public ParticleSystem ExplodingParticles;
    public LayerMask LM_GroundCast;
    public Vector2 DesiredHeight = new Vector2(2f, 2.2f);
    private float m_desiredHeight = 4f;
    [Header("StealthPart")]
    public Renderer Rend_Base;
    public Renderer Rend_Cloaked;
    public LayerMask LM_Stealth;
    private float m_timeTilTeleport = 10f;
    private Vector3 startPoint = Vector3.zero;
    private float m_tickDownToSpeak = 1f;
    private RaycastHit h;
    private Vector3 latestTargetPos = Vector3.zero;
    private float moveTowardTick;
    private float m_respondToEventCooldown = 0.1f;
    private float m_stun;

    public void Damage(FistVR.Damage d) => this.m_stun = 0.4f;

    private void Start()
    {
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.m_tickDownToSpeak = Random.Range(5f, 20f);
      this.m_desiredHeight = Random.Range(this.DesiredHeight.x, this.DesiredHeight.y);
      this.m_timeTilTeleport = Random.Range(10f, 30f);
      this.startPoint = this.transform.position;
      this.Rend_Cloaked.material.SetVector("_MainTexVelocity", (Vector4) new Vector2(Random.Range(0.2104f, 0.241f), 1f));
    }

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    private void Teleport()
    {
      this.m_timeTilTeleport = Random.Range(5f, 15f);
      Vector3 vector3 = this.startPoint + Random.onUnitSphere * 50f;
      Vector3 a = new Vector3(GM.CurrentPlayerBody.Head.position.x, 0.0f, GM.CurrentPlayerBody.Head.position.z);
      Vector3 b = new Vector3(vector3.x, 0.0f, vector3.z);
      if ((double) Vector3.Distance(a, b) < 10.0)
        b += (b - a).normalized * 10f;
      if (!Physics.SphereCast(b + Vector3.up * 500f, 1.5f, Vector3.down, out this.h, 1000f, (int) this.LM_Stealth))
        return;
      b.y = this.h.point.y + Random.Range(2f, 4f);
      this.ExplodingParticles.Emit(10);
      this.transform.position = b;
      float num = Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position);
      this.ExplodingParticles.Emit(10);
      if ((double) num >= 25.0)
        return;
      SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, this.AudEvent_TP, this.transform.position);
    }

    private void Update()
    {
      if ((double) this.m_stun > 0.0)
        this.m_stun -= Time.deltaTime;
      if ((double) this.m_respondToEventCooldown > 0.0)
        this.m_respondToEventCooldown -= Time.deltaTime;
      ParticleSystem.EmissionModule emission1 = this.ActivatedParticles.emission;
      ParticleSystem.EmissionModule emission2 = this.ExplodingParticles.emission;
      switch (this.State)
      {
        case EncryptionBotStealthMine.BotState.Deactivated:
          this.m_tickDownToSpeak -= Time.deltaTime;
          if ((double) this.m_tickDownToSpeak <= 0.0)
          {
            this.m_tickDownToSpeak = Random.Range(8f, 20f);
            if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 50.0)
              SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Passive, this.transform.position).FollowThisTransform(this.transform);
            emission1.rateOverTimeMultiplier = 0.0f;
          }
          if (Physics.Raycast(this.transform.position, -Vector3.up, this.m_desiredHeight, (int) this.LM_GroundCast))
            this.RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
          if (this.Rend_Base.enabled)
            this.Rend_Base.enabled = false;
          if (!this.Rend_Cloaked.enabled)
            this.Rend_Base.enabled = true;
          this.m_timeTilTeleport -= Time.deltaTime;
          if ((double) this.m_timeTilTeleport <= 0.0)
          {
            this.Teleport();
            break;
          }
          break;
        case EncryptionBotStealthMine.BotState.Activating:
          this.m_activateTick += Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick >= 1.0)
            this.SetState(EncryptionBotStealthMine.BotState.Activated);
          emission1.rateOverTimeMultiplier = this.m_activateTick * 10f;
          if (this.Rend_Base.enabled)
            this.Rend_Base.enabled = false;
          if (!this.Rend_Cloaked.enabled)
          {
            this.Rend_Base.enabled = true;
            break;
          }
          break;
        case EncryptionBotStealthMine.BotState.Activated:
          this.m_cooldownTick -= Time.deltaTime * this.CooldownSpeed;
          if ((double) this.m_cooldownTick <= 0.0)
            this.SetState(EncryptionBotStealthMine.BotState.Deactivating);
          emission1.rateOverTimeMultiplier = this.m_activateTick * 10f;
          if (!this.Rend_Base.enabled)
            this.Rend_Base.enabled = true;
          if (this.Rend_Cloaked.enabled)
          {
            this.Rend_Base.enabled = false;
            break;
          }
          break;
        case EncryptionBotStealthMine.BotState.Deactivating:
          this.m_activateTick -= Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick <= 0.0)
          {
            this.m_activateTick = 0.0f;
            this.SetState(EncryptionBotStealthMine.BotState.Deactivated);
          }
          if (this.Rend_Base.enabled)
            this.Rend_Base.enabled = false;
          if (!this.Rend_Cloaked.enabled)
            this.Rend_Base.enabled = true;
          emission1.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotStealthMine.BotState.Exploding:
          emission1.rateOverTimeMultiplier = 0.0f;
          emission2.rateOverTimeMultiplier = 80f;
          this.m_explodingTick += Time.deltaTime * 2f;
          if ((double) this.m_explodingTick >= 1.0)
            this.Shatter();
          if (!this.Rend_Base.enabled)
            this.Rend_Base.enabled = true;
          if (this.Rend_Cloaked.enabled)
          {
            this.Rend_Base.enabled = false;
            break;
          }
          break;
      }
      if ((double) this.moveTowardTick <= 0.0 || (double) this.m_stun > 0.0)
        return;
      this.moveTowardTick -= Time.deltaTime;
      this.RB.velocity = this.MoveSpeed * (this.latestTargetPos - this.transform.position).normalized;
    }

    public void EventReceive(AIEvent e)
    {
      if ((double) this.m_respondToEventCooldown >= 0.100000001490116 || e.IsEntity && e.Entity.IFFCode == this.E.IFFCode)
        return;
      this.TargetSighted(e.Pos);
    }

    private void TargetSighted(Vector3 v)
    {
      if (this.State == EncryptionBotStealthMine.BotState.Deactivating)
        return;
      this.latestTargetPos = v;
      this.moveTowardTick = 1f;
      if (this.State == EncryptionBotStealthMine.BotState.Deactivated)
      {
        this.SetState(EncryptionBotStealthMine.BotState.Activating);
      }
      else
      {
        if (this.State != EncryptionBotStealthMine.BotState.Activated && this.State != EncryptionBotStealthMine.BotState.Activating || (double) Vector3.Distance(v, this.transform.position) > (double) this.DetonationRange)
          return;
        this.Explode();
      }
    }

    private void OnCollisionEnter(Collision collision) => this.Explode();

    private void SetState(EncryptionBotStealthMine.BotState S)
    {
      if (this.State == EncryptionBotStealthMine.BotState.Exploding || this.State == S)
        return;
      this.State = S;
      switch (this.State)
      {
        case EncryptionBotStealthMine.BotState.Deactivated:
          this.m_activateTick = 0.0f;
          break;
        case EncryptionBotStealthMine.BotState.Activating:
          this.m_activateTick = 0.0f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 50.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Activating, this.transform.position).FollowThisTransform(this.transform);
          break;
        case EncryptionBotStealthMine.BotState.Activated:
          this.m_cooldownTick = 1f;
          this.m_activateTick = 1f;
          break;
        case EncryptionBotStealthMine.BotState.Deactivating:
          this.m_activateTick = 1f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 50.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Deactivating, this.transform.position).FollowThisTransform(this.transform);
          break;
      }
    }

    public void Explode()
    {
      if (this.State == EncryptionBotStealthMine.BotState.Exploding)
        return;
      this.SetState(EncryptionBotStealthMine.BotState.Exploding);
      this.m_explodingTick = 0.0f;
      SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Scream, this.transform.position).FollowThisTransform(this.transform);
    }

    private void Shatter()
    {
      for (int index = 0; index < this.SpawnOnDestroy.Count; ++index)
      {
        Rigidbody component = Object.Instantiate<GameObject>(this.SpawnOnDestroy[index], this.SpawnOnDestroyPoints[index].position, this.SpawnOnDestroyPoints[index].rotation).GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
          component.AddExplosionForce((float) Random.Range(1, 10), this.transform.position + Random.onUnitSphere, 5f);
      }
      Object.Destroy((Object) this.gameObject);
    }

    public enum BotState
    {
      Deactivated,
      Activating,
      Activated,
      Deactivating,
      Exploding,
    }
  }
}
