// Decompiled with JetBrains decompiler
// Type: FistVR.Balisong
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class Balisong : FVRMeleeWeapon
  {
    public List<WaggleJoint> Joints;
    public List<Transform> JointTransforms;
    private bool m_isLocked;
    public Vector2 BladeYRotRange = new Vector2(-90f, 90f);
    public Vector2 SwingYRotRange = new Vector2(-87.2f, 87.4f);
    public Vector2 LockYRotRange = new Vector2(-92.77f, 90f);
    private Balisong.BalisongState bState;
    public AudioEvent AudEvent_BalisongLatch;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (this.bState != Balisong.BalisongState.Free)
        return;
      for (int index = 0; index < this.Joints.Count; ++index)
        this.Joints[index].Execute();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.IsHeld || !this.m_hand.Input.TriggerDown || !this.m_hasTriggeredUpSinceBegin)
        return;
      this.ToggleBalisonLock();
    }

    private void ToggleBalisonLock()
    {
      if (this.MP.IsJointedToObject)
        return;
      if (this.bState == Balisong.BalisongState.LockedClosed || this.bState == Balisong.BalisongState.LockedOpen)
      {
        this.bState = Balisong.BalisongState.Free;
        this.SetLockRot(0.5f);
        this.MP.CanNewStab = false;
        for (int index = 0; index < this.Joints.Count; ++index)
          this.Joints[index].ResetParticlePos();
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_BalisongLatch, this.transform.position);
      }
      else if (this.CanLockOpen())
      {
        this.SetBalisonRot(1f);
        this.bState = Balisong.BalisongState.LockedOpen;
        this.SetLockRot(1f);
        this.MP.CanNewStab = true;
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_BalisongLatch, this.transform.position);
      }
      else
      {
        if (!this.CanLockShut())
          return;
        this.SetBalisonRot(0.0f);
        this.bState = Balisong.BalisongState.LockedClosed;
        this.SetLockRot(0.0f);
        this.MP.CanNewStab = false;
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.AudEvent_BalisongLatch, this.transform.position);
      }
    }

    private bool CanLockOpen()
    {
      float num1 = Mathf.Abs(this.JointTransforms[0].localEulerAngles.x - this.BladeYRotRange.y);
      float num2 = Mathf.Abs(this.JointTransforms[1].localEulerAngles.x - this.SwingYRotRange.y);
      return (double) num1 < 3.0 && (double) num2 < 3.0;
    }

    private bool CanLockShut()
    {
      float x1 = this.JointTransforms[0].localEulerAngles.x;
      float x2 = this.JointTransforms[1].localEulerAngles.x;
      if ((double) x1 < -90.0999984741211)
        x1 += 180f;
      if ((double) x2 < -90.0999984741211)
        x2 += 180f;
      float f1 = Mathf.Abs(x1 - this.BladeYRotRange.x);
      float f2 = Mathf.Abs(x2 - this.SwingYRotRange.x);
      if ((double) Mathf.Abs(f1) > 359.0)
        f1 = Mathf.Abs(f1) - 359f;
      if ((double) Mathf.Abs(f2) > 359.0)
        f2 = Mathf.Abs(f2) - 359f;
      return (double) f1 < 3.0 && (double) f2 < 3.0;
    }

    private void SetBalisonRot(float f)
    {
      this.JointTransforms[0].localEulerAngles = new Vector3(Mathf.Lerp(this.BladeYRotRange.x, this.BladeYRotRange.y, f), 0.0f, 0.0f);
      this.JointTransforms[1].localEulerAngles = new Vector3(Mathf.Lerp(this.SwingYRotRange.x, this.SwingYRotRange.y, f), 0.0f, 0.0f);
    }

    private void SetLockRot(float f) => this.JointTransforms[2].localEulerAngles = new Vector3(Mathf.Lerp(this.LockYRotRange.x, this.LockYRotRange.y, f), 0.0f, 0.0f);

    public enum BalisongState
    {
      LockedClosed,
      LockedOpen,
      Free,
    }
  }
}
