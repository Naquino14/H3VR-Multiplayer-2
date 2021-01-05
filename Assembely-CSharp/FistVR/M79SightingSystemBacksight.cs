using UnityEngine;

namespace FistVR
{
	public class M79SightingSystemBacksight : FVRInteractiveObject
	{
		public M79SightingSystemBase SightBase;

		public Transform MinPoint;

		public Transform MaxPoint;

		public override bool IsInteractable()
		{
			return SightBase.IsFlippedUp;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			base.transform.position = GetClosestValidPointBackSight(hand.transform.position);
		}

		private Vector3 GetClosestValidPointBackSight(Vector3 vPoint)
		{
			return GetClosestValidPoint(MaxPoint.position, MinPoint.position, vPoint);
		}
	}
}
