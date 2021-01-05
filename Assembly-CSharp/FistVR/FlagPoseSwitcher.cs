// Decompiled with JetBrains decompiler
// Type: FistVR.FlagPoseSwitcher
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlagPoseSwitcher : FVRFireArmAttachmentInterface
  {
    public Transform Flag;
    public Transform[] Poses;
    private int m_index;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.NextPose();
      }
      else if (hand.Input.TouchpadDown)
      {
        if ((double) touchpadAxes.magnitude > 0.25 && (double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0)
          this.NextPose();
        if ((double) touchpadAxes.magnitude > 0.25 && (double) Vector2.Angle(touchpadAxes, Vector2.right) <= 45.0)
          this.PrevPose();
      }
      base.UpdateInteraction(hand);
    }

    private void NextPose()
    {
      ++this.m_index;
      if (this.m_index >= this.Poses.Length)
        this.m_index = 0;
      this.Flag.localPosition = this.Poses[this.m_index].localPosition;
      this.Flag.localRotation = this.Poses[this.m_index].localRotation;
    }

    private void PrevPose()
    {
      --this.m_index;
      if (this.m_index < 0)
        this.m_index = this.Poses.Length - 1;
      this.Flag.localPosition = this.Poses[this.m_index].localPosition;
      this.Flag.localRotation = this.Poses[this.m_index].localRotation;
    }
  }
}
