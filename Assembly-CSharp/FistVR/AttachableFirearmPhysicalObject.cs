// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableFirearmPhysicalObject
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AttachableFirearmPhysicalObject : FVRFireArmAttachment
  {
    public AttachableFirearm FA;

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.FA.ProcessInput(hand, false, (FVRInteractiveObject) this);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      this.FA.Tick(Time.deltaTime, this.m_hand);
    }
  }
}
