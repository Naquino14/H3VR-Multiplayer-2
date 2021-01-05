// Decompiled with JetBrains decompiler
// Type: FistVR.SLAM_Detonator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SLAM_Detonator : FVRPhysicalObject
  {
    public Transform Trigger;
    public Vector2 TriggerRange;
    private float m_triggerfloat;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin)
        return;
      this.m_triggerfloat = hand.Input.TriggerFloat;
      float val = Mathf.Lerp(this.TriggerRange.x, this.TriggerRange.y, this.m_triggerfloat);
      if (hand.Input.TriggerDown)
        FXM.DetonateSPAAMS();
      this.SetAnimatedComponent(this.Trigger, val, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      base.EndInteraction(hand);
      this.m_triggerfloat = 0.0f;
      this.SetAnimatedComponent(this.Trigger, this.TriggerRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
    }
  }
}
