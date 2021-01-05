// Decompiled with JetBrains decompiler
// Type: FistVR.RevolverEjector
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RevolverEjector : FVRInteractiveObject
  {
    public Revolver Magnum;
    public Transform Ejector;
    public Vector3 ForwardPos;
    public Vector3 RearPos;

    public override bool IsInteractable() => !this.Magnum.isCylinderArmLocked;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      if ((Object) this.Ejector != (Object) null)
        this.Ejector.localPosition = this.RearPos;
      this.Magnum.EjectChambers();
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if (!((Object) this.Ejector != (Object) null))
        return;
      this.Ejector.localPosition = Vector3.Lerp(this.Ejector.localPosition, this.ForwardPos, Time.deltaTime * 6f);
    }
  }
}
