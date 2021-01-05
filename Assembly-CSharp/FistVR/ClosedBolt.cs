// Decompiled with JetBrains decompiler
// Type: FistVR.ClosedBolt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ClosedBolt : FVRInteractiveObject
  {
    [Header("Bolt")]
    public ClosedBoltWeapon Weapon;
    public float Speed_Forward;
    public float Speed_Rearward;
    public float Speed_Held;
    public float SpringStiffness = 5f;
    public ClosedBolt.BoltPos CurPos;
    public ClosedBolt.BoltPos LastPos;
    public Transform Point_Bolt_Forward;
    public Transform Point_Bolt_LockPoint;
    public Transform Point_Bolt_Rear;
    public Transform Point_Bolt_SafetyLock;
    public bool HasLastRoundBoltHoldOpen = true;
    public bool UsesAKSafetyLock;
    public bool DoesClipHoldBoltOpen = true;
    private float m_curBoltSpeed;
    private float m_boltZ_current;
    private float m_boltZ_heldTarget;
    private float m_boltZ_forward;
    private float m_boltZ_lock;
    private float m_boltZ_rear;
    private float m_boltZ_safetylock;
    private bool m_isBoltLocked;
    private bool m_isHandleHeld;
    private float m_handleLerp;
    [Header("Reciprocating Barrel")]
    public bool HasReciprocatingBarrel;
    public Transform Barrel;
    public Vector3 BarrelForward;
    public Vector3 BarrelRearward;
    private bool m_isBarrelReciprocating;
    [Header("Hammer")]
    public bool HasHammer;
    public Transform Hammer;
    public Vector3 HammerForward;
    public Vector3 HammerRearward;
    [Header("Rotating Bit")]
    public bool HasRotatingPart;
    public Transform RotatingPart;
    public Vector3 RotatingPartNeutralEulers;
    public Vector3 RotatingPartLeftEulers;
    public Vector3 RotatingPartRightEulers;
    [Header("Z Rot Part")]
    public bool HasZRotPart;
    public Transform ZRotPiece;
    public AnimationCurve ZRotCurve;
    public Vector2 ZAngles;
    [Header("Z Scale Part")]
    public bool HasZScalePart;
    public Transform ZScalePiece;
    public AnimationCurve ZScaleCurve;
    public bool ZRotPieceDips;
    public float DipMagnitude;

    protected override void Awake()
    {
      base.Awake();
      this.m_boltZ_current = this.transform.localPosition.z;
      this.m_boltZ_forward = this.Point_Bolt_Forward.localPosition.z;
      this.m_boltZ_lock = this.Point_Bolt_LockPoint.localPosition.z;
      this.m_boltZ_rear = this.Point_Bolt_Rear.localPosition.z;
      if (!this.UsesAKSafetyLock)
        return;
      this.m_boltZ_safetylock = this.Point_Bolt_SafetyLock.localPosition.z;
    }

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
      if (!this.IsHeld)
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
      this.m_isBoltLocked = false;
    }

    public bool IsBoltLocked() => this.m_isBoltLocked;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.HasRotatingPart)
        return;
      if ((double) Vector3.Dot((this.transform.position - this.m_hand.PalmTransform.position).normalized, this.transform.right) > 0.0)
        this.RotatingPart.localEulerAngles = this.RotatingPartLeftEulers;
      else
        this.RotatingPart.localEulerAngles = this.RotatingPartRightEulers;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if (this.HasRotatingPart)
        this.RotatingPart.localEulerAngles = this.RotatingPartNeutralEulers;
      if (!this.m_isBoltLocked)
        this.m_curBoltSpeed = this.Speed_Forward;
      if (this.CurPos > ClosedBolt.BoltPos.Forward)
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
      base.EndInteraction(hand);
    }

    public void UpdateHandleHeldState(bool state, float lerp)
    {
      this.m_isHandleHeld = state;
      this.m_handleLerp = lerp;
    }

    public void ImpartFiringImpulse()
    {
      this.m_curBoltSpeed = this.Speed_Rearward;
      if (this.CurPos != ClosedBolt.BoltPos.Forward)
        return;
      this.m_isBarrelReciprocating = true;
    }

    public bool IsBoltForwardOfSafetyLock() => (double) this.m_boltZ_current > (double) this.m_boltZ_safetylock;

    public void UpdateBolt()
    {
      bool flag = false;
      if (this.IsHeld || this.m_isHandleHeld)
        flag = true;
      if (this.IsHeld)
        this.m_boltZ_heldTarget = this.Weapon.transform.InverseTransformPoint(this.GetClosestValidPoint(this.Point_Bolt_Forward.position, this.Point_Bolt_Rear.position, this.m_hand.Input.Pos)).z;
      else if (this.m_isHandleHeld)
        this.m_boltZ_heldTarget = Mathf.Lerp(this.m_boltZ_forward, this.m_boltZ_rear, this.m_handleLerp);
      Vector2 vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_forward);
      if ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock && this.m_isBoltLocked)
        vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_lock);
      if (this.UsesAKSafetyLock && this.Weapon.IsWeaponOnSafe() && (double) this.m_boltZ_current <= (double) this.m_boltZ_safetylock)
        vector2 = new Vector2(this.m_boltZ_safetylock, this.m_boltZ_forward);
      if (this.DoesClipHoldBoltOpen && (Object) this.Weapon.Clip != (Object) null)
        vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_lock);
      if (flag)
        this.m_curBoltSpeed = 0.0f;
      else if (this.CurPos < ClosedBolt.BoltPos.LockedToRear && (double) this.m_curBoltSpeed >= 0.0 || this.LastPos >= ClosedBolt.BoltPos.Rear)
        this.m_curBoltSpeed = Mathf.MoveTowards(this.m_curBoltSpeed, this.Speed_Forward, Time.deltaTime * this.SpringStiffness);
      float boltZCurrent1 = this.m_boltZ_current;
      float boltZCurrent2 = this.m_boltZ_current;
      float num1 = Mathf.Clamp(!flag ? this.m_boltZ_current + this.m_curBoltSpeed * Time.deltaTime : Mathf.MoveTowards(this.m_boltZ_current, this.m_boltZ_heldTarget, this.Speed_Held * Time.deltaTime), vector2.x, vector2.y);
      if ((double) Mathf.Abs(num1 - this.m_boltZ_current) > (double) Mathf.Epsilon)
      {
        this.m_boltZ_current = num1;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_boltZ_current);
        if (this.HasReciprocatingBarrel && this.m_isBarrelReciprocating)
          this.Barrel.localPosition = Vector3.Lerp(this.BarrelForward, this.BarrelRearward, 1f - this.GetBoltLerpBetweenLockAndFore());
        if (this.HasZRotPart)
        {
          float num2 = 1f - this.GetBoltLerpBetweenLockAndFore();
          this.ZRotPiece.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.ZAngles.x, this.ZAngles.y, this.ZRotCurve.Evaluate(num2)));
          if (this.ZRotPieceDips)
            this.ZRotPiece.localPosition = Vector3.Lerp(Vector3.zero, Vector3.down * this.DipMagnitude, num2);
        }
        if (this.HasZScalePart)
          this.ZScalePiece.localScale = new Vector3(1f, 1f, this.ZScaleCurve.Evaluate(1f - this.GetBoltLerpBetweenLockAndFore()));
      }
      else
        this.m_curBoltSpeed = 0.0f;
      if (this.HasHammer)
        this.Hammer.localEulerAngles = !this.Weapon.IsHammerCocked ? Vector3.Lerp(this.HammerRearward, this.HammerForward, this.GetBoltLerpBetweenLockAndFore()) : this.HammerRearward;
      ClosedBolt.BoltPos curPos1 = this.CurPos;
      ClosedBolt.BoltPos boltPos = (double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_forward) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_lock) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_rear) >= 1.0 / 1000.0 ? ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock ? ClosedBolt.BoltPos.LockedToRear : ClosedBolt.BoltPos.ForwardToMid) : ClosedBolt.BoltPos.Rear) : ClosedBolt.BoltPos.Locked) : ClosedBolt.BoltPos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = boltPos;
      if (this.CurPos >= ClosedBolt.BoltPos.ForwardToMid)
        this.Weapon.IsBreachOpenForGasOut = true;
      else
        this.Weapon.IsBreachOpenForGasOut = false;
      if (this.CurPos == ClosedBolt.BoltPos.Rear && this.LastPos != ClosedBolt.BoltPos.Rear)
        this.BoltEvent_SmackRear();
      if (this.CurPos == ClosedBolt.BoltPos.Locked && this.LastPos != ClosedBolt.BoltPos.Locked)
        this.BoltEvent_BoltCaught();
      if (this.CurPos >= ClosedBolt.BoltPos.Locked && this.LastPos < ClosedBolt.BoltPos.Locked)
        this.BoltEvent_EjectRound();
      if (this.CurPos < ClosedBolt.BoltPos.Locked && this.LastPos > ClosedBolt.BoltPos.ForwardToMid)
        this.BoltEvent_ExtractRoundFromMag();
      if (this.CurPos == ClosedBolt.BoltPos.Forward && this.LastPos != ClosedBolt.BoltPos.Forward)
        this.BoltEvent_ArriveAtFore();
      if (this.CurPos >= ClosedBolt.BoltPos.Locked && (this.HasLastRoundBoltHoldOpen && (Object) this.Weapon.Magazine != (Object) null && (!this.Weapon.Magazine.HasARound() && !this.Weapon.Chamber.IsFull) || this.Weapon.IsBoltCatchButtonHeld))
        this.LockBolt();
      this.LastPos = this.CurPos;
    }

    private void BoltEvent_ArriveAtFore()
    {
      this.Weapon.ChamberRound();
      if (this.HasReciprocatingBarrel && this.m_isBarrelReciprocating)
      {
        this.m_isBarrelReciprocating = false;
        this.Barrel.localPosition = this.BarrelForward;
      }
      if (this.IsHeld)
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
      else
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
    }

    private void BoltEvent_EjectRound()
    {
      this.Weapon.EjectExtractedRound();
      this.Weapon.CockHammer();
    }

    private void BoltEvent_ExtractRoundFromMag() => this.Weapon.BeginChamberingRound();

    private void BoltEvent_BoltCaught()
    {
      if (!this.m_isBoltLocked)
        return;
      this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackLocked);
    }

    private void BoltEvent_SmackRear()
    {
      if ((this.IsHeld || this.m_isHandleHeld) && (!this.Weapon.BoltLocksWhenNoMagazineFound || (Object) this.Weapon.Magazine != (Object) null))
        this.ReleaseBolt();
      if (this.Weapon.EjectsMagazineOnEmpty && (Object) this.Weapon.Magazine != (Object) null && !this.Weapon.Magazine.HasARound())
        this.Weapon.EjectMag();
      if (this.Weapon.BoltLocksWhenNoMagazineFound && (Object) this.Weapon.Magazine == (Object) null)
        this.LockBolt();
      if (this.IsHeld)
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
      else
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
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
