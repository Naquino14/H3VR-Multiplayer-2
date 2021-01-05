// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigEndingStarter
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class ZosigEndingStarter : FVRPhysicalObject
  {
    public AudioEvent AudEvent_Eat;
    public ZosigEndingManager M;

    public void Update()
    {
      if (!this.IsHeld)
        return;
      FVRViveHand hand = this.m_hand;
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f) >= 0.150000005960464)
        return;
      this.EndInteraction(this.m_hand);
      hand.ForceSetInteractable((FVRInteractiveObject) null);
      SM.PlayGenericSound(this.AudEvent_Eat, this.transform.position);
      this.M.InitEnding();
      Object.Destroy((Object) this.gameObject);
    }
  }
}
