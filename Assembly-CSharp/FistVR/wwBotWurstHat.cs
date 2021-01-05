// Decompiled with JetBrains decompiler
// Type: FistVR.wwBotWurstHat
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class wwBotWurstHat : FVRPhysicalObject
  {
    [Header("Hat Stuff")]
    public wwBotManager Manager;
    public wwBotWurst Wurst;
    public Collider[] Cols;
    private bool m_isRemovedYet;
    public int HatBanditIndex;
    public bool IsPosse;

    public override bool IsDistantGrabbable() => this.m_isRemovedYet;

    public override bool IsInteractable() => !this.IsPosse && base.IsInteractable();

    public void Remove() => this.m_isRemovedYet = true;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (this.m_isRemovedYet || !((Object) this.Wurst != (Object) null))
        return;
      this.m_isRemovedYet = true;
      this.DistantGrabbable = true;
      foreach (Collider col in this.Cols)
        col.enabled = true;
      this.Wurst.HatRemoved();
    }

    public void OnTriggerEnter(Collider col)
    {
      if (!col.gameObject.CompareTag("wwHatReturn"))
        return;
      this.Manager.BotHatRetrieved(this.HatBanditIndex);
      if (this.IsHeld)
      {
        FVRViveHand hand = this.m_hand;
        this.EndInteraction(this.m_hand);
        hand.ForceSetInteractable((FVRInteractiveObject) null);
      }
      Object.Destroy((Object) this.gameObject);
    }
  }
}
