// Decompiled with JetBrains decompiler
// Type: FistVR.SosigGrenadePin
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SosigGrenadePin : FVRInteractiveObject
  {
    public SosigWeapon Grenade;
    public GameObject PinDiscardGO;
    public Renderer PinRend;
    private bool hasSpawned;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      if ((Object) this.Grenade.O.QuickbeltSlot == (Object) null)
      {
        this.Grenade.FuseGrenade();
        this.SpawnPinAndDisableProxy();
      }
      base.SimpleInteraction(hand);
    }

    public void ForceExpelPin()
    {
      if (this.hasSpawned)
        return;
      this.Grenade.FuseGrenade();
      this.SpawnPinAndDisableProxy();
    }

    private void SpawnPinAndDisableProxy()
    {
      if (this.hasSpawned)
        return;
      this.hasSpawned = true;
      if ((Object) this.PinRend != (Object) null)
        this.PinRend.enabled = false;
      Object.Instantiate<GameObject>(this.PinDiscardGO, this.transform.position, this.transform.rotation).GetComponent<Rigidbody>().velocity = this.Grenade.transform.right;
    }
  }
}
