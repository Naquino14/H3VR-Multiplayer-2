using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SlicerBladeMaster : MonoBehaviour
	{
		public Rigidbody BaseRB;

		public SlicerComputer Comp;

		public Transform Blade1;

		public Transform Blade2;

		public Collider[] Blade1Cols;

		public Collider[] Blade2Cols;

		private HashSet<Collider> m_blade1Cols = new HashSet<Collider>();

		private HashSet<Collider> m_blade2Cols = new HashSet<Collider>();

		public Collider[] Blade1Shards;

		public Collider[] Blade2Shards;

		public ParticleSystem PSystem_Sparks;

		public ParticleSystem PSystem_Sparks2;

		private ParticleSystem.EmitParams emitParams;

		public int MaxParticlesPerCollision = 1;

		private int m_myTurn;

		public AudioSource AudSource_SawImpact;

		private float m_sawImpactVolume;

		public AudioSource AudSource_SawCollision;

		private float m_sawCollisionSoundRefire;

		public AudioClip[] AudClip_SawCollision;

		private bool isBlade1Active = true;

		private bool isBlade2Active = true;

		private float m_Blade1Integrity = 2000f;

		private float m_Blade2Integrity = 2000f;

		private int currentBladeCastIndexA;

		private int currentBladeCastIndexB = 1;

		private RaycastHit m_hit;

		public LayerMask BladeLM;

		public LayerMask BladeLMPlayer;

		private void Awake()
		{
			emitParams = default(ParticleSystem.EmitParams);
			for (int i = 0; i < Blade1Cols.Length; i++)
			{
				m_blade1Cols.Add(Blade1Cols[i]);
			}
			for (int j = 0; j < Blade2Cols.Length; j++)
			{
				m_blade2Cols.Add(Blade2Cols[j]);
			}
		}

		private void Update()
		{
			m_sawImpactVolume -= Time.deltaTime * 3f;
			if (m_sawImpactVolume <= 0f && AudSource_SawImpact.isPlaying)
			{
				AudSource_SawImpact.Stop();
			}
			if (m_sawCollisionSoundRefire > 0f)
			{
				m_sawCollisionSoundRefire -= Time.deltaTime;
			}
			if (isBlade1Active)
			{
				Blade1.localEulerAngles = new Vector3(0f, Mathf.Repeat(Blade1.localEulerAngles.y + Time.deltaTime * 3600f, 360f), 0f);
			}
			if (isBlade2Active)
			{
				Blade2.localEulerAngles = new Vector3(0f, Mathf.Repeat(Blade2.localEulerAngles.y + Time.deltaTime * 3600f, 360f), 0f);
			}
		}

		private void FixedUpdate()
		{
			if (isBlade1Active)
			{
				BaseRB.AddTorque(-Blade1.up * Random.Range(-0.4f, 0.4f));
			}
			if (isBlade2Active)
			{
				BaseRB.AddTorque(-Blade2.up * Random.Range(-0.4f, 0.4f));
			}
			currentBladeCastIndexA++;
			if (currentBladeCastIndexA > 7)
			{
				currentBladeCastIndexA = 0;
			}
			currentBladeCastIndexB++;
			if (currentBladeCastIndexB > 7)
			{
				currentBladeCastIndexB = 0;
			}
			if (isBlade1Active)
			{
				Vector3 vector = Blade1Cols[currentBladeCastIndexB].transform.position - Blade1Cols[currentBladeCastIndexA].transform.position;
				if (Physics.Raycast(Blade1Cols[currentBladeCastIndexA].transform.position, vector.normalized, out m_hit, vector.magnitude, BladeLM, QueryTriggerInteraction.Collide))
				{
					IFVRDamageable component = m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
					if (component != null)
					{
						Damage damage = new Damage();
						damage.hitNormal = m_hit.normal;
						damage.Dam_Cutting = 600f;
						damage.Dam_TotalKinetic = 600f;
						damage.Class = Damage.DamageClass.Melee;
						damage.point = m_hit.point;
						damage.strikeDir = -m_hit.normal;
						component.Damage(damage);
					}
				}
			}
			if (!isBlade2Active)
			{
				return;
			}
			Vector3 vector2 = Blade2Cols[currentBladeCastIndexB].transform.position - Blade2Cols[currentBladeCastIndexA].transform.position;
			if (Physics.Raycast(Blade2Cols[currentBladeCastIndexA].transform.position, vector2.normalized, out m_hit, vector2.magnitude, BladeLM, QueryTriggerInteraction.Collide))
			{
				IFVRDamageable component2 = m_hit.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component2 != null)
				{
					Damage damage2 = new Damage();
					damage2.hitNormal = m_hit.normal;
					damage2.Dam_Cutting = 600f;
					damage2.Dam_TotalKinetic = 600f;
					damage2.Class = Damage.DamageClass.Melee;
					damage2.point = m_hit.point;
					damage2.strikeDir = -m_hit.normal;
					component2.Damage(damage2);
				}
			}
		}

		private void OnCollisionEnter(Collision col)
		{
			if (m_sawCollisionSoundRefire <= 0f)
			{
				for (int i = 0; i < col.contacts.Length; i++)
				{
					if ((isBlade1Active && m_blade1Cols.Contains(col.contacts[i].thisCollider)) || (isBlade2Active && m_blade2Cols.Contains(col.contacts[i].thisCollider)))
					{
						AudSource_SawCollision.pitch = Random.Range(0.65f, 1f);
						AudSource_SawCollision.PlayOneShot(AudClip_SawCollision[Random.Range(0, AudClip_SawCollision.Length)], Random.Range(0.6f, 0.9f));
						m_sawCollisionSoundRefire = Random.Range(0.25f, 0.6f);
					}
				}
			}
			for (int j = 0; j < col.contacts.Length; j++)
			{
				float num = BaseRB.mass * col.relativeVelocity.magnitude;
				if (col.contacts[j].otherCollider.attachedRigidbody != null)
				{
					num *= col.contacts[j].otherCollider.attachedRigidbody.mass / BaseRB.mass;
				}
				float num2 = num / Time.fixedDeltaTime;
				float num3 = 0.0008f;
				float num4 = num2 / num3;
				float num5 = num4 / 1000000f;
				float num6 = Mathf.Pow(num3, 0.25f) * num4 / 1000000f;
				if (isBlade1Active && m_blade1Cols.Contains(col.contacts[j].thisCollider))
				{
					DamageBlade(num6 / (float)col.contacts.Length, 0, col.contacts[j].point);
				}
				else if (isBlade2Active && m_blade2Cols.Contains(col.contacts[j].thisCollider))
				{
					DamageBlade(num6 / (float)col.contacts.Length, 1, col.contacts[j].point);
				}
			}
		}

		public void DamageBlade(float MPaRM, int BladeNum, Vector3 point)
		{
			if (BladeNum == 0)
			{
				m_Blade1Integrity -= MPaRM;
				if (m_Blade1Integrity <= 0f)
				{
					DestroyBlade(0, point);
				}
			}
			else
			{
				m_Blade2Integrity -= MPaRM;
				if (m_Blade2Integrity <= 0f)
				{
					DestroyBlade(1, point);
				}
			}
		}

		public void DestroyBlade(int BladeNum, Vector3 point)
		{
			if (BladeNum == 0 && isBlade1Active)
			{
				isBlade1Active = false;
				Blade1.gameObject.SetActive(value: false);
				for (int i = 0; i < Blade1Shards.Length; i++)
				{
					Blade1Shards[i].gameObject.SetActive(value: true);
					Blade1Cols[i].gameObject.SetActive(value: false);
					Blade1Shards[i].transform.SetParent(null);
					Rigidbody rigidbody = Blade1Shards[i].gameObject.AddComponent<Rigidbody>();
					rigidbody.mass = 0.25f;
					rigidbody.AddForceAtPosition(Random.onUnitSphere * Random.Range(1f, 10f), point, ForceMode.Impulse);
				}
				DamageDealt damageDealt = default(DamageDealt);
				damageDealt.force = Vector3.up * 0.1f;
				damageDealt.hitNormal = damageDealt.force;
				damageDealt.IsInside = true;
				damageDealt.IsInitialContact = true;
				damageDealt.MPa = 50f;
				damageDealt.MPaRootMeter = 10f;
				damageDealt.point = point;
				damageDealt.PointsDamage = 600f;
				damageDealt.ShotOrigin = null;
				damageDealt.strikeDir = -damageDealt.force;
				damageDealt.uvCoords = new Vector2(0.5f, 0.5f);
				if (!(Comp != null))
				{
				}
			}
			else if (BladeNum == 1 && isBlade2Active)
			{
				isBlade2Active = false;
				Blade2.gameObject.SetActive(value: false);
				for (int j = 0; j < Blade2Shards.Length; j++)
				{
					Blade2Shards[j].gameObject.SetActive(value: true);
					Blade2Cols[j].gameObject.SetActive(value: false);
					Blade2Shards[j].transform.SetParent(null);
					Rigidbody rigidbody2 = Blade2Shards[j].gameObject.AddComponent<Rigidbody>();
					rigidbody2.mass = 0.25f;
					rigidbody2.AddForceAtPosition(Random.onUnitSphere * Random.Range(1f, 10f), point, ForceMode.Impulse);
				}
				DamageDealt damageDealt2 = default(DamageDealt);
				damageDealt2.force = Vector3.up * 0.1f;
				damageDealt2.hitNormal = damageDealt2.force;
				damageDealt2.IsInside = true;
				damageDealt2.MPa = 50f;
				damageDealt2.IsInitialContact = true;
				damageDealt2.MPaRootMeter = 10f;
				damageDealt2.point = point;
				damageDealt2.PointsDamage = 600f;
				damageDealt2.ShotOrigin = null;
				damageDealt2.strikeDir = -damageDealt2.force;
				damageDealt2.uvCoords = new Vector2(0.5f, 0.5f);
				if (!(Comp != null))
				{
				}
			}
			if (!isBlade1Active && !isBlade2Active)
			{
				if (Comp != null)
				{
					Comp.SetToughnessPercentageIfHigher(0.2f);
				}
			}
			else if ((!isBlade1Active || !isBlade2Active) && Comp != null)
			{
				Comp.SetToughnessPercentageIfHigher(0.5f);
			}
		}

		private void OnCollisionStay(Collision col)
		{
			int num = 0;
			for (int i = 0; i < col.contacts.Length; i++)
			{
				if (num >= MaxParticlesPerCollision)
				{
					continue;
				}
				if (m_myTurn == 0)
				{
					m_myTurn = 1;
				}
				else
				{
					m_myTurn = 0;
				}
				if (isBlade1Active && m_blade1Cols.Contains(col.contacts[i].thisCollider))
				{
					num++;
					emitParams.position = col.contacts[i].point;
					Vector3 normalized = (col.contacts[i].point - base.transform.position).normalized;
					normalized = Vector3.Cross(normalized, Blade1.up) * 30f;
					normalized += Random.onUnitSphere * 3f;
					emitParams.velocity = normalized;
					PSystem_Sparks.Emit(emitParams, 1);
					emitParams.velocity = normalized * 0.2f + Random.onUnitSphere * 15f;
					PSystem_Sparks2.Emit(emitParams, 1);
					BaseRB.AddForceAtPosition(-normalized, col.contacts[i].point, ForceMode.Force);
					BaseRB.AddForceAtPosition(col.contacts[i].normal * 6f, col.contacts[i].point, ForceMode.Force);
					if (m_myTurn == 0)
					{
						FXM.InitiateMuzzleFlash(col.contacts[i].point + col.contacts[i].normal * 0.025f, col.contacts[i].normal, Random.Range(0.25f, 2f), Color.white, Random.Range(0.5f, 2f));
					}
					m_sawImpactVolume = Random.Range(0.9f, 1f);
					AudSource_SawImpact.pitch = Random.Range(0.97f, 1.03f);
					if (!AudSource_SawImpact.isPlaying)
					{
						AudSource_SawImpact.time = Random.Range(0f, 2f);
						AudSource_SawImpact.Play();
					}
				}
				if (isBlade2Active && m_blade2Cols.Contains(col.contacts[i].thisCollider))
				{
					num++;
					emitParams.position = col.contacts[i].point;
					Vector3 normalized2 = (col.contacts[i].point - base.transform.position).normalized;
					normalized2 = Vector3.Cross(normalized2, Blade2.up) * 30f;
					normalized2 += Random.onUnitSphere * 3f;
					emitParams.velocity = normalized2;
					PSystem_Sparks.Emit(emitParams, 1);
					emitParams.velocity = normalized2 * 0.15f + Random.onUnitSphere * 5f;
					PSystem_Sparks2.Emit(emitParams, 1);
					BaseRB.AddForceAtPosition(-normalized2, col.contacts[i].point, ForceMode.Force);
					BaseRB.AddForceAtPosition(col.contacts[i].normal * 6f, col.contacts[i].point, ForceMode.Force);
					if (m_myTurn == 1)
					{
						FXM.InitiateMuzzleFlash(col.contacts[i].point + col.contacts[i].normal * 0.025f, col.contacts[i].normal, Random.Range(0.25f, 2f), Color.white, Random.Range(0.5f, 2f));
					}
					m_sawImpactVolume = Random.Range(0.9f, 1f);
					AudSource_SawImpact.pitch = Random.Range(0.97f, 1.03f);
					if (!AudSource_SawImpact.isPlaying)
					{
						AudSource_SawImpact.time = Random.Range(0f, 2f);
						AudSource_SawImpact.Play();
					}
				}
			}
		}
	}
}
