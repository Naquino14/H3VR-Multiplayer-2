// Decompiled with JetBrains decompiler
// Type: FistVR.Mac11_Stock
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Mac11_Stock : FVRInteractiveObject
  {
    [Header("Mac11 Stock Config")]
    public Transform ForwardPos;
    public Transform RearPos;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.transform.position = this.GetClosestValidPoint(this.ForwardPos.position, this.RearPos.position, hand.transform.position);
    }
  }
}
