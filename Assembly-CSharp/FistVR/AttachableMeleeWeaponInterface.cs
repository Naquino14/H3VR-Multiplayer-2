// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableMeleeWeaponInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace FistVR
{
  public class AttachableMeleeWeaponInterface : FVRFireArmAttachmentInterface
  {
    public override void OnAttach()
    {
      base.OnAttach();
      if (!(this.Attachment.curMount.GetRootMount().Parent is FVRFireArm))
        return;
      (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterAttachedMeleeWeapon(this.Attachment as AttachableMeleeWeapon);
    }

    public override void OnDetach()
    {
      if (this.Attachment.curMount.GetRootMount().Parent is FVRFireArm)
        (this.Attachment.curMount.GetRootMount().Parent as FVRFireArm).RegisterAttachedMeleeWeapon((AttachableMeleeWeapon) null);
      base.OnDetach();
    }
  }
}
