// Decompiled with JetBrains decompiler
// Type: FistVR.TubeFedShotgunBolt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TubeFedShotgunBolt : FVRInteractiveObject
  {
    [Header("Shotgun Bolt")]
    public TubeFedShotgun Shotgun;
    public float Speed_Forward;
    public float Speed_Rearward;
    public float Speed_Held;
    public float SpringStiffness = 5f;
    public TubeFedShotgunBolt.BoltPos CurPos;
    public TubeFedShotgunBolt.BoltPos LastPos;
    public Transform Point_Bolt_Forward;
    public Transform Point_Bolt_LockPoint;
    public Transform Point_Bolt_Rear;
    public bool HasLastRoundBoltHoldOpen = true;
    private float m_curBoltSpeed;
    private float m_boltZ_current;
    private float m_boltZ_heldTarget;
    private float m_boltZ_forward;
    private float m_boltZ_lock;
    private float m_boltZ_rear;
    private bool m_isBoltLocked;
    private bool m_isHandleHeld;
    private float m_HandleLerp;
    [Header("Reciprocating Barrel")]
    public bool HasReciprocatingBarrel;
    public Transform Barrel;
    public Vector3 BarrelForward;
    public Vector3 BarrelRearward;
    private bool m_isBarrelReciprocating;
    [Header("Elevator")]
    public bool HasElevator;
    public Transform Elevator;
    public Vector3 ElevatorForward;
    public Vector3 ElevatorRearward;
    [Header("Hammer")]
    public bool HasHammer;
    public Transform Hammer;
    public Vector3 HammerForward;
    public Vector3 HammerRearward;

    protected override void Awake()
    {
      base.Awake();
      this.m_boltZ_current = this.transform.localPosition.z;
      this.m_boltZ_forward = this.Point_Bolt_Forward.localPosition.z;
      this.m_boltZ_lock = this.Point_Bolt_LockPoint.localPosition.z;
      this.m_boltZ_rear = this.Point_Bolt_Rear.localPosition.z;
    }

    public override bool IsInteractable() => this.Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic && base.IsInteractable();

    public float GetBoltLerpBetweenLockAndFore() => Mathf.InverseLerp(this.m_boltZ_lock, this.m_boltZ_forward, this.m_boltZ_current);

    public float GetBoltLerpBetweenRearAndFore() => Mathf.InverseLerp(this.m_boltZ_rear, this.m_boltZ_forward, this.m_boltZ_current);

    public void LockBolt()
    {
      if (this.m_isBoltLocked)
        return;
      this.m_isBoltLocked = true;
    }

    public void ReleaseBolt()
    {
      if (!this.m_isBoltLocked)
        return;
      this.Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
      this.m_isBoltLocked = false;
    }

    public void UpdateHandleHeldState(bool state, float lerp)
    {
      this.m_isHandleHeld = state;
      this.m_HandleLerp = lerp;
    }

    public void ImpartFiringImpulse()
    {
      this.m_curBoltSpeed = this.Speed_Rearward;
      if (this.CurPos != TubeFedShotgunBolt.BoltPos.Forward)
        return;
      this.m_isBarrelReciprocating = true;
    }

    public void UpdateBolt()
    {
      bool flag = false;
      if (this.IsHeld || this.m_isHandleHeld)
        flag = true;
      if (this.IsHeld)
        this.m_boltZ_heldTarget = this.Shotgun.transform.InverseTransformPoint(this.GetClosestValidPoint(this.Point_Bolt_Forward.position, this.Point_Bolt_Rear.position, this.m_hand.Input.Pos)).z;
      else if (this.m_isHandleHeld)
        this.m_boltZ_heldTarget = Mathf.Lerp(this.m_boltZ_forward, this.m_boltZ_rear, this.m_HandleLerp);
      Vector2 vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_forward);
      if ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock && this.m_isBoltLocked)
        vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_lock);
      if (flag)
        this.m_curBoltSpeed = 0.0f;
      else if (this.Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic && (this.CurPos < TubeFedShotgunBolt.BoltPos.LockedToRear && (double) this.m_curBoltSpeed >= 0.0 || this.LastPos >= TubeFedShotgunBolt.BoltPos.Rear))
        this.m_curBoltSpeed = Mathf.MoveTowards(this.m_curBoltSpeed, this.Speed_Forward, Time.deltaTime * this.SpringStiffness);
      float num1 = this.m_boltZ_current;
      float boltZCurrent = this.m_boltZ_current;
      if (flag)
        num1 = Mathf.MoveTowards(this.m_boltZ_current, this.m_boltZ_heldTarget, this.Speed_Held * Time.deltaTime);
      else if (this.Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic)
        num1 = this.m_boltZ_current + this.m_curBoltSpeed * Time.deltaTime;
      float num2 = Mathf.Clamp(num1, vector2.x, vector2.y);
      if ((double) Mathf.Abs(num2 - this.m_boltZ_current) > (double) Mathf.Epsilon)
      {
        this.m_boltZ_current = num2;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_boltZ_current);
        if (this.HasElevator)
          this.Elevator.localEulerAngles = Vector3.Lerp(this.ElevatorRearward, this.ElevatorForward, this.GetBoltLerpBetweenLockAndFore());
      }
      else
        this.m_curBoltSpeed = 0.0f;
      if (this.HasHammer)
        this.Hammer.localEulerAngles = !this.Shotgun.IsHammerCocked ? Vector3.Lerp(this.HammerRearward, this.HammerForward, this.GetBoltLerpBetweenLockAndFore()) : this.HammerRearward;
      if (this.HasReciprocatingBarrel && this.m_isBarrelReciprocating)
      {
        float t = 0.0f;
        if (!this.m_isBoltLocked && !this.IsHeld)
          t = 1f - this.GetBoltLerpBetweenLockAndFore();
        this.Barrel.localPosition = Vector3.Lerp(this.BarrelForward, this.BarrelRearward, t);
      }
      TubeFedShotgunBolt.BoltPos curPos1 = this.CurPos;
      TubeFedShotgunBolt.BoltPos boltPos = (double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_forward) >= 0.00150000001303852 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_lock) >= 0.00150000001303852 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_rear) >= 0.00150000001303852 ? ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock ? TubeFedShotgunBolt.BoltPos.LockedToRear : TubeFedShotgunBolt.BoltPos.ForwardToMid) : TubeFedShotgunBolt.BoltPos.Rear) : TubeFedShotgunBolt.BoltPos.Locked) : TubeFedShotgunBolt.BoltPos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = (TubeFedShotgunBolt.BoltPos) Mathf.Clamp((int) boltPos, curPos2 - 1, curPos2 + 1);
      this.Shotgun.Chamber.IsAccessible = this.CurPos >= TubeFedShotgunBolt.BoltPos.Locked;
      if (this.CurPos >= TubeFedShotgunBolt.BoltPos.ForwardToMid)
        this.Shotgun.IsBreachOpenForGasOut = true;
      else
        this.Shotgun.IsBreachOpenForGasOut = false;
      if (this.CurPos == TubeFedShotgunBolt.BoltPos.Forward && this.LastPos != TubeFedShotgunBolt.BoltPos.Forward)
        this.BoltEvent_ArriveAtFore();
      else if (this.CurPos == TubeFedShotgunBolt.BoltPos.ForwardToMid && this.LastPos == TubeFedShotgunBolt.BoltPos.Forward)
        this.BoltEvent_ExtractRoundFromMag();
      else if (this.CurPos == TubeFedShotgunBolt.BoltPos.Locked && this.LastPos == TubeFedShotgunBolt.BoltPos.ForwardToMid)
        this.BoltEvent_EjectRound();
      else if (this.CurPos != TubeFedShotgunBolt.BoltPos.ForwardToMid || this.LastPos != TubeFedShotgunBolt.BoltPos.Locked)
      {
        if (this.CurPos == TubeFedShotgunBolt.BoltPos.Locked && this.LastPos == TubeFedShotgunBolt.BoltPos.LockedToRear)
          this.BoltEvent_BoltCaught();
        else if (this.CurPos == TubeFedShotgunBolt.BoltPos.Rear && this.LastPos != TubeFedShotgunBolt.BoltPos.Rear)
          this.BoltEvent_SmackRear();
      }
      if (this.CurPos >= TubeFedShotgunBolt.BoltPos.Locked && this.Shotgun.Mode == TubeFedShotgun.ShotgunMode.Automatic && (this.HasLastRoundBoltHoldOpen && (Object) this.Shotgun.Magazine != (Object) null) && (!this.Shotgun.HasExtractedRound() && !this.Shotgun.Magazine.HasARound() && (!this.Shotgun.Chamber.IsFull && !this.Shotgun.IsSlideReleaseButtonHeld)))
        this.LockBolt();
      this.LastPos = this.CurPos;
    }

    private void BoltEvent_ArriveAtFore()
    {
      this.Shotgun.ChamberRound();
      this.Shotgun.ReturnCarrierRoundToMagazineIfRelevant();
      if (this.HasReciprocatingBarrel && this.m_isBarrelReciprocating)
      {
        this.m_isBarrelReciprocating = false;
        this.Barrel.localPosition = this.BarrelForward;
      }
      if (this.IsHeld)
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
      else
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
    }

    private void BoltEvent_EjectRound()
    {
      this.Shotgun.EjectExtractedRound();
      this.Shotgun.TransferShellToUpperTrack();
      this.Shotgun.CockHammer();
    }

    private void BoltEvent_ExtractRoundFromMag() => this.Shotgun.ExtractRound();

    private void BoltEvent_SmackRear()
    {
      if (this.IsHeld || this.m_isHandleHeld)
        this.ReleaseBolt();
      if (this.IsHeld)
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
      else
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
    }

    private void BoltEvent_BoltCaught()
    {
      if (!this.m_isBoltLocked)
        return;
      if (this.HasReciprocatingBarrel && this.m_isBarrelReciprocating)
        this.m_isBarrelReciprocating = false;
      this.Shotgun.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackLocked);
    }

    public enum BoltPos
    {
      Forward,
      ForwardToMid,
      Locked,
      LockedToRear,
      Rear,
    }
  }
}
