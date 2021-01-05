using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	[RequireComponent(typeof(Interactable))]
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
			TextMesh[] componentsInChildren = GetComponentsInChildren<TextMesh>();
			generalText = componentsInChildren[0];
			hoveringText = componentsInChildren[1];
			generalText.text = "No Hand Hovering";
			hoveringText.text = "Hovering: False";
			interactable = GetComponent<Interactable>();
		}

		private void OnHandHoverBegin(Hand hand)
		{
			generalText.text = "Hovering hand: " + hand.name;
		}

		private void OnHandHoverEnd(Hand hand)
		{
			generalText.text = "No Hand Hovering";
		}

		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting();
			bool flag = hand.IsGrabEnding(base.gameObject);
			if (interactable.attachedToHand == null && grabStarting != 0)
			{
				oldPosition = base.transform.position;
				oldRotation = base.transform.rotation;
				hand.HoverLock(interactable);
				hand.AttachObject(base.gameObject, grabStarting, attachmentFlags);
			}
			else if (flag)
			{
				hand.DetachObject(base.gameObject);
				hand.HoverUnlock(interactable);
				base.transform.position = oldPosition;
				base.transform.rotation = oldRotation;
			}
		}

		private void OnAttachedToHand(Hand hand)
		{
			generalText.text = $"Attached: {hand.name}";
			attachTime = Time.time;
		}

		private void OnDetachedFromHand(Hand hand)
		{
			generalText.text = $"Detached: {hand.name}";
		}

		private void HandAttachedUpdate(Hand hand)
		{
			generalText.text = $"Attached: {hand.name} :: Time: {Time.time - attachTime:F2}";
		}

		private void Update()
		{
			if (interactable.isHovering != lastHovering)
			{
				hoveringText.text = $"Hovering: {interactable.isHovering}";
				lastHovering = interactable.isHovering;
			}
		}

		private void OnHandFocusAcquired(Hand hand)
		{
		}

		private void OnHandFocusLost(Hand hand)
		{
		}
	}
}
