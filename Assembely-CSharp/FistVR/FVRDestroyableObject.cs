using System;
using UnityEngine;

namespace FistVR
{
	public class FVRDestroyableObject : MonoBehaviour, IFVRDamageable
	{
		[Serializable]
		public struct DetachRBParams
		{
			public float Mass;

			public float Drag;

			public float AngularDrag;
		}

		[Serializable]
		public class ProgressiveDamageFX
		{
			public ParticleSystem Effect;

			public Vector2 EmitRange;

			public float DamageThreshold;

			public void UpdateEffect(float percent)
			{
				if (Effect != null && percent >= DamageThreshold)
				{
					ParticleSystem.EmissionModule emission = Effect.emission;
					ParticleSystem.MinMaxCurve rate = emission.rate;
					rate.mode = ParticleSystemCurveMode.Constant;
					float num3 = (rate.constantMin = (rate.constantMax = Mathf.Lerp(EmitRange.x, EmitRange.y, percent)));
					emission.rate = rate;
				}
			}
		}

		[Header("Destroyable Params")]
		public float StartingToughness;

		public Vector2 ResistMult_Blunt = new Vector2(0f, 1f);

		public Vector2 ResistMult_Piercing = new Vector2(0f, 1f);

		public Vector2 ResistMult_Cutting = new Vector2(0f, 1f);

		protected float m_currentToughness;

		protected bool m_isDestroyed;

		public bool ReceivesCollisionDamage;

		public bool DestroyThisObjectOnDestruction;

		public GameObject[] SpawnOnDestruction;

		public GameObject[] DetachAddRigidbodyBlowAway;

		public bool UsesParams = true;

		public DetachRBParams DetachRigidbodyParams;

		public Vector2 ExplosiveForceToDetach = new Vector2(80f, 150f);

		public FVRDestroyableObject[] SendDestroyEventOnDestruction;

		public Vector2 DestroyEventTimeRange;

		public ProgressiveDamageFX[] ProgressiveDamageEffects;

		public bool DoesDecayWhenDamaged;

		public float PercentThresholdForDecay;

		public float DecayRate;

		private FVRPhysicalObject m_po;

		public Rigidbody Rb;

		public bool UsesDestructionStageRenderers;

		public Renderer[] DestructionRenderers;

		private int m_currentDestructionRenderer = -1;

		public void SetToughnessPercentageIfHigher(float f)
		{
			m_currentToughness = Mathf.Min(StartingToughness * f, m_currentToughness);
		}

		private void UpdateDestructionRenderers()
		{
			float num = 1f - m_currentToughness / StartingToughness;
			num *= (float)DestructionRenderers.Length;
			int value = Mathf.RoundToInt(num);
			value = Mathf.Clamp(value, 0, DestructionRenderers.Length - 1);
			if (m_currentDestructionRenderer == value)
			{
				return;
			}
			m_currentDestructionRenderer = value;
			for (int i = 0; i < DestructionRenderers.Length; i++)
			{
				if (i == m_currentDestructionRenderer)
				{
					DestructionRenderers[i].enabled = true;
				}
				else
				{
					DestructionRenderers[i].enabled = false;
				}
			}
		}

		public virtual void Awake()
		{
			m_currentToughness = StartingToughness;
			m_po = GetComponent<FVRPhysicalObject>();
			Rb = GetComponent<Rigidbody>();
			if (UsesDestructionStageRenderers)
			{
				UpdateDestructionRenderers();
			}
		}

		public virtual void Update()
		{
			for (int i = 0; i < ProgressiveDamageEffects.Length; i++)
			{
				ProgressiveDamageEffects[i].UpdateEffect(1f - m_currentToughness / StartingToughness);
			}
			if (DoesDecayWhenDamaged)
			{
				float num = 1f - m_currentToughness / StartingToughness;
				if (num >= PercentThresholdForDecay)
				{
					m_currentToughness -= DecayRate * Time.deltaTime;
				}
			}
			if (m_currentToughness <= 0f)
			{
				DestroyEvent();
			}
		}

