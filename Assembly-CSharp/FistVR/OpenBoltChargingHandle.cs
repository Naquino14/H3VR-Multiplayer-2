// Decompiled with JetBrains decompiler
// Type: FistVR.OpenBoltChargingHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class OpenBoltChargingHandle : FVRInteractiveObject
  {
    [Header("ChargingHandle")]
    public OpenBoltReceiver Receiver;
    public Transform Point_Fore;
    public Transform Point_Rear;
    public OpenBoltReceiverBolt Bolt;
    public float ForwardSpeed = 1f;
    private float m_boltZ_forward;
    private float m_boltZ_rear;
    private float m_currentHandleZ;
    public OpenBoltChargingHandle.BoltHandlePos CurPos;
    public OpenBoltChargingHandle.BoltHandlePos LastPos;
    [Header("Rotating Bit")]
    public bool HasRotatingPart;
    public Transform RotatingPart;
    public Vector3 RotatingPartNeutralEulers;
    public Vector3 RotatingPartLeftEulers;
    public Vector3 RotatingPartRightEulers;

    protected override void Awake()
    {
      base.Awake();
      this.m_boltZ_forward = this.Point_Fore.localPosition.z;
      this.m_boltZ_rear = this.Point_Rear.localPosition.z;
      this.m_currentHandleZ = this.transform.localPosition.z;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.transform.position = this.GetClosestValidPoint(this.Point_Fore.position, this.Point_Rear.position, this.m_hand.Input.Pos);
      this.m_currentHandleZ = this.transform.localPosition.z;
      this.Bolt.ChargingHandleHeld(Mathf.InverseLerp(this.m_boltZ_forward, this.m_boltZ_rear, this.m_currentHandleZ));
      if (!this.HasRotatingPart)
        return;
      if ((double) Vector3.Dot((this.transform.position - this.m_hand.PalmTransform.position).normalized, this.transform.right) > 0.0)
        this.RotatingPart.localEulerAngles = this.RotatingPartLeftEulers;
      else
        this.RotatingPart.localEulerAngles = this.RotatingPartRightEulers;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      if (this.HasRotatingPart)
        this.RotatingPart.localEulerAngles = this.RotatingPartNeutralEulers;
      base.EndInteraction(hand);
      this.Bolt.ChargingHandleReleased();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.IsHeld && (double) Mathf.Abs(this.m_currentHandleZ - this.m_boltZ_forward) > 1.0 / 1000.0)
      {
        this.m_currentHandleZ = Mathf.MoveTowards(this.m_currentHandleZ, this.m_boltZ_forward, Time.deltaTime * this.ForwardSpeed);
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.m_currentHandleZ);
      }
      this.CurPos = (double) Mathf.Abs(this.m_currentHandleZ - this.m_boltZ_forward) >= 0.00499999988824129 ? ((double) Mathf.Abs(this.m_currentHandleZ - this.m_boltZ_rear) >= 0.00499999988824129 ? OpenBoltChargingHandle.BoltHandlePos.Middle : OpenBoltChargingHandle.BoltHandlePos.Rear) : OpenBoltChargingHandle.BoltHandlePos.Forward;
      if (this.CurPos == OpenBoltChargingHandle.BoltHandlePos.Forward && this.LastPos != OpenBoltChargingHandle.BoltHandlePos.Forward)
      {
        if ((Object) this.Receiver != (Object) null)
          this.Receiver.PlayAudioEvent(FirearmAudioEventType.HandleForward);
      }
      else if (this.CurPos == OpenBoltChargingHandle.BoltHandlePos.Rear && this.LastPos != OpenBoltChargingHandle.BoltHandlePos.Rear && (Object) this.Receiver != (Object) null)
        this.Receiver.PlayAudioEvent(FirearmAudioEventType.HandleBack);
      this.LastPos = this.CurPos;
    }

    public enum BoltHandlePos
    {
      Forward,
      Middle,
      Rear,
    }
  }
}
