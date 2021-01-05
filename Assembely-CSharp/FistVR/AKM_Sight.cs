using UnityEngine;

namespace FistVR
{
	public class AKM_Sight : FVRInteractiveObject
	{
		public Transform Gun;

		public Transform SightPoint0;

		public Transform SightPoint1;

		public Transform SightPoint2;

		public Transform SightRoundPart;

		public Transform SightStraightPart;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			Vector3 closestValidPoint = GetClosestValidPoint(SightPoint0.position, SightPoint1.position, hand.transform.position);
			Vector3 closestValidPoint2 = GetClosestValidPoint(SightPoint1.position, SightPoint2.position, hand.transform.position);
			float num = Vector3.Distance(closestValidPoint, hand.transform.position);
			float num2 = Vector3.Distance(closestValidPoint2, hand.transform.position);
			if (num <= num2)
			{
				SightRoundPart.position = closestValidPoint;
			}
			else
			{
				SightRoundPart.position = closestValidPoint2;
			}
			Vector3 forward = -(SightRoundPart.position - SightStraightPart.position).normalized;
			SightStraightPart.rotation = Quaternion.LookRotation(forward, Gun.up);
			base.UpdateInteraction(hand);
		}
	}
}
