// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmTopCover
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmTopCover : FVRInteractiveObject
  {
    public FVRFireArm FireArm;

    public override void BeginInteraction(FVRViveHand hand)
    {
      if ((double) Mathf.Clamp(Vector3.Angle(Vector3.ProjectOnPlane(hand.Input.Pos - this.transform.position, this.transform.right), -this.FireArm.gameObject.transform.forward), 0.0f, 90f) < 10.0)
        this.FireArm.PlayAudioEvent(FirearmAudioEventType.TopCoverUp);
      base.BeginInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 from = Vector3.ProjectOnPlane(hand.Input.Pos - this.transform.position, this.transform.right);
      float x = Mathf.Clamp(Vector3.Angle(from, -this.FireArm.gameObject.transform.forward), 0.0f, 90f);
      if ((double) Vector3.Angle(from, this.FireArm.gameObject.transform.up) > 90.0)
        x = 0.0f;
      this.transform.localEulerAngles = new Vector3(x, 0.0f, 0.0f);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      Vector3 from = Vector3.ProjectOnPlane(hand.Input.Pos - this.transform.position, this.transform.right);
      float x = Mathf.Clamp(Vector3.Angle(from, -this.FireArm.gameObject.transform.forward), 0.0f, 90f);
      if ((double) Vector3.Angle(from, this.FireArm.gameObject.transform.up) > 90.0)
        x = 0.0f;
      this.FireArm.IsTopCoverUp = true;
      if ((double) x > 75.0)
        x = 90f;
      if ((double) x < 15.0)
      {
        this.FireArm.IsTopCoverUp = false;
        x = 0.0f;
        this.FireArm.PlayAudioEvent(FirearmAudioEventType.TopCoverDown);
      }
      this.transform.localEulerAngles = new Vector3(x, 0.0f, 0.0f);
      base.EndInteraction(hand);
    }
  }
}
