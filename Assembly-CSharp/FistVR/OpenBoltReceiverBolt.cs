// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltReceiverBolt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class OpenBoltReceiverBolt : FVRInteractiveObject
  {
    [Header("Bolt Config")]
    public OpenBoltReceiver Receiver;
    public float BoltSpeed_Forward;
    public float BoltSpeed_Rearward;
    public float BoltSpeed_Held;
    public float BoltSpringStiffness = 5f;
    public OpenBoltReceiverBolt.BoltPos CurPos;
    public OpenBoltReceiverBolt.BoltPos LastPos;
    public Transform Point_Bolt_Forward;
    public Transform Point_Bolt_LockPoint;
    public Transform Point_Bolt_Rear;
    public Transform Point_Bolt_SafetyCatch;
    public Transform Point_Bolt_SafetyRotLimit;
    public bool HasLastRoundBoltHoldOpen;
    public bool UsesRotatingSafety = true;
    private bool m_doesFiringPinStrikeOnArrivalAtFore = true;
    private float m_curBoltSpeed;
    private float m_boltZ_current;
    private float m_boltZ_heldTarget;
    private float m_boltZ_forward;
    private float m_boltZ_lock;
    private float m_boltZ_rear;
    private float m_boltZ_safetyCatch;
    private float m_boltZ_safetyrotLimit;
    [Header("Safety Catch Config")]
    public float BoltRot_Standard;
    public float BoltRot_Safe;
    public float BoltRot_SlipDistance;
    private bool m_hasSafetyCatch;
    private float m_currentBoltRot;
    [Header("Spring Config")]
    public Transform Spring;
    public Vector2 SpringScales;
    public OpenBoltReceiverBolt.BoltSlidingPiece[] SlidingPieces;
    private bool m_isChargingHandleHeld;
    private float m_chargingHandleLerp;

    protected override void Awake()
    {
      base.Awake();
      this.m_boltZ_current = this.transform.localPosition.z;
      this.m_boltZ_forward = this.Point_Bolt_Forward.localPosition.z;
      this.m_boltZ_lock = this.Point_Bolt_LockPoint.localPosition.z;
      this.m_boltZ_rear = this.Point_Bolt_Rear.localPosition.z;
      if (!((UnityEngine.Object) this.Point_Bolt_SafetyCatch != (UnityEngine.Object) null) || !this.UsesRotatingSafety)
        return;
      this.m_boltZ_safetyCatch = this.Point_Bolt_SafetyCatch.localPosition.z;
      this.m_boltZ_safetyrotLimit = this.Point_Bolt_SafetyRotLimit.localPosition.z;
      this.m_hasSafetyCatch = true;
      this.m_currentBoltRot = this.BoltRot_Standard;
    }

    public override void UpdateInteraction(FVRViveHand hand) => base.UpdateInteraction(hand);

    public override void EndInteraction(FVRViveHand hand) => base.EndInteraction(hand);

    public void ChargingHandleHeld(float l)
    {
      this.m_isChargingHandleHeld = true;
      this.m_chargingHandleLerp = l;
    }

    public void ChargingHandleReleased()
    {
      this.m_isChargingHandleHeld = false;
      this.m_chargingHandleLerp = 0.0f;
    }

    public float GetBoltLerpBetweenLockAndFore() => Mathf.InverseLerp(this.m_boltZ_lock, this.m_boltZ_forward, this.m_boltZ_current);

    public void SetBoltToRear() => this.m_boltZ_current = this.m_boltZ_rear;

    public void UpdateBolt()
    {
      bool flag = false;
      if (this.IsHeld || this.m_isChargingHandleHeld)
        flag = true;
      if (this.IsHeld)
        this.m_boltZ_heldTarget = this.Receiver.transform.InverseTransformPoint(this.GetClosestValidPoint(this.Point_Bolt_Forward.position, this.Point_Bolt_Rear.position, this.m_hand.Input.Pos)).z;
      else if (this.m_isChargingHandleHeld)
        this.m_boltZ_heldTarget = Mathf.Lerp(this.m_boltZ_forward, this.m_boltZ_rear, this.m_chargingHandleLerp);
      Vector2 vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_forward);
      if ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock && this.Receiver.IsBoltCatchEngaged())
        vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_lock);
      if (this.m_hasSafetyCatch)
      {
        float z = this.m_currentBoltRot;
        float num = Mathf.InverseLerp(Mathf.Min(this.BoltRot_Standard, this.BoltRot_Safe), Mathf.Max(this.BoltRot_Standard, this.BoltRot_Safe), z);
        if (this.IsHeld)
        {
          if ((double) this.m_boltZ_current < (double) this.m_boltZ_safetyrotLimit)
          {
            Vector3 rhs = Vector3.ProjectOnPlane(this.m_hand.Input.Pos - this.transform.position, this.transform.forward);
            rhs = rhs.normalized;
            Vector3 up = this.Receiver.transform.up;
            z = Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(up, rhs)), Vector3.Dot(up, rhs)) * 57.29578f, Mathf.Min(this.BoltRot_Standard, this.BoltRot_Safe), Mathf.Max(this.BoltRot_Standard, this.BoltRot_Safe));
          }
        }
        else if (!this.m_isChargingHandleHeld)
          z = (double) num > 0.5 ? Mathf.Max(this.BoltRot_Standard, this.BoltRot_Safe) : Mathf.Min(this.BoltRot_Standard, this.BoltRot_Safe);
        if ((double) Mathf.Abs(z - this.BoltRot_Safe) < (double) this.BoltRot_SlipDistance)
          vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_safetyCatch);
        else if ((double) Mathf.Abs(z - this.BoltRot_Standard) >= (double) this.BoltRot_SlipDistance)
          vector2 = new Vector2(this.m_boltZ_rear, this.m_boltZ_safetyrotLimit);
        if ((double) Mathf.Abs(z - this.m_currentBoltRot) > 0.100000001490116)
          this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, z);
        this.m_currentBoltRot = z;
      }
      if (flag)
        this.m_curBoltSpeed = 0.0f;
      else if ((double) this.m_curBoltSpeed >= 0.0 || this.CurPos >= OpenBoltReceiverBolt.BoltPos.Locked)
        this.m_curBoltSpeed = Mathf.MoveTowards(this.m_curBoltSpeed, this.BoltSpeed_Forward, Time.deltaTime * this.BoltSpringStiffness);
      float boltZCurrent = this.m_boltZ_current;
      float target = this.m_boltZ_current;
      if (flag)
        target = this.m_boltZ_heldTarget;
      float num1 = Mathf.Clamp(!flag ? this.m_boltZ_current + this.m_curBoltSpeed * Time.deltaTime : Mathf.MoveTowards(this.m_boltZ_current, target, this.BoltSpeed_Held * Time.deltaTime), vector2.x, vector2.y);
      if ((double) Mathf.Abs(num1 - this.m_boltZ_current) > (double) Mathf.Epsilon)
      {
        this.m_boltZ_current = num1;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_boltZ_current);
        if (this.SlidingPieces.Length > 0)
        {
          float z1 = this.Point_Bolt_Rear.localPosition.z;
          for (int index = 0; index < this.SlidingPieces.Length; ++index)
          {
            Vector3 localPosition = this.SlidingPieces[index].Piece.localPosition;
            float z2 = Mathf.Lerp(this.m_boltZ_current, z1, this.SlidingPieces[index].DistancePercent);
            this.SlidingPieces[index].Piece.localPosition = new Vector3(localPosition.x, localPosition.y, z2);
          }
        }
        if ((UnityEngine.Object) this.Spring != (UnityEngine.Object) null)
          this.Spring.localScale = new Vector3(1f, 1f, Mathf.Lerp(this.SpringScales.x, this.SpringScales.y, Mathf.InverseLerp(this.m_boltZ_rear, this.m_boltZ_forward, this.m_boltZ_current)));
      }
      else
        this.m_curBoltSpeed = 0.0f;
      OpenBoltReceiverBolt.BoltPos curPos1 = this.CurPos;
      OpenBoltReceiverBolt.BoltPos boltPos = (double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_forward) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_lock) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_boltZ_current - this.m_boltZ_rear) >= 1.0 / 1000.0 ? ((double) this.m_boltZ_current <= (double) this.m_boltZ_lock ? OpenBoltReceiverBolt.BoltPos.LockedToRear : OpenBoltReceiverBolt.BoltPos.ForwardToMid) : OpenBoltReceiverBolt.BoltPos.Rear) : OpenBoltReceiverBolt.BoltPos.Locked) : OpenBoltReceiverBolt.BoltPos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = boltPos;
      if (this.CurPos == OpenBoltReceiverBolt.BoltPos.Rear && this.LastPos != OpenBoltReceiverBolt.BoltPos.Rear)
        this.BoltEvent_BoltSmackRear();
      if (this.CurPos == OpenBoltReceiverBolt.BoltPos.Locked && this.LastPos != OpenBoltReceiverBolt.BoltPos.Locked)
        this.BoltEvent_BoltCaught();
      if (this.CurPos >= OpenBoltReceiverBolt.BoltPos.Locked && this.LastPos < OpenBoltReceiverBolt.BoltPos.Locked)
        this.BoltEvent_EjectRound();
      if (this.CurPos < OpenBoltReceiverBolt.BoltPos.Locked && this.LastPos > OpenBoltReceiverBolt.BoltPos.ForwardToMid)
        this.BoltEvent_BeginChambering();
      if (this.CurPos == OpenBoltReceiverBolt.BoltPos.Forward && this.LastPos != OpenBoltReceiverBolt.BoltPos.Forward)
        this.BoltEvent_ArriveAtFore();
      this.LastPos = this.CurPos;
    }

    private void BoltEvent_ArriveAtFore()
    {
      if (!this.Receiver.ChamberRound())
        ;
      if (this.m_doesFiringPinStrikeOnArrivalAtFore && this.Receiver.Fire())
        this.ImpartFiringImpulse();
      if (this.IsHeld || this.m_isChargingHandleHeld)
        this.Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideForwardHeld);
      else
        this.Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideForward);
    }

    public void ImpartFiringImpulse() => this.m_curBoltSpeed = this.BoltSpeed_Rearward;

    private void BoltEvent_BoltCaught()
    {
      if (!this.Receiver.IsBoltCatchEngaged())
        return;
      this.Receiver.PlayAudioEvent(FirearmAudioEventType.CatchOnSear);
    }

    private void BoltEvent_EjectRound() => this.Receiver.EjectExtractedRound();

    private void BoltEvent_BeginChambering() => this.Receiver.BeginChamberingRound();

    private void BoltEvent_BoltSmackRear()
    {
      if (this.IsHeld || this.m_isChargingHandleHeld)
        this.Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideBackHeld);
      else
        this.Receiver.PlayAudioEvent(FirearmAudioEventType.BoltSlideBack);
    }

    public enum BoltPos
    {
      Forward,
      ForwardToMid,
      Locked,
      LockedToRear,
      Rear,
    }

    [Serializable]
    public class BoltSlidingPiece
    {
      public Transform Piece;
      public float DistancePercent;
    }
  }
}
