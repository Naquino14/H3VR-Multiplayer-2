using UnityEngine;

namespace FistVR
{
	public class FVRPalm : MonoBehaviour
	{
		public FVRViveHand Hand;

		private void OnTriggerEnter(Collider collider)
		{
			Hand.TestCollider(collider, isEnter: true, isPalm: true);
		}

		private void OnTriggerStay(Collider collider)
		{
			Hand.TestCollider(collider, isEnter: false, isPalm: false);
		}

		private void OnTriggerExit(Collider collider)
		{
			Hand.HandTriggerExit(collider, isPalm: false);
		}
	}
}
