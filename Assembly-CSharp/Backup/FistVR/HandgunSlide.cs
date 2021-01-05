// Decompiled with JetBrains decompiler
// Type: FistVR.HandgunSlide
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HandgunSlide : FVRInteractiveObject
  {
    public Handgun Handgun;
    public float Speed_Forward;
    public float Speed_Rearward;
    public float Speed_Held;
    public float SpringStiffness = 5f;
    public HandgunSlide.SlidePos CurPos;
    public HandgunSlide.SlidePos LastPos;
    public Transform Point_Slide_Forward;
    public Transform Point_Slide_LockPoint;
    public Transform Point_Slide_Rear;
    public bool HasLastRoundSlideHoldOpen = true;
    private float m_curSlideSpeed;
    private float m_slideZ_current;
    private float m_slideZ_heldTarget;
    private float m_slideZ_forward;
    private float m_slideZ_lock;
    private float m_slideZ_rear;
    private float m_handZOffset;

    protected override void Awake()
    {
      base.Awake();
      this.m_slideZ_current = this.transform.localPosition.z;
      this.m_slideZ_forward = this.Point_Slide_Forward.localPosition.z;
      this.m_slideZ_lock = this.Point_Slide_LockPoint.localPosition.z;
      this.m_slideZ_rear = this.Point_Slide_Rear.localPosition.z;
    }

    public override bool IsInteractable() => !this.Handgun.IsSLideLockMechanismEngaged && base.IsInteractable();

    public float GetSlideSpeed() => this.m_curSlideSpeed;

    public float GetSlideLerpBetweenLockAndFore() => Mathf.InverseLerp(this.m_slideZ_lock, this.m_slideZ_forward, this.m_slideZ_current);

    public float GetSlideLerpBetweenRearAndFore() => Mathf.InverseLerp(this.m_slideZ_rear, this.m_slideZ_forward, this.m_slideZ_current);

    public void ImpartFiringImpulse() => this.m_curSlideSpeed = this.Speed_Rearward;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if ((Object) this.Handgun.Clip != (Object) null)
        this.Handgun.EjectClip();
      this.m_handZOffset = this.transform.InverseTransformPoint(hand.Input.Pos).z;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if (this.CurPos >= HandgunSlide.SlidePos.Locked && !this.Handgun.IsSlideLockUp)
        this.Handgun.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
      base.EndInteraction(hand);
    }

    public void KnockToRear() => this.ImpartFiringImpulse();

    public void UpdateSlide()
    {
      bool flag = false;
      if (this.IsHeld)
        flag = true;
      if (this.Handgun.DoesSafetyLockSlide && this.Handgun.IsSafetyEngaged || this.Handgun.IsSLideLockMechanismEngaged)
        return;
      if (this.IsHeld)
        this.m_slideZ_heldTarget = this.Handgun.transform.InverseTransformPoint(this.GetClosestValidPoint(this.Point_Slide_Forward.position, this.Point_Slide_Rear.position, this.m_hand.Input.Pos + -this.transform.forward * this.m_handZOffset * this.Handgun.transform.localScale.x)).z;
      Vector2 vector2 = new Vector2(this.m_slideZ_rear, this.m_slideZ_forward);
      if ((double) this.m_slideZ_current <= (double) this.m_slideZ_lock && this.Handgun.IsSlideCatchEngaged())
        vector2 = new Vector2(this.m_slideZ_rear, this.m_slideZ_lock);
      if ((Object) this.Handgun.Clip != (Object) null)
        vector2 = new Vector2(this.m_slideZ_rear, this.m_slideZ_lock);
      if (flag)
        this.m_curSlideSpeed = 0.0f;
      else if (this.CurPos < HandgunSlide.SlidePos.LockedToRear && (double) this.m_curSlideSpeed >= 0.0 || this.LastPos >= HandgunSlide.SlidePos.Rear)
        this.m_curSlideSpeed = Mathf.MoveTowards(this.m_curSlideSpeed, this.Speed_Forward, Time.deltaTime * this.SpringStiffness);
      float slideZCurrent1 = this.m_slideZ_current;
      float slideZCurrent2 = this.m_slideZ_current;
      float num = Mathf.Clamp(!flag ? this.m_slideZ_current + this.m_curSlideSpeed * Time.deltaTime : Mathf.MoveTowards(this.m_slideZ_current, this.m_slideZ_heldTarget, this.Speed_Held * Time.deltaTime), vector2.x, vector2.y);
      if ((double) Mathf.Abs(num - this.m_slideZ_current) > (double) Mathf.Epsilon)
      {
        this.m_slideZ_current = num;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_slideZ_current);
      }
      else
        this.m_curSlideSpeed = 0.0f;
      HandgunSlide.SlidePos curPos1 = this.CurPos;
      HandgunSlide.SlidePos slidePos = (double) Mathf.Abs(this.m_slideZ_current - this.m_slideZ_forward) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_slideZ_current - this.m_slideZ_lock) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_slideZ_current - this.m_slideZ_rear) >= 1.0 / 1000.0 ? ((double) this.m_slideZ_current <= (double) this.m_slideZ_lock ? HandgunSlide.SlidePos.LockedToRear : HandgunSlide.SlidePos.ForwardToMid) : HandgunSlide.SlidePos.Rear) : HandgunSlide.SlidePos.Locked) : HandgunSlide.SlidePos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = (HandgunSlide.SlidePos) Mathf.Clamp((int) slidePos, curPos2 - 1, curPos2 + 1);
      if (this.CurPos == HandgunSlide.SlidePos.Forward && this.LastPos != HandgunSlide.SlidePos.Forward)
        this.SlideEvent_ArriveAtFore();
      else if (this.CurPos != HandgunSlide.SlidePos.ForwardToMid || this.LastPos != HandgunSlide.SlidePos.Forward)
      {
        if (this.CurPos == HandgunSlide.SlidePos.Locked && this.LastPos == HandgunSlide.SlidePos.ForwardToMid)
          this.SlideEvent_EjectRound();
        else if (this.CurPos == HandgunSlide.SlidePos.ForwardToMid && this.LastPos == HandgunSlide.SlidePos.Locked)
          this.SlideEvent_ExtractRoundFromMag();
        else if (this.CurPos == HandgunSlide.SlidePos.Locked && this.LastPos == HandgunSlide.SlidePos.LockedToRear)
          this.SlideEvent_SlideCaught();
        else if (this.CurPos == HandgunSlide.SlidePos.Rear && this.LastPos != HandgunSlide.SlidePos.Rear)
          this.SlideEvent_SmackRear();
      }
      if (this.CurPos >= HandgunSlide.SlidePos.Locked && this.HasLastRoundSlideHoldOpen && ((Object) this.Handgun.Magazine != (Object) null && !this.Handgun.Magazine.HasARound()) && !this.Handgun.IsSlideCatchEngaged())
        this.Handgun.EngageSlideRelease();
      this.LastPos = this.CurPos;
    }

    private void SlideEvent_ArriveAtFore()
    {
      this.Handgun.ChamberRound();
      if (this.IsHeld)
        this.Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
      else
        this.Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
    }

    private void SlideEvent_EjectRound()
    {
      this.Handgun.EjectExtractedRound();
      if (this.Handgun.TriggerType == Handgun.TriggerStyle.DAO)
        return;
      this.Handgun.CockHammer(false);
    }

    private void SlideEvent_ExtractRoundFromMag() => this.Handgun.ExtractRound();

    private void SlideEvent_SmackRear()
    {
      this.Handgun.DropSlideRelease();
      if (this.IsHeld)
        this.Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
      else
        this.Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
    }

    private void SlideEvent_SlideCaught()
    {
      if (!this.Handgun.IsSlideCatchEngaged())
        return;
      this.Handgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackLocked);
    }

    public enum SlidePos
    {
      Forward,
      ForwardToMid,
      Locked,
      LockedToRear,
      Rear,
    }
  }
}
