// Decompiled with JetBrains decompiler
// Type: FistVR.ShotgunMoveableStock
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ShotgunMoveableStock : FVRInteractiveObject
  {
    public Transform ForwardPoint;
    public Transform RearwardPoint;
    public FVRFireArm Firearm;
    [Header("SecondPoint")]
    public bool HasSecondPoint;
    public Transform SecondPoint;
    public Transform SecondForwardPoint;
    public Transform SecondRearwardPoint;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.transform.position = this.GetClosestValidPoint(this.ForwardPoint.position, this.RearwardPoint.position, this.m_handPos);
      float t = Mathf.InverseLerp(this.ForwardPoint.localPosition.z, this.RearwardPoint.localPosition.z, this.transform.localPosition.z);
      if (!this.HasSecondPoint)
        return;
      this.SecondPoint.position = Vector3.Lerp(this.SecondForwardPoint.position, this.SecondRearwardPoint.position, t);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      if (!((Object) this.Firearm != (Object) null))
        return;
      float num1 = Vector3.Distance(this.transform.position, this.ForwardPoint.position);
      float num2 = Vector3.Distance(this.transform.position, this.RearwardPoint.position);
      if ((double) num1 < 0.00999999977648258)
      {
        this.Firearm.PlayAudioEvent(FirearmAudioEventType.StockClosed);
      }
      else
      {
        if ((double) num2 >= 0.00999999977648258)
          return;
        this.Firearm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
      }
    }
  }
}
