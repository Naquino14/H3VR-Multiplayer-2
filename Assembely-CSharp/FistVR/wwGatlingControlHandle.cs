using UnityEngine;

namespace FistVR
{
	public class wwGatlingControlHandle : FVRInteractiveObject
	{
		public Transform BaseFrame;

		public Transform MountingBracket;

		public float HorizontalClamp = 25f;

		public float VerticalClamp = 20f;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = -(hand.transform.position - base.transform.position);
			Vector3 target = Vector3.ProjectOnPlane(vector, BaseFrame.up);
			Vector3 forward = Vector3.RotateTowards(BaseFrame.forward, target, HorizontalClamp * 0.0174533f, 0f);
			Vector3 target2 = Vector3.ProjectOnPlane(vector, MountingBracket.right);
			Vector3 forward2 = Vector3.RotateTowards(MountingBracket.forward, target2, VerticalClamp * 0.0174533f, 0f);
			MountingBracket.rotation = Quaternion.Slerp(MountingBracket.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 4f);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(forward2, Vector3.up), Time.deltaTime * 4f);
		}
	}
}
