// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019BoltHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019BoltHandle : FVRInteractiveObject
  {
    public LAPD2019 Gun;
    public bool UsesQuickRelease;
    public Transform BoltActionHandleRoot;
    public Transform BoltActionHandle;
    public float BaseRotOffset;
    private float rotAngle;
    public float MinRot;
    public float MaxRot;
    public float UnlockThreshold = 70f;
    public Transform Point_Forward;
    public Transform Point_Rearward;
    public Vector3 HandPosOffset = new Vector3(0.0f, 0.0f, 0.0f);
    [Header("CartridgeDoor")]
    public Transform CartridgeDoor;
    private float m_cartridgeDoorClosed;
    private float m_cartridgeDoorOpen = 72f;
    private float m_curCartridgeDoorRot;
    public LAPD2019BoltHandle.BoltActionHandleState HandleState;
    public LAPD2019BoltHandle.BoltActionHandleState LastHandleState;
    public LAPD2019BoltHandle.BoltActionHandleRot HandleRot = LAPD2019BoltHandle.BoltActionHandleRot.Down;
    public LAPD2019BoltHandle.BoltActionHandleRot LastHandleRot = LAPD2019BoltHandle.BoltActionHandleRot.Down;

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      Vector3 a = this.BoltActionHandleRoot.position;
      bool flag = false;
      if (this.IsHeld)
      {
        if (this.HandleState == LAPD2019BoltHandle.BoltActionHandleState.Forward)
        {
          Vector3 rhs = Vector3.ProjectOnPlane(this.m_hand.Input.Pos - this.BoltActionHandle.position, this.BoltActionHandleRoot.transform.forward);
          rhs = rhs.normalized;
          Vector3 right = this.BoltActionHandleRoot.transform.right;
          this.rotAngle = Mathf.Atan2(Vector3.Dot(this.BoltActionHandleRoot.forward, Vector3.Cross(right, rhs)), Vector3.Dot(right, rhs)) * 57.29578f;
          this.rotAngle += this.BaseRotOffset;
          this.rotAngle = Mathf.Clamp(this.rotAngle, this.MinRot, this.MaxRot);
          this.BoltActionHandle.localEulerAngles = new Vector3(0.0f, 0.0f, this.rotAngle);
          this.CartridgeDoor.localEulerAngles = new Vector3(0.0f, 0.0f, Mathf.Lerp(this.m_cartridgeDoorClosed, this.m_cartridgeDoorOpen, Mathf.InverseLerp(this.MinRot, this.MaxRot, this.rotAngle)));
          this.HandleRot = (double) Mathf.Abs(this.rotAngle - this.MaxRot) >= 2.0 ? ((double) Mathf.Abs(this.rotAngle - this.MinRot) >= 2.0 ? LAPD2019BoltHandle.BoltActionHandleRot.Mid : LAPD2019BoltHandle.BoltActionHandleRot.Down) : LAPD2019BoltHandle.BoltActionHandleRot.Up;
          if (this.HandleRot == LAPD2019BoltHandle.BoltActionHandleRot.Up && this.LastHandleRot != LAPD2019BoltHandle.BoltActionHandleRot.Up)
            this.Gun.PlayAudioEvent(FirearmAudioEventType.HandleUp);
          else if (this.HandleRot == LAPD2019BoltHandle.BoltActionHandleRot.Down && this.LastHandleRot != LAPD2019BoltHandle.BoltActionHandleRot.Down)
            this.Gun.PlayAudioEvent(FirearmAudioEventType.HandleDown);
          this.LastHandleRot = this.HandleRot;
        }
        if ((double) this.rotAngle >= (double) this.UnlockThreshold)
        {
          a = this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, this.m_hand.Input.Pos - (this.HandPosOffset.x * this.BoltActionHandleRoot.right + this.HandPosOffset.y * this.BoltActionHandleRoot.up + this.HandPosOffset.z * this.BoltActionHandleRoot.forward));
          flag = true;
        }
      }
      else if (this.HandleState != LAPD2019BoltHandle.BoltActionHandleState.Forward)
      {
        a = Vector3.Lerp(a, this.Point_Forward.position, Time.deltaTime * 12f);
        flag = true;
      }
      if (flag)
        this.BoltActionHandleRoot.position = a;
      float num = Mathf.InverseLerp(this.Point_Forward.localPosition.z, this.Point_Rearward.localPosition.z, this.BoltActionHandleRoot.localPosition.z);
      this.HandleState = (double) num >= 0.0500000007450581 ? ((double) num <= 0.949999988079071 ? LAPD2019BoltHandle.BoltActionHandleState.Mid : LAPD2019BoltHandle.BoltActionHandleState.Rear) : LAPD2019BoltHandle.BoltActionHandleState.Forward;
      if (this.HandleState == LAPD2019BoltHandle.BoltActionHandleState.Forward && this.LastHandleState != LAPD2019BoltHandle.BoltActionHandleState.Forward)
        this.Gun.PlayAudioEvent(FirearmAudioEventType.HandleForward);
      else if (this.HandleState == LAPD2019BoltHandle.BoltActionHandleState.Rear && this.LastHandleState != LAPD2019BoltHandle.BoltActionHandleState.Rear)
      {
        this.Gun.PlayAudioEvent(FirearmAudioEventType.HandleBack);
        this.Gun.EjectThermalClip();
      }
      this.LastHandleState = this.HandleState;
    }

    public override void UpdateInteraction(FVRViveHand hand) => base.UpdateInteraction(hand);

    public enum BoltActionHandleState
    {
      Forward,
      Mid,
      Rear,
    }

    public enum BoltActionHandleRot
    {
      Up,
      Mid,
      Down,
    }
  }
}
