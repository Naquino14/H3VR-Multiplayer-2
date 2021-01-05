using UnityEngine;

namespace FistVR
{
	public class Russet : FVRPhysicalObject
	{
		public Transform Beam;

		public LayerMask LM_Beam;

		private RaycastHit m_hit;

		private FVRPhysicalObject m_selectedObj;

		private bool m_isObjectInTransit;

		private float m_reset = 1f;

		public AudioEvent AudEvent_PewPew;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.Input.TriggerDown && m_selectedObj == null)
			{
				CastToGrab();
			}
			if (hand.Input.TriggerUp && !m_isObjectInTransit)
			{
				m_selectedObj = null;
			}
			if (m_selectedObj != null && !m_isObjectInTransit)
			{
				float num = 3f;
				if (Mathf.Abs(hand.Input.VelAngularLocal.x) > num || Mathf.Abs(hand.Input.VelAngularLocal.y) > num)
				{
					BeginFlick(m_selectedObj);
				}
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_reset >= 0f && m_isObjectInTransit)
			{
				m_reset -= Time.deltaTime;
				if (m_reset <= 0f)
				{
					m_isObjectInTransit = false;
					m_selectedObj = null;
				}
			}
		}

		private void BeginFlick(FVRPhysicalObject o)
		{
			Vector3 vector = base.transform.position - o.transform.position;
			float proj_speed = Mathf.Clamp(Vector3.Distance(base.transform.position, o.transform.position) * 2f, 3f, 10f);
			Vector3 s;
			Vector3 s2;
			int num = fts.solve_ballistic_arc(o.transform.position, proj_speed, base.transform.position, Mathf.Abs(Physics.gravity.y), out s, out s2);
			Debug.Log(num);
			if (num < 1)
			{
				m_isObjectInTransit = false;
				m_selectedObj = null;
				return;
			}
			Vector3 velocity = s;
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_PewPew, base.transform.position);
			m_selectedObj.RootRigidbody.velocity = velocity;
			m_isObjectInTransit = true;
			m_reset = 2f;
		}

		private void CastToGrab()
		{
			if (Physics.Raycast(Beam.position, Beam.forward, out m_hit, 20f, LM_Beam, QueryTriggerInteraction.Collide) && m_hit.collider.attachedRigidbody != null)
			{
				FVRPhysicalObject component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
				if (component != null)
				{
					m_selectedObj = component;
					SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_PewPew, base.transform.position);
				}
			}
		}
	}
}