		public virtual void Damage(Damage dam)
		{
			float num = Mathf.Clamp((dam.Dam_Blunt - ResistMult_Blunt.x) * ResistMult_Blunt.y, 0f, dam.Dam_Blunt);
			float num2 = Mathf.Clamp((dam.Dam_Cutting - ResistMult_Cutting.x) * ResistMult_Cutting.y, 0f, dam.Dam_Cutting);
			float num3 = Mathf.Clamp((dam.Dam_Piercing - ResistMult_Piercing.x) * ResistMult_Piercing.y, 0f, dam.Dam_Piercing);
			float num4 = num + num2 + num3;
			m_currentToughness -= num4;
			if (UsesDestructionStageRenderers)
			{
				UpdateDestructionRenderers();
			}
			if (m_currentToughness <= 0f)
			{
				DestroyEvent();
			}
		}

		public void OnCollisionEnter(Collision col)
		{
			if (ReceivesCollisionDamage)
			{
				float num = 100f;
				if (Rb != null)
				{
					num = Rb.mass;
				}
				float num2 = num;
				if (col.rigidbody != null)
				{
					num2 = col.rigidbody.mass;
				}
				float magnitude = col.relativeVelocity.magnitude;
				if (magnitude >= 2.5f)
				{
					float num3 = num2 / num;
					float num4 = col.relativeVelocity.magnitude * num3;
					Vector3 relativeVelocity = col.relativeVelocity;
					Damage damage = new Damage();
					damage.Dam_Blunt = num4 * relativeVelocity.magnitude * 50f;
					damage.hitNormal = col.contacts[0].normal;
					damage.strikeDir = col.relativeVelocity.normalized;
					damage.point = col.contacts[0].point;
					damage.Class = FistVR.Damage.DamageClass.Environment;
					Damage(damage);
				}
			}
		}

		public virtual void DestroyEvent()
		{
			if (m_isDestroyed)
			{
				return;
			}
			m_isDestroyed = true;
			if (m_po != null && m_po.IsHeld && m_po.m_hand != null)
			{
				m_po.m_hand.ForceSetInteractable(null);
				m_po.EndInteraction(m_po.m_hand);
			}
			for (int i = 0; i < DetachAddRigidbodyBlowAway.Length; i++)
			{
				if (DetachAddRigidbodyBlowAway[i] != null)
				{
					DetachAddRigidbodyBlowAway[i].SetActive(value: true);
					DetachAddRigidbodyBlowAway[i].transform.SetParent(null);
					Rigidbody rigidbody = ((!(DetachAddRigidbodyBlowAway[i].GetComponent<Rigidbody>() == null)) ? DetachAddRigidbodyBlowAway[i].GetComponent<Rigidbody>() : DetachAddRigidbodyBlowAway[i].AddComponent<Rigidbody>());
					rigidbody.isKinematic = false;
					if (Rb != null)
					{
						rigidbody.velocity = Rb.velocity;
						rigidbody.angularVelocity = Rb.angularVelocity;
					}
					if (UsesParams)
					{
						rigidbody.mass = DetachRigidbodyParams.Mass;
						rigidbody.drag = DetachRigidbodyParams.Drag;
						rigidbody.angularDrag = DetachRigidbodyParams.AngularDrag;
					}
					rigidbody.AddForceAtPosition((rigidbody.transform.position - base.transform.position).normalized * UnityEngine.Random.Range(ExplosiveForceToDetach.x, ExplosiveForceToDetach.y), base.transform.position);
				}
			}
			for (int j = 0; j < SpawnOnDestruction.Length; j++)
			{
				UnityEngine.Object.Instantiate(SpawnOnDestruction[j], base.transform.position, base.transform.rotation);
			}
			for (int k = 0; k < SendDestroyEventOnDestruction.Length; k++)
			{
				if (SendDestroyEventOnDestruction[k] != null)
				{
					SendDestroyEventOnDestruction[k].Invoke("DestroyEvent", UnityEngine.Random.Range(DestroyEventTimeRange.x, DestroyEventTimeRange.y));
				}
			}
			if (DestroyThisObjectOnDestruction)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
