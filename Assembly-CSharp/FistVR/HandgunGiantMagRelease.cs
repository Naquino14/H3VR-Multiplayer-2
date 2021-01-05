// Decompiled with JetBrains decompiler
// Type: FistVR.HandgunGiantMagRelease
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class HandgunGiantMagRelease : FVRInteractiveObject
  {
    public Handgun Handgun;

    public override void Poke(FVRViveHand hand)
    {
      if ((Object) this.Handgun.m_hand == (Object) null || (Object) hand != (Object) this.Handgun.m_hand)
        this.Handgun.EjectMag();
      base.Poke(hand);
    }
  }
}
