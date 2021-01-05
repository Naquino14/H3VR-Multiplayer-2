// Decompiled with JetBrains decompiler
// Type: FistVR.LeverActionTubeACtion
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class LeverActionTubeACtion : FVRInteractiveObject
  {
    public FVRFireArm FA;
    public Transform Gun;
    public Transform Root;
    public Transform SwingMag;
    public FVRFireArmMagazine Mag;
    public Transform Spring;
    private int m_cachedCapacity = -1;
    public Transform Pos_Forward;
    public Transform Pos_Rearward;
    public Vector3 SpringScaleForward;
    public Vector3 SpringScaleRearward;
    public List<Vector3> LocalPos_PerRound;
    public List<float> LocalSpringScale_PerRound;
    private float m_zCurrent;
    private float m_zForward;
    private float m_zRearward;
    private float m_zRoundRearClamp;
    private float m_zHeldTarget;
    private bool m_isTubeOpen;
    public float ZClampWhenOpened;
    public float Speed = -5f;
    public LeverActionTubeACtion.FollowerPos FCurPos = LeverActionTubeACtion.FollowerPos.Rear;
    public LeverActionTubeACtion.FollowerPos FLastPos = LeverActionTubeACtion.FollowerPos.Rear;
    private float m_curRot;
    private float m_lastRot;
    public AudioEvent AudEvent_FollowerForward;
    public AudioEvent AudEvent_FollowerRearward;
    public AudioEvent AudEvent_FollowerForwardHeld;
    public AudioEvent AudEvent_FollowerRearwardHeld;
    public AudioEvent AudEvent_SwingHit;

    protected override void Awake()
    {
      base.Awake();
      this.m_zCurrent = this.transform.localPosition.z;
      this.m_zHeldTarget = this.transform.localPosition.z;
      this.m_zForward = this.Pos_Forward.localPosition.z;
      this.m_zRearward = this.Pos_Rearward.localPosition.z;
      this.m_zRoundRearClamp = this.Pos_Rearward.localPosition.z;
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.m_cachedCapacity != this.Mag.m_numRounds)
      {
        this.m_cachedCapacity = this.Mag.m_numRounds;
        this.m_zRoundRearClamp = this.LocalPos_PerRound[this.m_cachedCapacity].z;
      }
      bool flag = false;
      if (this.IsHeld)
        flag = true;
      Vector3 zero = Vector3.zero;
      if (this.IsHeld)
        this.m_zHeldTarget = this.Root.InverseTransformPoint(this.GetClosestValidPoint(this.Pos_Forward.position, this.Pos_Rearward.position, this.m_hand.Input.Pos)).z;
      Vector2 vector2 = new Vector2(Mathf.Min(this.m_zRoundRearClamp, this.m_zForward), Mathf.Max(this.m_zRoundRearClamp, this.m_zForward));
      if (this.m_isTubeOpen)
        vector2.x = this.ZClampWhenOpened;
      float zCurrent = this.m_zCurrent;
      float zRoundRearClamp = this.m_zRoundRearClamp;
      float num1 = Mathf.Clamp(!flag ? this.m_zCurrent + this.Speed * Time.deltaTime : Mathf.MoveTowards(this.m_zCurrent, this.m_zHeldTarget, 5f * Time.deltaTime), Mathf.Min(vector2.x, vector2.y), Mathf.Max(vector2.x, vector2.y));
      if ((double) Mathf.Abs(num1 - this.m_zCurrent) > (double) Mathf.Epsilon)
      {
        this.m_zCurrent = num1;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_zCurrent);
        this.Spring.localScale = Vector3.Lerp(this.SpringScaleForward, this.SpringScaleRearward, Mathf.InverseLerp(this.m_zForward, this.m_zRearward, this.m_zCurrent));
      }
      LeverActionTubeACtion.FollowerPos fcurPos1 = this.FCurPos;
      LeverActionTubeACtion.FollowerPos followerPos = (double) Mathf.Abs(this.m_zCurrent - this.m_zForward) < 1.0 / 1000.0 || this.m_isTubeOpen ? LeverActionTubeACtion.FollowerPos.Forward : ((double) Mathf.Abs(this.m_zCurrent - this.m_zRoundRearClamp) >= 1.0 / 1000.0 ? ((double) Mathf.Abs(this.m_zCurrent - this.m_zRearward) >= 1.0 / 1000.0 ? LeverActionTubeACtion.FollowerPos.Middle : LeverActionTubeACtion.FollowerPos.Rear) : LeverActionTubeACtion.FollowerPos.ClampedRear);
      int fcurPos2 = (int) this.FCurPos;
      this.FCurPos = followerPos;
      if ((this.FCurPos == LeverActionTubeACtion.FollowerPos.Rear || this.FCurPos == LeverActionTubeACtion.FollowerPos.ClampedRear) && (this.FLastPos != LeverActionTubeACtion.FollowerPos.Rear && this.FLastPos != LeverActionTubeACtion.FollowerPos.ClampedRear))
      {
        if (this.IsHeld)
          this.FA.PlayAudioAsHandling(this.AudEvent_FollowerRearwardHeld, this.transform.position);
        else
          this.FA.PlayAudioAsHandling(this.AudEvent_FollowerRearward, this.transform.position);
      }
      else if (this.FCurPos == LeverActionTubeACtion.FollowerPos.Forward && this.FLastPos != LeverActionTubeACtion.FollowerPos.Forward)
      {
        if (this.IsHeld)
          this.FA.PlayAudioAsHandling(this.AudEvent_FollowerForwardHeld, this.transform.position);
        else
          this.FA.PlayAudioAsHandling(this.AudEvent_FollowerForward, this.transform.position);
      }
      this.FLastPos = this.FCurPos;
      if (this.FCurPos == LeverActionTubeACtion.FollowerPos.Forward && this.IsHeld)
      {
        Vector3 rhs = Vector3.ProjectOnPlane(this.m_hand.Input.Pos - this.SwingMag.position, this.Gun.forward);
        rhs = rhs.normalized;
        Vector3 lhs = -this.Gun.up;
        float num2 = Mathf.Clamp(Mathf.Atan2(Vector3.Dot(this.Gun.forward, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f, 0.0f, 90f);
        Debug.Log((object) num2);
        if ((double) Mathf.Abs(num2 - this.m_curRot) > (double) Mathf.Epsilon)
        {
          this.m_curRot = num2;
          this.SwingMag.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_curRot);
        }
        if ((double) this.m_curRot < 1.0 / 1000.0)
        {
          this.m_curRot = 0.0f;
          this.m_isTubeOpen = false;
        }
        else
          this.m_isTubeOpen = true;
        if ((double) this.m_curRot <= 0.0 && (double) this.m_lastRot > 0.0)
          this.FA.PlayAudioAsHandling(this.AudEvent_SwingHit, this.SwingMag.position);
        else if ((double) this.m_curRot >= 90.0 && (double) this.m_lastRot < 90.0)
          this.FA.PlayAudioAsHandling(this.AudEvent_SwingHit, this.SwingMag.position);
      }
      this.m_lastRot = this.m_curRot;
    }

    public enum FollowerPos
    {
      Forward,
      Middle,
      ClampedRear,
      Rear,
    }
  }
}
