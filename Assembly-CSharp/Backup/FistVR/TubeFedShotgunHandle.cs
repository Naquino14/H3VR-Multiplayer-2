// Decompiled with JetBrains decompiler
// Type: FistVR.TubeFedShotgunHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TubeFedShotgunHandle : FVRAlternateGrip
  {
    [Header("Shotgun Handle")]
    public TubeFedShotgun Shotgun;
    public float Speed_Held;
    public TubeFedShotgunHandle.BoltPos CurPos;
    public TubeFedShotgunHandle.BoltPos LastPos;
    public Transform Point_Bolt_Forward;
    public Transform Point_Bolt_LockPoint;
    public Transform Point_Bolt_Rear;
    private float m_handZOffset;
    private float m_boltZ_current;
    private float m_boltZ_heldTarget;
    private float m_boltZ_forward;
    private float m_boltZ_lock;
    private float m_boltZ_rear;
    private bool m_isHandleLocked;

    protected override void Awake()
    {
      base.Awake();
      this.m_boltZ_current = this.transform.localPosition.z;
      this.m_boltZ_forward = this.Point_Bolt_Forward.localPosition.z;
      this.m_boltZ_lock = this.Point_Bolt_LockPoint.localPosition.z;
      this.m_boltZ_rear = this.Point_Bolt_Rear.localPosition.z;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      this.m_handZOffset = this.transform.InverseTransformPoint(hand.Input.Pos).z;
      base.BeginInteraction(hand);
    }

    public float GetBoltLerpBetweenLockAndFore() => Mathf.InverseLerp(this.m_boltZ_lock, this.m_boltZ_forward, this.m_boltZ_current);

    public float GetBoltLerpBetweenRearAndFore() => Mathf.InverseLerp(this.m_boltZ_rear, this.m_boltZ_forward, this.m_boltZ_current);

    public void LockHandle() => this.m_isHandleLocked = true;

    public void UnlockHandle() => this.m_isHandleLocked = false;

    public void UpdateHandle()
    {
      bool flag = false;
      if (this.IsHeld || this.Shotgun.IsAltHeld)
        flag = true;
      if (flag && !this.m_isHandleLocked)
      {
        Vector3 closestValidPoint;
        if (this.IsHeld)
        {
          closestValidPoint = this.GetClosestValidPoint(this.Point_Bolt_Forward.position, this.Point_Bolt_Rear.position, this.m_hand.Input.Pos + -this.transform.forward * this.m_handZOffset * this.Shotgun.transform.localScale.x);
        }
        else
        {
          if (this.Shotgun.m_hand.Input.TriggerPressed)
          {
            Vector3 vector3 = this.Shotgun.transform.InverseTransformDirection(Vector3.Project(this.Shotgun.m_hand.Input.VelLinearWorld, this.Shotgun.transform.forward));
            if ((double) Mathf.Abs(vector3.z) > 1.0)
              this.Shotgun.GrabPointTransform.localPosition = new Vector3(this.Shotgun.GrabPointTransform.localPosition.x, this.Shotgun.GrabPointTransform.localPosition.y, Mathf.Clamp(this.Shotgun.GrabPointTransform.localPosition.z + -vector3.z * Time.deltaTime, this.Point_Bolt_Rear.localPosition.z - 0.1f, this.Point_Bolt_Forward.localPosition.z + 0.1f));
          }
          closestValidPoint = this.GetClosestValidPoint(this.Point_Bolt_Forward.position, this.Point_Bolt_Rear.position, this.Shotgun.m_hand.Input.Pos);
        }
        this.m_boltZ_heldTarget = this.Shotgun.transform.InverseTransformPoint(closestValidPoint).z;
      }
      float num = this.m_boltZ_current;
      float boltZCurrent = this.m_boltZ_current;
      if (flag && !this.m_isHandleLocked)
        num = Mathf.MoveTowards(this.m_boltZ_current, this.m_boltZ_heldTarget, this.Speed_Held * Time.deltaTime);
      if ((double) Mathf.Abs(num - this.m_boltZ_current) > (double) Mathf.Epsilon)
      {
        this.m_boltZ_current = num;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_boltZ_current);
      }
      TubeFedShotgunHandle.BoltPos curPos1 = this.CurPos;
      TubeFedShotgunHandle.BoltPos boltPos = (double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_forward) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_lock) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_rear) >= 1.0 / 1000.0 ? ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock ? TubeFedShotgunHandle.BoltPos.LockedToRear : TubeFedShotgunHandle.BoltPos.ForwardToMid) : TubeFedShotgunHandle.BoltPos.Rear) : TubeFedShotgunHandle.BoltPos.Locked) : TubeFedShotgunHandle.BoltPos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = (TubeFedShotgunHandle.BoltPos) Mathf.Clamp((int) boltPos, curPos2 - 1, curPos2 + 1);
      if (this.CurPos == TubeFedShotgunHandle.BoltPos.Forward && this.LastPos != TubeFedShotgunHandle.BoltPos.Forward)
        this.BoltEvent_ArriveAtFore();
      else if (this.CurPos == TubeFedShotgunHandle.BoltPos.Rear && this.LastPos != TubeFedShotgunHandle.BoltPos.Rear)
        this.BoltEvent_SmackRear();
      this.LastPos = this.CurPos;
    }

    private void BoltEvent_ArriveAtFore()
    {
      if (this.Shotgun.Mode != TubeFedShotgun.ShotgunMode.PumpMode)
        return;
      if (this.Shotgun.IsHammerCocked)
        this.LockHandle();
      if (this.Shotgun.Chamber.IsFull || this.Shotgun.HasExtractedRound())
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleForward);
      else
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleForwardEmpty);
    }

    private void BoltEvent_SmackRear()
    {
      if (this.Shotgun.Mode != TubeFedShotgun.ShotgunMode.PumpMode)
        return;
      if (this.Shotgun.Chamber.IsFull || this.Shotgun.HasExtractedRound())
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleBack);
      else
        this.Shotgun.PlayAudioEvent(FirearmAudioEventType.HandleBackEmpty);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      bool flag = false;
      if (hand.IsInStreamlinedMode && hand.Input.BYButtonDown)
        flag = true;
      else if (hand.Input.TouchpadDown)
        flag = true;
      if (flag && this.CurPos == TubeFedShotgunHandle.BoltPos.Forward && this.Shotgun.CanModeSwitch)
        this.Shotgun.ToggleMode();
      base.UpdateInteraction(hand);
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
