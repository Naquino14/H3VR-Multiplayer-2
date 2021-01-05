// Decompiled with JetBrains decompiler
// Type: FistVR.Mac11_StockButt
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Mac11_StockButt : FVRInteractiveObject
  {
    [Header("Mac11 Stock Butt Config")]
    public Transform Stock;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      Vector3 from = Vector3.ProjectOnPlane(hand.transform.position - this.transform.position, this.Stock.right);
      this.transform.localEulerAngles = new Vector3((double) Vector3.Angle(from, this.Stock.up) <= 90.0 ? ((double) Vector3.Angle(from, this.Stock.forward) >= 90.0 ? Vector3.Angle(from, this.Stock.up) * -1f : 0.0f) : -90f, 0.0f, 0.0f);
    }
  }
}
