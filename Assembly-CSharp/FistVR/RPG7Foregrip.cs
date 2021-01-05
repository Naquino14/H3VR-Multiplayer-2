// Decompiled with JetBrains decompiler
// Type: FistVR.RPG7Foregrip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RPG7Foregrip : FVRAlternateGrip
  {
    [Header("RPG-7 Config")]
    public RPG7 RPG;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      bool flag = false;
      if (hand.IsInStreamlinedMode && hand.Input.AXButtonDown)
        flag = true;
      else if (!hand.IsInStreamlinedMode && hand.Input.TouchpadDown)
        flag = true;
      if (flag)
        this.RPG.CockHammer();
      if (hand.Input.TriggerDown)
        this.RPG.Fire();
      this.RPG.UpdateTriggerRot(hand.Input.TriggerFloat);
    }
  }
}
