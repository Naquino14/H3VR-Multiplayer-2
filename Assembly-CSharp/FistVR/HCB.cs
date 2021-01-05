// Decompiled with JetBrains decompiler
// Type: FistVR.HCB
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class HCB : FVRFireArm
  {
    [Header("Component Connections")]
    public FVRFireArmChamber Chamber;
    public Transform Trigger;
    public Vector2 TriggerRotRange;
    private Renderer ProxyRend;
    private float m_cookedAmount;
    private float m_cookSpeed = 1f;
    [Header("Sled String Part")]
    public Transform Sled;
    public List<Transform> Strings;
    public List<Transform> StringTargets;
    public Transform BoltHolder;
    public Transform SledPos_Forward;
    public Transform SledPos_Rearward;
    public Vector2 HolderRotRange;
    private bool m_isLoaded;
    [Header("Roller Parts")]
    public List<Transform> Rollers;
    private bool m_isPowered;
    private float m_rollingRot;
    private float m_shotCooldown = 0.3f;
    public GameObject BoltPrefab;
    private HCB.SledState m_sledState;
    private float m_sledLerp;
    public float SledLerpSpeed = 3f;
    private float m_rollingSpeedMag = 1f;
    [Header("AudioEvents")]
    public AudioEvent AudEvent_SledStart;
    public AudioEvent AudEvent_SledComplete;
    public AudioEvent AudEvent_PowerOn;
    public AudioEvent AudEvent_PowerOut;
    public AudioEvent AudEvent_Char;
    public AudioEvent AudEvent_Fire;
    [Header("VFX")]
    public ParticleSystem PSystem;
    public AudioSource AudSource_Sizzle;
    public ParticleSystem PSystem_Sparks;
    public AudioEvent AudEvent_Spark;
    private float m_timeTilSpark = 0.04f;
    private bool m_isCooking;

    protected override void Start()
    {
      base.Start();
      this.Sled.localPosition = this.SledPos_Forward.localPosition;
      this.UpdateStrings();
      this.ProxyRend = (Renderer) this.Chamber.ProxyRenderer;
    }

    private void UpdateStrings()
    {
      for (int index = 0; index < this.Strings.Count; ++index)
      {
        Vector3 forward = this.StringTargets[index].position - this.Strings[index].position;
        this.Strings[index].rotation = Quaternion.LookRotation(forward, this.transform.up);
        this.Strings[index].localScale = new Vector3(1f, 1f, forward.magnitude);
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      bool isPowered = this.m_isPowered;
      this.m_isPowered = !((Object) this.Magazine == (Object) null) && this.Magazine.HasFuel();
      if (this.m_isPowered && !isPowered)
      {
        this.m_shotCooldown = 0.3f;
        this.PlayAudioAsHandling(this.AudEvent_PowerOn, this.Sled.transform.position);
      }
      else if (!this.m_isPowered && isPowered)
        this.PlayAudioAsHandling(this.AudEvent_PowerOut, this.Sled.transform.position);
      if (this.m_isPowered)
      {
        this.m_rollingRot += Time.deltaTime * 90f * this.m_rollingSpeedMag;
        this.m_rollingRot = Mathf.Repeat(this.m_rollingRot, 360f);
        for (int index = 0; index < this.Rollers.Count; ++index)
          this.Rollers[index].localEulerAngles = new Vector3(0.0f, 0.0f, this.m_rollingRot);
        this.Chamber.ProxyMesh.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_rollingRot);
      }
      if (this.m_isPowered)
        this.Magazine.DrainFuel(Time.deltaTime * 0.05f);
      if (this.IsHeld && (double) this.m_shotCooldown > 0.0)
        this.m_shotCooldown -= Time.deltaTime;
      switch (this.m_sledState)
      {
        case HCB.SledState.Forward:
          this.Chamber.IsAccessible = false;
          if (this.m_isPowered && (double) this.m_shotCooldown <= 0.0 && (this.IsHeld && !this.IsAltHeld))
          {
            this.m_sledLerp = 0.0f;
            this.m_sledState = HCB.SledState.Winding;
            this.PlayAudioAsHandling(this.AudEvent_SledStart, this.Sled.transform.position);
            break;
          }
          break;
        case HCB.SledState.Winding:
          this.Chamber.IsAccessible = false;
          this.m_shotCooldown = 0.3f;
          if (this.m_isPowered)
          {
            this.m_sledLerp += this.SledLerpSpeed * Time.deltaTime;
            this.Sled.localPosition = Vector3.Lerp(this.SledPos_Forward.localPosition, this.SledPos_Rearward.localPosition, this.m_sledLerp);
            this.UpdateStrings();
            if ((double) this.m_sledLerp > 1.0)
            {
              this.m_sledLerp = 1f;
              this.m_sledState = HCB.SledState.Rear;
              this.PlayAudioAsHandling(this.AudEvent_SledComplete, this.Sled.transform.position);
            }
          }
          if (this.m_isPowered && !isPowered)
          {
            this.PlayAudioAsHandling(this.AudEvent_SledStart, this.Sled.transform.position);
            break;
          }
          break;
        case HCB.SledState.Rear:
          this.Chamber.IsAccessible = true;
          this.m_shotCooldown = 0.3f;
          break;
      }
      bool isLoaded = this.m_isLoaded;
      this.m_isLoaded = this.Chamber.IsFull;
      if (this.m_isLoaded && !isLoaded)
        this.SetAnimatedComponent(this.BoltHolder, this.HolderRotRange.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      else if (!this.m_isLoaded && isLoaded)
        this.SetAnimatedComponent(this.BoltHolder, this.HolderRotRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
      if (this.m_isCooking)
      {
        this.m_rollingSpeedMag = this.m_cookedAmount * 5f;
        if (!this.Chamber.IsFull)
          return;
        this.PSystem.Emit(4);
        if (!this.AudSource_Sizzle.isPlaying)
          this.AudSource_Sizzle.Play();
        if ((double) this.m_cookedAmount >= 1.0)
          return;
        this.AddCookedAmount(Time.deltaTime * 0.5f);
        this.m_timeTilSpark -= Time.deltaTime;
        if ((double) this.m_timeTilSpark > 0.0)
          return;
        this.m_timeTilSpark = Random.Range(0.04f, 0.3f);
        this.PSystem_Sparks.Emit(4);
        this.PlayAudioAsHandling(this.AudEvent_Spark, this.PSystem_Sparks.transform.position);
      }
      else
      {
        this.m_rollingSpeedMag = 1f;
        if (!this.AudSource_Sizzle.isPlaying)
          return;
        this.AudSource_Sizzle.Stop();
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      bool flag = false;
      base.UpdateInteraction(hand);
      if (this.m_hasTriggeredUpSinceBegin && this.m_sledState == HCB.SledState.Rear)
      {
        if (hand.Input.TriggerPressed)
          flag = true;
        if (this.m_isCooking && hand.Input.TriggerUp)
          this.ReleaseSled();
      }
      this.m_isCooking = flag;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if (this.m_isCooking)
        this.ReleaseSled();
      base.EndInteraction(hand);
      this.m_isCooking = false;
    }

    private void SetCookedAmount(float f)
    {
      this.m_cookedAmount = 0.0f;
      this.m_cookedAmount = Mathf.Clamp(this.m_cookedAmount, 0.0f, 1f);
      this.ProxyRend.material.SetFloat("_BlendScale", this.m_cookedAmount);
    }

    private void AddCookedAmount(float f)
    {
      this.m_cookedAmount += f;
      this.m_cookedAmount = Mathf.Clamp(this.m_cookedAmount, 0.0f, 1f);
      this.ProxyRend.material.SetFloat("_BlendScale", this.m_cookedAmount);
    }

    private void ReleaseSled()
    {
      this.m_sledState = HCB.SledState.Forward;
      this.Sled.localPosition = this.SledPos_Forward.localPosition;
      this.UpdateStrings();
      if (this.Chamber.IsFull)
      {
        HCBBolt component = Object.Instantiate<GameObject>(this.BoltPrefab, this.MuzzlePos.position, this.MuzzlePos.rotation).GetComponent<HCBBolt>();
        component.Fire(this.MuzzlePos.forward, this.MuzzlePos.position, 1f);
        component.SetCookedAmount(this.m_cookedAmount);
        this.Chamber.SetRound((FVRFireArmRound) null);
      }
      this.SetCookedAmount(0.0f);
      this.PlayAudioAsHandling(this.AudEvent_Fire, this.Sled.transform.position);
    }

    public enum SledState
    {
      Forward,
      Winding,
      Rear,
    }
  }
}
