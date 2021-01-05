// Decompiled with JetBrains decompiler
// Type: FistVR.RevolvingShotgunTrigger
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class RevolvingShotgunTrigger : FVRInteractiveObject
  {
    public RevolvingShotgun Shotgun;
    public RevolvingShotgunTrigger.TrigType TType;

    public override bool IsInteractable() => this.TType == RevolvingShotgunTrigger.TrigType.Grabbing && this.Shotgun.CylinderLoaded;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      base.SimpleInteraction(hand);
      if (this.TType == RevolvingShotgunTrigger.TrigType.CockHammer)
        ;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (this.TType != RevolvingShotgunTrigger.TrigType.Grabbing || !this.Shotgun.CylinderLoaded)
        return;
      this.EndInteraction(hand);
      Speedloader speedloader = this.Shotgun.EjectCylinder();
      hand.ForceSetInteractable((FVRInteractiveObject) speedloader);
      speedloader.BeginInteraction(hand);
    }

    public enum TrigType
    {
      Loading,
      Grabbing,
      CockHammer,
    }
  }
}
