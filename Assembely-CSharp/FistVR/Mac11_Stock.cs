using UnityEngine;

namespace FistVR
{
	public class Mac11_Stock : FVRInteractiveObject
	{
		[Header("Mac11 Stock Config")]
		public Transform ForwardPos;

		public Transform RearPos;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			base.transform.position = GetClosestValidPoint(ForwardPos.position, RearPos.position, hand.transform.position);
		}
	}
}
