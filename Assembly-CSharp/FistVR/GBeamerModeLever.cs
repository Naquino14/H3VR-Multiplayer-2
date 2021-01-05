// Decompiled with JetBrains decompiler
// Type: FistVR.GBeamerModeLever
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class GBeamerModeLever : FVRInteractiveObject
  {
    public GBeamer GBeam;
    public Transform Point_Forward;
    public Transform Point_Rearward;
    public GBeamerModeLever.LeverMode Mode = GBeamerModeLever.LeverMode.Rearward;

    public override void BeginInteraction(FVRViveHand hand) => base.BeginInteraction(hand);

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      Vector3 closestValidPoint = this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, hand.Input.Pos);
      if ((double) Vector3.Distance(closestValidPoint, this.Point_Forward.position) < (double) Vector3.Distance(closestValidPoint, this.Point_Rearward.position))
      {
        this.Mode = GBeamerModeLever.LeverMode.Forward;
        this.transform.rotation = Quaternion.LookRotation(this.Point_Forward.position - this.transform.position, this.GBeam.transform.right);
      }
      else
      {
        this.Mode = GBeamerModeLever.LeverMode.Rearward;
        this.transform.rotation = Quaternion.LookRotation(this.Point_Rearward.position - this.transform.position, this.GBeam.transform.right);
      }
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 closestValidPoint = this.GetClosestValidPoint(this.Point_Forward.position, this.Point_Rearward.position, hand.Input.Pos);
      this.transform.rotation = Quaternion.LookRotation(closestValidPoint - this.transform.position, this.GBeam.transform.right);
      if ((double) Vector3.Distance(closestValidPoint, this.Point_Forward.position) < 0.100000001490116)
        this.Mode = GBeamerModeLever.LeverMode.Forward;
      else if ((double) Vector3.Distance(closestValidPoint, this.Point_Rearward.position) < 0.100000001490116)
        this.Mode = GBeamerModeLever.LeverMode.Rearward;
      else
        this.Mode = GBeamerModeLever.LeverMode.InBetween;
    }

    public enum LeverMode
    {
      Forward,
      Rearward,
      InBetween,
    }
  }
}
