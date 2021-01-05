using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FistVR
{
	public class MG_FlyingHotDogSwarm : MonoBehaviour
	{
		public GameObject FlyingHotDogPrefab;

		public Transform FlyingHotDogTargetRoot;

		public Transform[] FlyingHotDogTargets;

		public NavMeshAgent Agent;

		private List<FVRPhysicalObject> m_flyers = new List<FVRPhysicalObject>();

		private List<float> m_diveTicks = new List<float>();

		private List<AudioSource> m_auds = new List<AudioSource>();

		public float NavStabilizePower = 1f;

		public float NavStabilizeDeltaPower = 1f;

		public float NavStabilizeRotPower = 100f;

		public float NavStabilizeRotDeltaPower = 100f;

		public AudioClip Clip_DrillOn;

		public AudioClip Clip_DrillDive;

		private float strikeDistance = 0.27f;

		public LayerMask DamageLM_player;

		private RaycastHit m_hit;

		private float m_refireTick;

		private float ReOrientTick = 1f;

		private void Awake()
		{
			for (int i = 0; i < FlyingHotDogTargets.Length; i++)
			{
				GameObject gameObject = Object.Instantiate(FlyingHotDogPrefab, FlyingHotDogTargets[i].position, FlyingHotDogTargets[i].rotation);
				FVRPhysicalObject component = gameObject.GetComponent<FVRPhysicalObject>();
				m_flyers.Add(component);
				m_diveTicks.Add(Random.Range(1f, 4f));
				AudioSource component2 = gameObject.GetComponent<AudioSource>();
				m_auds.Add(component2);
			}
		}

		private void Start()
		{
			if (Agent != null)
			{
				Agent.enabled = true;
			}
		}

		private void FireShot(int i)
		{
			if (i < m_flyers.Count && !(m_flyers[i] == null) && Physics.Raycast(m_flyers[i].transform.position, m_flyers[i].transform.forward, out m_hit, strikeDistance, DamageLM_player, QueryTriggerInteraction.Collide))
			{
				Damage damage = new Damage();
				damage.Class = Damage.DamageClass.Melee;
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

		private void FixedUpdate()
		{
			for (int i = 0; i < m_flyers.Count; i++)
			{
				if (!(m_flyers[i] == null) && !m_flyers[i].IsHeld)
				{
					if (Vector3.Distance(m_flyers[i].transform.position, FlyingHotDogTargets[i].position) > 4f)
					{
						m_flyers[i].transform.position = FlyingHotDogTargets[i].position;
					}
					StabilizeTowardsTarget(m_flyers[i], FlyingHotDogTargets[i], m_diveTicks[i]);
					TurnTowardsTarget(m_flyers[i], FlyingHotDogTargets[i]);
				}
			}
			if (ReOrientTick > 0f)
			{
				ReOrientTick -= Time.deltaTime;
			}
			else
			{
				FlyingHotDogTargetRoot.localRotation = Random.rotation;
				ReOrientTick = Random.Range(1f, 5f);
			}
			FlyingHotDogTargetRoot.transform.localPosition = new Vector3(0f, 1.2f + Mathf.Sin(Time.time) * 0.1f, 0f);
			if (GM.CurrentPlayerBody != null && GM.CurrentPlayerBody.Head != null)
			{
				Vector3 vector = GM.CurrentPlayerBody.Head.transform.position + Vector3.up * -0.25f;
				for (int j = 0; j < FlyingHotDogTargets.Length; j++)
				{
					FlyingHotDogTargets[j].LookAt(vector + Random.onUnitSphere * 0.04f, Vector3.up);
				}
			}
			for (int k = 0; k < m_diveTicks.Count; k++)
			{
				m_diveTicks[k] -= Time.deltaTime;
				if (m_diveTicks[k] < -1f)
				{
					m_diveTicks[k] = Random.Range(0.5f, 3f);
				}
				if (m_diveTicks[k] > 0f)
				{
					if (m_auds[k] != null && m_auds[k].clip != Clip_DrillOn)
					{
						m_auds[k].clip = Clip_DrillOn;
						if (!m_auds[k].isPlaying)
						{
							m_auds[k].Play();
						}
					}
					continue;
				}
				if (m_refireTick <= 0f)
				{
					m_refireTick = Random.Range(0.05f, 0.1f);
					FireShot(k);
				}
				if (m_auds[k] != null && m_auds[k].clip != Clip_DrillDive)
				{
					m_auds[k].clip = Clip_DrillDive;
					if (!m_auds[k].isPlaying)
					{
						m_auds[k].Play();
					}
				}
			}
			if (m_refireTick > 0f)
			{
				m_refireTick -= Time.deltaTime;
			}
		}

		private void Update()
		{
			int num = 0;
			for (int i = 0; i < m_flyers.Count; i++)
			{
				if (m_flyers[i] == null)
				{
					num++;
				}
			}
			if (num >= 3)
			{
				Object.Destroy(base.gameObject);
			}
			float num2 = Vector3.Distance(GM.CurrentPlayerBody.transform.position, base.transform.position);
			if (num2 > 20f || m_flyers.Count <= 0)
			{
				for (int j = 0; j < m_flyers.Count; j++)
				{
					m_flyers[j].GetComponent<FVRDestroyableObject>().DestroyEvent();
				}
				Object.Destroy(base.gameObject);
			}
			else if (num2 > 2f && !Agent.pathPending)
			{
				Agent.SetDestination(GM.CurrentPlayerBody.transform.position + GM.CurrentPlayerBody.Head.forward * 0.5f);
			}
		}

		private void StabilizeTowardsTarget(FVRPhysicalObject obj, Transform targ, float diveTick)
		{
			Vector3 vector = targ.position;
			if (diveTick < 0f)
			{
				vector = targ.position + targ.forward * 1.7f;
			}
			Vector3 position = obj.transform.position;
			Vector3 vector2 = vector - position;
			Vector3 target = vector2 * NavStabilizePower * Time.fixedDeltaTime;
			obj.RootRigidbody.velocity = Vector3.MoveTowards(obj.RootRigidbody.velocity, target, NavStabilizeDeltaPower * Time.fixedDeltaTime);
		}

		private void TurnTowardsTarget(FVRPhysicalObject obj, Transform targ)
		{
			Quaternion rotation = targ.rotation;
			Quaternion rotation2 = obj.transform.rotation;
			(rotation * Quaternion.Inverse(rotation2)).ToAngleAxis(out var angle, out var axis);
			if (angle > 180f)
			{
				angle -= 360f;
			}
			if (angle != 0f)
			{
				Vector3 target = Time.fixedDeltaTime * angle * axis * NavStabilizeRotPower;
				obj.RootRigidbody.angularVelocity = Vector3.MoveTowards(obj.RootRigidbody.angularVelocity, target, NavStabilizeRotDeltaPower * Time.fixedDeltaTime);
			}
		}
	}
}
