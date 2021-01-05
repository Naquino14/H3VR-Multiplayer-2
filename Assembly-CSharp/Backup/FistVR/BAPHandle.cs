// Decompiled with JetBrains decompiler
// Type: FistVR.BAPHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BAPHandle : FVRInteractiveObject
  {
    public BAP Frame;
    public Transform HandleRoot;
    public Transform Handle;
    private float m_rotAngle;
    public float MinRot;
    public float MaxRot = 90f;
    public float UnlockThreshold = 87f;
    public Transform Point_Forward;
    public Transform Point_Rearward;
    public BAPHandle.HandleSlideState CurHandleSlideState;
    public BAPHandle.HandleSlideState LastHandleSlideState;
    public BAPHandle.HandleRotState CurHandleRotState;
    public BAPHandle.HandleRotState LastHandleRotState;
    private Vector3 lastHandForward = Vector3.zero;
    private Vector3 lastMountForward = Vector3.zero;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.lastHandForward = this.m_hand.transform.up;
      this.lastMountForward = this.Frame.transform.up;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      if (this.CurHandleSlideState == BAPHandle.HandleSlideState.Forward)
      {
        float rotAngle1 = this.m_rotAngle;
        float rotAngle2 = this.m_rotAngle;
        Vector3 lhs1 = Vector3.ProjectOnPlane(hand.transform.up, this.Frame.transform.forward);
        Vector3 rhs1 = Vector3.ProjectOnPlane(this.lastHandForward, this.Frame.transform.forward);
        float num1 = Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs1, rhs1)), Vector3.Dot(lhs1, rhs1)) * 57.29578f;
        float num2 = rotAngle2 - num1;
        Vector3 lhs2 = Vector3.ProjectOnPlane(this.Frame.transform.up, this.transform.forward);
        Vector3 rhs2 = Vector3.ProjectOnPlane(this.lastMountForward, this.transform.forward);
        float num3 = Mathf.Atan2(Vector3.Dot(this.transform.forward, Vector3.Cross(lhs2, rhs2)), Vector3.Dot(lhs2, rhs2)) * 57.29578f;
        this.m_rotAngle = Mathf.Clamp(num2 + num3, this.MinRot, this.MaxRot);
        this.lastHandForward = this.m_hand.transform.up;
        this.lastMountForward = this.Frame.transform.up;
        this.Handle.localEulerAngles = new Vector3(0.0f, 0.0f, this.m_rotAngle);
        this.CurHandleRotState = (double) this.m_rotAngle < (double) this.UnlockThreshold ? ((double) Mathf.Abs(this.m_rotAngle - this.MinRot) >= 3.0 ? BAPHandle.HandleRotState.Mid : BAPHandle.HandleRotState.Closed) : BAPHandle.HandleRotState.Open;
        if (this.CurHandleRotState == BAPHandle.HandleRotState.Open && this.LastHandleRotState != BAPHandle.HandleRotState.Open)
          this.Frame.PlayAudioEvent(FirearmAudioEventType.HandleUp);
        else if (this.CurHandleRotState == BAPHandle.HandleRotState.Closed && this.LastHandleRotState != BAPHandle.HandleRotState.Closed)
        {
          this.HandleRoot.localPosition = this.Point_Forward.localPosition;
          this.Frame.PlayAudioEvent(FirearmAudioEventType.HandleDown);
        }
        this.LastHandleRotState = this.CurHandleRotState;
      }
      if ((double) this.m_rotAngle >= (double) this.UnlockThreshold)
        this.HandleRoot.position = this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, hand.Input.FilteredPos);
      float BoltLerp = Mathf.InverseLerp(this.Point_Forward.localPosition.z, this.Point_Rearward.localPosition.z, this.HandleRoot.localPosition.z);
      this.CurHandleSlideState = (double) BoltLerp >= 0.0500000007450581 ? ((double) BoltLerp <= 0.949999988079071 ? BAPHandle.HandleSlideState.Mid : BAPHandle.HandleSlideState.Rear) : BAPHandle.HandleSlideState.Forward;
      this.Frame.UpdateBolt(BoltLerp);
      this.LastHandleSlideState = this.CurHandleSlideState;
    }

    public enum HandleSlideState
    {
      Forward,
      Mid,
      Rear,
    }

    public enum HandleRotState
    {
      Closed,
      Mid,
      Open,
    }
  }
}
