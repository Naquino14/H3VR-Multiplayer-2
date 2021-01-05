// Decompiled with JetBrains decompiler
// Type: FistVR.FVRVerticleRope
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRVerticleRope : FVRInteractiveObject
  {
    private Vector3 lastHandPos = Vector3.zero;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.lastHandPos = hand.transform.localPosition;
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.m_hand.WholeRig.position += new Vector3(0.0f, this.lastHandPos.y - this.m_hand.transform.localPosition.y, 0.0f);
      this.lastHandPos = this.m_hand.transform.localPosition;
    }

    public void LateUpdate()
    {
    }
  }
}
