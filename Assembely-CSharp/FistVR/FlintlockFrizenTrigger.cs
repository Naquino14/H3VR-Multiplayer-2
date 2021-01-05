using UnityEngine;

namespace FistVR
{
	public class FlintlockFrizenTrigger : FVRInteractiveObject
	{
		public FlintlockFlashPan FlashPan;

		public Transform DistPoint;

		public override bool IsInteractable()
		{
			if (FlashPan.GetWeapon().HammerState == FlintlockWeapon.HState.Uncocked)
			{
				return false;
			}
			return true;
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			FlashPan.ToggleFrizenState();
			base.SimpleInteraction(hand);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!FlashPan.GetWeapon().IsHeld)
			{
				return;
			}
			FVRViveHand otherHand = FlashPan.GetWeapon().m_hand.OtherHand;
			Vector3 closestValidPoint = GetClosestValidPoint(otherHand.Input.Pos, otherHand.PalmTransform.position, DistPoint.position);
			float num = Vector3.Distance(DistPoint.position, closestValidPoint);
			if (!(num < 0.045f))
			{
				return;
			}
			Vector3 vector = FlashPan.GetWeapon().transform.InverseTransformVector(otherHand.Input.VelLinearWorld);
			if (FlashPan.FrizenState == FlintlockFlashPan.FState.Up)
			{
				if (vector.z < -0.5f)
				{
					FlashPan.ToggleFrizenState();
				}
			}
			else if (vector.z > 0.5f)
			{
				FlashPan.ToggleFrizenState();
			}
		}
	}
}
