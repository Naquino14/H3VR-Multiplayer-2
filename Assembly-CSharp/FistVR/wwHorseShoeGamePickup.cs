// Decompiled with JetBrains decompiler
// Type: FistVR.wwHorseShoeGamePickup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwHorseShoeGamePickup : FVRInteractiveObject
  {
    [Header("Pickup Stuff")]
    public wwHorseShoePlinth Plinth;
    public GameObject HorseshoePrefab;

    public override void BeginInteraction(FVRViveHand hand)
    {
      wwHorseShoe component = Object.Instantiate<GameObject>(this.HorseshoePrefab, this.transform.position, this.transform.rotation).GetComponent<wwHorseShoe>();
      component.Plinth = this.Plinth;
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(hand);
      this.Plinth.GrabbedHorseshoe();
    }
  }
}
