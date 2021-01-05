using UnityEngine;

namespace FistVR
{
	public class wwEventPuzzle_Churner_ButterStick : FVRPhysicalObject
	{
		public wwEventPuzzle_Churner Churner;

		public int ButterIndex;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.transform.position) < 0.15f)
			{
				EndInteraction(hand);
				hand.ForceSetInteractable(null);
				Churner.AteButter(ButterIndex, GM.CurrentPlayerBody.Head.transform.position);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
