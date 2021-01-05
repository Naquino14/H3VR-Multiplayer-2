// Decompiled with JetBrains decompiler
// Type: FistVR.wwEventPuzzle_Churner_ButterStick
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwEventPuzzle_Churner_ButterStick : FVRPhysicalObject
  {
    public wwEventPuzzle_Churner Churner;
    public int ButterIndex;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      FVRViveHand fvrViveHand = hand;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position) >= 0.150000005960464)
        return;
      this.EndInteraction(hand);
      fvrViveHand.ForceSetInteractable((FVRInteractiveObject) null);
      this.Churner.AteButter(this.ButterIndex, GM.CurrentPlayerBody.Head.transform.position);
      Object.Destroy((Object) this.gameObject);
    }
  }
}
