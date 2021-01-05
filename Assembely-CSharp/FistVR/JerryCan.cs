using UnityEngine;

namespace FistVR
{
	public class JerryCan : FVRPhysicalObject, IFVRDamageable
	{
		private bool m_hasExploded;

		public AudioEvent AudEvent_Ignite;

		public GameObject Prefab_FireSplosion;

		public GameObject Prefab_GroundFire;

		public float GroundFireRange = 5f;

		public LayerMask LM_Env;

		private RaycastHit m_hit;

		public override bool IsDistantGrabbable()
		{
			return base.IsDistantGrabbable();
		}

		public override bool IsInteractable()
		{
			return base.IsInteractable();
		}

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 20f || d.Dam_TotalKinetic > 500f)
			{
				Explode();
			}
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		private void Explode()
		{
			if (m_hasExploded)
			{
				return;
			}
			m_hasExploded = true;
			Object.Instantiate(Prefab_FireSplosion, base.transform.position, base.transform.rotation);
			SM.PlayGenericSound(AudEvent_Ignite, base.transform.position);
			int num = 0;
			for (int i = 0; i < 5; i++)
			{
				if (num > 2)
				{
					break;
				}
				Vector3 direction = -Vector3.up;
				if (i > 0)
				{
					direction = Random.onUnitSphere;
					if (direction.y > 0f)
					{
						direction.y = 0f - direction.y;
					}
				}
				if (Physics.Raycast(base.transform.position + Vector3.up, direction, out m_hit, GroundFireRange, LM_Env, QueryTriggerInteraction.Ignore))
				{
					Object.Instantiate(Prefab_GroundFire, m_hit.point, Quaternion.LookRotation(Vector3.up));
					num++;
				}
			}
			Object.Destroy(base.gameObject);
		}
	}
}
