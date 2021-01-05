// Decompiled with JetBrains decompiler
// Type: FistVR.RailCam
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RailCam : FVRFireArmAttachmentInterface
  {
    public AudioEvent AudEvent_Trigger;
    public Transform CamPoint;
    private bool m_isEngaged;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (hand.IsInStreamlinedMode)
      {
        if (!hand.Input.BYButtonDown)
          return;
        GM.CurrentSceneSettings.SetCamObjectPoint(this.CamPoint);
        this.m_isEngaged = true;
        SM.PlayGenericSound(this.AudEvent_Trigger, this.transform.position);
      }
      else
      {
        if (!hand.Input.TouchpadDown || (double) Vector2.Angle(Vector2.up, hand.Input.TouchpadAxes) > 90.0)
          return;
        GM.CurrentSceneSettings.SetCamObjectPoint(this.CamPoint);
        this.m_isEngaged = true;
        SM.PlayGenericSound(this.AudEvent_Trigger, this.transform.position);
      }
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.m_isEngaged || !((Object) GM.CurrentSceneSettings.GetCamObjectPoint() != (Object) this.CamPoint))
        return;
      this.m_isEngaged = false;
      SM.PlayGenericSound(this.AudEvent_Trigger, this.transform.position);
    }
  }
}
