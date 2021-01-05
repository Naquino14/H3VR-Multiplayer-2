using UnityEngine;

namespace FistVR
{
	public class AttachableBipodInterface : FVRFireArmAttachmentInterface
	{
		public FVRFireArmBipod Bipod;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			Bipod.Toggle();
			base.SimpleInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					Bipod.NextML();
				}
			}
			else if (hand.Input.TouchpadDown)
			{
				Vector2 touchpadAxes = hand.Input.TouchpadAxes;
				if (touchpadAxes.magnitude > 0.3f)
				{
					if (Vector2.Angle(touchpadAxes, Vector2.left) < 45f)
					{
						Bipod.PrevML();
					}
					else if (Vector2.Angle(touchpadAxes, Vector2.right) < 45f)
					{
						Bipod.NextML();
					}
				}
			}
			base.UpdateInteraction(hand);
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			Bipod.Toggle();
		}

		public override void OnAttach()
		{
			base.OnAttach();
			Attachment.curMount.GetRootMount().Parent.Bipod = Bipod;
			Bipod.FireArm = Attachment.curMount.GetRootMount().Parent;
		}

		public override void OnDetach()
		{
			if (Bipod.isActiveAndEnabled)
			{
				Bipod.Contract(playSound: true);
			}
			Attachment.curMount.GetRootMount().Parent.Bipod = null;
			Bipod.FireArm = null;
			base.OnDetach();
		}

		[ContextMenu("Config")]
		public void Config()
		{
			Bipod = base.transform.GetComponent<FVRFireArmBipod>();
		}
	}
}
