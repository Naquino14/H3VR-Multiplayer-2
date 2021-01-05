// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltRotatingChargingHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace FistVR
{
  public class OpenBoltRotatingChargingHandle : FVRInteractiveObject
  {
    [Header("ChargingHandle")]
    public Transform Handle;
    public Transform ReferenceVector;
    public float RotLimit;
    public OpenBoltReceiverBolt Bolt;
    public float ForwardSpeed = 360f;
    private float m_currentHandleZ;
    private OpenBoltRotatingChargingHandle.Placement m_curPos;
    private OpenBoltRotatingChargingHandle.Placement m_lastPos;

    protected override void Awake()
    {
      base.Awake();
      this.m_currentHandleZ = this.RotLimit;
      this.Handle.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(this.m_currentHandleZ, this.ReferenceVector.up) * this.ReferenceVector.forward, this.ReferenceVector.up);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      float num = this.AngleSigned(this.ReferenceVector.forward, Vector3.RotateTowards(this.ReferenceVector.forward, Vector3.ProjectOnPlane(this.m_hand.Input.Pos - this.Handle.transform.position, this.ReferenceVector.up), (float) Math.PI / 180f * this.RotLimit, 1f), this.ReferenceVector.up);
      this.m_currentHandleZ = num;
      this.Handle.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(this.m_currentHandleZ, this.ReferenceVector.up) * this.ReferenceVector.forward, this.ReferenceVector.up);
      this.Bolt.ChargingHandleHeld(Mathf.InverseLerp(this.RotLimit, -this.RotLimit, num));
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.Bolt.ChargingHandleReleased();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      float num = Mathf.InverseLerp(this.RotLimit, -this.RotLimit, this.m_currentHandleZ);
      this.m_curPos = (double) num >= 0.00999999977648258 ? ((double) num <= 0.990000009536743 ? OpenBoltRotatingChargingHandle.Placement.Middle : OpenBoltRotatingChargingHandle.Placement.Rearward) : OpenBoltRotatingChargingHandle.Placement.Forward;
      if (!this.IsHeld && (double) Mathf.Abs(this.m_currentHandleZ - this.RotLimit) >= 0.00999999977648258)
      {
        this.m_currentHandleZ = Mathf.MoveTowards(this.m_currentHandleZ, this.RotLimit, Time.deltaTime * this.ForwardSpeed);
        this.Handle.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(this.m_currentHandleZ, this.ReferenceVector.up) * this.ReferenceVector.forward, this.ReferenceVector.up);
      }
      if (this.m_curPos == OpenBoltRotatingChargingHandle.Placement.Forward && this.m_lastPos != OpenBoltRotatingChargingHandle.Placement.Forward)
        this.Bolt.Receiver.PlayAudioEvent(FirearmAudioEventType.HandleForward);
      else if (this.m_lastPos == OpenBoltRotatingChargingHandle.Placement.Rearward && this.m_lastPos != OpenBoltRotatingChargingHandle.Placement.Rearward)
        this.Bolt.Receiver.PlayAudioEvent(FirearmAudioEventType.HandleBack);
      this.m_lastPos = this.m_curPos;
    }

    public float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n) => Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;

    public enum Placement
    {
      Forward,
      Middle,
      Rearward,
    }
  }
}
