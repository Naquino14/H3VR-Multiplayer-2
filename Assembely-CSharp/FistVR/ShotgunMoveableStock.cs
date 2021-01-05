using UnityEngine;

namespace FistVR
{
	public class ShotgunMoveableStock : FVRInteractiveObject
	{
		public Transform ForwardPoint;

		public Transform RearwardPoint;

		public FVRFireArm Firearm;

		[Header("SecondPoint")]
		public bool HasSecondPoint;

		public Transform SecondPoint;

		public Transform SecondForwardPoint;

		public Transform SecondRearwardPoint;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			base.transform.position = GetClosestValidPoint(ForwardPoint.position, RearwardPoint.position, base.m_handPos);
			float t = Mathf.InverseLerp(ForwardPoint.localPosition.z, RearwardPoint.localPosition.z, base.transform.localPosition.z);
			if (HasSecondPoint)
			{
				SecondPoint.position = Vector3.Lerp(SecondForwardPoint.position, SecondRearwardPoint.position, t);
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			if (Firearm != null)
			{
				float num = Vector3.Distance(base.transform.position, ForwardPoint.position);
				float num2 = Vector3.Distance(base.transform.position, RearwardPoint.position);
				if (num < 0.01f)
				{
					Firearm.PlayAudioEvent(FirearmAudioEventType.StockClosed);
				}
				else if (num2 < 0.01f)
				{
					Firearm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
				}
			}
		}
	}
}
