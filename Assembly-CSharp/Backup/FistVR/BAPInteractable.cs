// Decompiled with JetBrains decompiler
// Type: FistVR.BAPInteractable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class BAPInteractable : FVRInteractiveObject
  {
    public BAP Frame;

    public override bool IsInteractable() => this.Frame.HasBaffle || this.Frame.HasCap;

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      if (this.Frame.HasCap)
      {
        this.EndInteraction(hand);
        BAPCap component = Object.Instantiate<GameObject>(this.Frame.Prefab_Cap, this.Frame.PPoint_Cap.position, this.Frame.PPoint_Cap.rotation).GetComponent<BAPCap>();
        this.Frame.SetCapState(false);
        hand.ForceSetInteractable((FVRInteractiveObject) component);
        component.BeginInteraction(hand);
        this.Frame.RemoveThing();
      }
      else
      {
        if (!this.Frame.HasBaffle)
          return;
        this.EndInteraction(hand);
        BAPBaffle component = Object.Instantiate<GameObject>(this.Frame.Prefab_Baffle, this.Frame.PPoint_Baffle.position, this.Frame.PPoint_Baffle.rotation).GetComponent<BAPBaffle>();
        component.SetState(this.Frame.BaffleState);
        this.Frame.SetBaffleState(false, 0);
        hand.ForceSetInteractable((FVRInteractiveObject) component);
        component.BeginInteraction(hand);
        this.Frame.RemoveThing();
      }
    }

    private void OnTriggerEnter(Collider other)
    {
      if (!this.Frame.CanDetectPiece() || this.Frame.HasBaffle && this.Frame.HasCap)
        return;
      if (!this.Frame.HasCap && !this.Frame.HasBaffle && (Object) other.gameObject.GetComponent<BAPBaffle>() != (Object) null)
      {
        BAPBaffle component = other.gameObject.GetComponent<BAPBaffle>();
        this.Frame.SetBaffleState(true, component.BaffleState);
        Object.Destroy((Object) component.GameObject);
      }
      else
      {
        if (this.Frame.HasCap || !((Object) other.gameObject.GetComponent<BAPCap>() != (Object) null))
          return;
        BAPCap component = other.gameObject.GetComponent<BAPCap>();
        this.Frame.SetCapState(true);
        Object.Destroy((Object) component.GameObject);
      }
    }
  }
}
