using UnityEngine;

namespace FistVR
{
	public class ItemSpawnerVerticalMover : FVRInteractiveObject
	{
		public Transform root;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector3 closestValidPoint = GetClosestValidPoint(root.transform.position + Vector3.up, root.transform.position - Vector3.up, hand.Input.Pos);
			root.transform.position = closestValidPoint;
			base.UpdateInteraction(hand);
		}
	}
}
