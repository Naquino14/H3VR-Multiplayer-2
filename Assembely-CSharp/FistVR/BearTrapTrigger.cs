using UnityEngine;

namespace FistVR
{
	public class BearTrapTrigger : MonoBehaviour
	{
		public BearTrap Trap;

		private void OnTriggerEnter(Collider col)
		{
			if (col.attachedRigidbody != null)
			{
				Trap.ThingDetected();
			}
		}
	}
}
