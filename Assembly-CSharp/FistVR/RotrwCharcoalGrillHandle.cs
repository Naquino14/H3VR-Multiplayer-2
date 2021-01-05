// Decompiled with JetBrains decompiler
// Type: FistVR.RotrwCharcoalGrillHandle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RotrwCharcoalGrillHandle : FVRInteractiveObject
  {
    public Transform RefVector;
    public AudioEvent AudEvent_DoorOpen;
    public AudioEvent AudEvent_DoorClose;

    public bool IsLidClosed() => (double) Vector3.Angle(this.transform.forward, this.RefVector.forward) < 2.0;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      SM.PlayGenericSound(this.AudEvent_DoorOpen, this.transform.position);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 vector3 = Vector3.ProjectOnPlane(hand.Input.Pos - this.transform.position, this.RefVector.right);
      if ((double) Vector3.Angle(vector3, this.RefVector.up) >= 90.0)
        vector3 = this.RefVector.forward;
      else if ((double) Vector3.Angle(vector3, this.RefVector.forward) >= 90.0)
        vector3 = this.RefVector.up;
      this.transform.rotation = Quaternion.LookRotation(vector3, this.RefVector.right);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      if ((double) Vector3.Angle(this.transform.forward, this.RefVector.up) < 20.0)
      {
        this.transform.rotation = Quaternion.LookRotation(this.RefVector.up, this.RefVector.right);
      }
      else
      {
        if ((double) Vector3.Angle(this.transform.forward, this.RefVector.forward) >= 20.0)
          return;
        this.transform.rotation = Quaternion.LookRotation(this.RefVector.forward, this.RefVector.right);
        SM.PlayGenericSound(this.AudEvent_DoorClose, this.transform.position);
      }
    }
  }
}
