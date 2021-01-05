using UnityEngine;

namespace FistVR
{
	public class DropTrapLogs : MonoBehaviour
	{
		public Transform ColRef;

		public Rigidbody RB;

		private Vector3 m_vel = Vector3.zero;

		private void Start()
		{
		}

		private void FixedUpdate()
		{
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (RB.isKinematic || collision.collider.attachedRigidbody == null)
			{
				return;
			}
			float y = ColRef.transform.position.y;
			ContactPoint contactPoint = collision.contacts[0];
			float num = Mathf.Abs(contactPoint.point.y - y);
			if (!(num > 0.4f))
			{
				IFVRDamageable component = collision.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				if (component != null)
				{
					Damage damage = new Damage();
					damage.Class = Damage.DamageClass.Environment;
					damage.damageSize = 0.1f;
					damage.Dam_Blunt = 20000f;
					damage.Dam_TotalKinetic = 20000f;
					damage.hitNormal = contactPoint.normal;
					damage.point = contactPoint.point;
					damage.Source_IFF = 0;
					damage.strikeDir = -contactPoint.normal;
					damage.Source_Point = contactPoint.point;
					component.Damage(damage);
				}
			}
		}
	}
}
