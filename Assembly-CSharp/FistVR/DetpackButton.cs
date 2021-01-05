// Decompiled with JetBrains decompiler
// Type: FistVR.DetpackButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class DetpackButton : FVRInteractiveObject
  {
    public MF2_Detpack Det;
    public bool isStart = true;

    public override void Poke(FVRViveHand hand)
    {
      if ((Object) this.Det.QuickbeltSlot != (Object) null)
        return;
      base.Poke(hand);
      if (this.isStart)
        this.Det.InitiateCountDown(hand);
      else
        this.Det.ResetCountDown(hand);
    }
  }
}
