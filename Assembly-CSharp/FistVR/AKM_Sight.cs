// Decompiled with JetBrains decompiler
// Type: FistVR.AKM_Sight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AKM_Sight : FVRInteractiveObject
  {
    public Transform Gun;
    public Transform SightPoint0;
    public Transform SightPoint1;
    public Transform SightPoint2;
    public Transform SightRoundPart;
    public Transform SightStraightPart;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector3 closestValidPoint1 = this.GetClosestValidPoint(this.SightPoint0.position, this.SightPoint1.position, hand.transform.position);
      Vector3 closestValidPoint2 = this.GetClosestValidPoint(this.SightPoint1.position, this.SightPoint2.position, hand.transform.position);
      this.SightRoundPart.position = (double) Vector3.Distance(closestValidPoint1, hand.transform.position) > (double) Vector3.Distance(closestValidPoint2, hand.transform.position) ? closestValidPoint2 : closestValidPoint1;
      this.SightStraightPart.rotation = Quaternion.LookRotation(-(this.SightRoundPart.position - this.SightStraightPart.position).normalized, this.Gun.up);
      base.UpdateInteraction(hand);
    }
  }
}
