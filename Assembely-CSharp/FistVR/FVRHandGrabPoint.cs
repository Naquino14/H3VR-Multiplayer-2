using UnityEngine;

namespace FistVR
{
	public class FVRHandGrabPoint : FVRInteractiveObject
	{
		private Vector3 lastHandPos = Vector3.zero;

		private FVRMovementManager m_manager;

		private Vector3 MoveDir;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_manager = hand.MovementManager;
			m_manager.BeginGrabPointMove(this);
			lastHandPos = hand.Input.Pos;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_manager.EndGrabPointMove(this);
			base.EndInteraction(hand);
			if (hand.OtherHand.CurrentInteractable == null && hand.OtherHand.ClosestPossibleInteractable == this && hand.OtherHand.Input.IsGrabbing)
			{
				BeginInteraction(hand.OtherHand);
				m_hand.ForceSetInteractable(this);
			}
		}
	}
}
