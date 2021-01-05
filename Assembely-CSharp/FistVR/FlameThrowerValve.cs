using UnityEngine;

namespace FistVR
{
	public class FlameThrowerValve : FVRInteractiveObject
	{
		public float MaxRot = 20f;

		public Transform RefVector;

		public float ValvePos = 0.5f;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - base.transform.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, base.transform.up);
			float a = Vector3.Angle(RefVector.forward, vector2);
			Vector3 vector3 = Vector3.RotateTowards(RefVector.forward, vector2, Mathf.Min(a, MaxRot) * 0.0174533f, 0f);
			base.transform.rotation = Quaternion.LookRotation(vector3, RefVector.up);
			float num = Vector3.Angle(RefVector.right, vector3);
			ValvePos = (num - 90f) / (MaxRot * 2f) + 0.5f;
		}
	}
}
