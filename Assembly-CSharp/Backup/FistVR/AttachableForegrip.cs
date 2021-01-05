// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableForegrip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AttachableForegrip : FVRFireArmAttachmentInterface
  {
    public Transform ForePosePoint;
    public FVRFireArm OverrideFirearm;
    public bool DoesBracing = true;

    public override void BeginInteraction(FVRViveHand hand)
    {
      FVRFireArm fvrFireArm = this.OverrideFirearm;
      if ((Object) fvrFireArm == (Object) null)
        fvrFireArm = this.Attachment.GetRootObject() as FVRFireArm;
      if (!((Object) fvrFireArm != (Object) null) || !((Object) fvrFireArm.Foregrip != (Object) null))
        return;
      FVRAlternateGrip component = fvrFireArm.Foregrip.GetComponent<FVRAlternateGrip>();
      hand.ForceSetInteractable((FVRInteractiveObject) component);
      component.BeginInteractionFromAttachedGrip(this, hand);
    }

    public virtual void PassHandInput(FVRViveHand hand, FVRInteractiveObject o)
    {
    }
  }
}
