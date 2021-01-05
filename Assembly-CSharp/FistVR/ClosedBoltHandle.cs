// Decompiled with JetBrains decompiler
// Type: FistVR.ClosedBoltHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ClosedBoltHandle : FVRInteractiveObject
  {
    [Header("Bolt Handle")]
    public ClosedBoltWeapon Weapon;
    public float Speed_Forward;
    public float Speed_Held;
    public float SpringStiffness = 100f;
    public ClosedBoltHandle.HandlePos CurPos;
    public ClosedBoltHandle.HandlePos LastPos;
    public Transform Point_Forward;
    public Transform Point_LockPoint;
    public Transform Point_Rear;
    public Transform Point_SafetyRotLimit;
    private float m_curSpeed;
    private float m_posZ_current;
    private float m_posZ_heldTarget;
    private float m_posZ_forward;
    private float m_posZ_lock;
    private float m_posZ_rear;
    private float m_posZ_safetyrotLimit;
    [Header("Safety Catch Config")]
    public bool UsesRotation = true;
    public float Rot_Standard;
    public float Rot_Safe;
    public float Rot_SlipDistance;
    public bool IsSlappable;
    public Transform SlapPoint;
    public float SlapDistance = 0.1f;
    private bool m_hasRotCatch;
    private float m_currentRot;
    [Header("Rotating Bit")]
    public bool HasRotatingPart;
    public Transform RotatingPart;
    public Vector3 RotatingPartNeutralEulers;
    public Vector3 RotatingPartLeftEulers;
    public Vector3 RotatingPartRightEulers;
    public bool StaysRotatedWhenBack;
    public bool UsesSoundOnGrab;
    private bool m_isHandleHeld;
    private float m_HandleLerp;
    private bool m_isAtLockAngle;

    protected override void Awake()
    {
      base.Awake();
      this.m_posZ_current = this.transform.localPosition.z;
      this.m_posZ_forward = this.Point_Forward.localPosition.z;
      this.m_posZ_lock = this.Point_LockPoint.localPosition.z;
      this.m_posZ_rear = this.Point_Rear.localPosition.z;
      if (!((Object) this.Point_SafetyRotLimit != (Object) null) || !this.UsesRotation)
        return;
      this.m_posZ_safetyrotLimit = this.Point_SafetyRotLimit.localPosition.z;
      this.m_hasRotCatch = true;
      this.m_currentRot = this.Rot_Standard;
    }

    public float GetBoltLerpBetweenLockAndFore() => Mathf.InverseLerp(this.m_posZ_lock, this.m_posZ_forward, this.m_posZ_current);

    public float GetBoltLerpBetweenRearAndFore() => Mathf.InverseLerp(this.m_posZ_rear, this.m_posZ_forward, this.m_posZ_current);

    public bool ShouldControlBolt()
    {
      if (!this.UsesRotation)
        return this.IsHeld;
      return this.IsHeld || this.m_isAtLockAngle;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (this.UsesSoundOnGrab)
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.HandleGrab);
      base.BeginInteraction(hand);
    }

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
      if (this.HasRotatingPart && !this.StaysRotatedWhenBack)
        this.RotatingPart.localEulerAngles = this.RotatingPartNeutralEulers;
      if (!this.Weapon.Bolt.IsBoltLocked())
        this.Weapon.PlayAudioEvent(FirearmAudioEventType.BoltRelease);
      base.EndInteraction(hand);
    }

    public void UpdateHandle()
    {
      bool flag = false;
      if (this.IsHeld)
        flag = true;
      if (flag)
        this.m_posZ_heldTarget = this.Weapon.transform.InverseTransformPoint(this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rear.position, this.m_hand.Input.Pos)).z;
      Vector2 vector2 = new Vector2(this.m_posZ_rear, this.m_posZ_forward);
      if (this.m_hasRotCatch)
      {
        float z = this.m_currentRot;
        if (!this.IsHeld && this.IsSlappable && (this.Weapon.IsHeld && this.m_isAtLockAngle))
        {
          FVRViveHand otherHand = this.Weapon.m_hand.OtherHand;
          float num1 = Vector3.Distance(this.SlapPoint.position, otherHand.Input.Pos);
          float num2 = Vector3.Dot(this.SlapPoint.forward, otherHand.Input.VelLinearWorld.normalized);
          if ((double) num1 < (double) this.SlapDistance && (double) num2 > 0.300000011920929 && (double) otherHand.Input.VelLinearWorld.magnitude > 1.0)
          {
            z = this.Rot_Standard;
            this.Weapon.Bolt.ReleaseBolt();
            if (this.HasRotatingPart)
              this.RotatingPart.localEulerAngles = this.RotatingPartNeutralEulers;
          }
        }
        float num = Mathf.InverseLerp(Mathf.Min(this.Rot_Standard, this.Rot_Safe), Mathf.Max(this.Rot_Standard, this.Rot_Safe), z);
        if (this.IsHeld)
        {
          if ((double) this.m_posZ_current < (double) this.m_posZ_safetyrotLimit)
          {
            Vector3 rhs = Vector3.ProjectOnPlane(this.m_hand.Input.Pos - this.transform.position, this.transform.forward);
            rhs = rhs.normalized;
            Vector3 up = this.Weapon.transform.up;
            z = Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(up, rhs)), Vector3.Dot(up, rhs)) * 57.29578f, Mathf.Min(this.Rot_Standard, this.Rot_Safe), Mathf.Max(this.Rot_Standard, this.Rot_Safe));
          }
        }
        else
          z = (double) num > 0.5 ? Mathf.Max(this.Rot_Standard, this.Rot_Safe) : Mathf.Min(this.Rot_Standard, this.Rot_Safe);
        if ((double) Mathf.Abs(z - this.Rot_Safe) < (double) this.Rot_SlipDistance)
        {
          vector2 = new Vector2(this.m_posZ_rear, this.m_posZ_lock);
          this.m_isAtLockAngle = true;
        }
        else if ((double) Mathf.Abs(z - this.Rot_Standard) < (double) this.Rot_SlipDistance)
        {
          this.m_isAtLockAngle = false;
        }
        else
        {
          vector2 = new Vector2(this.m_posZ_rear, this.m_posZ_safetyrotLimit);
          this.m_isAtLockAngle = true;
        }
        if ((double) Mathf.Abs(z - this.m_currentRot) > 0.100000001490116)
          this.transform.localEulerAngles = new Vector3(0.0f, 0.0f, z);
        this.m_currentRot = z;
      }
      if (flag)
        this.m_curSpeed = 0.0f;
      else if ((double) this.m_curSpeed >= 0.0 || this.CurPos > ClosedBoltHandle.HandlePos.Forward)
        this.m_curSpeed = Mathf.MoveTowards(this.m_curSpeed, this.Speed_Forward, Time.deltaTime * this.SpringStiffness);
      float posZCurrent1 = this.m_posZ_current;
      float posZCurrent2 = this.m_posZ_current;
      float num3 = Mathf.Clamp(!flag ? this.m_posZ_current + this.m_curSpeed * Time.deltaTime : Mathf.MoveTowards(this.m_posZ_current, this.m_posZ_heldTarget, this.Speed_Held * Time.deltaTime), vector2.x, vector2.y);
      if ((double) Mathf.Abs(num3 - this.m_posZ_current) > (double) Mathf.Epsilon)
      {
        this.m_posZ_current = num3;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_posZ_current);
      }
      else
        this.m_curSpeed = 0.0f;
      ClosedBoltHandle.HandlePos curPos1 = this.CurPos;
      ClosedBoltHandle.HandlePos handlePos = (double) Mathf.Abs(this.m_posZ_current - this.m_posZ_forward) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_posZ_current - this.m_posZ_lock) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_posZ_current - this.m_posZ_rear) >= 1.0 / 1000.0 ? ((double) this.m_posZ_current <= (double) this.m_posZ_lock ? ClosedBoltHandle.HandlePos.LockedToRear : ClosedBoltHandle.HandlePos.ForwardToMid) : ClosedBoltHandle.HandlePos.Rear) : ClosedBoltHandle.HandlePos.Locked) : ClosedBoltHandle.HandlePos.Forward;
      int curPos2 = (int) this.CurPos;
      this.CurPos = (ClosedBoltHandle.HandlePos) Mathf.Clamp((int) handlePos, curPos2 - 1, curPos2 + 1);
      if (this.CurPos == ClosedBoltHandle.HandlePos.Forward && this.LastPos != ClosedBoltHandle.HandlePos.Forward)
        this.Event_ArriveAtFore();
      else if ((this.CurPos != ClosedBoltHandle.HandlePos.ForwardToMid || this.LastPos != ClosedBoltHandle.HandlePos.Forward) && (this.CurPos != ClosedBoltHandle.HandlePos.Locked || this.LastPos != ClosedBoltHandle.HandlePos.ForwardToMid) && (this.CurPos != ClosedBoltHandle.HandlePos.ForwardToMid || this.LastPos != ClosedBoltHandle.HandlePos.Locked))
      {
        if (this.CurPos == ClosedBoltHandle.HandlePos.Locked && this.LastPos == ClosedBoltHandle.HandlePos.LockedToRear && this.m_isAtLockAngle)
          this.Event_HitLockPosition();
        else if (this.CurPos == ClosedBoltHandle.HandlePos.Rear && this.LastPos != ClosedBoltHandle.HandlePos.Rear)
          this.Event_SmackRear();
      }
      this.LastPos = this.CurPos;
    }

    private void Event_ArriveAtFore()
    {
      this.Weapon.PlayAudioEvent(FirearmAudioEventType.HandleForward);
      if (!this.HasRotatingPart)
        return;
      this.RotatingPart.localEulerAngles = this.RotatingPartNeutralEulers;
    }

    private void Event_HitLockPosition() => this.Weapon.PlayAudioEvent(FirearmAudioEventType.HandleForward);

    private void Event_SmackRear() => this.Weapon.PlayAudioEvent(FirearmAudioEventType.HandleBack);

    public enum HandlePos
    {
      Forward,
      ForwardToMid,
      Locked,
      LockedToRear,
      Rear,
    }
  }
}
