// Decompiled with JetBrains decompiler
// Type: FistVR.EncryptionBotMine
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class EncryptionBotMine : MonoBehaviour
  {
    public AIEntity E;
    public List<GameObject> SpawnOnDestroy;
    public List<Transform> SpawnOnDestroyPoints;
    public Rigidbody RB;
    public EncryptionBotMine.BotState State;
    private float m_activateTick;
    public float ActivateSpeed = 1f;
    public float DeactivateSpeed = 1f;
    public float CooldownSpeed = 1f;
    private float m_cooldownTick = 1f;
    private float m_explodingTick;
    public float DetonationRange = 10f;
    public float MoveSpeed = 10f;
    [Header("Audio")]
    public AudioEvent AudEvent_Passive;
    public AudioEvent AudEvent_Activating;
    public AudioEvent AudEvent_Deactivating;
    public AudioEvent AudEvent_Scream;
    public ParticleSystem ActivatedParticles;
    public ParticleSystem ExplodingParticles;
    public LayerMask LM_GroundCast;
    public Vector2 DesiredHeight = new Vector2(4f, 6f);
    private float m_desiredHeight = 4f;
    private float m_tickDownToSpeak = 1f;
    private Vector3 latestTargetPos = Vector3.zero;
    private float moveTowardTick;
    private float m_respondToEventCooldown = 0.1f;

    private void Start()
    {
      this.E.AIEventReceiveEvent += new AIEntity.AIEventReceive(this.EventReceive);
      this.m_tickDownToSpeak = Random.Range(5f, 20f);
      this.m_desiredHeight = Random.Range(this.DesiredHeight.x, this.DesiredHeight.y);
    }

    private void OnDestroy() => this.E.AIEventReceiveEvent -= new AIEntity.AIEventReceive(this.EventReceive);

    private void TestMe()
    {
    }

    private void Update()
    {
      if ((double) this.m_respondToEventCooldown > 0.0)
        this.m_respondToEventCooldown -= Time.deltaTime;
      ParticleSystem.EmissionModule emission1 = this.ActivatedParticles.emission;
      ParticleSystem.EmissionModule emission2 = this.ExplodingParticles.emission;
      switch (this.State)
      {
        case EncryptionBotMine.BotState.Deactivated:
          this.m_tickDownToSpeak -= Time.deltaTime;
          if ((double) this.m_tickDownToSpeak <= 0.0)
          {
            this.m_tickDownToSpeak = Random.Range(8f, 20f);
            if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) <= 50.0)
              SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Passive, this.transform.position).FollowThisTransform(this.transform);
            emission1.rateOverTimeMultiplier = 0.0f;
          }
          if (Physics.Raycast(this.transform.position, -Vector3.up, this.m_desiredHeight, (int) this.LM_GroundCast))
          {
            this.RB.AddForce(Vector3.up * 20f, ForceMode.Acceleration);
            break;
          }
          break;
        case EncryptionBotMine.BotState.Activating:
          this.m_activateTick += Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick >= 1.0)
            this.SetState(EncryptionBotMine.BotState.Activated);
          emission1.rateOverTimeMultiplier = this.m_activateTick * 80f;
          break;
        case EncryptionBotMine.BotState.Activated:
          this.m_cooldownTick -= Time.deltaTime * this.CooldownSpeed;
          if ((double) this.m_cooldownTick <= 0.0)
            this.SetState(EncryptionBotMine.BotState.Deactivating);
          emission1.rateOverTimeMultiplier = this.m_activateTick * 80f;
          break;
        case EncryptionBotMine.BotState.Deactivating:
          this.m_activateTick -= Time.deltaTime * this.ActivateSpeed;
          if ((double) this.m_activateTick <= 0.0)
          {
            this.m_activateTick = 0.0f;
            this.SetState(EncryptionBotMine.BotState.Deactivated);
          }
          emission1.rateOverTimeMultiplier = 0.0f;
          break;
        case EncryptionBotMine.BotState.Exploding:
          emission1.rateOverTimeMultiplier = 0.0f;
          emission2.rateOverTimeMultiplier = 80f;
          this.m_explodingTick += Time.deltaTime * 2f;
          if ((double) this.m_explodingTick >= 1.0)
          {
            this.Shatter();
            break;
          }
          break;
      }
      if ((double) this.moveTowardTick <= 0.0)
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
      if (this.State == EncryptionBotMine.BotState.Deactivating)
        return;
      this.latestTargetPos = v;
      this.moveTowardTick = 1f;
      if (this.State == EncryptionBotMine.BotState.Deactivated)
      {
        this.SetState(EncryptionBotMine.BotState.Activating);
      }
      else
      {
        if (this.State != EncryptionBotMine.BotState.Activated && this.State != EncryptionBotMine.BotState.Activating || (double) Vector3.Distance(v, this.transform.position) > (double) this.DetonationRange)
          return;
        this.Explode();
      }
    }

    private void OnCollisionEnter(Collision collision)
    {
      if (this.State != EncryptionBotMine.BotState.Activating && this.State != EncryptionBotMine.BotState.Activated || collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        return;
      this.Explode();
    }

    private void SetState(EncryptionBotMine.BotState S)
    {
      if (this.State == EncryptionBotMine.BotState.Exploding || this.State == S)
        return;
      this.State = S;
      switch (this.State)
      {
        case EncryptionBotMine.BotState.Deactivated:
          this.m_activateTick = 0.0f;
          break;
        case EncryptionBotMine.BotState.Activating:
          this.m_activateTick = 0.0f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 50.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Activating, this.transform.position).FollowThisTransform(this.transform);
          break;
        case EncryptionBotMine.BotState.Activated:
          this.m_cooldownTick = 1f;
          this.m_activateTick = 1f;
          break;
        case EncryptionBotMine.BotState.Deactivating:
          this.m_activateTick = 1f;
          if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.position) > 50.0)
            break;
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AudEvent_Deactivating, this.transform.position).FollowThisTransform(this.transform);
          break;
      }
    }

    public void Explode()
    {
      if (this.State == EncryptionBotMine.BotState.Exploding)
        return;
      this.SetState(EncryptionBotMine.BotState.Exploding);
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
