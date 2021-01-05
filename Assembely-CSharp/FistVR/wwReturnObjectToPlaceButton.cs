using UnityEngine;

namespace FistVR
{
	public class wwReturnObjectToPlaceButton : FVRInteractiveObject
	{
		public FVRPhysicalObject Obj;

		public Transform ReturnPoint;

		public AudioSource Aud;

		public override void Poke(FVRViveHand hand)
		{
			base.Poke(hand);
			if (!Obj.IsHeld && Obj.QuickbeltSlot == null)
			{
				Aud.Play();
				Obj.transform.position = ReturnPoint.position;
				Obj.transform.rotation = ReturnPoint.rotation;
				Obj.RootRigidbody.velocity = Vector3.zero;
				Obj.RootRigidbody.angularVelocity = Vector3.zero;
			}
		}
	}
}
