// Decompiled with JetBrains decompiler
// Type: FistVR.FVRCappedGrenadeCap
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRCappedGrenadeCap : FVRInteractiveObject
  {
    public FVRCappedGrenade Grenade;
    public bool IsPrimaryCap = true;

    public override bool IsInteractable() => !((Object) this.Grenade.QuickbeltSlot != (Object) null) && (this.IsPrimaryCap || this.Grenade.IsPrimaryCapRemoved) && base.IsInteractable();

    public override void BeginInteraction(FVRViveHand hand) => this.Grenade.CapRemoved(this.IsPrimaryCap, hand, this);
  }
}
