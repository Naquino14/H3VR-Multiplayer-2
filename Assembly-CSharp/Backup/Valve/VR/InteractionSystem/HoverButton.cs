// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.HoverButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  [RequireComponent(typeof (Interactable))]
  public class HoverButton : MonoBehaviour
  {
    public Transform movingPart;
    public Vector3 localMoveDistance = new Vector3(0.0f, -0.1f, 0.0f);
    [Range(0.0f, 1f)]
    public float engageAtPercent = 0.95f;
    [Range(0.0f, 1f)]
    public float disengageAtPercent = 0.9f;
    public HandEvent onButtonDown;
    public HandEvent onButtonUp;
    public HandEvent onButtonIsPressed;
    public bool engaged;
    public bool buttonDown;
    public bool buttonUp;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 handEnteredPosition;
    private bool hovering;
    private Hand lastHoveredHand;

    private void Start()
    {
      if ((Object) this.movingPart == (Object) null && this.transform.childCount > 0)
        this.movingPart = this.transform.GetChild(0);
      this.startPosition = this.movingPart.localPosition;
      this.endPosition = this.startPosition + this.localMoveDistance;
      this.handEnteredPosition = this.endPosition;
    }

    private void HandHoverUpdate(Hand hand)
    {
      this.hovering = true;
      this.lastHoveredHand = hand;
      bool engaged = this.engaged;
      float num1 = Vector3.Distance(this.movingPart.parent.InverseTransformPoint(hand.transform.position), this.endPosition);
      float num2 = Vector3.Distance(this.handEnteredPosition, this.endPosition);
      if ((double) num1 > (double) num2)
      {
        num2 = num1;
        this.handEnteredPosition = this.movingPart.parent.InverseTransformPoint(hand.transform.position);
      }
      float t = Mathf.InverseLerp(0.0f, this.localMoveDistance.magnitude, num2 - num1);
      if ((double) t > (double) this.engageAtPercent)
        this.engaged = true;
      else if ((double) t < (double) this.disengageAtPercent)
        this.engaged = false;
      this.movingPart.localPosition = Vector3.Lerp(this.startPosition, this.endPosition, t);
      this.InvokeEvents(engaged, this.engaged);
    }

    private void LateUpdate()
    {
      if (!this.hovering)
      {
        this.movingPart.localPosition = this.startPosition;
        this.handEnteredPosition = this.endPosition;
        this.InvokeEvents(this.engaged, false);
        this.engaged = false;
      }
      this.hovering = false;
    }

    private void InvokeEvents(bool wasEngaged, bool isEngaged)
    {
      this.buttonDown = !wasEngaged && isEngaged;
      this.buttonUp = wasEngaged && !isEngaged;
      if (this.buttonDown && this.onButtonDown != null)
        this.onButtonDown.Invoke(this.lastHoveredHand);
      if (this.buttonUp && this.onButtonUp != null)
        this.onButtonUp.Invoke(this.lastHoveredHand);
      if (!isEngaged || this.onButtonIsPressed == null)
        return;
      this.onButtonIsPressed.Invoke(this.lastHoveredHand);
    }
  }
}
