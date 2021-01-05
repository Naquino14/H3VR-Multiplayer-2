// Decompiled with JetBrains decompiler
// Type: FistVR.SosigWeaponPlayerInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SosigWeaponPlayerInterface : FVRPhysicalObject
  {
    public SosigWeapon W;

    public override bool IsDistantGrabbable() => !this.W.IsHeldByBot && !this.W.IsInBotInventory && base.IsDistantGrabbable();

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.W.PlayerPickup();
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      if (!this.W.isFullAuto && hand.Input.TriggerDown && this.m_hasTriggeredUpSinceBegin)
      {
        this.W.FireGun(1f);
      }
      else
      {
        if (!this.W.isFullAuto || !hand.Input.TriggerPressed || !this.m_hasTriggeredUpSinceBegin)
          return;
        this.W.FireGun(1f);
      }
    }

    public override GameObject DuplicateFromSpawnLock(FVRViveHand hand)
    {
      GameObject gameObject = base.DuplicateFromSpawnLock(hand);
      gameObject.GetComponent<SosigWeapon>().SetAutoDestroy(true);
      return gameObject;
    }
  }
}
