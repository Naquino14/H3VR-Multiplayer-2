// Decompiled with JetBrains decompiler
// Type: FistVR.AttachableBipodInterface
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AttachableBipodInterface : FVRFireArmAttachmentInterface
  {
    public FVRFireArmBipod Bipod;

    public override void SimpleInteraction(FVRViveHand hand)
    {
      this.Bipod.Toggle();
      base.SimpleInteraction(hand);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      if (hand.IsInStreamlinedMode)
      {
        if (hand.Input.BYButtonDown)
          this.Bipod.NextML();
      }
      else if (hand.Input.TouchpadDown)
      {
        Vector2 touchpadAxes = hand.Input.TouchpadAxes;
        if ((double) touchpadAxes.magnitude > 0.300000011920929)
        {
          if ((double) Vector2.Angle(touchpadAxes, Vector2.left) < 45.0)
            this.Bipod.PrevML();
          else if ((double) Vector2.Angle(touchpadAxes, Vector2.right) < 45.0)
            this.Bipod.NextML();
        }
      }
      base.UpdateInteraction(hand);
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      base.BeginInteraction(hand);
      this.Bipod.Toggle();
    }

    public override void OnAttach()
    {
      base.OnAttach();
      this.Attachment.curMount.GetRootMount().Parent.Bipod = this.Bipod;
      this.Bipod.FireArm = this.Attachment.curMount.GetRootMount().Parent;
    }

    public override void OnDetach()
    {
      if (this.Bipod.isActiveAndEnabled)
        this.Bipod.Contract(true);
      this.Attachment.curMount.GetRootMount().Parent.Bipod = (FVRFireArmBipod) null;
      this.Bipod.FireArm = (FVRPhysicalObject) null;
      base.OnDetach();
    }

    [ContextMenu("Config")]
    public void Config() => this.Bipod = this.transform.GetComponent<FVRFireArmBipod>();
  }
}
