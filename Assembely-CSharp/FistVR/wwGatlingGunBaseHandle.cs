using UnityEngine;

namespace FistVR
{
	public class wwGatlingGunBaseHandle : FVRInteractiveObject
	{
		public Transform GunBase;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = -(hand.transform.position - GunBase.position);
			Vector3 forward = Vector3.ProjectOnPlane(vector, Vector3.up);
			GunBase.transform.rotation = Quaternion.Slerp(GunBase.transform.rotation, Quaternion.LookRotation(forward, Vector3.up), Time.deltaTime * 4f);
		}
	}
}
