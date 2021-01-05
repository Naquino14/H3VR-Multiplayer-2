// Decompiled with JetBrains decompiler
// Type: FistVR.FlintlockRamRodHolder
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FlintlockRamRodHolder : MonoBehaviour
  {
    public FlintlockWeapon Weapon;

    public void OnTriggerEnter(Collider other)
    {
      if ((Object) other.attachedRigidbody == (Object) null)
        return;
      GameObject gameObject = other.attachedRigidbody.gameObject;
      if (!gameObject.CompareTag("flintlock_ramrod"))
        return;
      FlintlockRamRod component = gameObject.GetComponent<FlintlockRamRod>();
      if (!component.IsHeld)
        return;
      FVRViveHand hand = component.m_hand;
      component.ForceBreakInteraction();
      this.Weapon.RamRod.gameObject.SetActive(true);
      this.Weapon.RamRod.RState = FlintlockPseudoRamRod.RamRodState.Lower;
      this.Weapon.RamRod.MountToUnder(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) this.Weapon.RamRod);
      this.Weapon.RamRod.BeginInteraction(hand);
      Object.Destroy((Object) other.gameObject);
    }
  }
}
