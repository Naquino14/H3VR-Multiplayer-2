// Decompiled with JetBrains decompiler
// Type: FistVR.M79SightingSystemBacksight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class M79SightingSystemBacksight : FVRInteractiveObject
  {
    public M79SightingSystemBase SightBase;
    public Transform MinPoint;
    public Transform MaxPoint;

    public override bool IsInteractable() => this.SightBase.IsFlippedUp;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.transform.position = this.GetClosestValidPointBackSight(hand.transform.position);
    }

    private Vector3 GetClosestValidPointBackSight(Vector3 vPoint) => this.GetClosestValidPoint(this.MaxPoint.position, this.MinPoint.position, vPoint);
  }
}
