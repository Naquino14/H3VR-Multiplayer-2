// Decompiled with JetBrains decompiler
// Type: FistVR.FVRAlternateGrip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRAlternateGrip : FVRInteractiveObject
  {
    [Header("Alternate Grip Config")]
    public FVRPhysicalObject PrimaryObject;
    public bool FunctionalityEnabled = true;
    private Vector3 m_origPoseOverridePos = Vector3.zero;
    private bool m_wasGrabbedFromAttachableForegrip;
    private AttachableForegrip m_lastGrabbedInGrip;
    private bool m_hasSavedPalmPoint;
    private Vector3 m_savedRigPalmPos = Vector3.zero;
    public bool DoesBracing = true;
    private bool tempFlag;

    public AttachableForegrip LastGrabbedInGrip => this.m_lastGrabbedInGrip;

    protected override void Awake()
    {
      base.Awake();
      this.m_origPoseOverridePos = this.PoseOverride.localPosition;
    }

    private void SetSavedRigLocalPos()
    {
      this.m_savedRigPalmPos = this.m_hand.PalmTransform.position;
      this.m_hasSavedPalmPoint = true;
      this.PrimaryObject.UseFilteredHandPosition = true;
      this.PrimaryObject.UseFilteredHandRotation = true;
    }

    private void ClearSavedPalmPoint()
    {
      if (!this.m_hasSavedPalmPoint)
        return;
      this.PrimaryObject.UseFilteredHandPosition = false;
      this.PrimaryObject.UseFilteredHandRotation = false;
      this.m_hasSavedPalmPoint = false;
      if (!((Object) this.m_hand != (Object) null))
        return;
      this.m_hand.Buzz(this.m_hand.Buzzer.Buzz_OnHoverInteractive);
    }

    private void AttemptToGenerateSavedPalmPoint()
    {
      if (!Physics.CheckSphere(this.m_hand.PalmTransform.position, 0.1f, (int) this.m_hand.BracingMask, QueryTriggerInteraction.Ignore))
        return;
      this.SetSavedRigLocalPos();
    }

    public Vector3 GetPalmPos(bool isDirectParent)
    {
      if (this.m_hasSavedPalmPoint)
        return this.m_savedRigPalmPos;
      return isDirectParent ? this.m_hand.PalmTransform.position : Vector3.Lerp(this.m_hand.Input.LastPalmPos1, this.m_hand.PalmTransform.position, 0.0f);
    }

    public bool HasLastGrabbedGrip() => this.tempFlag;

    public AttachableForegrip GetLastGrabbedGrip() => this.m_lastGrabbedInGrip;

    public override bool IsInteractable() => true;

    public override void PlayGrabSound(bool isHard, FVRViveHand hand)
    {
      if (this.PrimaryObject.IsHeld)
        isHard = false;
      if (this.HandlingGrabSound != HandlingGrabType.None)
      {
        if (!hand.CanMakeGrabReleaseSound)
          return;
        SM.PlayHandlingGrabSound(this.HandlingGrabSound, hand.Input.Pos, isHard);
        hand.HandMadeGrabReleaseSound();
      }
      else
      {
        if (!hand.CanMakeGrabReleaseSound)
          return;
        SM.PlayHandlingGrabSound(this.PrimaryObject.HandlingGrabSound, hand.Input.Pos, isHard);
        hand.HandMadeGrabReleaseSound();
      }
    }

    public void BeginInteractionFromAttachedGrip(AttachableForegrip aGrip, FVRViveHand hand)
    {
      if ((Object) aGrip == (Object) null)
      {
        this.m_wasGrabbedFromAttachableForegrip = false;
        this.tempFlag = false;
        this.m_lastGrabbedInGrip = (AttachableForegrip) null;
        this.BeginInteraction(hand);
      }
      else
      {
        this.PoseOverride.position = aGrip.ForePosePoint.position;
        this.m_wasGrabbedFromAttachableForegrip = true;
        this.BeginInteraction(hand);
        this.m_lastGrabbedInGrip = aGrip;
        this.tempFlag = true;
      }
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      this.PlayGrabSound(!this.IsHeld, hand);
      if (this.PrimaryObject is FVRFireArm)
      {
        FVRFireArm primaryObject = this.PrimaryObject as FVRFireArm;
        if ((Object) primaryObject.Bipod != (Object) null && primaryObject.Bipod.IsBipodActive)
          primaryObject.Bipod.Deactivate();
      }
      if (!this.m_wasGrabbedFromAttachableForegrip)
      {
        this.PoseOverride.localPosition = this.m_origPoseOverridePos;
        if ((Object) this.GrabPointTransform != (Object) null)
          this.GrabPointTransform.localPosition = this.m_origPoseOverridePos;
        this.m_lastGrabbedInGrip = (AttachableForegrip) null;
      }
      if (!this.PrimaryObject.IsHeld || this.PrimaryObject.IsAltHeld)
      {
        this.PrimaryObject.BeginInteractionThroughAltGrip(hand, this);
      }
      else
      {
        base.BeginInteraction(hand);
        if (!this.FunctionalityEnabled)
          return;
        this.PrimaryObject.AltGrip = this;
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      bool flag1 = true;
      if (!this.DoesBracing)
        flag1 = false;
      if (this.m_wasGrabbedFromAttachableForegrip && !this.m_lastGrabbedInGrip.DoesBracing)
        flag1 = false;
      if (this.PrimaryObject.IsAltHeld)
        flag1 = false;
      if (flag1 && hand.Input.TriggerPressed)
      {
        if (!this.m_hasSavedPalmPoint)
        {
          this.AttemptToGenerateSavedPalmPoint();
          if (this.m_hasSavedPalmPoint)
            hand.Buzz(hand.Buzzer.Buzz_BeginInteraction);
        }
        else if ((double) Vector3.Distance(this.m_savedRigPalmPos, this.m_hand.PalmTransform.position) > 0.200000002980232)
          this.ClearSavedPalmPoint();
      }
      else if (this.m_hasSavedPalmPoint)
        this.ClearSavedPalmPoint();
      if (hand.Input.TriggerUp)
        this.ClearSavedPalmPoint();
      if (this.m_wasGrabbedFromAttachableForegrip && (Object) this.m_lastGrabbedInGrip != (Object) null && ((Object) this.m_lastGrabbedInGrip.Attachment != (Object) null && (Object) this.m_lastGrabbedInGrip.Attachment.curMount != (Object) null) && (!this.m_lastGrabbedInGrip.HasAttachmentsOnIt() && this.m_lastGrabbedInGrip.Attachment.CanDetach()))
      {
        bool flag2 = false;
        if (hand.IsInStreamlinedMode)
        {
          if (hand.Input.AXButtonDown)
            flag2 = true;
        }
        else if (hand.Input.TouchpadDown && (double) Vector2.Angle(Vector2.down, touchpadAxes) < 45.0 && (double) touchpadAxes.magnitude > 0.200000002980232)
          flag2 = true;
        if (flag2)
        {
          this.EndInteraction(hand);
          this.m_lastGrabbedInGrip.DetachRoutine(hand);
        }
      }
      this.PassHandInput(hand, (FVRInteractiveObject) this);
    }

    public virtual void PassHandInput(FVRViveHand hand, FVRInteractiveObject o)
    {
      if (!((Object) this.m_lastGrabbedInGrip != (Object) null))
        return;
      this.m_lastGrabbedInGrip.PassHandInput(hand, o);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.ClearSavedPalmPoint();
      this.PoseOverride.localPosition = this.m_origPoseOverridePos;
      this.m_wasGrabbedFromAttachableForegrip = false;
      if (!this.FunctionalityEnabled || !((Object) this.PrimaryObject.AltGrip == (Object) this))
        return;
      this.PrimaryObject.AltGrip = (FVRAlternateGrip) null;
    }

    public override void TestHandDistance()
    {
    }
  }
}
