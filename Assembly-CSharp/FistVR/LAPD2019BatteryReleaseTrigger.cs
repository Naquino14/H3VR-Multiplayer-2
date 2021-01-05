// Decompiled with JetBrains decompiler
// Type: FistVR.LAPD2019BatteryReleaseTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class LAPD2019BatteryReleaseTrigger : FVRInteractiveObject
  {
    public LAPD2019 Gun;

    public override bool IsInteractable() => this.Gun.HasBattery;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      float battery = this.Gun.ExtractBattery(hand);
      if ((double) battery < -0.100000001490116)
        return;
      LAPD2019Battery component = Object.Instantiate<GameObject>(this.Gun.BatteryPrefab.GetGameObject(), hand.transform.position, hand.transform.rotation).GetComponent<LAPD2019Battery>();
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(hand);
      component.SetEnergy(battery);
    }
  }
}
