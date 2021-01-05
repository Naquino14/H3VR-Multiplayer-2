using UnityEngine;

namespace FistVR
{
	public class Molotov : FVRPhysicalObject, IFVRDamageable
	{
		[Header("Molotov Params")]
		public float ShatterThreshold = 3f;

		public FVRIgnitable Igniteable;

		public AudioEvent AudEvent_Ignite;

		public GameObject Prefab_ShatterFX;

		public GameObject Prefab_FireSplosion;

		public GameObject Prefab_GroundFire;

		public float GroundFireRange = 5f;

		public LayerMask LM_Env;

		private RaycastHit m_hit;

		private float TickDownToShatter = 28f;

		private bool m_hasShattered;

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 30f)
			{
				FXM.Ignite(Igniteable, 1f);
			}
			if (d.Dam_TotalKinetic > 100f)
			{
				Shatter();
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (Igniteable.IsOnFire())
			{
				TickDownToShatter -= Time.deltaTime;
				if (TickDownToShatter <= 0f)
				{
					Shatter();
				}
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			float magnitude = col.relativeVelocity.magnitude;
			if (magnitude > ShatterThreshold)
			{
				Shatter();
			}
		}

		private void Shatter()
		{
			if (m_hasShattered)
			{
				return;
			}
			m_hasShattered = true;
			if (Igniteable.IsOnFire())
			{
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
			}
			Object.Instantiate(Prefab_ShatterFX, base.transform.position, base.transform.rotation);
			Object.Destroy(base.gameObject);
		}

		private void OnParticleCollision(GameObject other)
		{
			Igniteable.OnParticleCollision(other);
		}
	}
}
