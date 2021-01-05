using UnityEngine;

namespace FistVR
{
	public class PieOMizerDoor : FVRInteractiveObject
	{
		public Transform RefVector;

		public AudioEvent AudEvent_DoorOpen;

		public AudioEvent AudEvent_DoorClose;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			SM.PlayGenericSound(AudEvent_DoorOpen, base.transform.position);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.Input.Pos - base.transform.position;
			Vector3 vector2 = Vector3.ProjectOnPlane(vector, RefVector.right);
			if (Vector3.Angle(vector2, RefVector.up) >= 90f)
			{
				vector2 = RefVector.forward;
			}
			else if (Vector3.Angle(vector2, RefVector.forward) >= 90f)
			{
				vector2 = RefVector.up;
			}
			base.transform.rotation = Quaternion.LookRotation(vector2, RefVector.right);
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			if (Vector3.Angle(base.transform.forward, RefVector.up) < 20f)
			{
				base.transform.rotation = Quaternion.LookRotation(RefVector.up, RefVector.right);
				SM.PlayGenericSound(AudEvent_DoorClose, base.transform.position);
			}
			else if (Vector3.Angle(base.transform.forward, RefVector.forward) < 20f)
			{
				base.transform.rotation = Quaternion.LookRotation(RefVector.forward, RefVector.right);
			}
		}
	}
}
