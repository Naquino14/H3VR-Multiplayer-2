// Decompiled with JetBrains decompiler
// Type: FistVR.FVRMatchboxNewMatchTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRMatchboxNewMatchTrigger : FVRInteractiveObject
  {
    public GameObject MatchPrefab;

    public override void BeginInteraction(FVRViveHand hand)
    {
      FVRPhysicalObject component = Object.Instantiate<GameObject>(this.MatchPrefab, hand.transform.position, hand.transform.rotation).GetComponent<FVRPhysicalObject>();
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteraction(hand);
    }
  }
}
