// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.SkeletonUIOptions
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  public class SkeletonUIOptions : MonoBehaviour
  {
    public void AnimateHandWithController()
    {
      for (int index = 0; index < Player.instance.hands.Length; ++index)
      {
        Hand hand = Player.instance.hands[index];
        if ((Object) hand != (Object) null)
          hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithController);
      }
    }

    public void AnimateHandWithoutController()
    {
      for (int index = 0; index < Player.instance.hands.Length; ++index)
      {
        Hand hand = Player.instance.hands[index];
        if ((Object) hand != (Object) null)
          hand.SetSkeletonRangeOfMotion(EVRSkeletalMotionRange.WithoutController);
      }
    }

    public void ShowController()
    {
      for (int index = 0; index < Player.instance.hands.Length; ++index)
      {
        Hand hand = Player.instance.hands[index];
        if ((Object) hand != (Object) null)
          hand.ShowController(true);
      }
    }

    public void SetRenderModel(RenderModelChangerUI prefabs)
    {
      for (int index = 0; index < Player.instance.hands.Length; ++index)
      {
        Hand hand = Player.instance.hands[index];
        if ((Object) hand != (Object) null)
        {
          if (hand.handType == SteamVR_Input_Sources.RightHand)
            hand.SetRenderModel(prefabs.rightPrefab);
          if (hand.handType == SteamVR_Input_Sources.LeftHand)
            hand.SetRenderModel(prefabs.leftPrefab);
        }
      }
    }

    public void HideController()
    {
      for (int index = 0; index < Player.instance.hands.Length; ++index)
      {
        Hand hand = Player.instance.hands[index];
        if ((Object) hand != (Object) null)
          hand.HideController(true);
      }
    }
  }
}
