using UnityEngine;

namespace FistVR
{
	public class FVRVerticleRope : FVRInteractiveObject
	{
		private Vector3 lastHandPos = Vector3.zero;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			lastHandPos = hand.transform.localPosition;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			float y = lastHandPos.y - m_hand.transform.localPosition.y;
			m_hand.WholeRig.position += new Vector3(0f, y, 0f);
			lastHandPos = m_hand.transform.localPosition;
		}

		public void LateUpdate()
		{
		}
	}
}
