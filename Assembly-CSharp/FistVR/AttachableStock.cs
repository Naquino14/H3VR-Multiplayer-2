// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableStock
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AttachableStock : FVRFireArmAttachmentInterface
  {
    public Transform Point_Stock;

    public override void OnAttach()
    {
      base.OnAttach();
      (this.Attachment.curMount.Parent as FVRFireArm).StockPos = this.Point_Stock;
      (this.Attachment.curMount.Parent as FVRFireArm).HasActiveShoulderStock = true;
    }

    public override void OnDetach()
    {
      (this.Attachment.curMount.Parent as FVRFireArm).StockPos = (Transform) null;
      (this.Attachment.curMount.Parent as FVRFireArm).HasActiveShoulderStock = false;
      base.OnDetach();
    }
  }
}
