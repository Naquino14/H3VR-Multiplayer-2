using UnityEngine;

namespace FistVR
{
	public class MF2_Wrench : MonoBehaviour
	{
		private void OnCollisionEnter(Collision collision)
		{
			if (!(collision.collider.attachedRigidbody == null) && !(collision.relativeVelocity.magnitude < 2f))
			{
				MF2_Dispenser component = collision.collider.attachedRigidbody.gameObject.GetComponent<MF2_Dispenser>();
				if (component != null)
				{
					component.HitCharge();
				}
			}
		}
	}
}
