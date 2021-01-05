// Decompiled with JetBrains decompiler
// Type: FistVR.SuppressorInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SuppressorInterface : MuzzleDeviceInterface
  {
    private Suppressor tempSup;
    private Vector3 lastHandForward = Vector3.zero;
    private Vector3 lastMountForward = Vector3.zero;

    protected override void Awake()
    {
      base.Awake();
      this.tempSup = this.Attachment as Suppressor;
    }

    public override void OnAttach()
    {
      this.tempSup = this.Attachment as Suppressor;
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterSuppressor(this.tempSup);
      base.OnAttach();
    }

    public override void OnDetach()
    {
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterSuppressor((Suppressor) null);
      base.OnDetach();
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.lastHandForward = this.m_hand.transform.up;
      this.lastMountForward = this.Attachment.curMount.transform.up;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (this.IsHeld)
        this.Attachment.transform.localEulerAngles = new Vector3(0.0f, 0.0f, this.tempSup.CatchRot);
      if ((double) this.tempSup.CatchRot > 5.0)
      {
        this.IsLocked = true;
      }
      else
      {
        this.IsLocked = false;
        if ((double) Vector3.Distance(this.m_hand.transform.position, this.GetClosestValidPoint(this.Attachment.curMount.Point_Front.position, (this.Attachment.curMount.GetRootMount().MyObject as FVRFireArm).MuzzlePos.position, this.transform.position)) <= 0.0799999982118607)
          return;
        this.EndInteraction(hand);
        hand.ForceSetInteractable((FVRInteractiveObject) this.Attachment);
        this.Attachment.BeginInteraction(hand);
      }
    }

    protected override void FVRFixedUpdate()
    {
      if (this.IsHeld)
      {
        float catchRot = this.tempSup.CatchRot;
        Vector3 lhs1 = Vector3.ProjectOnPlane(this.m_hand.transform.up, this.transform.forward);
        Vector3 rhs1 = Vector3.ProjectOnPlane(this.lastHandForward, this.transform.forward);
        this.tempSup.CatchRot -= Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs1, rhs1)), Vector3.Dot(lhs1, rhs1)) * 57.29578f;
        Vector3 lhs2 = Vector3.ProjectOnPlane(this.Attachment.curMount.transform.up, this.transform.forward);
        Vector3 rhs2 = Vector3.ProjectOnPlane(this.lastMountForward, this.transform.forward);
        this.tempSup.CatchRot += Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
        this.tempSup.CatchRot = Mathf.Clamp(this.tempSup.CatchRot, 0.0f, 360f);
        this.tempSup.CatchRotDeltaAdd(Mathf.Abs(this.tempSup.CatchRot - catchRot));
        this.lastHandForward = this.m_hand.transform.up;
        this.lastMountForward = this.Attachment.curMount.transform.up;
      }
      base.FVRFixedUpdate();
    }
  }
}
