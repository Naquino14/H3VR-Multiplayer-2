// Decompiled with JetBrains decompiler
// Type: FistVR.FVRPalm
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRPalm : MonoBehaviour
  {
    public FVRViveHand Hand;

    private void OnTriggerEnter(Collider collider) => this.Hand.TestCollider(collider, true, true);

    private void OnTriggerStay(Collider collider) => this.Hand.TestCollider(collider, false, false);

    private void OnTriggerExit(Collider collider) => this.Hand.HandTriggerExit(collider, false);
  }
}
