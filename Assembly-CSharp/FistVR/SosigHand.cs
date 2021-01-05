// Decompiled with JetBrains decompiler
// Type: FistVR.SosigHand
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public class SosigHand
  {
    public Sosig S;
    public Transform Root;
    public Transform Target;
    public bool IsHoldingObject;
    public SosigWeapon HeldObject;
    public bool IsRightHand = true;
    private Transform m_posedToward;
    public SosigHand.SosigHandPose Pose;
    public Transform Point_Aimed;
    public Transform Point_HipFire;
    public Transform Point_AtRest;
    public Transform Point_ShieldHold;
    private bool HasActiveAimPoint;
    private Vector3 m_aimTowardPoint = Vector3.zero;
    private List<float> vertOffsets = new List<float>()
    {
      0.0f,
      0.2f,
      0.4f,
      0.65f,
      -0.1f,
      -0.2f
    };
    private List<float> forwardOffsets = new List<float>()
    {
      0.0f,
      0.15f,
      0.1f,
      0.1f,
      0.05f,
      0.1f
    };
    private List<float> tiltLerpOffsets = new List<float>()
    {
      0.0f,
      0.2f,
      0.4f,
      0.7f,
      0.0f,
      0.0f
    };
    private int m_curFiringPose_Hip;
    private int m_prevFiringPose_Hip;
    private int m_curFiringPose_Aimed;
    private int m_prevFiringPose_Aimed;
    private int m_nextPoseToTestToTarget;
    private float m_timeSincePoseChange;
    private RaycastHit m_hit;
    private List<bool> m_sightValid_Hip = new List<bool>()
    {
      true,
      false,
      false
    };
    private List<bool> m_sightValid_Aimed = new List<bool>()
    {
      true,
      false,
      false
    };
    private float m_timeAwayFromTarget;
    private Vector3 m_lastPos = Vector3.zero;

    public void SetHandPose(SosigHand.SosigHandPose s)
    {
      if (this.Pose == s)
        return;
      this.Pose = s;
      if (this.IsHoldingObject && this.HeldObject.Type == SosigWeapon.SosigWeaponType.Melee && this.HeldObject.MeleeType == SosigWeapon.SosigMeleeWeaponType.Shield)
      {
        this.PoseToward(this.Point_ShieldHold);
      }
      else
      {
        switch (s)
        {
          case SosigHand.SosigHandPose.AtRest:
            this.PoseToward(this.Point_AtRest);
            break;
          case SosigHand.SosigHandPose.HipFire:
            this.PoseToward(this.Point_HipFire);
            break;
          case SosigHand.SosigHandPose.Aimed:
            this.PoseToward(this.Point_Aimed);
            break;
        }
      }
    }

    private void UpdateGunHandlingPose()
    {
      if (!this.HasActiveAimPoint)
        return;
      LayerMask visualOcclusionCheck = this.S.E.LM_VisualOcclusionCheck;
      Transform transform = this.S.Links[1].transform;
      Vector3 origin1 = this.Point_HipFire.position + transform.up * this.vertOffsets[this.m_nextPoseToTestToTarget];
      Vector3 vector3 = this.m_aimTowardPoint + UnityEngine.Random.onUnitSphere * 0.05f - origin1;
      this.m_sightValid_Hip[this.m_nextPoseToTestToTarget] = !Physics.Raycast(origin1, vector3.normalized, out this.m_hit, Mathf.Min(vector3.magnitude, 300f), (int) visualOcclusionCheck, QueryTriggerInteraction.Ignore);
      Vector3 origin2 = this.Point_Aimed.position + transform.up * this.vertOffsets[this.m_nextPoseToTestToTarget];
      vector3 = this.m_aimTowardPoint + UnityEngine.Random.onUnitSphere * 0.05f - origin2;
      this.m_sightValid_Aimed[this.m_nextPoseToTestToTarget] = !Physics.Raycast(origin2, vector3.normalized, out this.m_hit, Mathf.Min(vector3.magnitude, 300f), (int) visualOcclusionCheck, QueryTriggerInteraction.Ignore);
      ++this.m_nextPoseToTestToTarget;
      if (this.m_nextPoseToTestToTarget >= this.m_sightValid_Hip.Count)
        this.m_nextPoseToTestToTarget = 0;
      if ((double) this.m_timeSincePoseChange < 1.0)
      {
        this.m_timeSincePoseChange += Time.deltaTime * 3f;
      }
      else
      {
        if (!this.m_sightValid_Hip[this.m_curFiringPose_Hip])
        {
          int num = -1;
          for (int index = 0; index < this.m_sightValid_Hip.Count; ++index)
          {
            if (this.m_sightValid_Hip[index])
            {
              num = index;
              break;
            }
          }
          if (num > -1)
          {
            this.m_prevFiringPose_Hip = this.m_curFiringPose_Hip;
            this.m_curFiringPose_Hip = num;
            this.m_timeSincePoseChange = 0.0f;
          }
        }
        if (this.m_sightValid_Aimed[this.m_curFiringPose_Aimed])
          return;
        int num1 = -1;
        for (int index = 0; index < this.m_sightValid_Aimed.Count; ++index)
        {
          if (this.m_sightValid_Aimed[index])
          {
            num1 = index;
            break;
          }
        }
        if (num1 <= -1)
          return;
        this.m_prevFiringPose_Aimed = this.m_curFiringPose_Aimed;
        this.m_curFiringPose_Aimed = num1;
        this.m_timeSincePoseChange = 0.0f;
      }
    }

    public void SetAimTowardPoint(Vector3 v)
    {
      this.m_aimTowardPoint = v;
      if (this.S.IsPanicFiring())
        this.m_aimTowardPoint += UnityEngine.Random.onUnitSphere * 10f;
      this.HasActiveAimPoint = true;
    }

    public void ClearAimPoint() => this.HasActiveAimPoint = false;

    public void PoseToward(Transform t)
    {
      if ((UnityEngine.Object) this.m_posedToward == (UnityEngine.Object) t)
        return;
      this.m_posedToward = t;
    }

    public void SetPoseDirect(Vector3 pos, Quaternion rot)
    {
      this.Target.position = pos;
      this.Target.rotation = rot;
    }

    public void PickUp(SosigWeapon o)
    {
      this.HeldObject = o;
      this.HeldObject.IsHeldByBot = true;
      this.HeldObject.SosigHoldingThis = this.S;
      this.HeldObject.HandHoldingThis = this;
      this.IsHoldingObject = true;
      o.BotPickup(this.S);
      if (!this.S.IgnoreRBs.Contains(o.O.RootRigidbody))
        this.S.IgnoreRBs.Add(o.O.RootRigidbody);
      int num = 0;
      while (num < this.S.Links.Count)
        ++num;
    }

    public void PutAwayHeldObject()
    {
    }

    public void DropHeldObject()
    {
      if ((UnityEngine.Object) this.HeldObject != (UnityEngine.Object) null && this.S.IgnoreRBs.Contains(this.HeldObject.O.RootRigidbody))
        this.S.IgnoreRBs.Remove(this.HeldObject.O.RootRigidbody);
      if (!this.IsHoldingObject)
        return;
      this.IsHoldingObject = false;
      this.HeldObject.SosigHoldingThis = (Sosig) null;
      this.HeldObject.IsHeldByBot = false;
      this.HeldObject.HandHoldingThis = (SosigHand) null;
      this.HeldObject.BotDrop();
      this.HeldObject = (SosigWeapon) null;
    }

    public void ThrowObject(Vector3 velocity, Vector3 targPoint)
    {
      if ((UnityEngine.Object) this.HeldObject != (UnityEngine.Object) null && this.S.IgnoreRBs.Contains(this.HeldObject.O.RootRigidbody))
        this.S.IgnoreRBs.Remove(this.HeldObject.O.RootRigidbody);
      if (!this.IsHoldingObject)
        return;
      float magnitude = velocity.magnitude;
      velocity = velocity.normalized * Mathf.Max(magnitude, 5f);
      this.IsHoldingObject = false;
      this.HeldObject.SosigHoldingThis = (Sosig) null;
      this.HeldObject.IsHeldByBot = false;
      this.HeldObject.HandHoldingThis = (SosigHand) null;
      this.HeldObject.BotDrop();
      this.HeldObject.O.transform.position = this.HeldObject.O.transform.position + velocity.normalized;
      this.HeldObject.O.transform.rotation = Quaternion.LookRotation(targPoint - this.HeldObject.O.transform.position);
      this.HeldObject.O.RootRigidbody.velocity = velocity;
      this.HeldObject.O.RootRigidbody.angularVelocity = Vector3.zero;
      this.HeldObject.O.RootRigidbody.angularDrag = 3f;
      this.HeldObject = (SosigWeapon) null;
    }

    public void Hold()
    {
      if (!this.IsHoldingObject || (UnityEngine.Object) this.HeldObject == (UnityEngine.Object) null || (UnityEngine.Object) this.Root == (UnityEngine.Object) null)
        return;
      this.UpdateGunHandlingPose();
      Vector3 position1 = this.Target.position;
      Quaternion rotation1 = this.Target.rotation;
      Vector3 position2 = this.HeldObject.RecoilHolder.position;
      Quaternion rotation2 = this.HeldObject.RecoilHolder.rotation;
      if (this.HeldObject.O.IsHeld)
      {
        if ((double) Vector3.Distance(position1, position2) > 0.699999988079071)
        {
          this.DropHeldObject();
          return;
        }
      }
      else if ((double) Vector3.Distance(position1, position2) < 0.200000002980232)
      {
        this.m_timeAwayFromTarget = 0.0f;
      }
      else
      {
        this.m_timeAwayFromTarget += Time.deltaTime;
        if ((double) this.m_timeAwayFromTarget > 1.0)
        {
          this.HeldObject.O.RootRigidbody.position = position1;
          this.HeldObject.O.RootRigidbody.rotation = rotation1;
        }
      }
      if ((this.HeldObject.Type == SosigWeapon.SosigWeaponType.Melee || this.HeldObject.Type == SosigWeapon.SosigWeaponType.Grenade) && this.HeldObject.O.MP.IsMeleeWeapon)
        this.HeldObject.O.SetFakeHand((this.Target.position - this.m_lastPos) * (1f / Time.deltaTime), this.Target.position);
      float num1 = 0.0f;
      float num2 = 0.0f;
      float t = 0.0f;
      if ((UnityEngine.Object) this.m_posedToward != (UnityEngine.Object) null && this.Pose != SosigHand.SosigHandPose.Melee)
      {
        if (this.HasActiveAimPoint)
        {
          if (this.Pose == SosigHand.SosigHandPose.Aimed)
          {
            num1 = this.vertOffsets[this.m_curFiringPose_Aimed];
            num2 = this.forwardOffsets[this.m_curFiringPose_Aimed];
            t = this.tiltLerpOffsets[this.m_curFiringPose_Aimed];
          }
          else if (this.Pose == SosigHand.SosigHandPose.HipFire)
          {
            num1 = this.vertOffsets[this.m_curFiringPose_Hip];
            num2 = this.forwardOffsets[this.m_curFiringPose_Hip];
            t = this.tiltLerpOffsets[this.m_curFiringPose_Hip];
          }
        }
        Transform transform = this.S.Links[1].transform;
        float num3 = 4f;
        if (this.S.IsFrozen)
          num3 = 0.25f;
        if (this.S.IsSpeedUp)
          num3 = 8f;
        this.Target.position = Vector3.Lerp(position1, this.m_posedToward.position + transform.up * num1 + this.m_posedToward.forward * num2, Time.deltaTime * num3);
        this.Target.rotation = Quaternion.Slerp(rotation1, this.m_posedToward.rotation, Time.deltaTime * num3);
      }
      Vector3 vector3_1 = position2;
      Quaternion rotation3 = rotation2;
      Vector3 vector3_2 = position1;
      Quaternion quaternion1 = rotation1;
      if (this.HasActiveAimPoint && (this.Pose == SosigHand.SosigHandPose.HipFire || this.Pose == SosigHand.SosigHandPose.Aimed))
      {
        float num3 = 0.0f;
        float num4 = 0.0f;
        if (this.Pose == SosigHand.SosigHandPose.HipFire)
        {
          num3 = this.HeldObject.Hipfire_HorizontalLimit;
          num4 = this.HeldObject.Hipfire_VerticalLimit;
        }
        if (this.Pose == SosigHand.SosigHandPose.Aimed)
        {
          num3 = this.HeldObject.Aim_HorizontalLimit;
          num4 = this.HeldObject.Aim_VerticalLimit;
        }
        Vector3 vector3_3 = this.m_aimTowardPoint - position1;
        Vector3 forward = Vector3.RotateTowards(Vector3.RotateTowards(this.Target.forward, Vector3.ProjectOnPlane(vector3_3, this.Target.right), num4 * 0.0174533f, 0.0f), vector3_3, num3 * 0.0174533f, 0.0f);
        if ((double) t > 0.0)
        {
          Vector3 localPosition = this.Target.transform.localPosition;
          localPosition.z = 0.0f;
          localPosition.y = 0.0f;
          localPosition.Normalize();
          Vector3 upwards = Vector3.Slerp(this.Target.up, localPosition.x * -this.Target.right, t);
          quaternion1 = Quaternion.LookRotation(forward, upwards);
        }
        else
          quaternion1 = Quaternion.LookRotation(forward, this.Target.up);
      }
      Vector3 vector3_4 = vector3_2 - vector3_1;
      Quaternion quaternion2 = quaternion1 * Quaternion.Inverse(rotation3);
      float deltaTime = Time.deltaTime;
      float angle;
      Vector3 axis;
      quaternion2.ToAngleAxis(out angle, out axis);
      float num5 = 0.5f;
      if (this.S.IsConfused)
        num5 = 0.1f;
      if (this.S.IsStunned)
        num5 = 0.02f;
      if ((double) angle > 180.0)
        angle -= 360f;
      if ((double) angle != 0.0)
        this.HeldObject.O.RootRigidbody.angularVelocity = Vector3.MoveTowards(this.HeldObject.O.RootRigidbody.angularVelocity, deltaTime * angle * axis * this.S.AttachedRotationMultiplier * num5, this.S.AttachedRotationFudge * 0.5f * Time.fixedDeltaTime);
      this.HeldObject.O.RootRigidbody.velocity = Vector3.MoveTowards(this.HeldObject.O.RootRigidbody.velocity, vector3_4 * this.S.AttachedPositionMultiplier * 0.5f * deltaTime, this.S.AttachedPositionFudge * 0.5f * deltaTime);
      this.m_lastPos = this.Target.position;
    }

    public enum SosigHandPose
    {
      AtRest,
      HipFire,
      Aimed,
      Melee,
      ShieldHold,
    }
  }
}
