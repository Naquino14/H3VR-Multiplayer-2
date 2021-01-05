// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.ModalThrowable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class ModalThrowable : Throwable
  {
    [Tooltip("The local point which acts as a positional and rotational offset to use while held with a grip type grab")]
    public Transform gripOffset;
    [Tooltip("The local point which acts as a positional and rotational offset to use while held with a pinch type grab")]
    public Transform pinchOffset;

    protected override void HandHoverUpdate(Hand hand)
    {
      GrabTypes grabStarting = hand.GetGrabStarting();
      switch (grabStarting)
      {
        case GrabTypes.None:
          return;
        case GrabTypes.Pinch:
          hand.AttachObject(this.gameObject, grabStarting, this.attachmentFlags, this.pinchOffset);
          break;
        case GrabTypes.Grip:
          hand.AttachObject(this.gameObject, grabStarting, this.attachmentFlags, this.gripOffset);
          break;
        default:
          hand.AttachObject(this.gameObject, grabStarting, this.attachmentFlags, this.attachmentOffset);
          break;
      }
      hand.HideGrabHint();
    }

    protected override void HandAttachedUpdate(Hand hand)
    {
      if ((Object) this.interactable.skeletonPoser != (Object) null)
        this.interactable.skeletonPoser.SetBlendingBehaviourEnabled("PinchPose", hand.currentAttachedObjectInfo.Value.grabbedWithType == GrabTypes.Pinch);
      base.HandAttachedUpdate(hand);
    }
  }
}
