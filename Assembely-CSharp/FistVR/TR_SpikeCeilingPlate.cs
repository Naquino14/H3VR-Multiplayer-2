using UnityEngine;

namespace FistVR
{
	public class TR_SpikeCeilingPlate : MonoBehaviour
	{
		public Transform ReciprocatingSpikes;

		public Transform[] CastPointsLong;

		public Transform[] CastPointsShort;

		private int m_longIndex;

		private int m_shortIndex;

		public LayerMask PlayerLM;

		private RaycastHit m_hit;

		private float longLength = 1.9f;

		private float shortLength = 1.2f;

		public void Retract()
		{
			ReciprocatingSpikes.transform.localPosition = new Vector3(0f, 1.2f, 0f);
		}

		public void Expand()
		{
			ReciprocatingSpikes.transform.localPosition = new Vector3(0f, 0f, 0f);
		}

		public void Update()
		{
			m_longIndex++;
			if (m_longIndex >= CastPointsLong.Length)
			{
				m_longIndex = 0;
			}
			m_shortIndex++;
			if (m_shortIndex >= CastPointsShort.Length)
			{
				m_shortIndex = 0;
			}
			if (Physics.Raycast(CastPointsLong[m_longIndex].position, Vector3.down, out m_hit, longLength, PlayerLM, QueryTriggerInteraction.Collide) && m_hit.collider.attachedRigidbody != null)
			{
				FVRPlayerHitbox component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
				if (component != null)
				{
					DamageDealt dam = default(DamageDealt);
					dam.force = Vector3.zero;
					dam.hitNormal = Vector3.zero;
					dam.IsInside = false;
					dam.MPa = 1f;
					dam.MPaRootMeter = 1f;
					dam.point = base.transform.position;
					dam.PointsDamage = 6000f;
					dam.ShotOrigin = null;
					dam.strikeDir = Vector3.zero;
					dam.uvCoords = Vector2.zero;
					dam.IsInitialContact = true;
					component.Damage(dam);
				}
			}
			if (Physics.Raycast(CastPointsShort[m_shortIndex].position, Vector3.down, out m_hit, shortLength, PlayerLM, QueryTriggerInteraction.Collide) && m_hit.collider.attachedRigidbody != null)
			{
				FVRPlayerHitbox component2 = m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
				if (component2 != null)
				{
					DamageDealt dam2 = default(DamageDealt);
					dam2.force = Vector3.zero;
					dam2.hitNormal = Vector3.zero;
					dam2.IsInside = false;
					dam2.MPa = 1f;
					dam2.MPaRootMeter = 1f;
					dam2.point = base.transform.position;
					dam2.PointsDamage = 6000f;
					dam2.ShotOrigin = null;
					dam2.strikeDir = Vector3.zero;
					dam2.uvCoords = Vector2.zero;
					dam2.IsInitialContact = true;
					component2.Damage(dam2);
				}
			}
		}
	}
}
