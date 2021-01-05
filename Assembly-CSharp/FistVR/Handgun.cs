// Decompiled with JetBrains decompiler
// Type: FistVR.Handgun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Handgun : FVRFireArm
  {
    [Header("Handgun Params")]
    public HandgunSlide Slide;
    public FVRFireArmChamber Chamber;
    [Header("Component Connections")]
    public Transform Trigger;
    public Transform Safety;
    public Transform Barrel;
    public Transform Hammer;
    public Transform FireSelector;
    public Transform SlideRelease;
    public GameObject ReloadTriggerWell;
    [Header("Round Positions")]
    public Transform RoundPos_Ejecting;
    public Transform RoundPos_Ejection;
    public Transform RoundPos_Magazine;
    public Vector3 RoundEjectionSpeed;
    public Vector3 RoundEjectionSpin;
    private FVRFirearmMovingProxyRound m_proxy;
    [Header("Trigger Params")]
    public bool HasTrigger;
    public FVRPhysicalObject.InterpStyle TriggerInterp;
    public FVRPhysicalObject.Axis TriggerAxis;
    public float TriggerUnheld;
    public float TriggerHeld;
    public float TriggerResetThreshold = 0.45f;
    public float TriggerBreakThreshold = 0.85f;
    public Handgun.TriggerStyle TriggerType;
    public float TriggerSpeed = 20f;
    public bool HasManualDecocker;
    private float m_triggerTarget;
    private float m_triggerFloat;
    private bool m_isSeerReady = true;
    [Header("Slide Release Params")]
    public bool HasSlideRelease;
    public bool HasSlideReleaseControl = true;
    public FVRPhysicalObject.InterpStyle SlideReleaseInterp;
    public FVRPhysicalObject.Axis SlideReleaseAxis;
    public float SlideReleaseUp;
    public float SlideReleaseDown;
    public bool HasSlideLockFunctionality;
    public float SlideLockRot;
    private bool m_isSlideLockMechanismEngaged;
    [Header("Safety Params")]
    public bool HasSafety;
    public bool HasSafetyControl = true;
    public FVRPhysicalObject.InterpStyle Safety_Interp;
    public FVRPhysicalObject.Axis SafetyAxis;
    public float SafetyOff;
    public float SafetyOn;
    public bool DoesSafetyRequireSlideForward;
    public bool DoesSafetyLockSlide;
    public bool HasMagazineSafety;
    public bool DoesSafetyRequireCockedHammer;
    public bool DoesSafetyEngagingDecock;
    public bool DoesSafetyDisengageCockHammer;
    private bool m_isSafetyEngaged;
    [Header("Hammer Params")]
    public bool HasHammer;
    public bool HasHammerControl = true;
    public FVRPhysicalObject.InterpStyle Hammer_Interp;
    public FVRPhysicalObject.Axis HammerAxis;
    public float HammerForward;
    public float HammerRearward;
    private bool m_isHammerCocked;
    private float m_hammerDALerp;
    [Header("Barrel Params")]
    public bool HasTiltingBarrel;
    public FVRPhysicalObject.InterpStyle BarrelInterp;
    public FVRPhysicalObject.Axis BarrelAxis;
    public float BarrelUntilted;
    public float BarrelTilted;
    [Header("FireSelector Params")]
    public bool HasFireSelector;
    public FVRPhysicalObject.InterpStyle FireSelectorInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
    public FVRPhysicalObject.Axis FireSelectorAxis;
    public Handgun.FireSelectorMode[] FireSelectorModes;
    private int m_fireSelectorMode;
    private int m_CamBurst;
    [Header("Misc Control Vars")]
    public bool HasMagReleaseInput = true;
    [HideInInspector]
    public bool IsSlideLockPushedUp;
    [HideInInspector]
    public bool IsSlideLockHeldDown;
    [HideInInspector]
    public bool IsMagReleaseHeldDown;
    [HideInInspector]
    public bool IsSafetyOn;
    [HideInInspector]
    public bool HasTriggerReset = true;
    [HideInInspector]
    public bool IsSlideLockUp;
    [HideInInspector]
    public bool IsSlideLockExternalPushedUp;
    [HideInInspector]
    public bool IsSlideLockExternalHeldDown;
    public bool CanPhysicsSlideRack = true;
    private HashSet<Collider> m_slideCols = new HashSet<Collider>();
    private HashSet<Collider> m_unRackCols = new HashSet<Collider>();
    private Handgun.HeldTouchpadAction m_heldTouchpadAction;
    private Vector2 TouchpadClickInitiation = Vector2.zero;
    private float m_timeSinceFiredShot = 1f;

    public bool IsSLideLockMechanismEngaged => this.m_isSlideLockMechanismEngaged;

    public bool IsSafetyEngaged => this.m_isSafetyEngaged;

    public int FireSelectorModeIndex => this.m_fireSelectorMode;

    protected override void Awake()
    {
      base.Awake();
      this.m_CamBurst = 1;
      this.m_proxy = new GameObject("m_proxyRound").AddComponent<FVRFirearmMovingProxyRound>();
      this.m_proxy.Init(this.transform);
      if (this.CanPhysicsSlideRack)
        this.InitSlideCols();
      if (!((UnityEngine.Object) this.Chamber != (UnityEngine.Object) null))
        return;
      this.Chamber.Firearm = (FVRFireArm) this;
    }

    private void InitSlideCols()
    {
      foreach (Component componentsInChild in this.Slide.gameObject.GetComponentsInChildren<Transform>())
      {
        Collider component = componentsInChild.GetComponent<Collider>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null && !component.isTrigger)
          this.m_slideCols.Add(component);
      }
    }

    public void ResetCamBurst()
    {
      if (this.FireSelectorModes.Length <= 0)
        return;
      this.m_CamBurst = this.FireSelectorModes[this.m_fireSelectorMode].BurstAmount;
    }

    public override void OnCollisionEnter(Collision c)
    {
      if (this.IsHeld && this.CanPhysicsSlideRack && (c.contacts.Length > 0 && this.m_slideCols.Contains(c.contacts[0].thisCollider)) && (double) Vector3.Angle(this.transform.forward, c.relativeVelocity) > 135.0 && (double) c.relativeVelocity.magnitude > 3.0)
      {
        this.m_unRackCols.Add(c.collider);
        this.Slide.KnockToRear();
      }
      base.OnCollisionEnter(c);
    }

    public void OnCollisionExit(Collision c)
    {
      if (!this.IsHeld || !this.CanPhysicsSlideRack || !this.m_unRackCols.Contains(c.collider))
        return;
      if ((double) c.relativeVelocity.magnitude > 3.0)
        this.IsSlideLockExternalHeldDown = true;
      this.m_unRackCols.Clear();
    }

    public override int GetTutorialState()
    {
      if ((UnityEngine.Object) this.Magazine == (UnityEngine.Object) null)
        return 0;
      if (this.m_isSafetyEngaged)
        return 4;
      if (this.Chamber.IsFull || (double) this.Slide.GetSlideSpeed() > 0.0 || (double) this.m_timeSinceFiredShot <= 0.25)
        return 2;
      return this.Magazine.HasARound() ? 1 : 3;
    }

    public void EjectExtractedRound()
    {
      if (!this.Chamber.IsFull)
        return;
      this.Chamber.EjectRound(this.RoundPos_Ejection.position, this.transform.right * this.RoundEjectionSpeed.x + this.transform.up * this.RoundEjectionSpeed.y + this.transform.forward * this.RoundEjectionSpeed.z, this.transform.right * this.RoundEjectionSpin.x + this.transform.up * this.RoundEjectionSpin.y + this.transform.forward * this.RoundEjectionSpin.z);
    }

    public void ExtractRound()
    {
      if ((UnityEngine.Object) this.Magazine == (UnityEngine.Object) null || this.m_proxy.IsFull || !this.Magazine.HasARound())
        return;
      this.m_proxy.SetFromPrefabReference(this.Magazine.RemoveRound(false));
    }

    public bool ChamberRound()
    {
      if (!this.m_proxy.IsFull || this.Chamber.IsFull)
        return false;
      this.Chamber.SetRound(this.m_proxy.Round);
      this.m_proxy.ClearProxy();
      return true;
    }

    public bool CycleFireSelector()
    {
      if (this.FireSelectorModes.Length <= 1)
        return false;
      bool flag1 = true;
      bool flag2 = true;
      int fireSelectorMode1 = this.m_fireSelectorMode;
      ++this.m_fireSelectorMode;
      if (this.m_fireSelectorMode >= this.FireSelectorModes.Length)
        this.m_fireSelectorMode -= this.FireSelectorModes.Length;
      if (this.HasSafety)
      {
        if (this.FireSelectorModes[this.m_fireSelectorMode].ModeType == Handgun.FireSelectorModeType.Safe)
          flag2 = this.SetSafetyState(true);
        else
          this.SetSafetyState(false);
      }
      if (!flag2)
      {
        flag1 = false;
        this.m_fireSelectorMode = fireSelectorMode1;
      }
      if (flag1)
        this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
      if (this.FireSelectorModes.Length > 0)
      {
        Handgun.FireSelectorMode fireSelectorMode2 = this.FireSelectorModes[this.m_fireSelectorMode];
        if ((double) this.m_triggerFloat < 0.100000001490116)
          this.m_CamBurst = fireSelectorMode2.BurstAmount;
      }
      return true;
    }

    public bool SetFireSelectorByIndex(int i)
    {
      if (this.FireSelectorModes.Length <= 1)
        return false;
      this.m_fireSelectorMode = i;
      if (this.m_fireSelectorMode >= this.FireSelectorModes.Length)
        this.m_fireSelectorMode -= this.FireSelectorModes.Length;
      if (this.HasSafety)
      {
        if (this.FireSelectorModes[this.m_fireSelectorMode].ModeType == Handgun.FireSelectorModeType.Safe)
          this.SetSafetyState(true);
        else
          this.SetSafetyState(false);
      }
      return true;
    }

    public bool ToggleSafety()
    {
      if (!this.HasSafety || this.DoesSafetyRequireSlideForward && this.Slide.CurPos != HandgunSlide.SlidePos.Forward || this.Slide.CurPos != HandgunSlide.SlidePos.Forward && this.Slide.CurPos < HandgunSlide.SlidePos.Locked)
        return false;
      if (this.m_isSafetyEngaged)
      {
        this.PlayAudioEvent(FirearmAudioEventType.Safety);
        this.m_isSafetyEngaged = false;
        if (this.DoesSafetyDisengageCockHammer)
          this.CockHammer(true);
      }
      else
      {
        bool flag = true;
        if (this.DoesSafetyRequireCockedHammer && !this.m_isHammerCocked)
          flag = false;
        if (flag)
        {
          this.m_isSafetyEngaged = true;
          if (this.DoesSafetyEngagingDecock)
            this.DeCockHammer(true, true);
          this.PlayAudioEvent(FirearmAudioEventType.Safety);
        }
      }
      this.UpdateSafetyPos();
      return true;
    }

    public bool SetSafetyState(bool s)
    {
      if (!this.HasSafety || this.DoesSafetyRequireSlideForward && this.Slide.CurPos != HandgunSlide.SlidePos.Forward || this.Slide.CurPos != HandgunSlide.SlidePos.Forward && this.Slide.CurPos != HandgunSlide.SlidePos.Locked)
        return false;
      if (this.m_isSafetyEngaged && !s)
      {
        this.PlayAudioEvent(FirearmAudioEventType.Safety);
        this.m_isSafetyEngaged = false;
        if (this.DoesSafetyDisengageCockHammer)
          this.CockHammer(true);
        this.UpdateSafetyPos();
        return true;
      }
      if (this.m_isSafetyEngaged || !s)
        return false;
      bool flag = true;
      if (this.DoesSafetyRequireCockedHammer && !this.m_isHammerCocked)
        flag = false;
      if (flag)
      {
        this.m_isSafetyEngaged = true;
        if (this.DoesSafetyEngagingDecock)
          this.DeCockHammer(true, true);
        this.PlayAudioEvent(FirearmAudioEventType.Safety);
      }
      this.UpdateSafetyPos();
      return true;
    }

    private void UpdateSafetyPos()
    {
      if (!this.HasSafety)
        return;
      float val = this.SafetyOff;
      if (this.m_isSafetyEngaged)
        val = this.SafetyOn;
      this.SetAnimatedComponent(this.Safety, val, this.Safety_Interp, this.SafetyAxis);
    }

    public void ReleaseSeer()
    {
      this.HasTriggerReset = false;
      if (!this.m_isHammerCocked || !this.m_isSeerReady)
        return;
      if (this.FireSelectorModes[this.m_fireSelectorMode].ModeType == Handgun.FireSelectorModeType.Single || this.FireSelectorModes[this.m_fireSelectorMode].ModeType == Handgun.FireSelectorModeType.Burst && this.m_CamBurst < 1)
        this.m_isSeerReady = false;
      this.DropHammer(false);
    }

    public void CockHammer(bool isManual)
    {
      if (isManual && !this.m_isHammerCocked)
        this.PlayAudioEvent(FirearmAudioEventType.Prefire);
      this.m_isHammerCocked = true;
    }

    public void DeCockHammer(bool isManual, bool isLoud)
    {
      if (this.Slide.CurPos != HandgunSlide.SlidePos.Forward)
        return;
      if (isManual && this.m_isHammerCocked)
      {
        if (isLoud)
          this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
        else
          this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
      }
      this.m_isHammerCocked = false;
    }

    public void DropSlideRelease()
    {
      if (this.IsSlideLockUp)
        this.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
      this.IsSlideLockUp = false;
    }

    public void EngageSlideRelease() => this.IsSlideLockUp = true;

    private void EngageSlideLockMechanism()
    {
      if (this.m_isSlideLockMechanismEngaged)
        return;
      this.m_isSlideLockMechanismEngaged = true;
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
    }

    private void DisEngageSlideLockMechanism()
    {
      if (!this.m_isSlideLockMechanismEngaged)
        return;
      this.m_isSlideLockMechanismEngaged = false;
      this.PlayAudioEvent(FirearmAudioEventType.FireSelector);
    }

    public bool IsSlideCatchEngaged() => this.IsSlideLockUp;

    public void DropHammer(bool isManual)
    {
      if (this.Slide.CurPos != HandgunSlide.SlidePos.Forward || !this.m_isHammerCocked)
        return;
      this.m_isHammerCocked = false;
      if (this.m_CamBurst > 0)
        --this.m_CamBurst;
      this.Fire();
      this.PlayAudioEvent(FirearmAudioEventType.HammerHit);
    }

    public bool Fire()
    {
      if (!this.Chamber.Fire())
        return false;
      this.m_timeSinceFiredShot = 0.0f;
      this.Fire(this.Chamber, this.GetMuzzle(), true);
      this.FireMuzzleSmoke();
      bool twoHandStabilized = this.IsTwoHandStabilized();
      bool foregripStabilized = this.IsForegripStabilized();
      bool shoulderStabilized = this.IsShoulderStabilized();
      float globalLoudnessMultiplier = 1f;
      float VerticalRecoilMult = 1f;
      if (this.m_isSlideLockMechanismEngaged)
      {
        globalLoudnessMultiplier = 0.4f;
        VerticalRecoilMult = 1.5f;
      }
      this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, this.GetRecoilProfile(), VerticalRecoilMult);
      this.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment(), globalLoudnessMultiplier);
      if (!this.IsSLideLockMechanismEngaged)
        this.Slide.ImpartFiringImpulse();
      return true;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.UpdateComponents();
      this.Slide.UpdateSlide();
      this.UpdateDisplayRoundPositions();
      if ((double) this.m_timeSinceFiredShot >= 1.0)
        return;
      this.m_timeSinceFiredShot += Time.deltaTime;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.UpdateInputAndAnimate(hand);
    }

    private void UpdateInputAndAnimate(FVRViveHand hand)
    {
      this.IsSlideLockHeldDown = false;
      this.IsSlideLockPushedUp = false;
      this.IsMagReleaseHeldDown = false;
      if (this.IsAltHeld)
        return;
      this.m_triggerTarget = !this.m_hasTriggeredUpSinceBegin ? 0.0f : hand.Input.TriggerFloat;
      this.m_triggerFloat = (double) this.m_triggerTarget <= (double) this.m_triggerFloat ? Mathf.MoveTowards(this.m_triggerFloat, this.m_triggerTarget, (float) ((double) Time.deltaTime * (double) this.TriggerSpeed * 2.0)) : Mathf.MoveTowards(this.m_triggerFloat, this.m_triggerTarget, Time.deltaTime * this.TriggerSpeed);
      if (!this.HasTriggerReset && (double) this.m_triggerFloat <= (double) this.TriggerResetThreshold)
      {
        this.HasTriggerReset = true;
        this.m_isSeerReady = true;
        this.PlayAudioEvent(FirearmAudioEventType.TriggerReset);
        if (this.FireSelectorModes.Length > 0)
          this.m_CamBurst = this.FireSelectorModes[this.m_fireSelectorMode].BurstAmount;
      }
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.AXButtonPressed)
          this.IsMagReleaseHeldDown = true;
        if (hand.Input.BYButtonDown)
        {
          this.m_heldTouchpadAction = Handgun.HeldTouchpadAction.None;
          bool flag = true;
          if (this.HasSlideReleaseControl && this.IsSlideCatchEngaged())
          {
            this.DropSlideRelease();
            flag = false;
          }
          if (flag)
          {
            if (this.HasFireSelector)
              this.CycleFireSelector();
            else if (this.HasSafetyControl)
              this.ToggleSafety();
          }
        }
      }
      else
      {
        Vector2 touchpadAxes = hand.Input.TouchpadAxes;
        if (hand.Input.TouchpadDown)
        {
          this.TouchpadClickInitiation = touchpadAxes;
          if ((double) touchpadAxes.magnitude > 0.200000002980232)
          {
            if ((double) Vector2.Angle(touchpadAxes, Vector2.up) <= 45.0)
            {
              this.m_heldTouchpadAction = Handgun.HeldTouchpadAction.None;
              if (this.HasFireSelector)
                this.CycleFireSelector();
              else if (this.HasSafetyControl)
                this.ToggleSafety();
            }
            else if ((double) Vector2.Angle(touchpadAxes, Vector2.down) <= 45.0)
              this.m_heldTouchpadAction = Handgun.HeldTouchpadAction.MagRelease;
            else if ((double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0 && this.HasSlideRelease && this.HasSlideReleaseControl)
              this.m_heldTouchpadAction = Handgun.HeldTouchpadAction.SlideRelease;
            else if ((double) Vector2.Angle(touchpadAxes, Vector2.right) <= 45.0 && this.TriggerType != Handgun.TriggerStyle.DAO && this.HasHammerControl)
              this.m_heldTouchpadAction = Handgun.HeldTouchpadAction.Hammer;
          }
        }
        if (hand.Input.TouchpadPressed)
        {
          if (this.m_heldTouchpadAction == Handgun.HeldTouchpadAction.MagRelease)
          {
            if ((double) touchpadAxes.magnitude > 0.200000002980232 && (double) Vector2.Angle(touchpadAxes, Vector2.down) <= 45.0)
              this.IsMagReleaseHeldDown = true;
          }
          else if (this.m_heldTouchpadAction == Handgun.HeldTouchpadAction.SlideRelease)
          {
            if ((double) touchpadAxes.y >= (double) this.TouchpadClickInitiation.y + 0.0500000007450581)
              this.IsSlideLockPushedUp = true;
            else if ((double) touchpadAxes.y <= (double) this.TouchpadClickInitiation.y - 0.0500000007450581)
              this.IsSlideLockHeldDown = true;
          }
          else if (this.m_heldTouchpadAction == Handgun.HeldTouchpadAction.Hammer)
          {
            if ((double) touchpadAxes.y <= (double) this.TouchpadClickInitiation.y - 0.0500000007450581 && this.TriggerType != Handgun.TriggerStyle.DAO && (!this.m_isHammerCocked && this.Slide.CurPos == HandgunSlide.SlidePos.Forward))
              this.CockHammer(true);
            else if ((double) touchpadAxes.y >= (double) this.TouchpadClickInitiation.y + 0.0500000007450581 && this.TriggerType != Handgun.TriggerStyle.DAO && this.m_isHammerCocked && (((double) this.m_triggerFloat > 0.100000001490116 || this.HasManualDecocker) && this.Slide.CurPos == HandgunSlide.SlidePos.Forward))
              this.DeCockHammer(true, false);
          }
        }
        if (hand.Input.TouchpadUp)
          this.m_heldTouchpadAction = Handgun.HeldTouchpadAction.None;
      }
      if ((double) this.m_triggerFloat >= (double) this.TriggerBreakThreshold)
      {
        if (!this.m_isHammerCocked && (this.TriggerType == Handgun.TriggerStyle.SADA || this.TriggerType == Handgun.TriggerStyle.DAO) && (this.Slide.CurPos == HandgunSlide.SlidePos.Forward && (!this.HasMagazineSafety || (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null)))
        {
          if ((double) this.m_hammerDALerp >= 1.0 && this.m_isSeerReady)
            this.CockHammer(false);
        }
        else if (!this.m_isSafetyEngaged && (!this.HasMagazineSafety || (UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
          this.ReleaseSeer();
      }
      this.m_hammerDALerp = this.m_isHammerCocked || this.m_isSafetyEngaged || this.TriggerType != Handgun.TriggerStyle.SADA && this.TriggerType != Handgun.TriggerStyle.DAO || (this.Slide.CurPos != HandgunSlide.SlidePos.Forward || !this.m_isSeerReady) ? 0.0f : Mathf.InverseLerp(this.TriggerResetThreshold, this.TriggerBreakThreshold, this.m_triggerFloat);
      if (this.IsMagReleaseHeldDown && this.HasMagReleaseInput)
      {
        if ((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null)
          this.EjectMag();
        if (!((UnityEngine.Object) this.ReloadTriggerWell != (UnityEngine.Object) null))
          return;
        this.ReloadTriggerWell.SetActive(false);
      }
      else
      {
        if (!((UnityEngine.Object) this.ReloadTriggerWell != (UnityEngine.Object) null))
          return;
        this.ReloadTriggerWell.SetActive(true);
      }
    }

    public void ReleaseMag()
    {
      if (!((UnityEngine.Object) this.Magazine != (UnityEngine.Object) null))
        return;
      this.EjectMag();
    }

    private void UpdateComponents()
    {
      if (this.Slide.CurPos < HandgunSlide.SlidePos.Locked)
        this.IsSlideLockUp = false;
      else if (this.IsSlideLockPushedUp || this.IsSlideLockExternalPushedUp)
        this.EngageSlideRelease();
      else if ((this.IsSlideLockHeldDown || this.IsSlideLockExternalHeldDown) && (UnityEngine.Object) this.Slide.m_hand == (UnityEngine.Object) null)
      {
        this.DropSlideRelease();
        this.IsSlideLockExternalHeldDown = false;
      }
      this.IsSlideLockExternalHeldDown = false;
      if (this.HasSlideLockFunctionality && this.Slide.CurPos == HandgunSlide.SlidePos.Forward)
      {
        if (this.IsSlideLockHeldDown)
          this.EngageSlideLockMechanism();
        else if (this.IsSlideLockPushedUp)
          this.DisEngageSlideLockMechanism();
      }
      if (this.HasHammer)
      {
        float t = 0.0f;
        switch (this.TriggerType)
        {
          case Handgun.TriggerStyle.SA:
            t = !this.m_isHammerCocked ? 1f - this.Slide.GetSlideLerpBetweenLockAndFore() : 1f;
            break;
          case Handgun.TriggerStyle.SADA:
            t = !this.m_isHammerCocked ? (this.Slide.CurPos != HandgunSlide.SlidePos.Forward ? 1f - this.Slide.GetSlideLerpBetweenLockAndFore() : this.m_hammerDALerp) : 1f;
            break;
          case Handgun.TriggerStyle.DAO:
            t = this.Slide.CurPos != HandgunSlide.SlidePos.Forward || !this.m_isSeerReady ? 1f - this.Slide.GetSlideLerpBetweenLockAndFore() : this.m_hammerDALerp;
            break;
        }
        this.SetAnimatedComponent(this.Hammer, Mathf.Lerp(this.HammerForward, this.HammerRearward, t), this.Hammer_Interp, this.HammerAxis);
      }
      if (this.HasTiltingBarrel)
        this.SetAnimatedComponent(this.Barrel, Mathf.Lerp(this.BarrelUntilted, this.BarrelTilted, (1f - this.Slide.GetSlideLerpBetweenLockAndFore()) * 4f), this.BarrelInterp, this.BarrelAxis);
      if (this.HasSlideRelease)
      {
        float t = 0.0f;
        if (this.IsSlideLockUp)
          t = 1f;
        float a = this.SlideReleaseDown;
        float b = this.SlideReleaseUp;
        if (this.m_isSlideLockMechanismEngaged)
        {
          a = this.SlideLockRot;
          b = this.SlideLockRot;
        }
        switch (this.SlideReleaseInterp)
        {
          case FVRPhysicalObject.InterpStyle.Translate:
            Vector3 localPosition = this.SlideRelease.localPosition;
            switch (this.SlideReleaseAxis)
            {
              case FVRPhysicalObject.Axis.X:
                localPosition.x = Mathf.Lerp(a, b, t);
                break;
              case FVRPhysicalObject.Axis.Y:
                localPosition.y = Mathf.Lerp(a, b, t);
                break;
              case FVRPhysicalObject.Axis.Z:
                localPosition.z = Mathf.Lerp(a, b, t);
                break;
            }
            this.SlideRelease.localPosition = localPosition;
            break;
          case FVRPhysicalObject.InterpStyle.Rotation:
            Vector3 localEulerAngles = this.SlideRelease.localEulerAngles;
            switch (this.SlideReleaseAxis)
            {
              case FVRPhysicalObject.Axis.X:
                localEulerAngles.x = Mathf.Lerp(a, b, t);
                break;
              case FVRPhysicalObject.Axis.Y:
                localEulerAngles.y = Mathf.Lerp(a, b, t);
                break;
              case FVRPhysicalObject.Axis.Z:
                localEulerAngles.z = Mathf.Lerp(a, b, t);
                break;
            }
            this.SlideRelease.localEulerAngles = localEulerAngles;
            break;
        }
      }
      if (this.HasTrigger)
      {
        switch (this.TriggerInterp)
        {
          case FVRPhysicalObject.InterpStyle.Translate:
            Vector3 localPosition1 = this.Trigger.localPosition;
            switch (this.TriggerAxis)
            {
              case FVRPhysicalObject.Axis.X:
                localPosition1.x = Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat);
                break;
              case FVRPhysicalObject.Axis.Y:
                localPosition1.y = Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat);
                break;
              case FVRPhysicalObject.Axis.Z:
                localPosition1.z = Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat);
                break;
            }
            this.Trigger.localPosition = localPosition1;
            break;
          case FVRPhysicalObject.InterpStyle.Rotation:
            Vector3 localEulerAngles1 = this.Trigger.localEulerAngles;
            switch (this.TriggerAxis)
            {
              case FVRPhysicalObject.Axis.X:
                localEulerAngles1.x = Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat);
                break;
              case FVRPhysicalObject.Axis.Y:
                localEulerAngles1.y = Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat);
                break;
              case FVRPhysicalObject.Axis.Z:
                localEulerAngles1.z = Mathf.Lerp(this.TriggerUnheld, this.TriggerHeld, this.m_triggerFloat);
                break;
            }
            this.Trigger.localEulerAngles = localEulerAngles1;
            break;
        }
      }
      if (!this.HasFireSelector)
        return;
      switch (this.FireSelectorInterpStyle)
      {
        case FVRPhysicalObject.InterpStyle.Translate:
          Vector3 zero1 = Vector3.zero;
          switch (this.FireSelectorAxis)
          {
            case FVRPhysicalObject.Axis.X:
              zero1.x = this.FireSelectorModes[this.m_fireSelectorMode].SelectorPosition;
              break;
            case FVRPhysicalObject.Axis.Y:
              zero1.y = this.FireSelectorModes[this.m_fireSelectorMode].SelectorPosition;
              break;
            case FVRPhysicalObject.Axis.Z:
              zero1.z = this.FireSelectorModes[this.m_fireSelectorMode].SelectorPosition;
              break;
          }
          this.FireSelector.localPosition = zero1;
          break;
        case FVRPhysicalObject.InterpStyle.Rotation:
          Vector3 zero2 = Vector3.zero;
          switch (this.FireSelectorAxis)
          {
            case FVRPhysicalObject.Axis.X:
              zero2.x = this.FireSelectorModes[this.m_fireSelectorMode].SelectorPosition;
              break;
            case FVRPhysicalObject.Axis.Y:
              zero2.y = this.FireSelectorModes[this.m_fireSelectorMode].SelectorPosition;
              break;
            case FVRPhysicalObject.Axis.Z:
              zero2.z = this.FireSelectorModes[this.m_fireSelectorMode].SelectorPosition;
              break;
          }
          this.FireSelector.localEulerAngles = zero2;
          break;
      }
    }

    private void UpdateDisplayRoundPositions()
    {
      float betweenLockAndFore = this.Slide.GetSlideLerpBetweenLockAndFore();
      if (this.m_proxy.IsFull)
      {
        this.m_proxy.ProxyRound.position = Vector3.Lerp(this.RoundPos_Magazine.position, this.Chamber.transform.position, betweenLockAndFore);
        this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_Magazine.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
      }
      else if (this.Chamber.IsFull)
      {
        this.Chamber.ProxyRound.position = Vector3.Lerp(this.RoundPos_Ejecting.position, this.Chamber.transform.position, betweenLockAndFore);
        this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_Ejecting.rotation, this.Chamber.transform.rotation, betweenLockAndFore);
      }
      if (this.Slide.CurPos == HandgunSlide.SlidePos.Forward)
        this.Chamber.IsAccessible = false;
      else
        this.Chamber.IsAccessible = true;
    }

    public override List<FireArmRoundClass> GetChamberRoundList()
    {
      if (!this.Chamber.IsFull || this.Chamber.IsSpent)
        return (List<FireArmRoundClass>) null;
      return new List<FireArmRoundClass>()
      {
        this.Chamber.GetRound().RoundClass
      };
    }

    public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
    {
      if (rounds.Count <= 0)
        return;
      this.Chamber.Autochamber(rounds[0]);
    }

    public override List<string> GetFlagList() => (List<string>) null;

    public override void SetFromFlagList(List<string> flags)
    {
    }

    public override void ConfigureFromFlagDic(Dictionary<string, string> f)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string key1 = "HammerState";
      if (f.ContainsKey(key1) && f[key1] == "Cocked")
        this.CockHammer(false);
      if (this.HasSafety)
      {
        string key2 = "SafetyState";
        if (f.ContainsKey(key2) && f[key2] == "On")
          this.SetSafetyState(true);
      }
      if (this.HasFireSelector)
      {
        string key2 = "FireSelectorState";
        if (f.ContainsKey(key2))
        {
          string s = f[key2];
          int result = 0;
          int.TryParse(s, out result);
          this.SetFireSelectorByIndex(result);
        }
      }
      this.UpdateComponents();
      this.UpdateSafetyPos();
    }

    public override Dictionary<string, string> GetFlagDic()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      string key1 = "HammerState";
      string str1 = "Uncocked";
      if (this.m_isHammerCocked)
        str1 = "Cocked";
      dictionary.Add(key1, str1);
      if (this.HasSafety)
      {
        string key2 = "SafetyState";
        string str2 = "Off";
        if (this.m_isSafetyEngaged)
          str2 = "On";
        dictionary.Add(key2, str2);
      }
      if (this.HasFireSelector)
      {
        string key2 = "FireSelectorState";
        string str2 = this.m_fireSelectorMode.ToString();
        dictionary.Add(key2, str2);
      }
      return dictionary;
    }

    public enum FireSelectorModeType
    {
      Single = 1,
      FullAuto = 2,
      Safe = 3,
      Burst = 4,
    }

    [Serializable]
    public class FireSelectorMode
    {
      public float SelectorPosition;
      public Handgun.FireSelectorModeType ModeType;
      public int BurstAmount = 3;
    }

    public enum TriggerStyle
    {
      SA,
      SADA,
      DAO,
    }

    public enum HeldTouchpadAction
    {
      None,
      SlideRelease,
      Hammer,
      MagRelease,
    }
  }
}
