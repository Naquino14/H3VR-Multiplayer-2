// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.InteractableExample
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
  [RequireComponent(typeof (Interactable))]
  public class InteractableExample : MonoBehaviour
  {
    private TextMesh generalText;
    private TextMesh hoveringText;
    private Vector3 oldPosition;
    private Quaternion oldRotation;
    private float attachTime;
    private Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;
    private Interactable interactable;
    private bool lastHovering;

    private void Awake()
    {
      TextMesh[] componentsInChildren = this.GetComponentsInChildren<TextMesh>();
      this.generalText = componentsInChildren[0];
      this.hoveringText = componentsInChildren[1];
      this.generalText.text = "No Hand Hovering";
      this.hoveringText.text = "Hovering: False";
      this.interactable = this.GetComponent<Interactable>();
    }

    private void OnHandHoverBegin(Hand hand) => this.generalText.text = "Hovering hand: " + hand.name;

    private void OnHandHoverEnd(Hand hand) => this.generalText.text = "No Hand Hovering";

    private void HandHoverUpdate(Hand hand)
    {
      GrabTypes grabStarting = hand.GetGrabStarting();
      bool flag = hand.IsGrabEnding(this.gameObject);
      if ((Object) this.interactable.attachedToHand == (Object) null && grabStarting != GrabTypes.None)
      {
        this.oldPosition = this.transform.position;
        this.oldRotation = this.transform.rotation;
        hand.HoverLock(this.interactable);
        hand.AttachObject(this.gameObject, grabStarting, this.attachmentFlags);
      }
      else
      {
        if (!flag)
          return;
        hand.DetachObject(this.gameObject);
        hand.HoverUnlock(this.interactable);
        this.transform.position = this.oldPosition;
        this.transform.rotation = this.oldRotation;
      }
    }

    private void OnAttachedToHand(Hand hand)
    {
      this.generalText.text = string.Format("Attached: {0}", (object) hand.name);
      this.attachTime = Time.time;
    }

    private void OnDetachedFromHand(Hand hand) => this.generalText.text = string.Format("Detached: {0}", (object) hand.name);

    private void HandAttachedUpdate(Hand hand) => this.generalText.text = string.Format("Attached: {0} :: Time: {1:F2}", (object) hand.name, (object) (float) ((double) Time.time - (double) this.attachTime));

    private void Update()
    {
      if (this.interactable.isHovering == this.lastHovering)
        return;
      this.hoveringText.text = string.Format("Hovering: {0}", (object) this.interactable.isHovering);
      this.lastHovering = this.interactable.isHovering;
    }

    private void OnHandFocusAcquired(Hand hand)
    {
    }

    private void OnHandFocusLost(Hand hand)
    {
    }
  }
}
