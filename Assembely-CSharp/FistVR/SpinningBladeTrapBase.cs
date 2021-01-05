using UnityEngine;

namespace FistVR
{
	public class SpinningBladeTrapBase : MonoBehaviour
	{
		public Rigidbody rb;

		private void OnCollisionEnter(Collision col)
		{
			if (rb.angularVelocity.magnitude > 20f)
			{
				IFVRDamageable component = col.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component == null && col.collider.attachedRigidbody != null)
				{
					component = col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (component != null)
				{
					Damage damage = new Damage();
					damage.Dam_Cutting = 20000f;
					damage.Dam_TotalKinetic = 20000f;
					damage.hitNormal = col.contacts[0].normal;
					damage.strikeDir = -damage.hitNormal;
					damage.point = col.contacts[0].point;
					damage.Class = Damage.DamageClass.Environment;
					component.Damage(damage);
				}
			}
		}
	}
}
