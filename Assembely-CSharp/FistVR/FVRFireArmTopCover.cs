using UnityEngine;

namespace FistVR
{
	public class FVRFireArmTopCover : FVRInteractiveObject
	{
		public FVRFireArm FireArm;

		public override void BeginInteraction(FVRViveHand hand)
		{
			Vector3 vector = hand.Input.Pos - base.transform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, base.transform.right);
			float value = Vector3.Angle(from, -FireArm.gameObject.transform.forward);
			value = Mathf.Clamp(value, 0f, 90f);
			if (value < 10f)
			{
				FireArm.PlayAudioEvent(FirearmAudioEventType.TopCoverUp);
			}
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.Input.Pos - base.transform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, base.transform.right);
			float value = Vector3.Angle(from, -FireArm.gameObject.transform.forward);
			value = Mathf.Clamp(value, 0f, 90f);
			if (Vector3.Angle(from, FireArm.gameObject.transform.up) > 90f)
			{
				value = 0f;
			}
			base.transform.localEulerAngles = new Vector3(value, 0f, 0f);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			Vector3 vector = hand.Input.Pos - base.transform.position;
			Vector3 from = Vector3.ProjectOnPlane(vector, base.transform.right);
			float value = Vector3.Angle(from, -FireArm.gameObject.transform.forward);
			value = Mathf.Clamp(value, 0f, 90f);
			if (Vector3.Angle(from, FireArm.gameObject.transform.up) > 90f)
			{
				value = 0f;
			}
			FireArm.IsTopCoverUp = true;
			if (value > 75f)
			{
				value = 90f;
			}
			if (value < 15f)
			{
				FireArm.IsTopCoverUp = false;
				value = 0f;
				FireArm.PlayAudioEvent(FirearmAudioEventType.TopCoverDown);
			}
			base.transform.localEulerAngles = new Vector3(value, 0f, 0f);
			base.EndInteraction(hand);
		}
	}
}
