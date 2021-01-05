using UnityEngine;

namespace FistVR
{
	public class Microtorch : MonoBehaviour
	{
		public FVRFireArmAttachment Attachment;

		public Transform FlamePoint;

		public GameObject FX;

		public bool m_isFireActive;

		public LayerMask LM_FireDamage;

		private RaycastHit m_hit;

		public float PointsDamage = 5f;

		private void Update()
		{
			if (Attachment != null)
			{
				if (!m_isFireActive && Attachment.curMount != null)
				{
					m_isFireActive = true;
					FX.SetActive(value: true);
				}
				if (m_isFireActive && Attachment.curMount == null)
				{
					m_isFireActive = false;
					FX.SetActive(value: false);
				}
			}
			if (!m_isFireActive)
			{
				return;
			}
			Vector3 position = FlamePoint.position;
			if (Physics.Raycast(position, FlamePoint.forward, out m_hit, 0.08f, LM_FireDamage, QueryTriggerInteraction.Collide))
			{
				IFVRDamageable component = m_hit.collider.gameObject.GetComponent<IFVRDamageable>();
				if (component == null && m_hit.collider.attachedRigidbody != null)
				{
					component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (component != null)
				{
					Damage damage = new Damage();
					damage.Class = Damage.DamageClass.Explosive;
					damage.Dam_Thermal = 50f;
					damage.Dam_TotalEnergetic = 50f;
					damage.point = m_hit.point;
					damage.hitNormal = m_hit.normal;
					damage.strikeDir = base.transform.forward;
					component.Damage(damage);
				}
				FVRIgnitable component2 = m_hit.collider.transform.gameObject.GetComponent<FVRIgnitable>();
				if (component2 == null && m_hit.collider.attachedRigidbody != null)
				{
					m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
				}
				if (component2 != null)
				{
					FXM.Ignite(component2, 1f);
				}
			}
		}
	}
}
