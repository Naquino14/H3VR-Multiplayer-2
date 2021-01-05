using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class MG_JerryTheLemon : MonoBehaviour, IFVRDamageable
	{
		public NavMeshAgent Agent;

		private float m_life = 9000f;

		public GameObject[] Spawns;

		private bool m_isExploded;

		private Vector3 pos;

		public GameObject[] BoltPrefabs;

		public AudioSource LightningSource;

		public AudioClip[] Audioclips;

		public LayerMask DamageLM_player;

		private RaycastHit m_hit;

		private float FireBoltTick = 0.25f;

		private int pulseTick;

		private void Start()
		{
			pos = base.transform.localPosition;
		}

		private void Update()
		{
			float num = Vector3.Distance(GM.CurrentPlayerBody.transform.position, base.transform.position);
			if (num > 23f)
			{
				Object.Destroy(Agent.gameObject);
			}
			else if (num > 5f && !Agent.pathPending)
			{
				Agent.SetDestination(GM.CurrentPlayerBody.transform.position + GM.CurrentPlayerBody.Head.forward * 2f);
			}
			if (num < 12f)
			{
				pulseTick--;
				if (pulseTick <= 0)
				{
					pulseTick = Random.Range(1, 4);
					FXM.InitiateMuzzleFlash(base.transform.position + Random.onUnitSphere, Random.onUnitSphere, Random.Range(2f, 4f), new Color(1f, 1f, 0.1f, 1f), Random.Range(2f, 4f));
				}
			}
			if (FireBoltTick > 0f)
			{
				FireBoltTick -= Time.deltaTime;
			}
			else
			{
				FireBoltTick = Random.Range(3.5f, 6f);
				if (num < 6f)
				{
					FireBolt();
				}
			}
			base.transform.localPosition = pos + Random.onUnitSphere * 0.01f;
		}

		private void FireBolt()
		{
			Vector3 vector = GM.CurrentPlayerBody.Head.position - Vector3.up * 0.35f + Random.onUnitSphere * 0.2f - base.transform.position;
			LightningSource.PlayOneShot(Audioclips[Random.Range(0, Audioclips.Length)], 1f);
			Vector3 zero = Vector3.zero;
			FXM.InitiateMuzzleFlash(zero, vector, Random.Range(4f, 10f), new Color(1f, 1f, 0.9f, 1f), Random.Range(3f, 5f));
			Object.Instantiate(BoltPrefabs[Random.Range(0, BoltPrefabs.Length)], base.transform.position, Quaternion.LookRotation(vector));
			if (Physics.Raycast(base.transform.position, vector.normalized, out m_hit, vector.magnitude + 1f, DamageLM_player, QueryTriggerInteraction.Collide))
			{
				Damage damage = new Damage();
				damage.Class = FistVR.Damage.DamageClass.Projectile;
				damage.Dam_Piercing = 500f;
				damage.Dam_TotalKinetic = 500f;
				damage.point = m_hit.point;
				damage.hitNormal = m_hit.normal;
				damage.strikeDir = base.transform.forward;
				IFVRDamageable component = m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component != null)
				{
					component.Damage(damage);
				}
				else if (component == null && m_hit.collider.attachedRigidbody != null)
				{
					m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>()?.Damage(damage);
				}
			}
		}

		public void Damage(Damage d)
		{
			m_life -= d.Dam_TotalKinetic;
			if (m_life <= 0f)
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_isExploded)
			{
				m_isExploded = true;
				for (int i = 0; i < Spawns.Length; i++)
				{
					Object.Instantiate(Spawns[i], base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
