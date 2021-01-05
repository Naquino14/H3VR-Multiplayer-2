// Decompiled with JetBrains decompiler
// Type: FistVR.FVRHandGrabPoint
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRHandGrabPoint : FVRInteractiveObject
  {
    private Vector3 lastHandPos = Vector3.zero;
    private FVRMovementManager m_manager;
    private Vector3 MoveDir;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.m_manager = hand.MovementManager;
      this.m_manager.BeginGrabPointMove(this);
      this.lastHandPos = hand.Input.Pos;
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_manager.EndGrabPointMove(this);
      base.EndInteraction(hand);
      if (!((Object) hand.OtherHand.CurrentInteractable == (Object) null) || !((Object) hand.OtherHand.ClosestPossibleInteractable == (Object) this) || !hand.OtherHand.Input.IsGrabbing)
        return;
      this.BeginInteraction(hand.OtherHand);
      this.m_hand.ForceSetInteractable((FVRInteractiveObject) this);
    }
  }
}
