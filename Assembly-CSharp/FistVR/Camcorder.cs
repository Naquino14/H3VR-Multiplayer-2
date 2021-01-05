// Decompiled with JetBrains decompiler
// Type: FistVR.Camcorder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class Camcorder : FVRPhysicalObject
  {
    public Transform CamPoint;
    public Transform Trigger;
    private bool m_isEngaged;
    public Vector2 trig;
    public AudioEvent AudEvent_Trigger;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.m_hasTriggeredUpSinceBegin || !hand.Input.TriggerDown)
        return;
      GM.CurrentSceneSettings.SetCamObjectPoint(this.CamPoint);
      this.m_isEngaged = true;
      this.Trigger.localEulerAngles = new Vector3(this.trig.y, 0.0f, 0.0f);
      SM.PlayGenericSound(this.AudEvent_Trigger, this.transform.position);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!this.m_isEngaged || !((Object) GM.CurrentSceneSettings.GetCamObjectPoint() != (Object) this.CamPoint))
        return;
      this.m_isEngaged = false;
      this.Trigger.localEulerAngles = new Vector3(this.trig.x, 0.0f, 0.0f);
      SM.PlayGenericSound(this.AudEvent_Trigger, this.transform.position);
    }
  }
}
